using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{

    [Table("ChiSoDienNuoc")]

    public class ChiSoDienNuoc
    {
        [Key]
        public int MaChiSo { get; set; }

        

        [Required]
        public decimal ChiSoDienCu { get; set; }

        [Required]
        public decimal ChiSoDienMoi { get; set; }

        [Required]
        public decimal ChiSoNuocCu { get; set; }

        [Required]
        public decimal ChiSoNuocMoi { get; set; }

        [NotMapped]
        public decimal SoDienTieuThu { get { return ChiSoDienMoi - ChiSoDienCu; } set { } } // Thêm setter rỗng
        [NotMapped]
        public decimal SoNuocTieuThu { get { return ChiSoNuocMoi - ChiSoNuocCu; } set { } } // Thêm setter rỗng

        [Required]
        public decimal DonGiaDien { get; set; }

        [Required]
        public decimal DonGiaNuoc { get; set; }

        [Required]
        public DateTime NgayGhi { get; set; }

        // Khóa ngoại liên kết với Phòng Trọ
        [ForeignKey("PhongTro")]
        [Required]
        public int MaPhong { get; set; }

        public PhongTro PhongTro { get; set; } = null!;
        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
