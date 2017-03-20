using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TravelInCloud.Migrations
{
    public partial class ChangeOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductInOrders");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductTypeId",
                table: "Orders",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ProductTypes_ProductTypeId",
                table: "Orders",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ProductTypes_ProductTypeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductTypeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "ProductInOrders",
                columns: table => new
                {
                    ProductInOrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ProductTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInOrders", x => x.ProductInOrderId);
                    table.ForeignKey(
                        name: "FK_ProductInOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductInOrders_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "ProductTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductInOrders_OrderId",
                table: "ProductInOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInOrders_ProductTypeId",
                table: "ProductInOrders",
                column: "ProductTypeId");
        }
    }
}
