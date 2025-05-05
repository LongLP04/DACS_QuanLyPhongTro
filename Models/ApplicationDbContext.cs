using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Định nghĩa DbSet cho các bảng và khởi tạo
        public DbSet<ChiSoDienNuoc> ChiSoDienNuocs { get; set; } = null!;
        public DbSet<ChuTro> ChuTros { get; set; } = null!;
        public DbSet<DichVu> DichVus { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<HopDong> HopDongs { get; set; } = null!;
        public DbSet<KhachThue> KhachThues { get; set; } = null!;
        public DbSet<PhieuDangKyDichVu> PhieuDangKyDichVus { get; set; } = null!;
        public DbSet<PhieuGhiNhanSuCo> PhieuGhiNhanSuCos { get; set; } = null!;
        public DbSet<PhieuThanhToan> PhieuThanhToans { get; set; } = null!;
        public DbSet<PhongTro> PhongTros { get; set; } = null!;
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; } = null!;
        public DbSet<ToaNha> ToaNhas { get; set; } = null!;
        public DbSet<ChiTietPhieuDangKyDichVu> ChiTietPhieuDangKyDichVus { get; set; } = null!;
        public DbSet<HienTrangVatDung> HienTrangVatDungs { get; set; } = null!;
        public DbSet<PhieuHienTrangNhanPhong> PhieuHienTrangNhanPhongs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PhongTro>()
        .HasOne(p => p.KhachThue)
        .WithMany(k => k.PhongTros)
        .HasForeignKey(p => p.MaKhachThue)
        .OnDelete(DeleteBehavior.ClientSetNull);
            // tránh xóa dây chuyền

            modelBuilder.Entity<ChiTietPhieuDangKyDichVu>()
                .HasKey(c => new { c.MaDangKyDichVu, c.MaDichVu });
            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.ChiSoDienNuoc)
                .WithMany()
                .HasForeignKey(h => h.MaChiSo)
                .OnDelete(DeleteBehavior.NoAction); // Thay vì Cascade

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.KhachThue)
                .WithMany()
                .HasForeignKey(h => h.MaKhachThue)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DichVu>()
            .Property(d => d.DonGiaDichVu)
            .HasColumnType("decimal(15,2)"); // Ví dụ: 18 chữ số, 4 chữ số thập phân

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TienDichVu)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TienDien)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TienNuoc)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TienPhong)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TongTien)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HopDong>()
                .Property(h => h.TienCoc)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<PhongTro>()
                .Property(p => p.GiaThue)
                .HasColumnType("decimal(15,2)");
            modelBuilder.Entity<PhongTro>()
                .Property(p => p.DienTich)
                .HasColumnType("decimal(15,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()    
                .Property(c => c.ChiSoDienCu)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.ChiSoDienMoi)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.ChiSoNuocCu)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.ChiSoNuocMoi)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.SoDienTieuThu)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.SoNuocTieuThu)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.DonGiaDien)
                .HasColumnType("decimal(15,2)");
            modelBuilder.Entity<ChiSoDienNuoc>()
                .Property(c => c.DonGiaNuoc)
                .HasColumnType("decimal(15,2)");

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.KhachThue)
                .WithMany()
                .HasForeignKey(h => h.MaKhachThue)
                .OnDelete(DeleteBehavior.NoAction);  // Thay vì CASCADE

        }

    }
}
