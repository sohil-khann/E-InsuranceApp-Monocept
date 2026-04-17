using EInsurance.Domain.Common;
using EInsurance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<InsuranceAgent> InsuranceAgents => Set<InsuranceAgent>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InsurancePlan> InsurancePlans => Set<InsurancePlan>();
    public DbSet<Scheme> Schemes => Set<Scheme>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<EmployeeScheme> EmployeeSchemes => Set<EmployeeScheme>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(type => typeof(AuditableEntity).IsAssignableFrom(type.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(AuditableEntity.CreatedAt))
                .HasDefaultValueSql("GETUTCDATE()");
        }


        // one customer can have one insurance agent, but an insurance agent can have many customers
        modelBuilder.Entity<InsuranceAgent>()
            .HasMany(agent => agent.Customers)
            .WithOne(customer => customer.Agent)
            .HasForeignKey(customer => customer.AgentId)
            .OnDelete(DeleteBehavior.SetNull);

        // one insurance agent can have many commissions, but a commission belongs to one insurance agent
        modelBuilder.Entity<InsuranceAgent>()
            .HasMany(agent => agent.Commissions)
            .WithOne(commission => commission.Agent)
            .HasForeignKey(commission => commission.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // one insurance plan can have many schemes, but a scheme belongs to one insurance plan
        modelBuilder.Entity<InsurancePlan>()
            .HasMany(plan => plan.Schemes)
            .WithOne(scheme => scheme.Plan)
            .HasForeignKey(scheme => scheme.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // one scheme can have many policies, but a policy belongs to one scheme
        modelBuilder.Entity<Scheme>()
            .HasMany(scheme => scheme.Policies)
            .WithOne(policy => policy.Scheme)
            .HasForeignKey(policy => policy.SchemeId)
            .OnDelete(DeleteBehavior.Restrict);

        // one customer can have many policies, but a policy belongs to one customer
        modelBuilder.Entity<Policy>()
            .HasOne(policy => policy.Customer)
            .WithMany(customer => customer.Policies)
            .HasForeignKey(policy => policy.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // one policy can have many payments, but a payment belongs to one policy
        modelBuilder.Entity<Policy>()
            .HasMany(policy => policy.Payments)
            .WithOne(payment => payment.Policy)
            .HasForeignKey(payment => payment.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // one policy can have many commissions, but a commission belongs to one policy
        modelBuilder.Entity<Policy>()
            .HasMany(policy => policy.Commissions)
            .WithOne(commission => commission.Policy)
            .HasForeignKey(commission => commission.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // one customer can have many payments, but a payment belongs to one customer
        modelBuilder.Entity<Payment>()
            .HasOne(payment => payment.Customer)
            .WithMany(customer => customer.Payments)
            .HasForeignKey(payment => payment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
