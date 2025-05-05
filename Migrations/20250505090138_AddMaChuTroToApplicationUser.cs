using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class AddMaChuTroToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhongTro_KhachThue_MaKhachThue",
                table: "PhongTro");

            migrationBuilder.AddColumn<int>(
                name: "MaChuTro",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTro_KhachThue_MaKhachThue",
                table: "PhongTro",
                column: "MaKhachThue",
                principalTable: "KhachThue",
                principalColumn: "MaKhachThue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhongTro_KhachThue_MaKhachThue",
                table: "PhongTro");

            migrationBuilder.DropColumn(
                name: "MaChuTro",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTro_KhachThue_MaKhachThue",
                table: "PhongTro",
                column: "MaKhachThue",
                principalTable: "KhachThue",
                principalColumn: "MaKhachThue",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
