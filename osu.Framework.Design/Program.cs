using System;

namespace osu.Framework.Design
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var host = Host.GetSuitableHost("osu-design", bindIPC: false))
            using (var game = new DesignGame())
                host.Run(game);
        }
    }
}
