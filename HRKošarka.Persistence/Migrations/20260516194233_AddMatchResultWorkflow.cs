using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchResultWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageFouls",
                table: "PlayerSeasonStats",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "DidNotPlay",
                table: "PlayerMatchStats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "PlayerMatchStats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultSubmissionStatus",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_TeamId",
                table: "PlayerMatchStats",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerMatchStats_Teams_TeamId",
                table: "PlayerMatchStats",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMatchStats_Teams_TeamId",
                table: "PlayerMatchStats");

            migrationBuilder.DropIndex(
                name: "IX_PlayerMatchStats_TeamId",
                table: "PlayerMatchStats");

            migrationBuilder.DropColumn(
                name: "AverageFouls",
                table: "PlayerSeasonStats");

            migrationBuilder.DropColumn(
                name: "DidNotPlay",
                table: "PlayerMatchStats");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "PlayerMatchStats");

            migrationBuilder.DropColumn(
                name: "ResultSubmissionStatus",
                table: "Matches");
        }
    }
}
