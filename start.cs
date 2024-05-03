using System;
using System.IO;
using System.Text;

namespace MagmaMC.PSC
{
    internal static class start
    {
        internal static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string data = File.ReadAllText("Permissions.PSC");
            PSC Config = new PSC(data);

            foreach (string layer in Config.GetPlayers("VIP"))
                Console.WriteLine(layer);
            Console.WriteLine("----");
            Config.AddPlayer("Exampleuser", "VIP");
            foreach (string layer in Config.GetPlayers("VIP"))
                Console.WriteLine(layer);
        }
    }
}
