using LBQuiz.Services;
using LBQuiz.Models.Lobby;

namespace LBQuiz.Test.Services.LobbyParticipantManagerTests;

public class GetParticipantsTests
{
    [Fact]
    public void GetParticipants_ReturnsParticipants_IfParticipantsExist()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            Nickname = "TestUser1",
            Score = 0
        };
        
        // Act
        var participants = manager.AddParticipant(1, participant);
        var result = manager.GetParticipants(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void GetParticipants_ReturnsEmptyList_IfParticipantNotFound()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        
        // Act
        var result =  manager.GetParticipants(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetParticipants_ReturnParticipants_FromSpecificLobby()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant1 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            Nickname = "TestUser1",
        };
        var participant2 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            Nickname = "TestUser2",
        };
        
        // Act
        var participants1 = manager.AddParticipant(1, participant1);
        var participants2 = manager.AddParticipant(2, participant2);
        var result = manager.GetParticipants(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(participant1.Nickname ==  result[0].Nickname);
    }
}