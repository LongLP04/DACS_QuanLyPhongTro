using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class addIDKhachThue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "KhachThue",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachThue_ApplicationUserId",
                table: "KhachThue",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachThue_AspNetUsers_ApplicationUserId",
                table: "KhachThue",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachThue_AspNetUsers_ApplicationUserId",
                table: "KhachThue");

            migrationBuilder.DropIndex(
                name: "IX_KhachThue_ApplicationUserId",
                table: "KhachThue");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "KhachThue");
        }
    }
}
