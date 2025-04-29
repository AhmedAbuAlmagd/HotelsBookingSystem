using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_CartItems_cartItemId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "cartItemId",
                table: "Services",
                newName: "CartItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_cartItemId",
                table: "Services",
                newName: "IX_Services_CartItemId");

            migrationBuilder.CreateTable(
                name: "SelectedServices",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedServices", x => new { x.CartId, x.ServiceID });
                    table.ForeignKey(
                        name: "FK_SelectedServices_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedServices_Services_ServiceID",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedServices_ServiceID",
                table: "SelectedServices",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_CartItems_CartItemId",
                table: "Services",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_CartItems_CartItemId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "SelectedServices");

            migrationBuilder.RenameColumn(
                name: "CartItemId",
                table: "Services",
                newName: "cartItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_CartItemId",
                table: "Services",
                newName: "IX_Services_cartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_CartItems_cartItemId",
                table: "Services",
                column: "cartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }
    }
}
