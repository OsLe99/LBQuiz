using LBQuiz.Hubs;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;
using LBQuiz.Data;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Test.Hubs;

public class LobbyHubFixture
{
    public Mock<ILobbyService> MockLobbyService { get; }
    public Mock<ILobbyParticipantManager> MockLobbyParticipantManager { get; }
    public Mock<IHubCallerClients> MockClients { get; }
    public Mock<HubCallerContext> MockContext { get; }
    public Mock<IGroupManager> MockGroups { get; }
    public Mock<IClientProxy> MockClientProxy { get; }
    public Mock<IClientProxy> MockCallerProxy { get; }
    public Mock<IQuestionScoringService> MockQuestionScoringService { get; }

    public LobbyHubFixture()
    {
        MockLobbyService = new Mock<ILobbyService>();
        MockLobbyParticipantManager = new Mock<ILobbyParticipantManager>();
        MockClients = new Mock<IHubCallerClients>();
        MockContext = new Mock<HubCallerContext>();
        MockGroups = new Mock<IGroupManager>();
        MockClientProxy = new Mock<IClientProxy>();
        MockCallerProxy = new Mock<IClientProxy>();
        MockQuestionScoringService = new Mock<IQuestionScoringService>();
        
        MockClients.As<IHubCallerClients<IClientProxy>>()
            .SetupGet(c => c.Caller)
            .Returns(MockCallerProxy.Object);
        
        MockClients.As<IHubCallerClients<IClientProxy>>()
            .Setup(c => c.Group(It.IsAny<string>()))
            .Returns(MockClientProxy.Object);
        
        MockContext.SetupGet(c => c.ConnectionId).Returns("conn-123");
    }

    private ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new ApplicationDbContext(options);
    }

    public LobbyHub CreateHub(string? userId = null, string connectionId = "conn-123")
    {
        MockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);

        if (userId != null)
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.NameIdentifier, userId)};
            var identity = new ClaimsIdentity(claims, "TestAuth");
            MockContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal(identity));
        }
        else
        {
            MockContext.SetupGet(c => c.User).Returns((ClaimsPrincipal?)null);
        }
        
        var dbContext = CreateInMemoryDb();
        
        return new LobbyHub(MockLobbyParticipantManager.Object, MockLobbyService.Object, MockQuestionScoringService.Object, dbContext)
        {
            Context = MockContext.Object,
            Clients = MockClients.Object,
            Groups = MockGroups.Object
        };
    }
    
    public QuizLobby CreateLobby(int id = 1, string joinCode = "ABC123", string hostId = "host123")
    {
        return new QuizLobby
        {
            Id =id,
            QuizId = 1,
            JoinCode = joinCode,
            QuizHostId = hostId,
            IsActive = true
        };
    }

    public LobbyParticipant CreateTestLobbyParticipant(string connectionId = "conn-123", int lobbyId = 1, string nickname = "Player1", int score = 0)
    {
        return new LobbyParticipant
        {
            ConnectionId = connectionId,
            LobbyId = lobbyId,
            Nickname = nickname,
            Score = score
        };
    }

    public void Reset()
    {
        MockLobbyService.Reset();
        MockLobbyParticipantManager.Reset();
        MockGroups.Reset();
        MockClientProxy.Reset();
        MockCallerProxy.Reset();
    }
}
