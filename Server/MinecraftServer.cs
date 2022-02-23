using System.Net;
using System.Net.Sockets;
using MCServer.Client;
using MCServer.Client.Packets;
using MCServer.Server.Packets;
using MCServer.Util;
using MCServer.Assets;

namespace MCServer.Server;

public class MinecraftServer : IDisposable
{
    private TcpListener? _server;

    private readonly List<MinecraftClient> _clients = new();

    public const string GAME_VERSION = "1.18.1";
    public const int PROTOCOL_VERSION = 757;

    // config
    // TODO: read from file
    public int Port { get; init; } = 25575;
    public int MaxPlayers { get; init; } = 20;
    public int ViewDistance { get; init; } = 10;
    public int SimulationDistance { get; init; } = 10;
    public string Description { get; init; } = "gaming";

    // json
    public GenericLoader<Item> Items { get; init; }
    public GenericLoader<Block> Blocks { get; init; }
    public GenericLoader<Entity> Entities { get; init; }
    public GenericLoader<Dimension> Dimensions { get; init; }
    public GenericLoader<Biome> Biomes { get; init; }
    public TagLoader Tags { get; init; }

    public MinecraftServer()
    {
        Items = new("./Resources/items.json");
        Log($"Loaded {Items.Count} items");

        Blocks = new("./Resources/blocks.json");
        Log($"Loaded {Blocks.Count} blocks");

        Entities = new("./Resources/entities.json");
        Log($"Loaded {Entities.Count} entities");

        Dimensions = new("./Resources/dimensions.json");
        Log($"Loaded {Dimensions.Count} dimensions");

        Biomes = new("./Resources/biomes.json");
        Log($"Loaded {Biomes.Count} biomes");

        Tags = new("./Resources/tags.json");
        Log($"Loaded {Tags.Types.Select(t => Tags[t].Count()).Sum()} tags");
    }

    public async Task Run()
    {
        Log($"Starting server on port {Port}...");
        _server = new(IPAddress.Any, Port);
        _server.Start();

        Log("Listening for client connections...");

        // this is extremely not how you're supposed to use tasks, change at some point
        Task keepAlive = Task.Run(SendKeepAlivePackets);

        while (true)
        {
            MinecraftClient newClient = new(await _server.AcceptTcpClientAsync());

            newClient.PacketReceived += HandlePacket;
            newClient.Disconnect += ClientDisconnected;
            newClient.StartReadWrite();

            lock (_clients)
            {
                _clients.Add(newClient);
            }
        }
    }

    public static void Log(object obj)
        => Console.WriteLine(obj);

    private async Task SendKeepAlivePackets()
    {
        while (true)
        {
            await Task.Delay(5000);
            lock (_clients)
            {
                foreach (MinecraftClient client in _clients
                    .Where(c => c.State == ClientState.Play))
                {
                    client.QueuePacket(new PongPacket(0x21, Environment.TickCount64));
                }
            }
        }
    }

    private void HandlePacket(MinecraftClient client, ClientPacket packet)
    {
        ServerPacket resp;

        switch (packet)
        {
            case HandshakePacket handshake:
                client.State = (ClientState)handshake.NextState;
                break;
            case StatusRequestPacket:
                resp = new StatusResponsePacket(GAME_VERSION, PROTOCOL_VERSION, Description, MaxPlayers);
                client.QueuePacket(resp);
                break;
            case PingPacket ping when client.State != ClientState.Play:
                resp = new PongPacket(ping.Id, ping.Payload);
                client.QueuePacket(resp);
                break;
            case PingPacket keepAlive when client.State == ClientState.Play:
                // TODO: validate payload
                break;
            case LoginStartPacket loginStart:
                // TODO: authentication and encryption
                resp = new LoginSuccessPacket($"OfflinePlayer:{loginStart.Username}", loginStart.Username);
                client.QueuePacket(resp);

                client.State = ClientState.Play;

                client.QueuePacket(new JoinGamePacket(this));
                client.QueuePacket(new PluginMessagePacket("minecraft:brand", "chad_serena_mc".GetBytes(true)));
                client.QueuePacket(new TagsPacket(this));
                client.QueuePacket(new CommandsPacket());
                client.QueuePacket(new DeclareRecipesPacket());
                client.QueuePacket(new UnlockRecipesPacket());
                client.QueuePacket(new PlayerInfoPacket());

                for (int chunkX = -4; chunkX <= 4; chunkX++)
                {
                    for (int chunkZ = -4; chunkZ <= 4; chunkZ++)
                    {
                        World.ChunkColumn chunk = new(384, chunkX, chunkZ);

                        if (chunkX == 0 && chunkZ == 0)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                for (int z = 0; z < 16; z++)
                                {
                                    chunk[x, 0, z] = new World.BlockState(x == z || (15 - x) == z ? Blocks["red_wool"] : Blocks["green_wool"]);
                                }
                            }
                        }

                        client.QueuePacket(new ChunkDataPacket(chunk));
                    }
                }

                client.QueuePacket(new SpawnPositionPacket());
                client.QueuePacket(new Packets.PlayerPositionAndLookPacket());
                client.QueuePacket(new TimeUpdatePacket());
                client.QueuePacket(new ChangeGameStatePacket());
                client.QueuePacket(new WindowItemsPacket());
                break;
            case Client.Packets.PlayerPositionAndLookPacket pos:
                client.Player.UpdatePosition(pos);
                break;
            default:
                throw new NotImplementedException(packet.GetType().FullName);
        }
    }

    private void ClientDisconnected(MinecraftClient client)
    {
        client.PrintError();
        client.Dispose();

        lock (_clients)
        {
            _clients.Remove(client);
        }
    }

    public void Dispose()
    {
        foreach (MinecraftClient client in _clients)
        {
            client.Dispose();
        }

        _server?.Stop();
        GC.SuppressFinalize(this);
    }
}
