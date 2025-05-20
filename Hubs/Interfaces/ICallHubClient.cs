using morse_service.DTO;

namespace morse_service.Hubs.Interfaces
{
    public interface ICallHubClient
    {
        Task ReceiveUserSearchResult(object[] users);

        Task ReceiveCreatedRoomData(string roomCode);

        Task ReceiveJoinRoomError(string error);

        Task ReceiveNewParticipant(string user);

        Task ReceiveParticipantLeft(string user);

        Task ReceiveWebRTCSignal(string senderConnectionId, string signal);

        Task ReceiveIceCandidate(string senderConnectionId, string candidate);

        Task ReceiveParticipantsForWebRTC(List<string> connectionIds);
    }
}
