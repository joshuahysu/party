namespace Party.Services.ChatRoom
{
    public class ChatRoom
    {
        public string RoomName { get; set; }
        public List<string> Participants { get; set; } = new List<string>();
        public List<string> Messages { get; set; } = new List<string>();
    }
}
