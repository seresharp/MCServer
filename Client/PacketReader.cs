using System.Diagnostics.CodeAnalysis;
using MCServer.Client.Packets;

namespace MCServer.Client;

public class PacketReader<T> : IPacketReader where T : ClientPacket, new()
{
    public int Id { get; init; }

    public ClientState State { get; init; }

    public PacketReader(int packetId, ClientState state)
    {
        Id = packetId;
        State = state;
    }

    public bool TryReadPacket(int packetId, ClientState state, byte[] data, [NotNullWhen(true)] out T? packet)
    {
        if (packetId != Id || state != State)
        {
            packet = null;
            return false;
        }

        packet = new T();
        packet.ReadData(Id, data);
        return true;
    }

    bool IPacketReader.TryReadPacket(int packetId, ClientState state, byte[] data, [NotNullWhen(true)] out ClientPacket? packet)
    {
        // for some reason doing 'out packet' doesn't work
        bool success = TryReadPacket(packetId, state, data, out T? p);
        packet = p;
        return success;
    }
}

public interface IPacketReader
{
    bool TryReadPacket(int packetId, ClientState state, byte[] data, [NotNullWhen(true)] out ClientPacket? packet);
}
