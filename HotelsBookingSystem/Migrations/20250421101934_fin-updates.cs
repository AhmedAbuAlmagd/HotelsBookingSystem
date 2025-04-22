using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class finupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRoom_Bookings_BookingId",
                table: "BookingRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRoom_Rooms_RoomId",
                table: "BookingRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Rooms_RoomId",
                table: "CartItem");

            migrationBuilder.DropTable(
                name: "CartItemService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cart",
                table: "Cart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRoom",
                table: "BookingRoom");

            migrationBuilder.RenameTable(
                name: "CartItem",
                newName: "CartItems");

            migrationBuilder.RenameTable(
                name: "Cart",
                newName: "Carts");

            migrationBuilder.RenameTable(
                name: "BookingRoom",
                newName: "BookingRooms");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_RoomId",
                table: "CartItems",
                newName: "IX_CartItems_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_CartId",
                table: "CartItems",
                newName: "IX_CartItems_CartId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_UserId",
                table: "Carts",
                newName: "IX_Carts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRoom_BookingId",
                table: "BookingRooms",
                newName: "IX_BookingRooms_BookingId");

            migrationBuilder.AddColumn<int>(
                name: "cartItemId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRooms",
                table: "BookingRooms",
                columns: new[] { "RoomId", "BookingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Services_cartItemId",
                table: "Services",
                column: "cartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRooms_Bookings_BookingId",
                table: "BookingRooms",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRooms_Rooms_RoomId",
                table: "BookingRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Rooms_RoomId",
                table: "CartItems",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_CartItems_cartItemId",
                table: "Services",
                column: "cartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRooms_Bookings_BookingId",
                table: "BookingRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRooms_Rooms_RoomId",
                table: "BookingRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Rooms_RoomId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_CartItems_cartItemId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_cartItemId",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRooms",
                table: "BookingRooms");

            migrationBuilder.DropColumn(
                name: "cartItemId",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "Cart");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "CartItem");

            migrationBuilder.RenameTable(
                name: "BookingRooms",
                newName: "BookingRoom");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UserId",
                table: "Cart",
                newName: "IX_Cart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_RoomId",
                table: "CartItem",
                newName: "IX_CartItem_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId",
                table: "CartItem",
                newName: "IX_CartItem_CartId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRooms_BookingId",
                table: "BookingRoom",
                newName: "IX_BookingRoom_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cart",
                table: "Cart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRoom",
                table: "BookingRoom",
                columns: new[] { "RoomId", "BookingId" });

            migrationBuilder.CreateTable(
                name: "CartItemService",
                columns: table => new
                {
                    CartItemsId = table.Column<int>(type: "int", nullable: false),
                    ServicesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemService", x => new { x.CartItemsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_CartItemService_CartItem_CartItemsId",
                        column: x => x.CartItemsId,
                        principalTable: "CartItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemService_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItemService_ServicesId",
                table: "CartItemService",
                column: "ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRoom_Bookings_BookingId",
                table: "BookingRoom",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRoom_Rooms_RoomId",
                table: "BookingRoom",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Rooms_RoomId",
                table: "CartItem",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
