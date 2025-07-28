using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRKošarka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesWithConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenueCapacity",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueName",
                table: "Clubs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgeCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeactivateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AgeCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DeactivateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_AgeCategories_AgeCategoryId",
                        column: x => x.AgeCategoryId,
                        principalTable: "AgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teams_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgeCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompetitionType = table.Column<int>(type: "int", nullable: false),
                    NumberOfRounds = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leagues_AgeCategories_AgeCategoryId",
                        column: x => x.AgeCategoryId,
                        principalTable: "AgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leagues_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTeamHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JerseyNumber = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTeamHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerTeamHistory_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTeamHistory_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerTeamHistory_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamRepresentatives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeactivateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRepresentatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamRepresentatives_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotifyByEmail = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueStandings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    MatchesPlayed = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    PointsFor = table.Column<int>(type: "int", nullable: false),
                    PointsAgainst = table.Column<int>(type: "int", nullable: false),
                    PointsDifference = table.Column<int>(type: "int", nullable: false),
                    LeaguePoints = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueStandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeagueStandings_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueStandings_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeagueStandings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeagueTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeagueTeams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VenueOverride = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DefaultScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    QuarterResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsResultConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SchedulingStatus = table.Column<int>(type: "int", nullable: false),
                    ConfirmedByUserId = table.Column<string>(type: "varchar(450)", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSchedulingUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.CheckConstraint("CK_Match_DifferentTeams", "HomeTeamId != AwayTeamId");
                    table.ForeignKey(
                        name: "FK_Matches_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSeasonStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    TotalFouls = table.Column<int>(type: "int", nullable: false),
                    TotalThreePointers = table.Column<int>(type: "int", nullable: false),
                    MatchesPlayed = table.Column<int>(type: "int", nullable: false),
                    AveragePoints = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AverageThreePointers = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeasonStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerSeasonStats_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSeasonStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSeasonStats_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSeasonStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailNotifications_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchReschedulingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    ProposedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponseByUserId = table.Column<string>(type: "varchar(450)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchReschedulingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchReschedulingRequests_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMatchStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Fouls = table.Column<int>(type: "int", nullable: false),
                    ThreePointers = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AgeCategories",
                columns: new[] { "Id", "Code", "CreatedBy", "DateCreated", "DateDeleted", "DateModified", "DeletedBy", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "U9", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "U9" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "U11", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "U11" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "U13", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "U13" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "U15", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "U15" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "U17", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "U17" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "JUNIORI", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "Juniori" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "SENIORI", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "Seniori" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "JUNIORKE", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "Juniorke" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "SENIORKE", null, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "Seniorke" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeCategories_Code",
                table: "AgeCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgeCategories_Name",
                table: "AgeCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotification_MatchId",
                table: "EmailNotifications",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotification_SentAt",
                table: "EmailNotifications",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotification_UserId",
                table: "EmailNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_League_AgeCategoryId",
                table: "Leagues",
                column: "AgeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_League_SeasonId",
                table: "Leagues",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStanding_Position",
                table: "LeagueStandings",
                columns: new[] { "LeagueId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStanding_Unique",
                table: "LeagueStandings",
                columns: new[] { "LeagueId", "TeamId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStandings_SeasonId",
                table: "LeagueStandings",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStandings_TeamId",
                table: "LeagueStandings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueTeam_TeamId",
                table: "LeagueTeams",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueTeam_Unique",
                table: "LeagueTeams",
                columns: new[] { "LeagueId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_LeagueId",
                table: "Matches",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_ScheduledDate",
                table: "Matches",
                column: "ActualScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_MatchReschedulingRequest_MatchId",
                table: "MatchReschedulingRequests",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchReschedulingRequest_Status",
                table: "MatchReschedulingRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_PlayerId",
                table: "PlayerMatchStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_Unique",
                table: "PlayerMatchStats",
                columns: new[] { "MatchId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasonStats_LeagueId",
                table: "PlayerSeasonStats",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasonStats_SeasonId",
                table: "PlayerSeasonStats",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasonStats_TeamId",
                table: "PlayerSeasonStats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasonStats_Unique",
                table: "PlayerSeasonStats",
                columns: new[] { "PlayerId", "LeagueId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_SeasonId",
                table: "PlayerTeamHistory",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_TeamId",
                table: "PlayerTeamHistory",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamHistory_Unique",
                table: "PlayerTeamHistory",
                columns: new[] { "PlayerId", "TeamId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamRepresentative_Unique",
                table: "TeamRepresentatives",
                columns: new[] { "UserId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamRepresentatives_TeamId",
                table: "TeamRepresentatives",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_AgeCategoryId",
                table: "Teams",
                column: "AgeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ClubId",
                table: "Teams",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteTeam_Unique",
                table: "UserFavoriteTeams",
                columns: new[] { "UserId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteTeams_TeamId",
                table: "UserFavoriteTeams",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailNotifications");

            migrationBuilder.DropTable(
                name: "LeagueStandings");

            migrationBuilder.DropTable(
                name: "LeagueTeams");

            migrationBuilder.DropTable(
                name: "MatchReschedulingRequests");

            migrationBuilder.DropTable(
                name: "PlayerMatchStats");

            migrationBuilder.DropTable(
                name: "PlayerSeasonStats");

            migrationBuilder.DropTable(
                name: "PlayerTeamHistory");

            migrationBuilder.DropTable(
                name: "TeamRepresentatives");

            migrationBuilder.DropTable(
                name: "UserFavoriteTeams");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "AgeCategories");

            migrationBuilder.DropColumn(
                name: "VenueCapacity",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "VenueName",
                table: "Clubs");
        }
    }
}
