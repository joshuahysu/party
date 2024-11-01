namespace Party.Services.ChatRoom
{
    public class ChatRoomService
    {
        private readonly Dictionary<string, ChatRoom> _rooms = new Dictionary<string, ChatRoom>();

        public void CreateRoom(string roomName)
        {
            if (!_rooms.ContainsKey(roomName))
            {
                _rooms[roomName] = new ChatRoom { RoomName = roomName };
            }
        }

        public void JoinRoom(string roomName, string user)
        {
            if (_rooms.ContainsKey(roomName) && !_rooms[roomName].Participants.Contains(user))
            {
                _rooms[roomName].Participants.Add(user);
            }
        }

        public void LeaveRoom(string roomName, string user)
        {
            if (_rooms.ContainsKey(roomName))
            {
                _rooms[roomName].Participants.Remove(user);
            }
        }

        public void SendMessage(string roomName, string message)
        {
            if (_rooms.ContainsKey(roomName))
            {
                _rooms[roomName].Messages.Add(message);
            }
        }

        public List<string> GetMessages(string roomName)
        {
            if (_rooms.ContainsKey(roomName))
            {
                return _rooms[roomName].Messages;
            }
            return new List<string>();
        }

    }

}
