using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Workspaces;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Design.Designer
{
    public class SolutionBrowser : ComponentWindow
    {
        public Action<Document> OpenDocument;

        FillFlowContainer<Item> _flow;

        public SolutionBrowser() : base("Explorer")
        {
        }

        BindableList<Document> _documents;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _documents = workspace.Documents.GetBoundCopy();
            _documents.ItemsAdded += handleAdded;
            _documents.ItemsRemoved += handleRemoved;

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = DesignerColours.Side
                },
                new ScrollContainer(Direction.Vertical)
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = _flow = new FillFlowContainer<Item>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding(5)
                    }
                }
            };

            handleAdded(_documents);
        }

        void handleAdded(IEnumerable<Document> docs)
        {
            foreach (var doc in docs)
            {
                Container<Item> folderFlow = _flow;
                var level = 0;

                foreach (var folder in doc.Folders)
                {
                    var flow = folderFlow.FirstOrDefault(i => i.Name == folder.Name);

                    if (flow == null)
                        folderFlow.Add(flow = new Item(folder.Name, null, level, null));

                    folderFlow = flow;
                    level++;
                }

                folderFlow.Add(new Item(doc.Name, doc, level, handleClick));
            }
        }

        Item _clickedItem;

        void handleClick(Item item)
        {
            // Unhighlight last item
            _clickedItem?.Highlight(false);

            _clickedItem = item;

            if (item != null)
            {
                item.Highlight(true);
                OpenDocument?.Invoke(item.Document);
            }
        }

        void handleRemoved(IEnumerable<Document> docs)
        {
            // TODO: Remove handling
        }

        public sealed class Item : Container<Item>
        {
            static readonly Color4[] folderColours = new[]
            {
                Color4.Red,
                Color4.Orange,
                Color4.Yellow,
                Color4.Green,
                Color4.Blue,
                Color4.White,
                Color4.Purple
            };

            Container<Item> _content;
            Drawable _head;
            Sprite _icon;

            Drawable _hover;
            Drawable _highlight;
            Drawable _hierarchyMarker;

            protected override Container<Item> Content => _content;

            public readonly Document Document;
            readonly Action<Item> _click;
            readonly Color4 _markerColour;

            public Item(string name, Document doc, int level, Action<Item> click)
            {
                Name = name;
                Document = doc;
                _click = click;
                _markerColour = folderColours[level % folderColours.Length].Opacity(0.5f);

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
            }

            Texture _tex;
            Texture _texOpen;

            [BackgroundDependencyLoader]
            void load(TextureStore textures)
            {
                string tex;

                if (Document == null)
                {
                    switch (Name.ToLowerInvariant())
                    {
                        case "bin": tex = "folder_type_binary"; break;
                        case "src": tex = "folder_type_src"; break;
                        case "temp":
                        case ".temp":
                        case "tmp":
                        case ".tmp": tex = "folder_type_temp"; break;
                        case "test":
                        case "tests": tex = "folder_type_test"; break;
                        case "tools":
                        case ".tools": tex = "folder_type_tools"; break;
                        case ".vscode": tex = "folder_type_vscode"; break;
                        default: tex = "default_folder"; break;
                    }
                }
                else
                {
                    switch (Document.File.Name.ToLowerInvariant())
                    {
                        case "license": tex = "file_type_license"; break;
                        case "launch.json": tex = "file_type_vscode"; break;
                        case "tasks.json": tex = "file_type_vscode"; break;
                        case "settings.json": tex = "file_type_vscode"; break;
                    }
                    switch (Document.File.Extension.ToLowerInvariant())
                    {
                        case ".mp3":
                        case ".ogg": tex = "file_type_audio"; break;
                        case ".a":
                        case ".bin":
                        case ".exe":
                        case ".dll": tex = "file_type_binary"; break;
                        case ".config": tex = "file_type_config"; break;
                        case ".cs": tex = "file_type_csharp"; break;
                        case ".csproj": tex = "file_type_csproj"; break;
                        case ".diff": tex = "file_type_diff"; break;
                        case ".gitignore":
                        case ".gitattributes": tex = "file_type_git"; break;
                        case ".json": tex = "file_type_json"; break;
                        case ".md":
                        case ".markdown": tex = "file_type_markdown"; break;
                        case ".text":
                        case ".txt": tex = "file_type_text"; break;
                        case ".vb": tex = "file_type_vb"; break;
                        case ".vbproj": tex = "file_type_vbproj"; break;
                        case ".avi":
                        case ".mp4": tex = "file_type_video"; break;
                        case ".jpg":
                        case ".png":
                        case ".tiff": tex = "file_type_image"; break;
                        case ".osuml": tex = "file_type_view"; break;
                        case ".xml": tex = "file_type_xml"; break;
                        default: tex = "default_file"; break;
                    }
                }

                _tex = textures.Get(tex);
                _texOpen = textures.Get($"{tex}_opened");

                InternalChildren = new Drawable[]
                {
                    _hierarchyMarker = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Colour = _markerColour,
                        Alpha = 0
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            _head = new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 5,
                                        Children = new[]
                                        {
                                            _highlight = new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = DesignerColours.Highlight.Opacity(0.4f),
                                                Alpha = 0
                                            },
                                            _hover = new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Color4.White.Opacity(0.04f),
                                                Alpha = 0
                                            }
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Horizontal,
                                        Padding = new MarginPadding(4),
                                        Spacing = new Vector2(5, 0),
                                        Children = new Drawable[]
                                        {
                                            _icon = new Sprite
                                            {
                                                Size = new Vector2(18),
                                                FillMode = FillMode.Fit,
                                                Texture = _tex
                                            },
                                            new SpriteText
                                            {
                                                Text = Name,
                                                TextSize = 18,
                                                Font = "Nunito",
                                                Colour = DesignerColours.SideForeground
                                            }
                                        }
                                    }
                                }
                            },
                            _content = new FillFlowContainer<Item>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding
                                {
                                    Left = 18
                                },
                                Alpha = 0
                            }
                        }
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                base.OnHover(e);

                _hover.FadeIn(30);

                if (Document == null)
                    _hierarchyMarker.FadeIn(30);

                return Document == null;
            }
            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                _hover.FadeOut(200);

                if (Document == null)
                    _hierarchyMarker.FadeOut(200);
            }

            bool _collapsed = true;

            protected override bool OnClick(ClickEvent e)
            {
                base.OnClick(e);

                if (Document == null)
                {
                    if (_collapsed)
                    {
                        _content.FadeIn(200);
                        _icon.Texture = _texOpen ?? _tex;
                    }
                    else
                    {
                        _content.FadeOut(30);
                        _icon.Texture = _tex;
                    }

                    _collapsed = !_collapsed;
                }
                else
                    _click?.Invoke(this);

                return true;
            }

            public void Highlight(bool toggled)
            {
                if (toggled)
                    _highlight.FadeIn(30);
                else
                    _highlight.FadeOut(200);
            }
        }
    }
}