using EInsurance.Models.Commission;

namespace EInsurance.Interfaces;


public interface ICommissionCalculationService
{
    Task<List<AgentSummaryViewModel>> GetAllAgentsAsync(CancellationToken cancellationToken = default);

    
    Task<CommissionCalculationResultViewModel?> CalculateAgentCommissionAsync(
        int agentId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    Task<AgentCommissionDashboardViewModel?> GetAgentDashboardAsync(
        int agentId,
        CancellationToken cancellationToken = default);

    Task<List<CommissionCalculationResultViewModel>> CalculateAllAgentCommissionsAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

   
    Task<CommissionReportViewModel?> FinalizeCommissionAsync(
        int agentId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    Task<bool> MarkCommissionAsPaidAsync(
        int commissionId,
        CancellationToken cancellationToken = default);

    Task<List<CommissionLedgerEntryViewModel>> GetCommissionLedgerAsync(
        int agentId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
