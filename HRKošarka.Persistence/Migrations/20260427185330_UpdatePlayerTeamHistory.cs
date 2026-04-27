using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlayerTeamHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamHistory_TeamId",
                table: "PlayerTeamHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamHistory_Unique",
                table: "PlayerTeamHistory");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_ActiveTeamJerseyNumber",
                table: "PlayerTeamHistory",
                columns: new[] { "TeamId", "JerseyNumber" },
                unique: true,
                filter: "[IsActive] = 1 AND [JerseyNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_Unique",
                table: "PlayerTeamHistory",
                columns: new[] { "PlayerId", "TeamId", "SeasonId" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamHistory_ActiveTeamJerseyNumber",
                table: "PlayerTeamHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamHistory_Unique",
                table: "PlayerTeamHistory");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_TeamId",
                table: "PlayerTeamHistory",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_Unique",
                table: "PlayerTeamHistory",
                columns: new[] { "PlayerId", "TeamId", "SeasonId" },
                unique: true);
        }
    }
}
