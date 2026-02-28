using BankingDashAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingDashAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<HomeKpi> HomeKpi => Set<HomeKpi>();
    public DbSet<DepositAnalysis> DepositAnalysis => Set<DepositAnalysis>();
    public DbSet<LoanAnalysis> LoanAnalysis => Set<LoanAnalysis>();
    public DbSet<MonthlyTrend> MonthlyTrend => Set<MonthlyTrend>();
    public DbSet<BankingSummary> BankingSummary => Set<BankingSummary>();

    public DbSet<CustomerCountByCategory> CustomerCountByCategory { get; set; }

    public DbSet<AvailableDate> AvailableDates { get; set; }

    public DbSet<DepositOpeningSummary> DepositOpeningSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure HomeKpi
        modelBuilder.Entity<HomeKpi>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("VW_HOME_KPI");

            // Map to the actual column names from your database
            entity.Property(e => e.TotalLoanAccounts).HasColumnName("TotalLoanAccounts");
            entity.Property(e => e.TotalLoanCustomers).HasColumnName("TotalLoanCustomers");
            entity.Property(e => e.TotalLoanOutstanding).HasColumnName("TotalLoanOutstanding");
            entity.Property(e => e.AvgLoanInterest).HasColumnName("AvgLoanInterest");
            entity.Property(e => e.TotalDepositAccounts).HasColumnName("TotalDepositAccounts");
            entity.Property(e => e.TotalDepositCustomers).HasColumnName("TotalDepositCustomers");
            entity.Property(e => e.TotalDepositBalance).HasColumnName("TotalDepositBalance");
            entity.Property(e => e.AvgDepositInterest).HasColumnName("AvgDepositInterest");
            entity.Property(e => e.ActiveLoanAccounts).HasColumnName("ActiveLoanAccounts");
            entity.Property(e => e.ActiveDepositAccounts).HasColumnName("ActiveDepositAccounts");

            // Note: HomeKpi doesn't have BRANCH_CODE or YEAR based on your output
            // If they exist in the view but weren't shown, add these:
            // entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
            // entity.Property(e => e.Year).HasColumnName("YEAR");
        });

        // Configure DepositAnalysis
        modelBuilder.Entity<DepositAnalysis>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("VW_DEPOSIT_ANALYSIS");

            entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
            entity.Property(e => e.ProductCode).HasColumnName("PRODUCT_CODE");
            entity.Property(e => e.SchemeCode).HasColumnName("SCHEMECODE");
            entity.Property(e => e.CustomerCategory).HasColumnName("CUSTOMER_CATEGORY");
            entity.Property(e => e.AccountStatus).HasColumnName("ACCOUNT_STATUS");
            entity.Property(e => e.CustomerGender).HasColumnName("CUSTOMER_GENDER");
            entity.Property(e => e.AccountCount).HasColumnName("AccountCount");
            entity.Property(e => e.TotalBalance).HasColumnName("TotalBalance");
            entity.Property(e => e.TotalDepositAmount).HasColumnName("TotalDepositAmount");
            entity.Property(e => e.AvgInterestRate).HasColumnName("AvgInterestRate");
            entity.Property(e => e.OpenYear).HasColumnName("OpenYear");
            entity.Property(e => e.OpenMonth).HasColumnName("OpenMonth");
        });

        // Configure LoanAnalysis
        modelBuilder.Entity<LoanAnalysis>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("VW_LOAN_ANALYSIS");

            entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
            entity.Property(e => e.LoanSchemeCode).HasColumnName("LOAN_SCHEEME_CODE");
            entity.Property(e => e.Purpose).HasColumnName("PURPOSE");
            entity.Property(e => e.PrioritySector).HasColumnName("PRIORITY_SECTOER");
            entity.Property(e => e.Segment).HasColumnName("SEGMENT");
            entity.Property(e => e.SecureUnsecure).HasColumnName("SECURE_UNSECURE");
            entity.Property(e => e.AccountStatus).HasColumnName("ACCOUNT_STATUS");
            entity.Property(e => e.LoanCount).HasColumnName("LoanCount");
            entity.Property(e => e.TotalSanctionAmount).HasColumnName("TotalSanctionAmount");
            entity.Property(e => e.TotalOutstanding).HasColumnName("TotalOutstanding");
            entity.Property(e => e.TotalOverdueAmount).HasColumnName("TotalOverdueAmount");
            entity.Property(e => e.AvgInterestRate).HasColumnName("AvgInterestRate");
            entity.Property(e => e.DisbursementYear).HasColumnName("DisbursementYear");
            entity.Property(e => e.DisbursementMonth).HasColumnName("DisbursementMonth");
        });

        // Configure MonthlyTrend
        modelBuilder.Entity<MonthlyTrend>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("VW_MONTHLY_TREND");

            // Based on your output, these are the likely column names
            // You may need to adjust these based on the actual view
            entity.Property(e => e.Year).HasColumnName("Year");
            entity.Property(e => e.Month).HasColumnName("Month");
            entity.Property(e => e.TotalLoanAmount).HasColumnName("TotalLoanAmount");
            entity.Property(e => e.TotalDepositAmount).HasColumnName("TotalDepositAmount");
            entity.Property(e => e.NetPosition).HasColumnName("NetPosition");
            entity.Property(e => e.LoanDepositRatio).HasColumnName("LoanDepositRatio");
        });

        // Configure BankingSummary
        modelBuilder.Entity<BankingSummary>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("VW_BANKING_SUMMARY");

            entity.Property(e => e.BranchCode).HasColumnName("BRANCH_CODE");
            entity.Property(e => e.TotalLoanAccounts).HasColumnName("TotalLoanAccounts");
            entity.Property(e => e.TotalLoanAmount).HasColumnName("TotalLoanAmount");
            entity.Property(e => e.TotalDepositAccounts).HasColumnName("TotalDepositAccounts");
            entity.Property(e => e.TotalDepositAmount).HasColumnName("TotalDepositAmount");
            entity.Property(e => e.NetPosition).HasColumnName("NetPosition");
            entity.Property(e => e.LoanDepositRatio).HasColumnName("LoanDepositRatio");
        });


        // For the Customer Count by Category

        modelBuilder.Entity<CustomerCountByCategory>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.TotalCustomer)
                  .HasColumnName("TotalCustomer"); 

            entity.Property(e => e.Cat)
                  .HasColumnName("Cat") 
                  .HasMaxLength(10);
        });


        modelBuilder.Entity<AvailableDate>(entity =>
        {
            entity.HasNoKey(); //  no primary key
            entity.Property(e => e.Date).HasColumnName("AvailableDate");
        });


        // Deposit Opening Summary
        modelBuilder.Entity<DepositOpeningSummary>(entity =>
        {
            entity.HasNoKey(); // Stored procedure result has no primary key
            entity.Property(e => e.TotalDepositOpenLast30Days).HasColumnName("Total Deposit Open Last 30 Days");
            entity.Property(e => e.TotalDepositAccountInBank).HasColumnName("Total Deposit Account In Bank");
            entity.Property(e => e.TotalDepositAmount).HasColumnName("Total Deposit Amount");
            entity.Property(e => e.OpeningPercentage).HasColumnName("Opening Percentage");
        });
    }
}
