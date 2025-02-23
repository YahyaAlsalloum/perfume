﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace perfume.Data.Migrations
{
    /// <inheritdoc />
    public partial class statustobase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BasePerfume",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BasePerfume");
        }
    }
}
