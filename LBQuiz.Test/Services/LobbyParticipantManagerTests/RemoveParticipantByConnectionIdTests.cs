using LBQuiz.Services;
using LBQuiz.Models.Lobby;

namespace LBQuiz.Test.Services.LobbyParticipantManagerTests;

public class RemoveParticipantByConnectionIdTests
{
    [Fact]
    public void RemoveParticipantByConnectionId_ReturnsNull_IfParticipantDoesNotExist()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        
        // Act
        var result = manager.RemoveParticipantByConnectionId("nonExistingId");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void RemoveParticipantByConnectionId_ReturnsParticipant_IfParticipantExists()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser1",
            Score = 1000
        };
        
        // Act
        manager.AddParticipant(1, participant);
        var result = manager.RemoveParticipantByConnectionId("newCon-123");

        // Assert
        Assert.Equal(participant, result);
    }

    [Fact]
    public void RemoveParticipantByConnectionId_ReturnsUser_IfMultipleExists()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant1 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser1",
            Score = 1000
        };
        var participant2 = new LobbyParticipant
        {
            ConnectionId = "newCon-456",
            LobbyId = 2,
            Nickname = "TestUser2",
            Score = 3000
        };
        
        // Act
        manager.AddParticipant(1, participant1);
        manager.AddParticipant(2, participant2);
        var result = manager.RemoveParticipantByConnectionId("newCon-456");
        
        // Assert
        Assert.NotEqual(participant1, result);
        Assert.Equal(participant2, result);
        Assert.DoesNotContain(participant2, manager.GetParticipants(2));
    }

    [Fact]
    public void RemoveParticipantByConnectionId_ReturnsNull_IfConnectionStringIsEmpty()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        
        // Act
        var result = manager.RemoveParticipantByConnectionId("");
        
        // Assert
        Assert.Null(result);
    }
}