using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Solution;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.MathUtils;
using osuTK.Graphics;

namespace osu.Framework.Design.Designer
{
    public class SolutionBrowser : CompositeDrawable
    {
        readonly FillFlowContainer<Item> _flow;

        public SolutionBrowser()
        {
            InternalChildren = new Drawable[]
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
                        Direction = FillDirection.Vertical
                    }
                }
            };
        }

        BindableList<Document> _documents;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _documents = workspace.Documents.GetBoundCopy();
            _documents.ItemsAdded += handleAdded;
            _documents.ItemsRemoved += handleRemoved;

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
                        folderFlow.Add(flow = new Item(folder.Name, null, level++));

                    folderFlow = flow;
                }

                folderFlow.Add(new Item(doc.Name, doc, level));
            }
        }

        void handleRemoved(IEnumerable<Document> docs)
        {
        }

        public sealed class Item : Container<Item>
        {
            readonly Container<Item> _content;
            readonly Drawable _head;

            readonly Drawable _hover;
            readonly Drawable _highlight;

            readonly Drawable _hierarchyMarker;

            protected override Container<Item> Content => _content;

            readonly Document _doc;

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

            public Item(string name, Document doc, int level)
            {
                Name = name;
                _doc = doc;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChildren = new Drawable[]
                {
                    _hierarchyMarker = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Colour = folderColours[level % folderColours.Length],
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
                                    _highlight = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 5,
                                        Alpha = 0,
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = DesignerColours.Highlight.Opacity(0.6f)
                                        }
                                    },
                                    _hover = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 5,
                                        Alpha = 0,
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Color4.White.Opacity(0.1f)
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Horizontal,
                                        Padding = new MarginPadding(3),
                                        Children = new Drawable[]
                                        {
                                            new SpriteText
                                            {
                                                Text = Name,
                                                TextSize = 18,
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
                                }
                            }
                        }
                    }
                };
            }

            Bindable<Document> _document { get; set; }

            [BackgroundDependencyLoader]
            void load(Workspace workspace)
            {
                _document = workspace.CurrentDocument.GetBoundCopy();
                _document.BindValueChanged(d => Highlight(_doc != null && d == _doc), true);
            }

            protected override bool OnHover(HoverEvent e)
            {
                base.OnHover(e);

                _hover.FadeIn(30);

                if (_doc == null)
                    _hierarchyMarker.FadeIn(30);

                return _doc == null;
            }
            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                _hover.FadeOut(200);

                if (_doc == null)
                    _hierarchyMarker.FadeOut(200);
            }

            bool _collapsed;

            protected override bool OnClick(ClickEvent e)
            {
                base.OnClick(e);

                if (_doc == null)
                {
                    if (_collapsed)
                        _content.FadeIn(200);
                    else
                        _content.FadeOut(30);

                    _collapsed = !_collapsed;
                }
                else
                    _document.Value = _doc;

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