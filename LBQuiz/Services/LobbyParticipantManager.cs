using System.Collections.Concurrent;
using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;

namespace LBQuiz.Services;

public class LobbyParticipantManager : ILobbyParticipantManager
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, LobbyParticipant>> _lobbyParticipants = new();
    private readonly ConcurrentDictionary<string, int> _connectionToLobby = new();

    public bool AddParticipant(int lobbyId, LobbyParticipant participant)
    {
        var participants = _lobbyParticipants.GetOrAdd(lobbyId, _ => new ConcurrentDictionary<string, LobbyParticipant>());

        // Add participant to lobby
        if (participants.TryAdd(participant.ConnectionId, participant))
        {
            _connectionToLobby.TryAdd(participant.ConnectionId, lobbyId);
            return true;
        }

        return false;
    }

    // Get all participants in a lobby
    public List<LobbyParticipant> GetParticipants(int lobbyId)
    {
        if (_lobbyParticipants.TryGetValue(lobbyId, out var participants))
        {
            return participants.Values.ToList();
        }

        return new List<LobbyParticipant>();
    }

    // Get leaderboard for a lobby, sorted by score
    public List<LobbyParticipant> GetLeaderboard(int lobbyId)
    {
        return GetParticipants(lobbyId).OrderByDescending(p => p.Score).ToList();
    }
}