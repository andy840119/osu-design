using System.IO.Abstractions;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Designer;
using osu.Framework.Design.Solution;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace osu.Framework.Design
{
    public class DesignGame : Game
    {
        readonly string _projectPath;

        public DesignGame(string projectPath = null)
        {
            _projectPath = projectPath;

            Name = "osu!designer";
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.Title = Name;
        }

        IFileSystem _fileSystem;
        Workspace _workspace;

        DependencyContainer _dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        void load()
        {
            // Cache dependencies
            _dependencies.CacheAs<IFileSystem>(_fileSystem = new FileSystem());
            _dependencies.Cache(_workspace = new Workspace());

            _workspace.Directory.Value = _fileSystem.DirectoryInfo.FromDirectoryName(
                _fileSystem.Path.GetDirectoryName(_projectPath)
            );
        }

        Screen _rootScreen;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(_rootScreen = new WorkspaceScreen());

            // Close host when root screen exits
            _rootScreen.Exited += _ => Host.Exit();
        }
    }
}