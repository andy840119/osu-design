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

                foreach (var folder in doc.Folders)
                {
                    var flow = folderFlow.FirstOrDefault(i => i.Name == folder.Name);

                    if (flow == null)
                        folderFlow.Add(flow = new Item(folder.Name, null));

                    folderFlow = flow;
                }

                folderFlow.Add(new Item(doc.Name, doc));
            }
        }

        void handleRemoved(IEnumerable<Document> docs)
        {
        }

        public sealed class Item : Container<Item>
        {
            readonly Container<Item> _content;
            readonly Drawable _head;

            readonly Drawable _background;
            readonly Drawable _highlight;

            protected override Container<Item> Content => _content;

            readonly Document _doc;

            public Item(string name, Document doc)
            {
                Name = name;
                _doc = doc;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChild = new FillFlowContainer
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
                                _highlight = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Green.Opacity(0.5f),
                                    Alpha = 0
                                },
                                _background = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.White.Opacity(0.2f),
                                    Alpha = 0
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
                                            TextSize = 20
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
                                Left = 20
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

                _background.FadeIn(30);

                return true;
            }
            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                _background.FadeOut(200);
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