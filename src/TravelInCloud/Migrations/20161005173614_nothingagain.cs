using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelInCloud.Migrations
{
    public partial class nothingagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_OwnerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInOrder_Order_OrderId",
                table: "ProductInOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInOrder_ProductType_ProductTypeId",
                table: "ProductInOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductType_Products_BelongingProductId",
                table: "ProductType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductType",
                table: "ProductType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductInOrder",
                table: "ProductInOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTypes",
                table: "ProductType",
                column: "ProductTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductInOrders",
                table: "ProductInOrder",
                column: "ProductInOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Order",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_OwnerId",
                table: "Order",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInOrders_Orders_OrderId",
                table: "ProductInOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInOrders_ProductTypes_ProductTypeId",
                table: "ProductInOrder",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_Products_BelongingProductId",
                table: "ProductType",
                column: "BelongingProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                name: "IX_ProductType_BelongingProductId",
                table: "ProductType",
                newName: "IX_ProductTypes_BelongingProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInOrder_ProductTypeId",
                table: "ProductInOrder",
                newName: "IX_ProductInOrders_ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInOrder_OrderId",
                table: "ProductInOrder",
                newName: "IX_ProductInOrders_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_OwnerId",
                table: "Order",
                newName: "IX_Orders_OwnerId");

            migrationBuilder.RenameTable(
                name: "ProductType",
                newName: "ProductTypes");

            migrationBuilder.RenameTable(
                name: "ProductInOrder",
                newName: "ProductInOrders");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_OwnerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInOrders_Orders_OrderId",
                table: "ProductInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInOrders_ProductTypes_ProductTypeId",
                table: "ProductInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_Products_BelongingProductId",
                table: "ProductTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTypes",
                table: "ProductTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductInOrders",
                table: "ProductInOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductType",
                table: "ProductTypes",
                column: "ProductTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductInOrder",
                table: "ProductInOrders",
                column: "ProductInOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Orders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_OwnerId",
                table: "Orders",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInOrder_Order_OrderId",
                table: "ProductInOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInOrder_ProductType_ProductTypeId",
                table: "ProductInOrders",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductType_Products_BelongingProductId",
                table: "ProductTypes",
                column: "BelongingProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                name: "IX_ProductTypes_BelongingProductId",
                table: "ProductTypes",
                newName: "IX_ProductType_BelongingProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInOrders_ProductTypeId",
                table: "ProductInOrders",
                newName: "IX_ProductInOrder_ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductInOrders_OrderId",
                table: "ProductInOrders",
                newName: "IX_ProductInOrder_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OwnerId",
                table: "Orders",
                newName: "IX_Order_OwnerId");

            migrationBuilder.RenameTable(
                name: "ProductTypes",
                newName: "ProductType");

            migrationBuilder.RenameTable(
                name: "ProductInOrders",
                newName: "ProductInOrder");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");
        }
    }
}
