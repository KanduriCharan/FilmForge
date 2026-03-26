using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Username = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    PrimaryCraft = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SecondaryCrafts = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ExperienceLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AvailabilityStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PortfolioUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstagramUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    YoutubeUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_Username",
                table: "user_profiles",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_profiles");
        }
    }
}
