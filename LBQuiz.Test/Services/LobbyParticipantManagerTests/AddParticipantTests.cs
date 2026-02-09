using LBQuiz.Models.Lobby;
using LBQuiz.Services;

namespace LBQuiz.Test.Services.LobbyParticipantManagerTests;

public class AddParticipantTests
{
    [Fact]
    public void AddParticipant_ShouldAddParticipantSuccessfully()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser",
            Score = 0
        };
        
        // Act
        var result = manager.AddParticipant(1, participant);
        
        // Assert
        Assert.True(result);
        var participants = manager.GetParticipants(1);
        Assert.Single(participants);
        Assert.Equal(participant.ConnectionId, participants[0].ConnectionId);
    }
    
    [Fact]
    public void AddParticipant_ShouldAddMultipleParticipantsSuccessfully()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant1 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser1",
            Score = 0
        };
        var participant2 = new LobbyParticipant
        {
            ConnectionId = "newCon-456",
            LobbyId = 1,
            Nickname = "TestUser2",
            Score = 0
        };
        
        // Act
        var result1 = manager.AddParticipant(1, participant1);
        var result2 =  manager.AddParticipant(1, participant2);
        
        // Assert
        Assert.True(result1);
        Assert.True(result2);
        var participants = manager.GetParticipants(1);
        Assert.Equal(2, participants.Count);
    }

    [Fact]
    public void AddParticipant_InvalidLobbyId_ShouldThrowException()
    {
        var manager = new LobbyParticipantManager();
        var participant = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            Nickname = "TestUser"
        };
        
        Assert.Throws<ArgumentException>(() => manager.AddParticipant(0, participant));
        Assert.Throws<ArgumentException>(() => manager.AddParticipant(-1, participant));
    }

    [Fact]
    public void AddParticipant_WithNullParticipant_ShouldThrowException()
    {
        var manager = new LobbyParticipantManager();
        
        Assert.Throws<ArgumentNullException>(() => manager.AddParticipant(1, null));
    }

    [Fact]
    public void AddParticipant_ShouldHandleMultipleLobbies()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant1 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            LobbyId = 1,
            Nickname = "TestUser1",
            Score = 0
        };
        var participant2 = new LobbyParticipant
        {
            ConnectionId = "newCon-456",
            LobbyId = 2,
            Nickname = "TestUser2",
            Score = 0
        };
        var participant3 = new LobbyParticipant
        {
            ConnectionId = "newCon-789",
            LobbyId = 2,
            Nickname = "TestUser3",
            Score = 0
        };
        
        // Act
        var result1 = manager.AddParticipant(1, participant1);
        var result2 = manager.AddParticipant(2, participant2);
        var result3 = manager.AddParticipant(2, participant3);
        
        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
        var participants1 = manager.GetParticipants(1);
        Assert.Single(participants1);
        var participants2 = manager.GetParticipants(2);
        Assert.Equal(2, participants2.Count);
    }
}