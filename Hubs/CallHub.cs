using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using morse_service.Hubs.Interfaces;
using morse_service.Interfaces.Services;
using morse_service.DTO;

namespace morse_service.Hubs
{
    public class CallHub : Hub<ICallHubClient>
    {
        private readonly IUserService _userService;
        private static readonly ConcurrentDictionary<string, List<string>> _rooms = new();
        private static readonly ConcurrentDictionary<string, string> _connectionUserMap = new();

        public CallHub(IUserService userService)
        {
            _userService = userService;
        }

        public async Task FindUserByLogin(int senderUserId, string login)
        {
            await Clients.Caller.ReceiveUserSearchResult(_userService.FindUserByLoginExcludingRequestSource(senderUserId, login));
        }

        public async Task<string> CreateRoom(string login)
        {
            var roomCode = GenerateRoomCode();
            _rooms.TryAdd(roomCode, new List<string> { Context.ConnectionId });
            _connectionUserMap[Context.ConnectionId] = login;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.ReceiveCreatedRoomData(roomCode);

            var user = _userService.FindUserByLogin(login);
            string data = $"{user?.Login}#{user?.Id}";
            await Clients.Group(roomCode).ReceiveNewParticipant(data);

            return roomCode;
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> JoinRoom(string roomCode, string login)
        {
            try
            {
                if (string.IsNullOrEmpty(roomCode) || string.IsNullOrEmpty(login))
                {
                    await Clients.Caller.ReceiveJoinRoomError("Invalid room code or login");
                    return false;
                }

                if (!_rooms.TryGetValue(roomCode, out var participants))
                {
                    await Clients.Caller.ReceiveJoinRoomError("Room not found");
                    return false;
                }

                var user = _userService.FindUserByLogin(login);
                if (user == null)
                {
                    await Clients.Caller.ReceiveJoinRoomError("User not found");
                    return false;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                participants.Add(Context.ConnectionId);
                _connectionUserMap[Context.ConnectionId] = login;

                // Отправляем новому участнику информацию о всех текущих участниках
                var currentParticipants = participants
                    .Where(cid => cid != Context.ConnectionId)
                    .Select(cid =>
                    {
                        _connectionUserMap.TryGetValue(cid, out var userLogin);
                        var existingUser = _userService.FindUserByLogin(userLogin);
                        return existingUser != null ? $"{existingUser.Login}#{existingUser.Id}" : null;
                    })
                    .Where(x => x != null)
                    .ToList();

                foreach (var participant in currentParticipants)
                {
                    await Clients.Caller.ReceiveNewParticipant(participant);
                }

                // Отправляем информацию о новом участнике всем в комнате
                string data = $"{user.Login}#{user.Id}";
                await Clients.Group(roomCode).ReceiveNewParticipant(data);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining room: {ex}");
                await Clients.Caller.ReceiveJoinRoomError("Internal server error");
                return false;
            }
        }

        public async Task LeaveRoom(string roomCode)
        {
            if (_rooms.TryGetValue(roomCode, out var participants))
            {
                _connectionUserMap.TryGetValue(Context.ConnectionId, out var login);

                participants.Remove(Context.ConnectionId);
                _connectionUserMap.TryRemove(Context.ConnectionId, out _);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

                if (!string.IsNullOrEmpty(login))
                {
                    var user = _userService.FindUserByLogin(login);
                    if (user != null)
                    {
                        string data = $"{user.Login}#{user.Id}";
                        await Clients.Group(roomCode).ReceiveParticipantLeft(data);
                    }
                }

                if (participants.Count == 0)
                {
                    _rooms.TryRemove(roomCode, out _);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _connectionUserMap.TryGetValue(connectionId, out var login);

            foreach (var room in _rooms.Where(r => r.Value.Contains(connectionId)))
            {
                room.Value.Remove(connectionId);

                if (login != null)
                {
                    var user = _userService.FindUserByLogin(login);
                    if (user != null)
                    {
                        string data = $"{user.Login}#{user.Id}";
                        await Clients.Group(room.Key).ReceiveParticipantLeft(data);
                    }
                }

                if (room.Value.Count == 0)
                {
                    _rooms.TryRemove(room.Key, out _);
                }
            }

            _connectionUserMap.TryRemove(connectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendWebRTCSignal(string roomCode, string targetConnectionId, string signal)
        {
            await Clients.Client(targetConnectionId).ReceiveWebRTCSignal(Context.ConnectionId, signal);
        }

        public async Task SendIceCandidate(string roomCode, string targetConnectionId, string candidate)
        {
            await Clients.Client(targetConnectionId).ReceiveIceCandidate(Context.ConnectionId, candidate);
        }

        public async Task GetParticipantsForWebRTC(string roomCode, string connectionId)
        {
            if (_rooms.TryGetValue(roomCode, out var participants))
            {
                var otherParticipants = participants
                    .Where(p => p != connectionId)
                    .ToList();

                await Clients.Client(connectionId).ReceiveParticipantsForWebRTC(otherParticipants);
            }
        }
    }
}