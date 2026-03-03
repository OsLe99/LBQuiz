namespace LBQuiz.Models.Helpers
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string LobbyId { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
