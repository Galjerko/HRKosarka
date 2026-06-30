using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayoffBracket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlayoffSeriesId",
                table: "Matches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPlayoff",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlayoffEndDate",
                table: "Leagues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlayoffGenerated",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlayoffHas3rdPlace",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PlayoffRoundWinsNeeded",
                table: "Leagues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayoffTeamCount",
                table: "Leagues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayoffSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    SeriesNumber = table.Column<int>(type: "int", nullable: false),
                    WinsNeeded = table.Column<int>(type: "int", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AwayTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HomeSeedNumber = table.Column<int>(type: "int", nullable: true),
                    AwaySeedNumber = table.Column<int>(type: "int", nullable: true),
                    WinnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    HomeFeederSeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AwayFeederSeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayoffSeries", x => x.Id);
                    table.CheckConstraint("CK_PlayoffSeries_WinsNeeded", "WinsNeeded BETWEEN 2 AND 4");
                    table.ForeignKey(
                        name: "FK_PlayoffSeries_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayoffSeries_PlayoffSeries_AwayFeederSeriesId",
                        column: x => x.AwayFeederSeriesId,
                        principalTable: "PlayoffSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayoffSeries_PlayoffSeries_HomeFeederSeriesId",
                        column: x => x.HomeFeederSeriesId,
                        principalTable: "PlayoffSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayoffSeries_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayoffSeries_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_PlayoffSeriesId",
                table: "Matches",
                column: "PlayoffSeriesId",
                filter: "[PlayoffSeriesId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_AwayFeederSeriesId",
                table: "PlayoffSeries",
                column: "AwayFeederSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_AwayTeamId",
                table: "PlayoffSeries",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_HomeFeederSeriesId",
                table: "PlayoffSeries",
                column: "HomeFeederSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_HomeTeamId",
                table: "PlayoffSeries",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_LeagueId",
                table: "PlayoffSeries",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_LeagueId_RoundNumber",
                table: "PlayoffSeries",
                columns: new[] { "LeagueId", "RoundNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffSeries_LeagueId_RoundNumber_SeriesNumber",
                table: "PlayoffSeries",
                columns: new[] { "LeagueId", "RoundNumber", "SeriesNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_PlayoffSeries_PlayoffSeriesId",
                table: "Matches",
                column: "PlayoffSeriesId",
                principalTable: "PlayoffSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_PlayoffSeries_PlayoffSeriesId",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "PlayoffSeries");

            migrationBuilder.DropIndex(
                name: "IX_Match_PlayoffSeriesId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PlayoffSeriesId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HasPlayoff",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PlayoffEndDate",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PlayoffGenerated",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PlayoffHas3rdPlace",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PlayoffRoundWinsNeeded",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PlayoffTeamCount",
                table: "Leagues");
        }
    }
}
