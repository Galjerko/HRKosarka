using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeactivateDateToLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivateDate",
                table: "Leagues",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivateDate",
                table: "Leagues");
        }
    }
}
