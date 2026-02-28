using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingDashAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AdminUsers",
                columns: new[] { "Id", "FullName", "PasswordHash", "UserName" },
                values: new object[] { 1, "Onkar Bhosale", "admin123", "NeurofinTech" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
