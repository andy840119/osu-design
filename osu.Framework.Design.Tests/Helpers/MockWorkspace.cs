using System.IO.Abstractions.TestingHelpers;
using osu.Framework.Design.Workspaces;

namespace osu.Framework.Design.Tests.Helpers
{
    public class MockWorkspace : Workspace
    {
        public MockWorkspace() : base(new MockFileSystem
        {
            FileSystemWatcher = new MockFileSystemWatcherFactory()
        }.GetCurrentDirectory())
        {
        }
    }
}