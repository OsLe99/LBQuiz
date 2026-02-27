using LBQuiz.Models.Lobby;
using System.Drawing;

namespace LBQuiz.Models.Helpers
{
    public class ParticipantAnswer
    {
        public LobbyParticipant Participant { get; set; }
        public string Answer { get; set; }
        public MudBlazor.Color AnswerColor { get; set; }
        public QuestionJsonBlob QuestionJB { get; set; }
    }
}
