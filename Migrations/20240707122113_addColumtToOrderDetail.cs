﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingWeb.Migrations
{
    /// <inheritdoc />
    public partial class addColumtToOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "ShoppingCarts");
        }
    }
}
