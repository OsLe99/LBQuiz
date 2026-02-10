using LBQuiz.Services;
using LBQuiz.Models.Lobby;

namespace LBQuiz.Test.Services.LobbyParticipantManagerTests;

public class GetLobbyParticipantTests
{
    [Fact]
    public void GetLobbyParticipant_EmptyConnectionId_KeyNotFoundException()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        
        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => manager.GetLobbyParticipant(""));
    }

    [Fact]
    public void GetLobbyParticipant_WithExistingConnectionId_KeyFound()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser1",
            Score = 3000
        };

        // Act
        var participant1 = manager.AddParticipant(1, participant);
        var result =  manager.GetLobbyParticipant("newCon-123");
        
        // Assert
        Assert.NotNull(result);
        Assert.Same(participant, result);
    }

    [Fact]
    public void GetLobbyParticipant_NullValue_KeyNotFoundException()
    {
        var manager = new LobbyParticipantManager();
        Assert.Throws<ArgumentNullException>(() => manager.GetLobbyParticipant(null));
    }
}