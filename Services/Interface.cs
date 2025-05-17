using System.Threading.Tasks;

namespace DACS_QuanLyPhongTro.Services
{
    public interface IUserService
    {
        Task<bool> HasActiveRentalAsync(string userId);
    }
}
