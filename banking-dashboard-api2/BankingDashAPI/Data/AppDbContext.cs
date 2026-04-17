using BankingDashAPI.Models.Entities;
using BankingDashAPI.Models.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace BankingDashAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerCountByCategory> CustomerCountByCategory { get; set; }

    public DbSet<AvailableDate> AvailableDates { get; set; }

    public DbSet<DepositOpeningSummary> DepositOpeningSummaries { get; set; }


    public DbSet<NPASummary> NPASummaries { get; set; }

    public DbSet<HCDistribution> HCDistributions { get; set; }


    public DbSet<CASASummary> CASASummaries { get; set; }


    public DbSet<GLDashboardSummary> GLDashboardSummaries { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<RolePageAccess> RolePageAccesses { get; set; }
    public DbSet<LoginAudit> LoginAudits { get; set; }

    public DbSet<UserBranchAccess> UserBranchAccesses { get; set; }
    public DbSet<UserRegionAccess> UserRegionAccesses { get; set; }

    public DbSet<PortfolioOverview> PortfolioOverviews { get; set; }
    public DbSet<InterestAndOverdueKPI> InterestAndOverdueKPIs { get; set; }

    public DbSet<DepositPortfolioOverview> DepositPortfolioOverviews { get; set; }

    public DbSet<LoanPortfolioOverview> LoanPortfolioOverviews { get; set; }

    public DbSet<DepositTrend> DepositTrends { get; set; }
    public DbSet<LoanTrend> LoanTrends { get; set; }
    public DbSet<AlmBucketRBI> AlmBucketRBIs { get; set; }

    public DbSet<DepLoanMonthlyTrend> DepLoanMonthlyTrends { get; set; }

    public DbSet<RbiLoanAuditDump> RbiLoanAuditDumps { get; set; }


    public DbSet<RbiDepositAuditDump> RbiDepositAuditDumps { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
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



        // NPASummary entity
        modelBuilder.Entity<NPASummary>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.TotalNPAOpenLast30Days).HasColumnName("Total NPA Open Last 30 Days");
            entity.Property(e => e.TotalNPAAccountInBank).HasColumnName("Total NPA Account In Bank");
            entity.Property(e => e.TotalNPAAmount).HasColumnName("Total NPA Amount");
            entity.Property(e => e.OpeningPercentage).HasColumnName("Opening Percentage");
        });

        // HCDistribution
        modelBuilder.Entity<HCDistribution>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.HC).HasColumnName("HC");
        });


        // CASASummary
        modelBuilder.Entity<CASASummary>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Deposit_Type).HasColumnName("Deposit_Type");
            entity.Property(e => e.Total_Balance).HasColumnName("Total_Balance");
            entity.Property(e => e.Cnt).HasColumnName("Cnt");
        });

        // GL Summary
        modelBuilder.Entity<GLDashboardSummary>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Total_Assets).HasColumnName("Total_Assets");
            entity.Property(e => e.Total_Liabilities).HasColumnName("Total_Liabilities");
            entity.Property(e => e.Total_Income).HasColumnName("Total_Income");
            entity.Property(e => e.Total_Expense).HasColumnName("Total_Expense");
            entity.Property(e => e.Total_Debit).HasColumnName("Total_Debit");
            entity.Property(e => e.Total_Credit).HasColumnName("Total_Credit");
            entity.Property(e => e.Net_Profit).HasColumnName("Net_Profit");
            entity.Property(e => e.Net_Position).HasColumnName("Net_Position");
        });

        // User Branch Access
        modelBuilder.Entity<UserBranchAccess>(entity =>
        {
            entity.HasKey(e => e.AccessID);
            entity.ToTable("User_Branch_Access");
            entity.Property(e => e.AccessID).HasColumnName("AccessID");
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.BranchID).HasColumnName("BranchID");
            entity.Property(e => e.AccessLevel).HasColumnName("AccessLevel");
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate");
        });

        // User Region Access
        modelBuilder.Entity<UserRegionAccess>(entity =>
        {
            entity.HasKey(e => e.AccessID);
            entity.ToTable("User_Region_Access");
            entity.Property(e => e.AccessID).HasColumnName("AccessID");
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.RegionID).HasColumnName("RegionID");
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate");
        });


        // Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role_Master");
            entity.HasKey(e => e.RoleID);
            entity.Property(e => e.RoleID).HasColumnName("RoleID").ValueGeneratedOnAdd();
            entity.Property(e => e.RoleName).HasColumnName("RoleName").HasMaxLength(50).IsRequired();
            entity.Property(e => e.RoleLevel).HasColumnName("RoleLevel");
            entity.Property(e => e.RoleDescription).HasColumnName("RoleDescription").HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETDATE()");

            // Add unique constraint
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Dashboard Page
        modelBuilder.Entity<Page>(entity =>
        {
            entity.ToTable("Dashboard_Page_Master");
            entity.HasKey(e => e.PageID);
            entity.Property(e => e.PageID).HasColumnName("PageID").ValueGeneratedOnAdd();
            entity.Property(e => e.PageName).HasColumnName("PageName").HasMaxLength(100);
            entity.Property(e => e.PageCode).HasColumnName("PageCode").HasMaxLength(50);
            entity.Property(e => e.PageCategory).HasColumnName("PageCategory").HasMaxLength(100);
            entity.Property(e => e.PageURL).HasColumnName("PageURL").HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).HasColumnName("DisplayOrder");
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETDATE()");
        });

        //User_Master
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User_Master");
            entity.HasKey(e => e.UserID);
            entity.Property(e => e.UserID).HasColumnName("UserID").ValueGeneratedOnAdd();
            entity.Property(e => e.UserLoginID).HasColumnName("UserLoginID").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EmployeeID).HasColumnName("EmployeeID").HasMaxLength(50);
            entity.Property(e => e.UserName).HasColumnName("UserName").HasMaxLength(150);
            entity.Property(e => e.EmailID).HasColumnName("EmailID").HasMaxLength(150);
            entity.Property(e => e.MobileNumber).HasColumnName("MobileNumber").HasMaxLength(20);
            entity.Property(e => e.RoleID).HasColumnName("RoleID");
            entity.Property(e => e.BranchID).HasColumnName("BranchID");
            entity.Property(e => e.RegionID).HasColumnName("RegionID");
            entity.Property(e => e.Department).HasColumnName("Department").HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(500);
            entity.Property(e => e.LastLoginDate).HasColumnName("LastLoginDate");
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate").HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.UserLoginID).IsUnique();

            // Relationship with Role
            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleID);
        });


        modelBuilder.Entity<RolePageAccess>(entity =>
        {
            entity.ToTable("Role_Page_Access");
            entity.HasKey(e => e.AccessID);
            entity.Property(e => e.AccessID).HasColumnName("AccessID").ValueGeneratedOnAdd();
            entity.Property(e => e.RoleID).HasColumnName("RoleID");
            entity.Property(e => e.PageID).HasColumnName("PageID");
            entity.Property(e => e.CanView).HasColumnName("CanView").HasDefaultValue(true);
            entity.Property(e => e.CanExport).HasColumnName("CanExport").HasDefaultValue(false);
            entity.Property(e => e.CanDrillDown).HasColumnName("CanDrillDown").HasDefaultValue(false);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETDATE()");

            // Relationships
            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleID);

            entity.HasOne<Page>()
                .WithMany()
                .HasForeignKey(e => e.PageID);
        });

        // Branch Access
        modelBuilder.Entity<UserBranchAccess>(entity =>
        {
            entity.ToTable("User_Branch_Access");
            entity.HasKey(e => e.AccessID);
            entity.Property(e => e.AccessID).HasColumnName("AccessID").ValueGeneratedOnAdd();
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.BranchID).HasColumnName("BranchID");
            entity.Property(e => e.AccessLevel).HasColumnName("AccessLevel").HasMaxLength(20);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETDATE()");

            // Relationship with User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID);
        });


        modelBuilder.Entity<LoginAudit>(entity =>
        {
            entity.ToTable("User_Login_Audit");
            entity.HasKey(e => e.LoginID);
            entity.Property(e => e.LoginID).HasColumnName("LoginID").ValueGeneratedOnAdd();
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.LoginTime).HasColumnName("LoginTime").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.LogoutTime).HasColumnName("LogoutTime");
            entity.Property(e => e.IPAddress).HasColumnName("IPAddress").HasMaxLength(50);
            entity.Property(e => e.DeviceType).HasColumnName("DeviceType").HasMaxLength(50);
            entity.Property(e => e.LoginStatus).HasColumnName("LoginStatus").HasMaxLength(20);
            entity.Property(e => e.FailedAttemptCount).HasColumnName("FailedAttemptCount").HasDefaultValue(0);

            // Relationship with User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserID);
        });


        modelBuilder.Entity<PortfolioOverview>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Total_Deposit).HasColumnName("Total_Deposit");
            entity.Property(e => e.Total_Loan).HasColumnName("Total_Loan");
            entity.Property(e => e.Net_Position).HasColumnName("Net_Position");
            entity.Property(e => e.Loan_To_Deposit_Ratio).HasColumnName("Loan_To_Deposit_Ratio");
        });

        // Configure InterestAndOverdueKPI
        modelBuilder.Entity<InterestAndOverdueKPI>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Avg_Loan_Interest_Rate).HasColumnName("Avg_Loan_Interest_Rate");
            entity.Property(e => e.Avg_Deposit_Interest_Rate).HasColumnName("Avg_Deposit_Interest_Rate");
            entity.Property(e => e.Overdue_Amount).HasColumnName("Overdue_Amount");
            entity.Property(e => e.Avg_Account_Size).HasColumnName("Avg_Account_Size");
        });

        modelBuilder.Entity<DepositPortfolioOverview>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Total_Balance).HasColumnName("Total_Balance");
            entity.Property(e => e.Total_Accounts).HasColumnName("Total_Accounts");
            entity.Property(e => e.Avg_Balance).HasColumnName("Avg_Balance");
            entity.Property(e => e.Avg_Interest_Rate).HasColumnName("Avg_Interest_Rate");
            entity.Property(e => e.Active_Accounts).HasColumnName("Active_Accounts");
            entity.Property(e => e.Dormant_Accounts).HasColumnName("Dormant_Accounts");
            entity.Property(e => e.Closed_Accounts).HasColumnName("Closed_Accounts");
            entity.Property(e => e.Avg_Account_Size).HasColumnName("Avg_Account_Size");
        });

        modelBuilder.Entity<LoanPortfolioOverview>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Total_Loan_Amount).HasColumnName("Total_Loan_Amount");
            entity.Property(e => e.Total_Outstanding).HasColumnName("Total_Outstanding");
            entity.Property(e => e.Total_Overdue).HasColumnName("Total_Overdue");
            entity.Property(e => e.Avg_Interest_Rate).HasColumnName("Avg_Interest_Rate");
            entity.Property(e => e.Total_Accounts).HasColumnName("Total_Accounts");
            entity.Property(e => e.Active_Accounts).HasColumnName("Active_Accounts");
            entity.Property(e => e.Overdue_Accounts).HasColumnName("Overdue_Accounts");
            entity.Property(e => e.Avg_Loan_Size).HasColumnName("Avg_Loan_Size");
        });


        modelBuilder.Entity<DepositTrend>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Year).HasColumnName("Year");
            entity.Property(e => e.Month).HasColumnName("Month");
            entity.Property(e => e.MonthName).HasColumnName("MonthName");
            entity.Property(e => e.TotalBalance).HasColumnName("TotalBalance");
            entity.Property(e => e.AccountCount).HasColumnName("AccountCount");
            entity.Property(e => e.AverageBalance).HasColumnName("AverageBalance");
        });

        modelBuilder.Entity<LoanTrend>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Year).HasColumnName("Year");
            entity.Property(e => e.Month).HasColumnName("Month");
            entity.Property(e => e.MonthName).HasColumnName("MonthName");
            entity.Property(e => e.TotalOutstanding).HasColumnName("TotalOutstanding");
            entity.Property(e => e.TotalSanctioned).HasColumnName("TotalSanctioned");
            entity.Property(e => e.AccountCount).HasColumnName("AccountCount");
            entity.Property(e => e.AverageLoanSize).HasColumnName("AverageLoanSize");
        });

        modelBuilder.Entity<AlmBucketRBI>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.RBI_BUCKET).HasColumnName("RBI_BUCKET");
            entity.Property(e => e.NO_OF_ACCOUNTS).HasColumnName("NO_OF_ACCOUNTS");
            entity.Property(e => e.OUTSTANDING_BALANCE).HasColumnName("OUTSTANDING_BALANCE");
            entity.Property(e => e.MATURITY_AMOUNT).HasColumnName("MATURITY_AMOUNT");
        });


        // Configure DepLoanMonthlyTrend
        modelBuilder.Entity<DepLoanMonthlyTrend>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.MONTH_END).HasColumnName("MONTH_END");
            entity.Property(e => e.DepositBal).HasColumnName("DepositBal");
            entity.Property(e => e.LoanBal).HasColumnName("LoanBal");
            entity.Property(e => e.CD_RATIO_PERCENT).HasColumnName("CD_RATIO_PERCENT");
            entity.Property(e => e.Deposit_tag).HasColumnName("Deposit_tag");
            entity.Property(e => e.Loan_tag).HasColumnName("Loan_tag");
        });



        // Configure RbiLoanAuditDump
        modelBuilder.Entity<RbiLoanAuditDump>(entity =>
        {
            entity.HasNoKey();

            // Reporting Date
            entity.Property(e => e.reporT_DATE).HasColumnName("REPORT_DATE");

            // Branch / Account
            entity.Property(e => e.brancH_ID).HasColumnName("BRANCH_ID");
            entity.Property(e => e.loaN_ACCOUNT_NO).HasColumnName("LOAN_ACCOUNT_NO");
            entity.Property(e => e.customeR_ID).HasColumnName("CUSTOMER_ID");
            entity.Property(e => e.borroweR_NAME).HasColumnName("BORROWER_NAME");

            // Customer Details
            entity.Property(e => e.customeR_TYPE).HasColumnName("CUSTOMER_TYPE");
            entity.Property(e => e.dob).HasColumnName("DOB");
            entity.Property(e => e.gender).HasColumnName("GENDER");
            entity.Property(e => e.paN_NO).HasColumnName("PAN_NO");
            entity.Property(e => e.ckyC_ID).HasColumnName("CKYC_ID");
            entity.Property(e => e.gsT_NO).HasColumnName("GST_NO");

            // Address
            entity.Property(e => e.addresS_LINE1).HasColumnName("ADDRESS_LINE1");
            entity.Property(e => e.addresS_LINE2).HasColumnName("ADDRESS_LINE2");
            entity.Property(e => e.addresS_LINE3).HasColumnName("ADDRESS_LINE3");
            entity.Property(e => e.city).HasColumnName("CITY");
            entity.Property(e => e.pincode).HasColumnName("PINCODE");
            entity.Property(e => e.mobilE_NO).HasColumnName("MOBILE_NO");
            entity.Property(e => e.emaiL_ID).HasColumnName("EMAIL_ID");

            // Loan Details
            entity.Property(e => e.loaN_PRODUCT_CODE).HasColumnName("LOAN_PRODUCT_CODE");
            entity.Property(e => e.loaN_PURPOSE).HasColumnName("LOAN_PURPOSE");
            entity.Property(e => e.prioritY_SECTOR).HasColumnName("PRIORITY_SECTOR");
            entity.Property(e => e.psL_CODE).HasColumnName("PSL_CODE");
            entity.Property(e => e.customeR_SEGMENT).HasColumnName("CUSTOMER_SEGMENT");

            // Sanction Details
            entity.Property(e => e.sanctioN_DATE).HasColumnName("SANCTION_DATE");
            entity.Property(e => e.disbursemenT_DATE).HasColumnName("DISBURSEMENT_DATE");
            entity.Property(e => e.sanctioN_AMOUNT).HasColumnName("SANCTION_AMOUNT");
            entity.Property(e => e.sanctioneD_BY).HasColumnName("SANCTIONED_BY");

            // Account Dates
            entity.Property(e => e.accounT_OPEN_DATE).HasColumnName("ACCOUNT_OPEN_DATE");
            entity.Property(e => e.maturitY_DATE).HasColumnName("MATURITY_DATE");

            // Financials
            entity.Property(e => e.outstandinG_AMOUNT).HasColumnName("OUTSTANDING_AMOUNT");
            entity.Property(e => e.interesT_RECEIVABLE).HasColumnName("INTEREST_RECEIVABLE");
            entity.Property(e => e.emI_AMOUNT).HasColumnName("EMI_AMOUNT");
            entity.Property(e => e.securitY_VALUE).HasColumnName("SECURITY_VALUE");

            // Interest
            entity.Property(e => e.roi).HasColumnName("ROI");

            // Repayment
            entity.Property(e => e.repaymenT_MODE).HasColumnName("REPAYMENT_MODE");
            entity.Property(e => e.tenure).HasColumnName("TENURE");

            // Asset Classification
            entity.Property(e => e.asseT_CLASSIFICATION).HasColumnName("ASSET_CLASSIFICATION");
            entity.Property(e => e.internaL_RATING).HasColumnName("INTERNAL_RATING");
            entity.Property(e => e.weakeR_SECTION_FLAG).HasColumnName("WEAKER_SECTION_FLAG");
            entity.Property(e => e.securitY_TYPE).HasColumnName("SECURITY_TYPE");

            // Overdue / Delinquency
            entity.Property(e => e.overduE_AMOUNT).HasColumnName("OVERDUE_AMOUNT");
            entity.Property(e => e.dayS_PAST_DUE).HasColumnName("DAYS_PAST_DUE");
            entity.Property(e => e.datE_OF_DEFAULT).HasColumnName("DATE_OF_DEFAULT");

            // Provisioning
            entity.Property(e => e.provisioN_PERCENT).HasColumnName("PROVISION_PERCENT");
            entity.Property(e => e.secureD_PROVISION).HasColumnName("SECURED_PROVISION");
            entity.Property(e => e.unsecureD_PROVISION).HasColumnName("UNSECURED_PROVISION");
            entity.Property(e => e.totaL_PROVISION).HasColumnName("TOTAL_PROVISION");

            // NPA Classification
            entity.Property(e => e.rbI_ASSET_CLASS).HasColumnName("RBI_ASSET_CLASS");

            // Tag
            entity.Property(e => e.reporT_TYPE).HasColumnName("REPORT_TYPE");
        });



        // Configure RbiDepositAuditDump
        modelBuilder.Entity<RbiDepositAuditDump>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.REPORT_DATE).HasColumnName("REPORT_DATE");
            entity.Property(e => e.BRANCH_ID).HasColumnName("BRANCH_ID");
            entity.Property(e => e.GL_CODE).HasColumnName("GL_CODE");
            entity.Property(e => e.ACCOUNT_NO).HasColumnName("ACCOUNT_NO");
            entity.Property(e => e.ACCOUNT_NAME).HasColumnName("ACCOUNT_NAME");
            entity.Property(e => e.SCHEME_CODE).HasColumnName("SCHEME_CODE");
            entity.Property(e => e.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            entity.Property(e => e.CUSTOMER_ID).HasColumnName("CUSTOMER_ID");
            entity.Property(e => e.CUSTOMER_NAME).HasColumnName("CUSTOMER_NAME");
            entity.Property(e => e.FATHER_NAME).HasColumnName("FATHER_NAME");
            entity.Property(e => e.MOTHER_NAME).HasColumnName("MOTHER_NAME");
            entity.Property(e => e.DOB).HasColumnName("DOB");
            entity.Property(e => e.GENDER).HasColumnName("GENDER");
            entity.Property(e => e.CUSTOMER_CATEGORY).HasColumnName("CUSTOMER_CATEGORY");
            entity.Property(e => e.CUSTOMER_CLASSIFICATION).HasColumnName("CUSTOMER_CLASSIFICATION");
            entity.Property(e => e.CUSTOMER_STATUS).HasColumnName("CUSTOMER_STATUS");
            entity.Property(e => e.KYC_STATUS).HasColumnName("KYC_STATUS");
            entity.Property(e => e.CKYC_NUMBER).HasColumnName("CKYC_NUMBER");
            entity.Property(e => e.PAN).HasColumnName("PAN");
            entity.Property(e => e.FATCA_STATUS).HasColumnName("FATCA_STATUS");
            entity.Property(e => e.AML_RISK_CATEGORY).HasColumnName("AML_RISK_CATEGORY");
            entity.Property(e => e.FORM15G_H_STATUS).HasColumnName("FORM15G_H_STATUS");
            entity.Property(e => e.ACCOUNT_STATUS).HasColumnName("ACCOUNT_STATUS");
            entity.Property(e => e.INOPERATIVE_FLAG).HasColumnName("INOPERATIVE_FLAG");
            entity.Property(e => e.DORMANT_STATUS).HasColumnName("DORMANT_STATUS");
            entity.Property(e => e.LIEN_STATUS).HasColumnName("LIEN_STATUS");
            entity.Property(e => e.ACCOUNT_OPEN_DATE).HasColumnName("ACCOUNT_OPEN_DATE");
            entity.Property(e => e.ACCOUNT_CLOSE_DATE).HasColumnName("ACCOUNT_CLOSE_DATE");
            entity.Property(e => e.LAST_TRANSACTION_DATE).HasColumnName("LAST_TRANSACTION_DATE");
            entity.Property(e => e.DEPOSIT_START_DATE).HasColumnName("DEPOSIT_START_DATE");
            entity.Property(e => e.MATURITY_DATE).HasColumnName("MATURITY_DATE");
            entity.Property(e => e.DEPOSIT_END_DATE).HasColumnName("DEPOSIT_END_DATE");
            entity.Property(e => e.CURRENT_BALANCE).HasColumnName("CURRENT_BALANCE");
            entity.Property(e => e.DEPOSIT_AMOUNT).HasColumnName("DEPOSIT_AMOUNT");
            entity.Property(e => e.MATURITY_AMOUNT).HasColumnName("MATURITY_AMOUNT");
            entity.Property(e => e.TOTAL_CREDITS).HasColumnName("TOTAL_CREDITS");
            entity.Property(e => e.TOTAL_DEBITS).HasColumnName("TOTAL_DEBITS");
            entity.Property(e => e.AVG_QUARTERLY_BALANCE).HasColumnName("AVG_QUARTERLY_BALANCE");
            entity.Property(e => e.INTEREST_RATE).HasColumnName("INTEREST_RATE");
            entity.Property(e => e.INTEREST_PAYOUT_MODE).HasColumnName("INTEREST_PAYOUT_MODE");
            entity.Property(e => e.TOTAL_TRANSACTIONS).HasColumnName("TOTAL_TRANSACTIONS");
            entity.Property(e => e.ADDRESS_LINE1).HasColumnName("ADDRESS_LINE1");
            entity.Property(e => e.ADDRESS_LINE2).HasColumnName("ADDRESS_LINE2");
            entity.Property(e => e.ADDRESS_LINE3).HasColumnName("ADDRESS_LINE3");
            entity.Property(e => e.CITY).HasColumnName("CITY");
            entity.Property(e => e.PIN_CODE).HasColumnName("PIN_CODE");
            entity.Property(e => e.STATE_CODE).HasColumnName("STATE_CODE");
            entity.Property(e => e.MOBILE_NO).HasColumnName("MOBILE_NO");
            entity.Property(e => e.EMAIL_ID).HasColumnName("EMAIL_ID");
            entity.Property(e => e.MEMBER_TYPE).HasColumnName("MEMBER_TYPE");
            entity.Property(e => e.MEMBER_ID).HasColumnName("MEMBER_ID");
            entity.Property(e => e.DEPOSIT_RECEIPT_NO).HasColumnName("DEPOSIT_RECEIPT_NO");
            entity.Property(e => e.DEPOSIT_TYPE_CODE).HasColumnName("DEPOSIT_TYPE_CODE");
            entity.Property(e => e.DEPOSIT_STATUS).HasColumnName("DEPOSIT_STATUS");
            entity.Property(e => e.DEPOSIT_TENURE).HasColumnName("DEPOSIT_TENURE");
            entity.Property(e => e.DEPOSIT_TYPE).HasColumnName("DEPOSIT_TYPE");
            entity.Property(e => e.DEPOSIT_SIZE_FLAG).HasColumnName("DEPOSIT_SIZE_FLAG");
            entity.Property(e => e.KYC_RISK_FLAG).HasColumnName("KYC_RISK_FLAG");
            entity.Property(e => e.AML_RISK_LEVEL).HasColumnName("AML_RISK_LEVEL");
            entity.Property(e => e.ACCOUNT_ACTIVITY_STATUS).HasColumnName("ACCOUNT_ACTIVITY_STATUS");
            entity.Property(e => e.AUDIT_REMARK).HasColumnName("AUDIT_REMARK");
        });

    }
}
