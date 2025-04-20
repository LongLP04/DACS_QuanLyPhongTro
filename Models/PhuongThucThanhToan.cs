using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("PhuongThucThanhToan")]
    public class PhuongThucThanhToan
    {
        [Key]
        public int MaPhuongThuc { get; set; }
        public string TenPhuongThuc { get; set; }
        public string MoTa { get; set; }

        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();
    }
}
