using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class AddThaydoiLocChuTro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ChuTro",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChuTro_ApplicationUserId",
                table: "ChuTro",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChuTro_AspNetUsers_ApplicationUserId",
                table: "ChuTro",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChuTro_AspNetUsers_ApplicationUserId",
                table: "ChuTro");

            migrationBuilder.DropIndex(
                name: "IX_ChuTro_ApplicationUserId",
                table: "ChuTro");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ChuTro");
        }
    }
}
