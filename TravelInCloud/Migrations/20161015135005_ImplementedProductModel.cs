using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TravelInCloud.Migrations
{
    public partial class ImplementedProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageOfProduct",
                columns: table => new
                {
                    ImageOfProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageDescription = table.Column<string>(nullable: true),
                    ImageSrc = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageOfProduct", x => x.ImageOfProductId);
                    table.ForeignKey(
                        name: "FK_ImageOfProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "ProductTypes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductTypes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductTypeDetails",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductTypeName",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductDetails",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductInfo",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductWarnning",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageOfProduct_ProductId",
                table: "ImageOfProduct",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ProductTypeDetails",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ProductTypeName",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ProductDetails",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductInfo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductWarnning",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ImageOfProduct");
        }
    }
}
