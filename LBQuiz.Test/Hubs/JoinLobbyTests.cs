using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace LBQuiz.Test.Hubs;

public class JoinLobbyTests : IClassFixture<LobbyHubFixture>
{
    private readonly LobbyHubFixture _fixture;
    
    public JoinLobbyTests(LobbyHubFixture fixture)
    {
        _fixture = fixture;
        _fixture.Reset();
    }

    [Fact]
    public async Task JoinLobby_WithValidCode_ShouldAddParticipantAndNotifyGroup()
    {
        // Arrange
        var hub = _fixture.CreateHub();
        var lobby =  _fixture.CreateLobby();

        _fixture.MockLobbyService.Setup(s => s.GetLobbyByJoinCodeAsync("ABC123"))
            .ReturnsAsync(lobby);
        
        _fixture.MockLobbyParticipantManager.Setup(m => m.AddParticipant(1, It.IsAny<LobbyParticipant>()))
            .Returns(true);
        
        _fixture.MockLobbyParticipantManager.Setup(m => m.GetParticipants(1))
            .Returns(new List<LobbyParticipant> { _fixture.CreateTestLobbyParticipant() });
        
        // Act
        await hub.JoinLobby("ABC123", "Player1");
        
        // Assert
        _fixture.MockGroups.Verify(
            g => g.AddToGroupAsync("conn-123", "1", default),
            Times.Once
        );
        
        _fixture.MockClientProxy.Verify(
            c => c.SendCoreAsync(
                "ParticipantJoined",
                It.Is<object?[]?>(args => 
                    args != null && 
                    args[0].ToString() == "Player1"
                ),
                default
            ),
            Times.Once
        );
    }
    
    [Fact]
    public async Task JoinLobby_WithInvalidCode_ShouldThrowHubException()
    {
        // Arrange
        var hub = _fixture.CreateHub();
        _fixture.MockLobbyService.Setup(s => s.GetLobbyByJoinCodeAsync("ABC123"))
            .ReturnsAsync((QuizLobby?) null);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<HubException>(() => hub.JoinLobby("InvalidCode", "Player1"));
        
        Assert.Equal("Invalid join code", exception.Message);
    }

    [Fact]
    public async Task JoinLobby_WhenAddParticipantsFails_ShouldNotAddToGroup()
    {
        // Arrange
        var hub = _fixture.CreateHub();
        var lobby = _fixture.CreateLobby();
        
        _fixture.MockLobbyService
            .Setup(s => s.GetLobbyByJoinCodeAsync("ABC123"))
            .ReturnsAsync(lobby);
        
        _fixture.MockLobbyParticipantManager
            .Setup(m => m.AddParticipant(1, It.IsAny<LobbyParticipant>()))
            .Returns(false);

        // Act
        await hub.JoinLobby("ABC123", "Player1");
        
        // Assert
        _fixture.MockGroups.Verify(
            g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), default),
            Times.Never
            );
    }
}