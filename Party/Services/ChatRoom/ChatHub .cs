using Microsoft.AspNetCore.SignalR;

namespace Party.Services.ChatRoom
{
    public class ChatHub : Hub
    {
        private readonly ChatRoomService _chatRoomService;

        public ChatHub(ChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        public async Task CreateRoom(string roomName)
        {
            _chatRoomService.CreateRoom(roomName);
            await Clients.All.SendAsync("RoomCreated", roomName);
        }

        public async Task JoinRoom(string roomName)
        {
            var user = Context.User.Identity.Name; // 獲取使用者名稱
            _chatRoomService.JoinRoom(roomName, user);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", user);
        }

        public async Task LeaveRoom(string roomName)
        {
            var user = Context.User.Identity.Name; // 獲取使用者名稱
            _chatRoomService.LeaveRoom(roomName, user);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserLeft", user);
        }

        public async Task SendMessage(string roomName, string message)
        {
            var user = Context.User.Identity.Name; // 獲取使用者名稱
            _chatRoomService.SendMessage(roomName, message);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }
    }

}
