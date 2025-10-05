using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using DACS_QuanLyPhongTro.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DACS_QuanLyPhongTro.Hubs
{
        public class ChatHub : Hub
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public ChatHub(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            public override async Task OnConnectedAsync()
            {
                Console.WriteLine($"[SignalR] OnConnectedAsync: ConnectionId={Context.ConnectionId}, UserIdentifier={Context.UserIdentifier}");
                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception exception)
            {
                Console.WriteLine($"[SignalR] OnDisconnectedAsync: ConnectionId={Context.ConnectionId}, UserIdentifier={Context.UserIdentifier}");
                await base.OnDisconnectedAsync(exception);
            }

        // Gửi tin nhắn tới 1 user cụ thể và lưu vào DB
        public async Task SendPrivateMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
            Console.WriteLine($"[SignalR] SendPrivateMessage: senderId={senderId}, receiverId={receiverId}, message={message}");
            try
            {
                // Lưu tin nhắn vào DB
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Messages.Add(new Message
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = message,
                        Timestamp = DateTime.Now,
                        IsGroup = false
                    });
                    await db.SaveChangesAsync();
                }
                await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, false);
                await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, message, false);
            }
            catch (Exception ex)
            {
                // Log lỗi ra console server
                Console.WriteLine($"SignalR SendPrivateMessage error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        // Gửi tin nhắn tới tất cả khách thuê của chủ trọ (chat nhóm)
        public async Task SendGroupMessage(string groupName, string message)
        {
            var senderId = Context.UserIdentifier;
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, message, true);
        }

        // Tham gia group chat chung
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Rời group chat chung
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
