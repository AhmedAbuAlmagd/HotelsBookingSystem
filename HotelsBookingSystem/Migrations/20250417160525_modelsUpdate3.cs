using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class modelsUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffd3f2a1-11e4-4e29-ab68-1decad791221", "AQAAAAIAAYagAAAAEIMBIaaA00shIKLH/ded+AvsfUCKu4gUQeUn/YSQBLUL+sb8mmKcMjsAPDdE8MW28g==", "a0b1c2d3-e4f5-6789-1234-abcd1234abcd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "138e2f3f-b36c-4d54-99e4-5bf01d863e59", "AQAAAAIAAYagAAAAEIcXEa7fW3RkSr15+HmXRWR0LRMQY6kSaMu/IK341P771YaQgyp5+maxJMBK4iF3Eg==", "STATIC-GUID-HERE" });
        }
    }
}
