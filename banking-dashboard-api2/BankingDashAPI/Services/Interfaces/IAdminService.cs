using BankingDashAPI.Models.Entities.Admin;
using System.Collections.Generic;

namespace BankingDashAPI.Services.Interfaces
{
    public interface IAdminService
    {
        // Dashboard Stats
        DashboardStats GetDashboardStats();

        // User Management
        List<User> GetAllUsers();
        User? GetUserById(int userId);  // Made nullable
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int userId);
        bool ToggleUserStatus(int userId);

        // Role Management
        List<Role> GetAllRoles();
        Role? GetRoleById(int roleId);  // Made nullable
        bool CreateRole(Role role);
        bool UpdateRole(Role role);
        bool DeleteRole(int roleId);

        // Page Management
        List<Page> GetAllPages();
        Page? GetPageById(int pageId);  // Made nullable
        bool CreatePage(Page page);
        bool UpdatePage(Page page);
        bool DeletePage(int pageId);

        // Role Page Access
        List<RolePageAccess> GetRolePageAccess(int roleId);
        bool UpdateRolePageAccess(List<RolePageAccess> accesses);

        // User Branch Access
        List<UserBranchAccess> GetUserBranchAccess(int userId);
        bool AssignUserBranchAccess(int userId, List<int> branchIds, string accessLevel);
        bool RemoveUserBranchAccess(int userId, int branchId);

        // User Region Access
        List<UserRegionAccess> GetUserRegionAccess(int userId);
        bool AssignUserRegionAccess(int userId, List<int> regionIds);
        bool RemoveUserRegionAccess(int userId, int regionId);

        // Audit & Monitoring
        List<LoginAudit> GetRecentLogins(int count);
        List<LoginAudit> GetFailedLogins(int count);
        List<User> GetActiveUsers();
    }
}