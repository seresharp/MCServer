using MCServer.Server;

namespace MCServer;

public static class Program
{
    public static async Task Main()
    {
        Util.CompactLongArray biomes = new(6, 64);
        for (int i = 0; i < 64; i++)
        {
            // desert
            biomes[i] = 5;
        }
        
        for (int i = 0; i < 64; i++)
        {
            Console.WriteLine(biomes[i]);
        }

        foreach (long l in biomes.Longs)
        {
            Console.WriteLine(l);
            byte[] bytes = Util.BitConversion.GetBytes(l, true);
            string[] binary = bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
            Console.WriteLine(string.Join(' ', binary));
        }

        await new MinecraftServer().Run();
    }
}
