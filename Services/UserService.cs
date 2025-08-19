using System.Threading.Tasks;
using DACS_QuanLyPhongTro.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasActiveRentalAsync(string userId)
        {
            // Lấy khách thuê theo ApplicationUserId
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null) return false;

            // Kiểm tra có hợp đồng thuê nào với trạng thái "Đã Xác Nhận" không
            return await _context.HopDongs.AnyAsync(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Đã Xác Nhận");
        }


    }
}
