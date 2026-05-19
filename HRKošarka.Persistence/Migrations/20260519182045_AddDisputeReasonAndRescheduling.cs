using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDisputeReasonAndRescheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestedByClubId",
                table: "MatchReschedulingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DisputeReason",
                table: "Matches",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedByClubId",
                table: "MatchReschedulingRequests");

            migrationBuilder.DropColumn(
                name: "DisputeReason",
                table: "Matches");
        }
    }
}
