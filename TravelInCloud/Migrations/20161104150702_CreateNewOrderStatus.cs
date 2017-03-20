using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TravelInCloud.Models;

namespace TravelInCloud.Migrations
{
    public partial class CreateNewOrderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "Orders");

            migrationBuilder.AddColumn<short>(
                name: "OrderStatus",
                table: "Orders",
                nullable: false,
                defaultValue: OrderStatus.unPaid);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }
    }
}
