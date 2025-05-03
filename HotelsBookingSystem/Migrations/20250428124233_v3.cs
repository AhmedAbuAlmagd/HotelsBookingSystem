using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_CartItems_CartItemId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_CartItemId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_CartItemId",
                table: "Services",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_CartItems_CartItemId",
                table: "Services",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }
    }
}
