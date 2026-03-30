using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "user_profiles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "user_profiles");
        }
    }
}
