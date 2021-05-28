using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBookAPI.Data.Migrations
{
    public partial class addeduserIdtoPageElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PageElements",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageElements_UserId",
                table: "PageElements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageElements_AspNetUsers_UserId",
                table: "PageElements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageElements_AspNetUsers_UserId",
                table: "PageElements");

            migrationBuilder.DropIndex(
                name: "IX_PageElements_UserId",
                table: "PageElements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PageElements");
        }
    }
}
