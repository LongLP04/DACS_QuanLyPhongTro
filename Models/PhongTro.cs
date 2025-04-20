using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DACS_QuanLyPhongTro.Models;

[Table("PhongTro")]
public class PhongTro
{
    [Key]
    public int MaPhong { get; set; }

    [Required]
    public string SoPhong { get; set; }
    public int Tang { get; set; }
    public decimal DienTich { get; set; }
    public decimal GiaThue { get; set; }
    public string TrangThai { get; set; }
    public string MoTa { get; set; }

    [Required]
    public int MaToaNha { get; set; }
    [ForeignKey("MaToaNha")]
    public ToaNha ToaNha { get; set; }

    public ICollection<KhachThue> KhachThues { get; set; } = new List<KhachThue>();
    public ICollection<ChiSoDienNuoc> ChiSoDienNuocs { get; set; } = new List<ChiSoDienNuoc>();
}
