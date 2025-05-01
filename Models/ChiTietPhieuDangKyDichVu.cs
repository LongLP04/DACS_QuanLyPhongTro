using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("ChiTietPhieuDangKyDichVu")]
    public class ChiTietPhieuDangKyDichVu
    {
        [Key, Column(Order = 0)]
        public int MaDangKyDichVu { get; set; }

        [Key, Column(Order = 1)]
        public int MaDichVu { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTienDichVu { get; set; }

        // Navigation properties
        public PhieuDangKyDichVu PhieuDangKyDichVu { get; set; } = null!;
        public DichVu DichVu { get; set; } = null!;
    }
}
