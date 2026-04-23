using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Models.Commission;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Services.Commission;

public class CommissionCalculationService(ApplicationDbContext dbContext) : ICommissionCalculationService
{
    private const decimal DefaultCommissionRate = 0.10m; // 10% default

    public async Task<List<AgentSummaryViewModel>> GetAllAgentsAsync(CancellationToken cancellationToken = default)
    {
        var agents = await dbContext.InsuranceAgents
            .AsNoTracking()
            .OrderBy(a => a.FullName)
            .ToListAsync(cancellationToken);

        var agentSummaries = new List<AgentSummaryViewModel>();

        foreach (var agent in agents)
        {
            var agentPolicies = await dbContext.Policies
                .Where(p => p.Customer.AgentId == agent.AgentId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var activePoliciesCount = agentPolicies.Count;
            var totalPremium = agentPolicies.Sum(p => p.Premium);

            agentSummaries.Add(new AgentSummaryViewModel
            {
                AgentId = agent.AgentId,
                FullName = agent.FullName,
                Email = agent.Email,
                ActivePoliciesCount = activePoliciesCount,
                TotalPremium = totalPremium,
                CommissionRate = GetCommissionRateForAgent(agent)
            });
        }

        return agentSummaries;
    }

    public async Task<CommissionCalculationResultViewModel?> CalculateAgentCommissionAsync(
        int agentId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var agent = await dbContext.InsuranceAgents
            .FirstOrDefaultAsync(a => a.AgentId == agentId, cancellationToken);

        if (agent == null)
            return null;

        var commissionRate = GetCommissionRateForAgent(agent);

        var fromDateOnly = DateOnly.FromDateTime(fromDate);
        var toDateOnly = DateOnly.FromDateTime(toDate);

        var policiesQuery = dbContext.Policies
            .Include(p => p.Customer)
            .Include(p => p.Scheme)
            .Include(p => p.Payments)
            .Where(p => p.Customer.AgentId == agentId &&
                        p.DateIssued >= fromDateOnly &&
                        p.DateIssued <= toDateOnly)
            .AsNoTracking();

        var policies = await policiesQuery.ToListAsync(cancellationToken);

        var paidPolicies = policies
            .Where(p => p.Payments.Any(pm => pm.Amount > 0))
            .ToList();

        var policyDetails = new List<PolicyCommissionDetailViewModel>();
        decimal totalCommission = 0;

        foreach (var policy in paidPolicies)
        {
            var commissionAmount = policy.Premium * commissionRate;
            totalCommission += commissionAmount;

            policyDetails.Add(new PolicyCommissionDetailViewModel
            {
                PolicyId = policy.PolicyId,
                PolicyNumber = $"POL-{policy.PolicyId:D6}",
                CustomerName = policy.Customer.FullName,
                SchemeName = policy.Scheme.SchemeName,
                Premium = policy.Premium,
                CommissionAmount = Math.Round(commissionAmount, 2),
                IssuedDate = policy.DateIssued.ToDateTime(TimeOnly.MinValue),
                Status = "Active"
            });
        }

        return new CommissionCalculationResultViewModel
        {
            AgentId = agentId,
            AgentName = agent.FullName,
            Email = agent.Email,
            FromDate = fromDate,
            ToDate = toDate,
            TotalPoliciesCount = paidPolicies.Count,
            TotalPremium = paidPolicies.Sum(p => p.Premium),
            CommissionRate = commissionRate,
            TotalCommissionAmount = Math.Round(totalCommission, 2),
            CalculatedAtUtc = DateTime.UtcNow,
            PolicyDetails = policyDetails
        };
    }

    public async Task<AgentCommissionDashboardViewModel?> GetAgentDashboardAsync(
        int agentId,
        CancellationToken cancellationToken = default)
    {
        var agent = await dbContext.InsuranceAgents
            .FirstOrDefaultAsync(a => a.AgentId == agentId, cancellationToken);

        if (agent == null)
            return null;

        var currentMonthStart = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

        var currentMonthPolicies = await dbContext.Policies
            .Include(p => p.Customer)
            .Include(p => p.Payments)
            .Where(p => p.Customer.AgentId == agentId &&
                        p.DateIssued >= currentMonthStart &&
                        p.DateIssued <= currentMonthEnd)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var paidCurrentMonth = currentMonthPolicies
            .Where(p => p.Payments.Any(pm => pm.Amount > 0))
            .ToList();

        var commissionRate = GetCommissionRateForAgent(agent);
        var currentMonthCommission = paidCurrentMonth.Sum(p => p.Premium * commissionRate);

        var ytdStart = new DateOnly(DateTime.UtcNow.Year, 1, 1);
        var ytdEnd = DateOnly.FromDateTime(DateTime.UtcNow);

        var ytdPolicies = await dbContext.Policies
            .Include(p => p.Customer)
            .Include(p => p.Payments)
            .Where(p => p.Customer.AgentId == agentId &&
                        p.DateIssued >= ytdStart &&
                        p.DateIssued <= ytdEnd)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var paidYTD = ytdPolicies
            .Where(p => p.Payments.Any(pm => pm.Amount > 0))
            .ToList();

        var ytdCommission = paidYTD.Sum(p => p.Premium * commissionRate);

        var recentCommissions = await dbContext.Commissions
            .Include(c => c.Policy)
            .Where(c => c.AgentId == agentId && c.Status == "Paid")
            .OrderByDescending(c => c.PaidAtUtc)
            .Take(5)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var disbursements = recentCommissions.Select(c => new DisbursementViewModel
        {
            CommissionId = c.CommissionId,
            PolicyReference = $"POL-{c.PolicyId:D6}",
            PolicyName = c.Policy.PolicyDetails,
            Amount = c.CommissionAmount,
            ProcessedDate = c.PaidAtUtc ?? DateTime.UtcNow
        }).ToList();

        var monthlyPerformance = new List<MonthlyPerformanceViewModel>();
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = currentMonthStart.AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var monthPolicies = await dbContext.Policies
                .Include(p => p.Customer)
                .Include(p => p.Payments)
                .Where(p => p.Customer.AgentId == agentId &&
                            p.DateIssued >= monthStart &&
                            p.DateIssued <= monthEnd)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var paidMonth = monthPolicies
                .Where(p => p.Payments.Any(pm => pm.Amount > 0))
                .ToList();

            var monthCommission = paidMonth.Sum(p => p.Premium * commissionRate);

            monthlyPerformance.Add(new MonthlyPerformanceViewModel
            {
                Month = monthStart.ToString("MMM"),
                Commission = Math.Round(monthCommission, 2)
            });
        }

        return new AgentCommissionDashboardViewModel
        {
            AgentId = agentId,
            AgentName = agent.FullName,
            Role = "Senior Underwriter",
            Region = "Pacific Northwest",
            PerformanceBadge = currentMonthCommission > 10000 ? "Top Performer" : "Active",
            ActivePoliciesCurrentMonth = paidCurrentMonth.Count,
            TotalPremiumCurrentMonth = paidCurrentMonth.Sum(p => p.Premium),
            CommissionEarnedCurrentMonth = Math.Round(currentMonthCommission, 2),
            CommissionStatus = "Paid & Finalized",
            TotalPremiumYTD = paidYTD.Sum(p => p.Premium),
            TotalCommissionYTD = Math.Round(ytdCommission, 2),
            RecentDisbursements = disbursements,
            MonthlyPerformance = monthlyPerformance
        };
    }

