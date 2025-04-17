using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class modelsUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "138e2f3f-b36c-4d54-99e4-5bf01d863e59", "AQAAAAIAAYagAAAAEIcXEa7fW3RkSr15+HmXRWR0LRMQY6kSaMu/IK341P771YaQgyp5+maxJMBK4iF3Eg==", "STATIC-GUID-HERE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3193e1f8-4350-48e4-808c-5586b5cc9024", "AQAAAAIAAYagAAAAEHtF44eveUrH6zIf+ZBYGtYhZ415eK380wMk6IMSfN4qGw7Klyu0hwLGSyPt/BsZVA==", "0c80b3af-9bcc-4492-99ec-458ad6b9782e" });
        }
    }
}
