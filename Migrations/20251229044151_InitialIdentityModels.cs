using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_QuanLyPhongTro.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonGiaDichVu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsGroup = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToan",
                columns: table => new
                {
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongThucThanhToan", x => x.MaPhuongThuc);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuTro",
                columns: table => new
                {
                    MaChuTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gioitinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuTro", x => x.MaChuTro);
                    table.ForeignKey(
                        name: "FK_ChuTro_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhachThue",
                columns: table => new
                {
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gioitinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachThue", x => x.MaKhachThue);
                    table.ForeignKey(
                        name: "FK_KhachThue_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToaNha",
                columns: table => new
                {
                    MaToaNha = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenToaNha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSoTang = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChuTro = table.Column<int>(type: "int", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNha", x => x.MaToaNha);
                    table.ForeignKey(
                        name: "FK_ToaNha_ChuTro_MaChuTro",
                        column: x => x.MaChuTro,
                        principalTable: "ChuTro",
                        principalColumn: "MaChuTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuDangKyDichVu",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDangKyDichVu", x => x.MaDangKyDichVu);
                    table.ForeignKey(
                        name: "FK_PhieuDangKyDichVu_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGhiNhanSuCo",
                columns: table => new
                {
                    MaPhieuSuCo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayGhiNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTaSuCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HienTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGhiNhanSuCo", x => x.MaPhieuSuCo);
                    table.ForeignKey(
                        name: "FK_PhieuGhiNhanSuCo_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuHienTrangNhanPhong",
                columns: table => new
                {
                    MaPhieuHienTrang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayNhanPhong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuHienTrangNhanPhong", x => x.MaPhieuHienTrang);
                    table.ForeignKey(
                        name: "FK_PhieuHienTrangNhanPhong_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhongTro",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    Hinhanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DienTich = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    GiaThue = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaToaNha = table.Column<int>(type: "int", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongTro", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_PhongTro_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_PhongTro_ToaNha_MaToaNha",
                        column: x => x.MaToaNha,
                        principalTable: "ToaNha",
                        principalColumn: "MaToaNha",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuDangKyDichVu",
                columns: table => new
                {
                    MaDangKyDichVu = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TongTienDichVu = table.Column<decimal>(type: "decimal(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuDangKyDichVu", x => new { x.MaDangKyDichVu, x.MaDichVu });
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVu_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuDangKyDichVu_PhieuDangKyDichVu_MaDangKyDichVu",
                        column: x => x.MaDangKyDichVu,
                        principalTable: "PhieuDangKyDichVu",
                        principalColumn: "MaDangKyDichVu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HienTrangVatDung",
                columns: table => new
                {
                    MaVatDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVatDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhieuHienTrang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HienTrangVatDung", x => x.MaVatDung);
                    table.ForeignKey(
                        name: "FK_HienTrangVatDung_PhieuHienTrangNhanPhong_MaPhieuHienTrang",
                        column: x => x.MaPhieuHienTrang,
                        principalTable: "PhieuHienTrangNhanPhong",
                        principalColumn: "MaPhieuHienTrang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoDienNuoc",
                columns: table => new
                {
                    MaChiSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiSoDienCu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoDienMoi = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoNuocCu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChiSoNuocMoi = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoDienTieuThu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoNuocTieuThu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoDienNuoc", x => x.MaChiSo);
                    table.ForeignKey(
                        name: "FK_ChiSoDienNuoc_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HopDong",
                columns: table => new
                {
                    MaHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    NoiDungHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDong", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDong_KhachThue_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HopDong_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HopDong_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHen",
                columns: table => new
                {
                    MaLichHen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    NgayHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioHen = table.Column<TimeSpan>(type: "time", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHen", x => x.MaLichHen);
                    table.ForeignKey(
                        name: "FK_LichHen_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHen_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienDien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienNuoc = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienPhong = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TienDichVu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    MaChiSo = table.Column<int>(type: "int", nullable: false),
                    MaKhachThue = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienNuocMaChiSo = table.Column<int>(type: "int", nullable: true),
                    KhachThueMaKhachThue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDon_ChiSoDienNuoc_ChiSoDienNuocMaChiSo",
                        column: x => x.ChiSoDienNuocMaChiSo,
                        principalTable: "ChiSoDienNuoc",
                        principalColumn: "MaChiSo");
                    table.ForeignKey(
                        name: "FK_HoaDon_ChiSoDienNuoc_MaChiSo",
                        column: x => x.MaChiSo,
                        principalTable: "ChiSoDienNuoc",
                        principalColumn: "MaChiSo");
                    table.ForeignKey(
                        name: "FK_HoaDon_KhachThue_KhachThueMaKhachThue",
                        column: x => x.KhachThueMaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HoaDon_KhachThue_MaKhachThue",
                        column: x => x.MaKhachThue,
                        principalTable: "KhachThue",
                        principalColumn: "MaKhachThue");
                    table.ForeignKey(
                        name: "FK_HoaDon_PhongTro_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongTro",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThanhToan",
                columns: table => new
                {
                    MaPhieuThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTienThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaPhuongThuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThanhToan", x => x.MaPhieuThanhToan);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_PhuongThucThanhToan_MaPhuongThuc",
                        column: x => x.MaPhuongThuc,
                        principalTable: "PhuongThucThanhToan",
                        principalColumn: "MaPhuongThuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoDienNuoc_MaPhong",
                table: "ChiSoDienNuoc",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuDangKyDichVu_MaDichVu",
                table: "ChiTietPhieuDangKyDichVu",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChuTro_ApplicationUserId",
                table: "ChuTro",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HienTrangVatDung_MaPhieuHienTrang",
                table: "HienTrangVatDung",
                column: "MaPhieuHienTrang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_ChiSoDienNuocMaChiSo",
                table: "HoaDon",
                column: "ChiSoDienNuocMaChiSo");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_KhachThueMaKhachThue",
                table: "HoaDon",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaChiSo",
                table: "HoaDon",
                column: "MaChiSo");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKhachThue",
                table: "HoaDon",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaPhong",
                table: "HoaDon",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_KhachThueMaKhachThue",
                table: "HopDong",
                column: "KhachThueMaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaKhachThue",
                table: "HopDong",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaPhong",
                table: "HopDong",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_KhachThue_ApplicationUserId",
                table: "KhachThue",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaKhachThue",
                table: "LichHen",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaPhong",
                table: "LichHen",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MaKhachThue",
                table: "Notification",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDangKyDichVu_MaKhachThue",
                table: "PhieuDangKyDichVu",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGhiNhanSuCo_MaKhachThue",
                table: "PhieuGhiNhanSuCo",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuHienTrangNhanPhong_MaKhachThue",
                table: "PhieuHienTrangNhanPhong",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_MaHoaDon",
                table: "PhieuThanhToan",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_MaPhuongThuc",
                table: "PhieuThanhToan",
                column: "MaPhuongThuc");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTro_MaKhachThue",
                table: "PhongTro",
                column: "MaKhachThue");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTro_MaToaNha",
                table: "PhongTro",
                column: "MaToaNha");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNha_MaChuTro",
                table: "ToaNha",
                column: "MaChuTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuDangKyDichVu");

            migrationBuilder.DropTable(
                name: "HienTrangVatDung");

            migrationBuilder.DropTable(
                name: "HopDong");

            migrationBuilder.DropTable(
                name: "LichHen");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PhieuGhiNhanSuCo");

            migrationBuilder.DropTable(
                name: "PhieuThanhToan");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "PhieuDangKyDichVu");

            migrationBuilder.DropTable(
                name: "PhieuHienTrangNhanPhong");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToan");

            migrationBuilder.DropTable(
                name: "ChiSoDienNuoc");

            migrationBuilder.DropTable(
                name: "PhongTro");

            migrationBuilder.DropTable(
                name: "KhachThue");

            migrationBuilder.DropTable(
                name: "ToaNha");

            migrationBuilder.DropTable(
                name: "ChuTro");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