    public async Task<List<CommissionCalculationResultViewModel>> CalculateAllAgentCommissionsAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var agents = await dbContext.InsuranceAgents
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var results = new List<CommissionCalculationResultViewModel>();

        foreach (var agent in agents)
        {
            var result = await CalculateAgentCommissionAsync(
                agent.AgentId,
                fromDate,
                toDate,
                cancellationToken);

            if (result != null && result.TotalPoliciesCount > 0)
            {
                results.Add(result);
            }
        }

        return results;
    }

   
    public async Task<CommissionReportViewModel?> FinalizeCommissionAsync(
        int agentId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        
        var calculation = await CalculateAgentCommissionAsync(agentId, fromDate, toDate, cancellationToken);
        if (calculation == null || calculation.TotalPoliciesCount == 0)
            return null;

        var fromDateOnly = DateOnly.FromDateTime(fromDate);
        var toDateOnly = DateOnly.FromDateTime(toDate);

        var existingPolicyIds = await dbContext.Commissions
            .Where(c => c.AgentId == agentId)
            .Select(c => c.PolicyId)
            .ToListAsync(cancellationToken);

        var commissionsToAdd = new List<Domain.Entities.Commission>();
        var newPoliciesCount = 0;

        foreach (var policyDetail in calculation.PolicyDetails)
        {
            if (existingPolicyIds.Contains(policyDetail.PolicyId))
                continue;

            var commission = new Domain.Entities.Commission
            {
                AgentId = agentId,
                PolicyId = policyDetail.PolicyId,
                CommissionAmount = policyDetail.CommissionAmount,
                Status = "Calculated",
                CreatedAt = DateTime.UtcNow
            };

            commissionsToAdd.Add(commission);
            newPoliciesCount++;
        }

        if (commissionsToAdd.Count > 0)
        {
            await dbContext.Commissions.AddRangeAsync(commissionsToAdd, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        
        return new CommissionReportViewModel
        {
            AgentId = calculation.AgentId,
            AgentName = calculation.AgentName,
            FromDate = calculation.FromDate,
            ToDate = calculation.ToDate,
            TotalPoliciesCount = calculation.TotalPoliciesCount,
            PaidPoliciesCount = newPoliciesCount,
            TotalPremium = calculation.TotalPremium,
            CommissionRate = calculation.CommissionRate,
            TotalCommissionAmount = commissionsToAdd.Sum(c => c.CommissionAmount),
            FinalizedAtUtc = DateTime.UtcNow,
            Status = newPoliciesCount > 0 ? "Success" : "AlreadyFinalized",
            PolicyDetails = calculation.PolicyDetails.Where(p => !existingPolicyIds.Contains(p.PolicyId)).ToList()
        };
    }

    public async Task<bool> MarkCommissionAsPaidAsync(int commissionId, CancellationToken cancellationToken = default)
    {
        var commission = await dbContext.Commissions
            .FirstOrDefaultAsync(c => c.CommissionId == commissionId, cancellationToken);

        if (commission == null)
            return false;

        commission.Status = "Paid";
        commission.PaidAtUtc = DateTime.UtcNow;

        dbContext.Commissions.Update(commission);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<CommissionLedgerViewModel> GetCommissionLedgerAsync(
        int agentId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.Commissions
            .Where(c => c.AgentId == agentId)
            .AsNoTracking();

        var totalRecords = await baseQuery.CountAsync(cancellationToken);
        var paidCount = await baseQuery.CountAsync(c => c.Status == "Paid", cancellationToken);
        var totalCommission = await baseQuery
            .Select(c => (decimal?)c.CommissionAmount)
            .SumAsync(cancellationToken) ?? 0m;

        var commissions = await baseQuery
            .Include(c => c.Policy)
                .ThenInclude(p => p.Customer)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = commissions.Select(c => new CommissionLedgerEntryViewModel
        {
            CommissionId = c.CommissionId,
            PolicyNumber = $"POL-{c.PolicyId:D6}",
            CustomerName = c.Policy.Customer.FullName,
            Premium = c.Policy.Premium,
            CommissionAmount = c.CommissionAmount,
            Status = c.Status,
            CalculatedDate = c.CreatedAt,
            PaidDate = c.PaidAtUtc
        }).ToList();

        return new CommissionLedgerViewModel
        {
            AgentId = agentId,
            Ledger = new EInsurance.Models.Common.PagedResult<CommissionLedgerEntryViewModel>(items, totalRecords, pageNumber, pageSize),
            TotalCommissionAmount = totalCommission,
            PaidCount = paidCount,
            PendingCount = totalRecords - paidCount
        };
    }

    private decimal GetCommissionRateForAgent(InsuranceAgent agent)
    {
        return agent.CommissionRate > 0 ? agent.CommissionRate : DefaultCommissionRate;
    }
}
