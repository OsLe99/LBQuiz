using LBQuiz.Services;
using LBQuiz.Models;
using LBQuiz.Models.Lobby;

namespace LBQuiz.Test.Services.LobbyParticipantManagerTests;

public class GetLeaderBoardTests
{
    [Fact]
    public void GetLeaderboard_EmptyLobby_ReturnsEmptyList()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        
        // Act
        var result = manager.GetLeaderboard(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetLeaderboard_MultipleParticipants_ReturnsScoreByDescendingOrder()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participant1 = new LobbyParticipant
        {
            ConnectionId = "newCon-123",
            Nickname = "TestUser1",
            Score = 1000,
        };
        var participant2 = new LobbyParticipant
        {
            ConnectionId = "newCon-456",
            Nickname = "TestUser2",
            Score = 3000,
        };
        var participant3 = new LobbyParticipant
        {
            ConnectionId = "newCon-789",
            Nickname = "TestUser3",
            Score = 1500
        };
        
        // Act
        var participants1 = manager.AddParticipant(1, participant1);
        var participants2 = manager.AddParticipant(1, participant2);
        var participants3 = manager.AddParticipant(1, participant3);
        var result = manager.GetLeaderboard(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Collection(result,
            p => Assert.Equal("TestUser2", p.Nickname),
            p => Assert.Equal("TestUser3", p.Nickname),
            p => Assert.Equal("TestUser1", p.Nickname)
        );
    }
}