using System.Collections.Concurrent;
using System.Net.Sockets;
using MCServer.Client.Packets;
using Newtonsoft.Json;

namespace MCServer.Client;

public class MinecraftClient : IDisposable
{
    private readonly TcpClient Tcp;
    private readonly NetworkStream Stream;

    private readonly byte[] Buffer = new byte[1024];
    private int _bufferPos;

    private Task? _readWriteTask;
    private CancellationTokenSource? _cancel;

    private readonly ConcurrentQueue<Server.Packets.ServerPacket> PacketQueue = new();

    public int State { get; set; } = 0;

    public MinecraftClient(TcpClient tcpClient)
    {
        Tcp = tcpClient;
        Stream = Tcp.GetStream();
    }

    public void StartReadWrite()
    {
        _cancel?.Cancel();

        _cancel = new();
        _readWriteTask = Task.Run(Run, _cancel.Token);
    }

    public async Task StopReadWrite()
    {
        _cancel?.Cancel();

        // just in case cancellation can't occur immediately
        // it usually will
        while (_readWriteTask?.IsCompleted is false)
        {
            await Task.Delay(50);
        }
    }

    public void QueuePacket(Server.Packets.ServerPacket packet)
    {
        PacketQueue.Enqueue(packet);
    }

    public void PrintError()
    {
        if (_readWriteTask?.Exception != null)
        {
            Console.WriteLine(_readWriteTask.Exception);
        }
    }

    private async Task Run()
    {
        try
        {
            Task<int>? readTask = null;
            Task? writeTask = null;
            CancellationTokenSource source = new();
            while (true)
            {
                if (readTask?.IsCompleted is null or true)
                {
                    readTask = Stream.ReadAsync(Buffer.AsMemory(_bufferPos), source.Token).AsTask();
                }

                if (writeTask?.IsCompleted is null or true)
                {
                    if (PacketQueue.TryDequeue(out Server.Packets.ServerPacket? packet) && packet != null)
                    {
                        writeTask = Stream.WriteAsync(packet.Build(), source.Token).AsTask();
                    }
                    else
                    {
                        writeTask = Task.Delay(50);
                    }
                }

                Task t = await Task.WhenAny(readTask, writeTask);
                if (t.Exception != null)
                {
                    source.Cancel();
                    throw t.Exception;
                }

                if (t == readTask)
                {
                    int dataRead = readTask.Result;
                    if (dataRead == 0)
                    {
                        source.Cancel();
                        break;
                    }

                    _bufferPos += dataRead;
                    while (ParsePacket()) ;
                }
                else
                {
                    // packet write. I don't think this requires any handling, maybe wrong?
                }
            }
        }
        finally
        {
            try
            {
                Disconnect?.Invoke(this);
            }
            catch (Exception e)
            {
                Server.MinecraftServer.Log("Error in disconnect handler:\n" + e);
            }
        }
    }

    private bool ParsePacket()
    {
        int len;
        try
        {
            len = ClientPacket.GetPacketLength(Buffer, 0);

            if (len > Buffer.Length)
            {
                throw new InvalidDataException();
            }
        }
        catch (InvalidDataException)
        {
            // can safely advance, this is (probably) an invalid packet
            Server.MinecraftServer.Log($"WARNING: skipping byte {Buffer[0]:X}");
            Array.Copy(Buffer, 1, Buffer, 0, --_bufferPos);
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            // not enough bytes in buffer to make a decision yet
            return false;
        }

        if (len > _bufferPos)
        {
            // not enough bytes again
            return false;
        }

        if (ClientPacket.TryParse(Buffer[0..len], State, out ClientPacket? packet, out string error))
        {
            Console.Write("Received " + packet.GetType().Name + ": ");
            JsonSerializer.CreateDefault().Serialize(Console.Out, packet);
            Console.WriteLine();

            // don't kill the client if the server throws on handling a packet
            try
            {
                PacketReceived?.Invoke(this, packet);
            }
            catch (Exception e)
            {
                Server.MinecraftServer.Log("Error in packet received handler:\n" + e);
            }
        }
        else
        {
            Console.WriteLine(error);
        }

        // shift packet left out of the buffer
        Array.Copy(Buffer, len, Buffer, 0, _bufferPos - len);
        _bufferPos -= len;

        return true;
    }

    public void Dispose()
    {
        Disconnect = null;
        PacketReceived = null;

        Task.Run(StopReadWrite);
        Stream?.Dispose();
        Tcp?.Dispose();

        GC.SuppressFinalize(this);
    }

    // TODO: create bodies for these with try/catch
    public event Action<MinecraftClient>? Disconnect;
    public event Action<MinecraftClient, ClientPacket>? PacketReceived;
}
