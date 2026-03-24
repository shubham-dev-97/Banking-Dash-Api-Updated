using BankingDashAPI.Models.Auth;
using BankingDashAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingDashAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly string _connectionString;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {UserLoginID}", request.UserLoginID);

                var user = await GetUserByLoginIdAsync(request.UserLoginID);

                if (user == null)
                {
                    await LogLoginAttempt(null, request.UserLoginID, "FAILED", "User not found");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // TODO: Implement proper password verification
                // For now, we'll accept any password (temporary)
                bool isValidPassword = true;

                if (!isValidPassword)
                {
                    await LogLoginAttempt(user.UserID, request.UserLoginID, "FAILED", "Invalid password");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                await UpdateLastLoginDate(user.UserID);
                var token = GenerateJwtToken(user);

                await LogLoginAttempt(user.UserID, request.UserLoginID, "SUCCESS", null);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<double>("Jwt:DurationInMinutes", 60)),
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {UserLoginID}", request.UserLoginID);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        private async Task<UserInfo?> GetUserByLoginIdAsync(string loginId)
        {
            try
            {
                var query = @"
                    SELECT u.UserID, u.UserLoginID, u.UserName, u.EmailID, 
                           u.RoleID, r.RoleName, u.BranchID, u.RegionID, u.Department,
                           u.PasswordHash
                    FROM User_Master u
                    LEFT JOIN Role_Master r ON u.RoleID = r.RoleID
                    WHERE u.UserLoginID = @LoginId AND u.IsActive = 1";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", loginId);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserInfo
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserLoginID = reader.GetString(reader.GetOrdinal("UserLoginID")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                EmailID = reader.GetString(reader.GetOrdinal("EmailID")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                BranchID = reader.IsDBNull(reader.GetOrdinal("BranchID")) ? null : reader.GetInt32(reader.GetOrdinal("BranchID")),
                                RegionID = reader.IsDBNull(reader.GetOrdinal("RegionID")) ? null : reader.GetInt32(reader.GetOrdinal("RegionID")),
                                Department = reader.GetString(reader.GetOrdinal("Department"))
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by login ID: {LoginId}", loginId);
                return null;
            }
        }

        private async Task UpdateLastLoginDate(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(
                    "UPDATE User_Master SET LastLoginDate = GETDATE() WHERE UserID = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login date for user: {UserId}", userId);
            }
        }

        private async Task LogLoginAttempt(int? userId, string loginId, string status, string? message)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(@"
                    INSERT INTO User_Login_Audit (UserID, LoginTime, IPAddress, DeviceType, LoginStatus, FailedAttemptCount)
                    VALUES (@UserId, GETDATE(), @IPAddress, @DeviceType, @Status, @FailedCount)", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IPAddress", "127.0.0.1");
                    command.Parameters.AddWithValue("@DeviceType", "Web");
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@FailedCount", status == "FAILED" ? 1 : 0);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login attempt");
            }
        }

        public string GenerateJwtToken(UserInfo user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ??
                "YourSuperSecretKeyForJWTTokenGenerationThatIsAtLeast32CharsLong");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserLoginID),
                new Claim(ClaimTypes.Email, user.EmailID),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim("Department", user.Department),
                new Claim("BranchID", user.BranchID?.ToString() ?? ""),
                new Claim("RegionID", user.RegionID?.ToString() ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("Jwt:DurationInMinutes", 60)),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}