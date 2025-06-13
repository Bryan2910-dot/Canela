using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canela.Data.Migrations
{
    /// <inheritdoc />
    public partial class SegMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "t_preorden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "t_preorden",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
