using System.IO.Abstractions;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Designer;
using osu.Framework.Design.Solution;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

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
            Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore("osu.Framework.Design.dll"), "Resources"));

            // Load fonts
            var fonts = new FontStore(new GlyphStore(Resources, "Fonts/Nunito"));
            _dependencies.Cache(fonts);

            fonts.AddStore(new GlyphStore(Resources, "Fonts/Nunito-Bold"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Nunito-Italic"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Nunito-BoldItalic"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Inconsolata"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Inconsolata-Bold"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Inconsolata-Italic"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Inconsolata-BoldItalic"));
            fonts.AddStore(new GlyphStore(Resources, "Fonts/Consolas"));

            // Global workspace
            _dependencies.CacheAs<IFileSystem>(_fileSystem = new FileSystem());
            _dependencies.Cache(_workspace = new Workspace());

            _workspace.Directory.Value = _fileSystem.DirectoryInfo.FromDirectoryName(
                _fileSystem.Path.GetDirectoryName(_projectPath)
            );
        }

        protected override void Update()
        {
            base.Update();

            // We need to update the workspace manually because all filesystem events are scheduled
            _workspace.Scheduler.Update();
        }

        DrawSizePreservingFillContainer _content;
        Screen _rootScreen;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Aspect ratio container
            Child = _content = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1440, 1080),
                Strategy = DrawSizePreservationStrategy.Minimum
            };

            _content.Add(_rootScreen = new WorkspaceScreen());

            // Close host when root screen exits
            _rootScreen.Exited += _ => Host.Exit();
        }
    }
}