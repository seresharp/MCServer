using MCServer.Server;

namespace MCServer;

public static class Program
{
    public static async Task Main()
    {
        await new MinecraftServer().Run();
    }
}
