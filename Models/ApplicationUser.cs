using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DACS_QuanLyPhongTro.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string HoTen { get; set; }

        [Required]
        public string GioiTinh { get; set; } // Nam, Nữ, Khác

        [Required]
        [StringLength(12)]
        public string CCCD { get; set; }

        [Required]
        public string SoDienThoai { get; set; }

    }
}
