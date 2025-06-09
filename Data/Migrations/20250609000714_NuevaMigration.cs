using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canela.Data.Migrations
{
    /// <inheritdoc />
    public partial class NuevaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_order",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_producto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ImageURL = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_producto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_pago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MontoTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    PayPalPaymentId = table.Column<string>(type: "TEXT", nullable: true),
                    PayPalPayerId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_pago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_pago_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_pago_t_order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "t_order",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "t_order_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrdenId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_order_detail_t_order_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "t_order",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_order_detail_t_producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "t_producto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "t_preorden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_preorden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_preorden_t_producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "t_producto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_OrdenId",
                table: "t_order_detail",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_ProductoId",
                table: "t_order_detail",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_t_pago_OrderId",
                table: "t_pago",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_pago_UserId",
                table: "t_pago",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_preorden_ProductoId",
                table: "t_preorden",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_order_detail");

            migrationBuilder.DropTable(
                name: "t_pago");

            migrationBuilder.DropTable(
                name: "t_preorden");

            migrationBuilder.DropTable(
                name: "t_order");

            migrationBuilder.DropTable(
                name: "t_producto");
        }
    }
}
