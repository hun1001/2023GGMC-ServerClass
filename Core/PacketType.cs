namespace Core;

public enum PacketType
{
    LoginRequest,
    LoginResponse,
    CreateRoomRequest,
    CreateRoomResponse,
    RoomListRequest,
    RoomListResponse,
    EnterRoomRequset,
    EnterRoomResponse,
    UserEnter,
    UserLeave,
    Chat,
    Duplicate,
    Heartbeat
}
