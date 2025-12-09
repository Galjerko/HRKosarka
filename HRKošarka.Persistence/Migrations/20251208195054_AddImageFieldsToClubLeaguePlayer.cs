using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFieldsToClubLeaguePlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Clubs");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "Players",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Players",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Players",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "Leagues",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Leagues",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Leagues",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "Clubs",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Clubs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Clubs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Clubs");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Clubs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
