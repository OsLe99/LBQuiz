using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LBQuiz.Services;

public class LobbyParticipantManager : ILobbyParticipantManager
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, LobbyParticipant>> _lobbyParticipants = new();
    private readonly ConcurrentDictionary<string, int> _connectionToLobby = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, string>> AnswerDictionary = new();
    
    public bool AddParticipant(int lobbyId, LobbyParticipant participant)
    {
        if (lobbyId <= 0)
        {
            throw new ArgumentException("Lobby Id must be greater than 0", nameof(lobbyId));
        }

        if (participant == null)
        {
            throw new ArgumentNullException(nameof(participant));
        }
        
        if (string.IsNullOrWhiteSpace(participant.ConnectionId))
        {
            throw new ArgumentException("ConnectionId cannot be null or empty", nameof(participant));
        }

        if (string.IsNullOrWhiteSpace(participant.Nickname))
        {
            throw new ArgumentException("Nickname cannot be null or empty", nameof(participant));
        }
        
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
    
    // Remove participant from lobby
    public LobbyParticipant? RemoveParticipantByConnectionId(string connectionId)
    {
        if (_connectionToLobby.TryRemove(connectionId, out var lobbyId))
        {
            if (_lobbyParticipants.TryGetValue(lobbyId, out var participants))
            {
                if (participants.TryRemove(connectionId, out var participant))
                {
                    return participant;
                }
            }
        }
        
        return null;
    }
    public LobbyParticipant GetLobbyParticipant(string connectionId)
    {
        if (_connectionToLobby.TryGetValue(connectionId, out var lobbyId))
        {
            if (_lobbyParticipants.TryGetValue(lobbyId, out var participants))
            {
                if (participants.TryGetValue(connectionId, out var participant))
                {
                    return participant;
                }
            }
        }
        throw new KeyNotFoundException("Participant not found");
    }
    public async Task SubmitParticipantAnswer(string connectionId, string answer, int questionId)
    {
        var lobbyParticipant = GetLobbyParticipant(connectionId);
        var dict = new ConcurrentDictionary<int, string>();
        if (dict.TryAdd(questionId, answer))
        {
            AnswerDictionary.TryAdd(connectionId, dict);
        }
    }
    public bool UpdateParticipantConnectionId(string oldConnectionId, string newConnectionId)
    {
        if (_connectionToLobby.TryRemove(oldConnectionId, out var lobbyId))
        {
            if (_lobbyParticipants.TryGetValue(lobbyId, out var participants))
            {
                if (participants.TryRemove(oldConnectionId, out var participant))
                {
                    participant.ConnectionId = newConnectionId;
                    if (participants.TryAdd(newConnectionId, participant))
                    {
                        _connectionToLobby.TryAdd(newConnectionId, lobbyId);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public async Task<bool> CheckAnswer(string correctAnswer, int questionId, string connectionId)
    {
        
        if(AnswerDictionary.TryGetValue(connectionId, out var participantAnswers))
        {
            if(participantAnswers.TryGetValue(questionId, out var participantAnswer))
            {
                if(correctAnswer == participantAnswer)
                {
                    return true;
                }
            }
        }
        return false;
    }
}