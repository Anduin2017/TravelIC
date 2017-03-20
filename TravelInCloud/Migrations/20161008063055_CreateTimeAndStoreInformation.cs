using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelInCloud.Migrations
{
    public partial class CreateTimeAndStoreInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterTime",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartStoreTime",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreDescription",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreLocation",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartStoreTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreDescription",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreLocation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "AspNetUsers");
        }
    }
}
