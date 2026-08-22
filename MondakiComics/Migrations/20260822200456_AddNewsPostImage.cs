using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MondakiComics.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsPostImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "NewsPosts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "NewsPosts");
        }
    }
}
