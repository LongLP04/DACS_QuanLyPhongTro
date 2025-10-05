using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Lấy ApplicationUserId từ claim NameIdentifier (Identity mặc định)
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
