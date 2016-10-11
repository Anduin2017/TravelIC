using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelInCloud.Migrations
{
    public partial class ChangeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreOwnerCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreOwnerName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreOwnerCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreOwnerName",
                table: "AspNetUsers");
        }
    }
}
