using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailNotificationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "MatchId",
                table: "EmailNotifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "EmailNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "EmailNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "EmailNotifications",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "EmailNotifications");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "EmailNotifications");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "EmailNotifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "MatchId",
                table: "EmailNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
