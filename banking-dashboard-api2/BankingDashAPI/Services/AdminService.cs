using BankingDashAPI.Data;
using BankingDashAPI.Models.Entities.Admin;
using BankingDashAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BankingDashAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(AppDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Dashboard Stats

        public DashboardStats GetDashboardStats()
        {
            try
            {
                _logger.LogInformation("Getting admin dashboard stats");

                var stats = new DashboardStats();

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();

                    // Total Users
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM User_Master", connection))
                    {
                        stats.TotalUsers = (int)cmd.ExecuteScalar();
                    }

                    // Active Users
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM User_Master WHERE IsActive = 1", connection))
                    {
                        stats.ActiveUsers = (int)cmd.ExecuteScalar();
                    }

                    // Total Roles
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Role_Master", connection))
                    {
                        stats.TotalRoles = (int)cmd.ExecuteScalar();
                    }

                    // Total Pages
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Dashboard_Page_Master WHERE IsActive = 1", connection))
                    {
                        stats.TotalPages = (int)cmd.ExecuteScalar();
                    }

                    // Today's Logins
                    using (var cmd = new SqlCommand(@"
                        SELECT COUNT(*) FROM User_Login_Audit 
                        WHERE CAST(LoginTime AS DATE) = CAST(GETDATE() AS DATE)
                        AND LoginStatus = 'SUCCESS'", connection))
                    {
                        stats.TodayLogins = (int)cmd.ExecuteScalar();
                    }

                    // Failed Logins Today
                    using (var cmd = new SqlCommand(@"
                        SELECT COUNT(*) FROM User_Login_Audit 
                        WHERE CAST(LoginTime AS DATE) = CAST(GETDATE() AS DATE)
                        AND LoginStatus = 'FAILED'", connection))
                    {
                        stats.FailedLogins = (int)cmd.ExecuteScalar();
                    }

                    // Active Sessions (users logged in today)
                    using (var cmd = new SqlCommand(@"
                        SELECT COUNT(DISTINCT UserID) FROM User_Login_Audit 
                        WHERE CAST(LoginTime AS DATE) = CAST(GETDATE() AS DATE)", connection))
                    {
                        stats.ActiveSessions = (int)cmd.ExecuteScalar();
                    }
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return new DashboardStats();
            }
        }

        #endregion

        #region User Management

        public List<User> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Getting all users");

                var users = new List<User>();
                var query = @"
                    SELECT u.*, r.RoleName 
                    FROM User_Master u
                    LEFT JOIN Role_Master r ON u.RoleID = r.RoleID
                    ORDER BY u.UserID";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserLoginID = reader.GetString(reader.GetOrdinal("UserLoginID")),
                                EmployeeID = reader.GetString(reader.GetOrdinal("EmployeeID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                EmailID = reader.GetString(reader.GetOrdinal("EmailID")),
                                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNumber")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                BranchID = reader.IsDBNull(reader.GetOrdinal("BranchID")) ? null : reader.GetInt32(reader.GetOrdinal("BranchID")),
                                RegionID = reader.IsDBNull(reader.GetOrdinal("RegionID")) ? null : reader.GetInt32(reader.GetOrdinal("RegionID")),
                                Department = reader.GetString(reader.GetOrdinal("Department")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? string.Empty : reader.GetString(reader.GetOrdinal("PasswordHash")),
                                LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginDate")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
                            });
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return new List<User>();
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                _logger.LogInformation("Getting user by ID: {UserId}", userId);

                User user = null;
                var query = @"
                    SELECT u.*, r.RoleName 
                    FROM User_Master u
                    LEFT JOIN Role_Master r ON u.RoleID = r.RoleID
                    WHERE u.UserID = @UserId";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserLoginID = reader.GetString(reader.GetOrdinal("UserLoginID")),
                                EmployeeID = reader.GetString(reader.GetOrdinal("EmployeeID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                EmailID = reader.GetString(reader.GetOrdinal("EmailID")),
                                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNumber")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                BranchID = reader.IsDBNull(reader.GetOrdinal("BranchID")) ? null : reader.GetInt32(reader.GetOrdinal("BranchID")),
                                RegionID = reader.IsDBNull(reader.GetOrdinal("RegionID")) ? null : reader.GetInt32(reader.GetOrdinal("RegionID")),
                                Department = reader.GetString(reader.GetOrdinal("Department")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? string.Empty : reader.GetString(reader.GetOrdinal("PasswordHash")),
                                LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginDate")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
                            };
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                return null;
            }
        }

        public bool CreateUser(User user)
        {
            try
            {
                _logger.LogInformation("Creating new user: {UserLoginId}", user.UserLoginID);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        INSERT INTO User_Master 
                        (UserLoginID, EmployeeID, UserName, EmailID, MobileNumber, RoleID, BranchID, RegionID, Department, IsActive, PasswordHash, CreatedDate, UpdatedDate)
                        VALUES 
                        (@UserLoginID, @EmployeeID, @UserName, @EmailID, @MobileNumber, @RoleID, @BranchID, @RegionID, @Department, @IsActive, @PasswordHash, GETDATE(), GETDATE())";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserLoginID", user.UserLoginID);
                        cmd.Parameters.AddWithValue("@EmployeeID", user.EmployeeID);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                        cmd.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                        cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
                        cmd.Parameters.AddWithValue("@BranchID", user.BranchID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegionID", user.RegionID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department", user.Department);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                _logger.LogInformation("Updating user: {UserId}", user.UserID);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        UPDATE User_Master 
                        SET UserLoginID = @UserLoginID,
                            EmployeeID = @EmployeeID,
                            UserName = @UserName,
                            EmailID = @EmailID,
                            MobileNumber = @MobileNumber,
                            RoleID = @RoleID,
                            BranchID = @BranchID,
                            RegionID = @RegionID,
                            Department = @Department,
                            IsActive = @IsActive,
                            UpdatedDate = GETDATE()
                        WHERE UserID = @UserID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", user.UserID);
                        cmd.Parameters.AddWithValue("@UserLoginID", user.UserLoginID);
                        cmd.Parameters.AddWithValue("@EmployeeID", user.EmployeeID);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                        cmd.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                        cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
                        cmd.Parameters.AddWithValue("@BranchID", user.BranchID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegionID", user.RegionID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department", user.Department);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", user.UserID);
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                _logger.LogInformation("Deleting user: {UserId}", userId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand("DELETE FROM User_Master WHERE UserID = @UserId", connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return false;
            }
        }

        public bool ToggleUserStatus(int userId)
        {
            try
            {
                _logger.LogInformation("Toggling user status: {UserId}", userId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = "UPDATE User_Master SET IsActive = ~IsActive WHERE UserID = @UserId";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Role Management

        public List<Role> GetAllRoles()
        {
            try
            {
                _logger.LogInformation("Getting all roles with user counts");

                var roles = new List<Role>();
                var query = @"
                    SELECT r.*, COUNT(u.UserID) as UserCount
                    FROM Role_Master r
                    LEFT JOIN User_Master u ON r.RoleID = u.RoleID
                    GROUP BY r.RoleID, r.RoleName, r.RoleLevel, r.RoleDescription, r.CreatedDate
                    ORDER BY r.RoleLevel";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                RoleLevel = reader.GetInt32(reader.GetOrdinal("RoleLevel")),
                                RoleDescription = reader.GetString(reader.GetOrdinal("RoleDescription")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                UserCount = reader.GetInt32(reader.GetOrdinal("UserCount"))
                            });
                        }
                    }
                }

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                return new List<Role>();
            }
        }

        public Role GetRoleById(int roleId)
        {
            try
            {
                _logger.LogInformation("Getting role by ID: {RoleId}", roleId);

                Role role = null;
                var query = @"
                    SELECT r.*, COUNT(u.UserID) as UserCount
                    FROM Role_Master r
                    LEFT JOIN User_Master u ON r.RoleID = u.RoleID
                    WHERE r.RoleID = @RoleId
                    GROUP BY r.RoleID, r.RoleName, r.RoleLevel, r.RoleDescription, r.CreatedDate";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = new Role
                            {
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                RoleLevel = reader.GetInt32(reader.GetOrdinal("RoleLevel")),
                                RoleDescription = reader.GetString(reader.GetOrdinal("RoleDescription")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                UserCount = reader.GetInt32(reader.GetOrdinal("UserCount"))
                            };
                        }
                    }
                }

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {RoleId}", roleId);
                return null;
            }
        }

        public bool CreateRole(Role role)
        {
            try
            {
                _logger.LogInformation("Creating new role: {RoleName}", role.RoleName);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        INSERT INTO Role_Master (RoleName, RoleLevel, RoleDescription, CreatedDate)
                        VALUES (@RoleName, @RoleLevel, @RoleDescription, GETDATE())";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                        cmd.Parameters.AddWithValue("@RoleLevel", role.RoleLevel);
                        cmd.Parameters.AddWithValue("@RoleDescription", role.RoleDescription);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return false;
            }
        }

        public bool UpdateRole(Role role)
        {
            try
            {
                _logger.LogInformation("Updating role: {RoleId}", role.RoleID);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        UPDATE Role_Master 
                        SET RoleName = @RoleName,
                            RoleLevel = @RoleLevel,
                            RoleDescription = @RoleDescription
                        WHERE RoleID = @RoleID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleID", role.RoleID);
                        cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                        cmd.Parameters.AddWithValue("@RoleLevel", role.RoleLevel);
                        cmd.Parameters.AddWithValue("@RoleDescription", role.RoleDescription);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", role.RoleID);
                return false;
            }
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                _logger.LogInformation("Deleting role: {RoleId}", roleId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand("DELETE FROM Role_Master WHERE RoleID = @RoleId", connection))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
                return false;
            }
        }

        #endregion

        #region Page Management

        public List<Page> GetAllPages()
        {
            try
            {
                _logger.LogInformation("Getting all pages");

                var pages = new List<Page>();
                var query = "SELECT * FROM Dashboard_Page_Master WHERE IsActive = 1 ORDER BY DisplayOrder";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pages.Add(new Page
                            {
                                PageID = reader.GetInt32(reader.GetOrdinal("PageID")),
                                PageName = reader.GetString(reader.GetOrdinal("PageName")),
                                PageCode = reader.GetString(reader.GetOrdinal("PageCode")),
                                PageCategory = reader.GetString(reader.GetOrdinal("PageCategory")),
                                PageURL = reader.GetString(reader.GetOrdinal("PageURL")),
                                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }

                return pages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pages");
                return new List<Page>();
            }
        }

        public Page GetPageById(int pageId)
        {
            try
            {
                _logger.LogInformation("Getting page by ID: {PageId}", pageId);

                Page page = null;
                var query = "SELECT * FROM Dashboard_Page_Master WHERE PageID = @PageId";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PageId", pageId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            page = new Page
                            {
                                PageID = reader.GetInt32(reader.GetOrdinal("PageID")),
                                PageName = reader.GetString(reader.GetOrdinal("PageName")),
                                PageCode = reader.GetString(reader.GetOrdinal("PageCode")),
                                PageCategory = reader.GetString(reader.GetOrdinal("PageCategory")),
                                PageURL = reader.GetString(reader.GetOrdinal("PageURL")),
                                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            };
                        }
                    }
                }

                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting page {PageId}", pageId);
                return null;
            }
        }

        public bool CreatePage(Page page)
        {
            try
            {
                _logger.LogInformation("Creating new page: {PageName}", page.PageName);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        INSERT INTO Dashboard_Page_Master 
                        (PageName, PageCode, PageCategory, PageURL, DisplayOrder, IsActive, CreatedDate)
                        VALUES 
                        (@PageName, @PageCode, @PageCategory, @PageURL, @DisplayOrder, @IsActive, GETDATE())";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PageName", page.PageName);
                        cmd.Parameters.AddWithValue("@PageCode", page.PageCode);
                        cmd.Parameters.AddWithValue("@PageCategory", page.PageCategory);
                        cmd.Parameters.AddWithValue("@PageURL", page.PageURL);
                        cmd.Parameters.AddWithValue("@DisplayOrder", page.DisplayOrder);
                        cmd.Parameters.AddWithValue("@IsActive", page.IsActive);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating page");
                return false;
            }
        }

        public bool UpdatePage(Page page)
        {
            try
            {
                _logger.LogInformation("Updating page: {PageId}", page.PageID);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var query = @"
                        UPDATE Dashboard_Page_Master 
                        SET PageName = @PageName,
                            PageCode = @PageCode,
                            PageCategory = @PageCategory,
                            PageURL = @PageURL,
                            DisplayOrder = @DisplayOrder,
                            IsActive = @IsActive
                        WHERE PageID = @PageID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PageID", page.PageID);
                        cmd.Parameters.AddWithValue("@PageName", page.PageName);
                        cmd.Parameters.AddWithValue("@PageCode", page.PageCode);
                        cmd.Parameters.AddWithValue("@PageCategory", page.PageCategory);
                        cmd.Parameters.AddWithValue("@PageURL", page.PageURL);
                        cmd.Parameters.AddWithValue("@DisplayOrder", page.DisplayOrder);
                        cmd.Parameters.AddWithValue("@IsActive", page.IsActive);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating page {PageId}", page.PageID);
                return false;
            }
        }

        public bool DeletePage(int pageId)
        {
            try
            {
                _logger.LogInformation("Deleting page: {PageId}", pageId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand("DELETE FROM Dashboard_Page_Master WHERE PageID = @PageId", connection))
                {
                    cmd.Parameters.AddWithValue("@PageId", pageId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting page {PageId}", pageId);
                return false;
            }
        }

        #endregion

        #region Role Page Access

        public List<RolePageAccess> GetRolePageAccess(int roleId)
        {
            try
            {
                _logger.LogInformation("Getting page access for role: {RoleId}", roleId);

                var accesses = new List<RolePageAccess>();
                var query = @"
                    SELECT rpa.*, r.RoleName, p.PageName
                    FROM Role_Page_Access rpa
                    INNER JOIN Role_Master r ON rpa.RoleID = r.RoleID
                    INNER JOIN Dashboard_Page_Master p ON rpa.PageID = p.PageID
                    WHERE rpa.RoleID = @RoleId
                    ORDER BY p.DisplayOrder";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accesses.Add(new RolePageAccess
                            {
                                AccessID = reader.GetInt32(reader.GetOrdinal("AccessID")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                PageID = reader.GetInt32(reader.GetOrdinal("PageID")),
                                PageName = reader.GetString(reader.GetOrdinal("PageName")),
                                CanView = reader.GetBoolean(reader.GetOrdinal("CanView")),
                                CanExport = reader.GetBoolean(reader.GetOrdinal("CanExport")),
                                CanDrillDown = reader.GetBoolean(reader.GetOrdinal("CanDrillDown")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }

                return accesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role page access for role {RoleId}", roleId);
                return new List<RolePageAccess>();
            }
        }

        public bool UpdateRolePageAccess(List<RolePageAccess> accesses)
        {
            try
            {
                if (accesses == null || accesses.Count == 0)
                    return false;

                int roleId = accesses[0].RoleID;
                _logger.LogInformation("Updating page access for role: {RoleId}", roleId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();

                    // Delete existing access for this role
                    using (var cmd = new SqlCommand("DELETE FROM Role_Page_Access WHERE RoleID = @RoleId", connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert new access records
                    foreach (var access in accesses)
                    {
                        using (var cmd = new SqlCommand(@"
                            INSERT INTO Role_Page_Access (RoleID, PageID, CanView, CanExport, CanDrillDown, CreatedDate)
                            VALUES (@RoleID, @PageID, @CanView, @CanExport, @CanDrillDown, GETDATE())", connection))
                        {
                            cmd.Parameters.AddWithValue("@RoleID", access.RoleID);
                            cmd.Parameters.AddWithValue("@PageID", access.PageID);
                            cmd.Parameters.AddWithValue("@CanView", access.CanView);
                            cmd.Parameters.AddWithValue("@CanExport", access.CanExport);
                            cmd.Parameters.AddWithValue("@CanDrillDown", access.CanDrillDown);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role page access");
                return false;
            }
        }

        #endregion

        #region User Branch Access

        public List<UserBranchAccess> GetUserBranchAccess(int userId)
        {
            try
            {
                _logger.LogInformation("Getting branch access for user: {UserId}", userId);

                var accesses = new List<UserBranchAccess>();
                var query = @"
                    SELECT uba.*, u.UserName 
                    FROM User_Branch_Access uba
                    INNER JOIN User_Master u ON uba.UserID = u.UserID
                    WHERE uba.UserID = @UserId
                    ORDER BY uba.BranchID";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accesses.Add(new UserBranchAccess
                            {
                                AccessID = reader.GetInt32(reader.GetOrdinal("AccessID")),
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                BranchID = reader.GetInt32(reader.GetOrdinal("BranchID")),
                                AccessLevel = reader.GetString(reader.GetOrdinal("AccessLevel")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }

                return accesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user branch access for user {UserId}", userId);
                return new List<UserBranchAccess>();
            }
        }

        public bool AssignUserBranchAccess(int userId, List<int> branchIds, string accessLevel)
        {
            try
            {
                _logger.LogInformation("Assigning branch access for user: {UserId}", userId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();

                    // First remove existing access
                    using (var cmd = new SqlCommand("DELETE FROM User_Branch_Access WHERE UserID = @UserId", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.ExecuteNonQuery();
                    }

                    // Then insert new access
                    foreach (var branchId in branchIds)
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO User_Branch_Access (UserID, BranchID, AccessLevel, CreatedDate) 
                              VALUES (@UserId, @BranchId, @AccessLevel, GETDATE())", connection))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@BranchId", branchId);
                            cmd.Parameters.AddWithValue("@AccessLevel", accessLevel);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning branch access for user {UserId}", userId);
                return false;
            }
        }

        public bool RemoveUserBranchAccess(int userId, int branchId)
        {
            try
            {
                _logger.LogInformation("Removing branch access for user: {UserId}, branch: {BranchId}", userId, branchId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(
                    "DELETE FROM User_Branch_Access WHERE UserID = @UserId AND BranchID = @BranchId", connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@BranchId", branchId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing branch access for user {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region User Region Access

        public List<UserRegionAccess> GetUserRegionAccess(int userId)
        {
            try
            {
                _logger.LogInformation("Getting region access for user: {UserId}", userId);

                var accesses = new List<UserRegionAccess>();
                var query = @"
                    SELECT ura.*, u.UserName 
                    FROM User_Region_Access ura
                    INNER JOIN User_Master u ON ura.UserID = u.UserID
                    WHERE ura.UserID = @UserId
                    ORDER BY ura.RegionID";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accesses.Add(new UserRegionAccess
                            {
                                AccessID = reader.GetInt32(reader.GetOrdinal("AccessID")),
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                RegionID = reader.GetInt32(reader.GetOrdinal("RegionID")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }

                return accesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user region access for user {UserId}", userId);
                return new List<UserRegionAccess>();
            }
        }

        public bool AssignUserRegionAccess(int userId, List<int> regionIds)
        {
            try
            {
                _logger.LogInformation("Assigning region access for user: {UserId}", userId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();

                    // First remove existing access
                    using (var cmd = new SqlCommand("DELETE FROM User_Region_Access WHERE UserID = @UserId", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.ExecuteNonQuery();
                    }

                    // Then insert new access
                    foreach (var regionId in regionIds)
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO User_Region_Access (UserID, RegionID, CreatedDate) 
                              VALUES (@UserId, @RegionId, GETDATE())", connection))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@RegionId", regionId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning region access for user {UserId}", userId);
                return false;
            }
        }

        public bool RemoveUserRegionAccess(int userId, int regionId)
        {
            try
            {
                _logger.LogInformation("Removing region access for user: {UserId}, region: {RegionId}", userId, regionId);

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(
                    "DELETE FROM User_Region_Access WHERE UserID = @UserId AND RegionID = @RegionId", connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@RegionId", regionId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing region access for user {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Audit & Monitoring

        public List<LoginAudit> GetRecentLogins(int count)
        {
            try
            {
                _logger.LogInformation("Getting recent {Count} logins", count);

                var logins = new List<LoginAudit>();
                var query = @"
                    SELECT TOP (@Count) l.*, u.UserName
                    FROM User_Login_Audit l
                    INNER JOIN User_Master u ON l.UserID = u.UserID
                    ORDER BY l.LoginTime DESC";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logins.Add(new LoginAudit
                            {
                                LoginID = reader.GetInt64(reader.GetOrdinal("LoginID")),
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                LoginTime = reader.GetDateTime(reader.GetOrdinal("LoginTime")),
                                LogoutTime = reader.IsDBNull(reader.GetOrdinal("LogoutTime")) ? null : reader.GetDateTime(reader.GetOrdinal("LogoutTime")),
                                IPAddress = reader.GetString(reader.GetOrdinal("IPAddress")),
                                DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                                LoginStatus = reader.GetString(reader.GetOrdinal("LoginStatus")),
                                FailedAttemptCount = reader.GetInt32(reader.GetOrdinal("FailedAttemptCount"))
                            });
                        }
                    }
                }

                return logins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent logins");
                return new List<LoginAudit>();
            }
        }

        public List<LoginAudit> GetFailedLogins(int count)
        {
            try
            {
                _logger.LogInformation("Getting recent failed logins");

                var logins = new List<LoginAudit>();
                var query = @"
                    SELECT TOP (@Count) l.*, u.UserName
                    FROM User_Login_Audit l
                    INNER JOIN User_Master u ON l.UserID = u.UserID
                    WHERE l.LoginStatus = 'FAILED'
                    ORDER BY l.LoginTime DESC";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logins.Add(new LoginAudit
                            {
                                LoginID = reader.GetInt64(reader.GetOrdinal("LoginID")),
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                LoginTime = reader.GetDateTime(reader.GetOrdinal("LoginTime")),
                                LogoutTime = reader.IsDBNull(reader.GetOrdinal("LogoutTime")) ? null : reader.GetDateTime(reader.GetOrdinal("LogoutTime")),
                                IPAddress = reader.GetString(reader.GetOrdinal("IPAddress")),
                                DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                                LoginStatus = reader.GetString(reader.GetOrdinal("LoginStatus")),
                                FailedAttemptCount = reader.GetInt32(reader.GetOrdinal("FailedAttemptCount"))
                            });
                        }
                    }
                }

                return logins;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting failed logins");
                return new List<LoginAudit>();
            }
        }

        public List<User> GetActiveUsers()
        {
            try
            {
                _logger.LogInformation("Getting active users");

                var users = new List<User>();
                var query = @"
                    SELECT DISTINCT u.*, r.RoleName
                    FROM User_Master u
                    INNER JOIN Role_Master r ON u.RoleID = r.RoleID
                    INNER JOIN User_Login_Audit l ON u.UserID = l.UserID
                    WHERE CAST(l.LoginTime AS DATE) = CAST(GETDATE() AS DATE)
                    AND u.IsActive = 1
                    ORDER BY u.UserName";

                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                using (var cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserLoginID = reader.GetString(reader.GetOrdinal("UserLoginID")),
                                EmployeeID = reader.GetString(reader.GetOrdinal("EmployeeID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                EmailID = reader.GetString(reader.GetOrdinal("EmailID")),
                                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNumber")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                BranchID = reader.IsDBNull(reader.GetOrdinal("BranchID")) ? null : reader.GetInt32(reader.GetOrdinal("BranchID")),
                                RegionID = reader.IsDBNull(reader.GetOrdinal("RegionID")) ? null : reader.GetInt32(reader.GetOrdinal("RegionID")),
                                Department = reader.GetString(reader.GetOrdinal("Department")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginDate")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
                            });
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                return new List<User>();
            }
        }

        #endregion
    }
}