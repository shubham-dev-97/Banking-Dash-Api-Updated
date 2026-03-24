using BankingDashAPI.Models.Entities.Admin;
using BankingDashAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BankingDashAPI.Models.DTOs.Admin;

namespace BankingDashAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController: ControllerBase
    {


        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // Dashboard Stats
        [HttpGet("dashboard-stats")]
        public IActionResult GetDashboardStats()
        {
            try
            {
                var stats = _adminService.GetDashboardStats();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // User Management
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _adminService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _adminService.GetUserById(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("users")]
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                var result = _adminService.CreateUser(user);
                if (result)
                    return Ok(new { message = "User created successfully" });
                return BadRequest(new { error = "Failed to create user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (id != user.UserID)
                    return BadRequest(new { error = "ID mismatch" });

                var result = _adminService.UpdateUser(user);
                if (result)
                    return Ok(new { message = "User updated successfully" });
                return BadRequest(new { error = "Failed to update user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var result = _adminService.DeleteUser(id);
                if (result)
                    return Ok(new { message = "User deleted successfully" });
                return BadRequest(new { error = "Failed to delete user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("users/{id}/toggle-status")]
        public IActionResult ToggleUserStatus(int id)
        {
            try
            {
                var result = _adminService.ToggleUserStatus(id);
                if (result)
                    return Ok(new { message = "User status updated successfully" });
                return BadRequest(new { error = "Failed to update user status" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Role Management
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _adminService.GetAllRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("roles/{id}")]
        public IActionResult GetRoleById(int id)
        {
            try
            {
                var role = _adminService.GetRoleById(id);
                if (role == null)
                    return NotFound();
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("roles")]
        public IActionResult CreateRole([FromBody] Role role)
        {
            try
            {
                var result = _adminService.CreateRole(role);
                if (result)
                    return Ok(new { message = "Role created successfully" });
                return BadRequest(new { error = "Failed to create role" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("roles/{id}")]
        public IActionResult UpdateRole(int id, [FromBody] Role role)
        {
            try
            {
                if (id != role.RoleID)
                    return BadRequest(new { error = "ID mismatch" });

                var result = _adminService.UpdateRole(role);
                if (result)
                    return Ok(new { message = "Role updated successfully" });
                return BadRequest(new { error = "Failed to update role" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("roles/{id}")]
        public IActionResult DeleteRole(int id)
        {
            try
            {
                var result = _adminService.DeleteRole(id);
                if (result)
                    return Ok(new { message = "Role deleted successfully" });
                return BadRequest(new { error = "Failed to delete role" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Page Management
        [HttpGet("pages")]
        public IActionResult GetAllPages()
        {
            try
            {
                var pages = _adminService.GetAllPages();
                return Ok(pages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pages");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("pages/{id}")]
        public IActionResult GetPageById(int id)
        {
            try
            {
                var page = _adminService.GetPageById(id);
                if (page == null)
                    return NotFound();
                return Ok(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting page {Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Role Page Access
        [HttpGet("role-access/{roleId}")]
        public IActionResult GetRolePageAccess(int roleId)
        {
            try
            {
                var accesses = _adminService.GetRolePageAccess(roleId);
                return Ok(accesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role page access for role {RoleId}", roleId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("role-access")]
        public IActionResult UpdateRolePageAccess([FromBody] List<RolePageAccess> accesses)
        {
            try
            {
                var result = _adminService.UpdateRolePageAccess(accesses);
                if (result)
                    return Ok(new { message = "Role page access updated successfully" });
                return BadRequest(new { error = "Failed to update role page access" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role page access");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Audit & Monitoring
        [HttpGet("recent-logins/{count}")]
        public IActionResult GetRecentLogins(int count)
        {
            try
            {
                var logins = _adminService.GetRecentLogins(count);
                return Ok(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent logins");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("failed-logins/{count}")]
        public IActionResult GetFailedLogins(int count)
        {
            try
            {
                var logins = _adminService.GetFailedLogins(count);
                return Ok(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting failed logins");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("active-users")]
        public IActionResult GetActiveUsers()
        {
            try
            {
                var users = _adminService.GetActiveUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // User Branch Access
        [HttpGet("users/{userId}/branch-access")]
        public IActionResult GetUserBranchAccess(int userId)
        {
            try
            {
                var accesses = _adminService.GetUserBranchAccess(userId);
                return Ok(accesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/branch-access")]
        public IActionResult AssignUserBranchAccess(int userId, [FromBody] BranchAccessRequest request)
        {
            try
            {
                var result = _adminService.AssignUserBranchAccess(userId, request.BranchIds, request.AccessLevel);
                if (result)
                    return Ok(new { message = "Branch access assigned successfully" });
                return BadRequest(new { error = "Failed to assign branch access" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning branch access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("users/{userId}/branch-access/{branchId}")]
        public IActionResult RemoveUserBranchAccess(int userId, int branchId)
        {
            try
            {
                var result = _adminService.RemoveUserBranchAccess(userId, branchId);
                if (result)
                    return Ok(new { message = "Branch access removed successfully" });
                return BadRequest(new { error = "Failed to remove branch access" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing branch access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // User Region Access
        [HttpGet("users/{userId}/region-access")]
        public IActionResult GetUserRegionAccess(int userId)
        {
            try
            {
                var accesses = _adminService.GetUserRegionAccess(userId);
                return Ok(accesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting region access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/region-access")]
        public IActionResult AssignUserRegionAccess(int userId, [FromBody] RegionAccessRequest request)
        {
            try
            {
                var result = _adminService.AssignUserRegionAccess(userId, request.RegionIds);
                if (result)
                    return Ok(new { message = "Region access assigned successfully" });
                return BadRequest(new { error = "Failed to assign region access" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning region access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("users/{userId}/region-access/{regionId}")]
        public IActionResult RemoveUserRegionAccess(int userId, int regionId)
        {
            try
            {
                var result = _adminService.RemoveUserRegionAccess(userId, regionId);
                if (result)
                    return Ok(new { message = "Region access removed successfully" });
                return BadRequest(new { error = "Failed to remove region access" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing region access for user {UserId}", userId);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
