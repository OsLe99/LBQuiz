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
        manager.AddParticipant(1, participant1);
        manager.AddParticipant(1, participant2);
        manager.AddParticipant(1, participant3);
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
    
    [Fact]
    public void GetLeaderboard_ShouldReturnListInOrderAdded_IfPointsAreEven()
    {
        // Arrange
        var manager = new LobbyParticipantManager();
        var participants = new[]
        {
            new LobbyParticipant { ConnectionId = "newCon-123", Nickname = "TestUser1", Score = 3000},
            new LobbyParticipant {  ConnectionId = "newCon-456", Nickname = "TestUser2", Score = 1000},
            new LobbyParticipant {  ConnectionId = "newCon-789", Nickname = "TestUser3", Score = 2000},
            new LobbyParticipant { ConnectionId = "newCon-111",  Nickname = "TestUser4", Score = 2000},
            new LobbyParticipant { ConnectionId = "newCon-112",  Nickname = "TestUser5", Score = 1500}
        };

        foreach (var participant in participants)
        {
            manager.AddParticipant(1, participant);
        }
        
        // Act
        var result = manager.GetLeaderboard(1);
        
        // Assert
        Assert.Equal(5, result.Count);
        Assert.Multiple(
            () => Assert.Equal(3000, result[0].Score),
            () => Assert.Equal(2000, result[1].Score),
            () => Assert.Equal(2000, result[2].Score),
            () => Assert.Equal(1500, result[3].Score),
            () => Assert.Equal(1000, result[4].Score)
            );
        var tiedNicknames = new[] { result[1].Nickname, result[2].Nickname };
        Assert.Multiple(
            () => Assert.Equal("TestUser3", tiedNicknames[0]),
            () => Assert.Equal("TestUser4", tiedNicknames[1])
            );
    }
}