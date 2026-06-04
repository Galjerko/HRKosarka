using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueDefaultVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultVenue",
                table: "Leagues",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultVenue",
                table: "Leagues");
        }
    }
}
