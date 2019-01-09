using System;
using System.Linq;

namespace osu.Framework.Design
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var host = Host.GetSuitableHost("osu-design", bindIPC: false))
            using (var game = new DesignGame(args.FirstOrDefault()))
                host.Run(game);
        }
    }
}
