using LogicSequencer.UI.Components;
using RichHudFramework;
using RichHudFramework.UI;
using RichHudFramework.UI.Client;
using RichHudFramework.UI.Rendering;
using Sandbox.ModAPI;
using VRageMath;

namespace LogicSequencer.UI
{
    public partial class SequenceEditor : WindowBase
    {
        Script.ScriptSequence _Script;
        public Script.ScriptSequence Script { get { return _Script; } set { _Script = value; Load(); } }

        //public SequencePart SelectedSequencePart => sequenceList.SelectedSequencePart;
        //public SequencePageBase SelectedPage => sequenceList.SelectedPage;

        private readonly SequenceList sequenceList;
        private readonly HudChain bodyChain;
        private readonly TexturedBox topDivider, middleDivider, bottomDivider;
        private readonly Button closeButton;
        //private SequencePageBase lastPage;
        private static readonly Material closeButtonMat = new Material("RichHudCloseButton", new Vector2(32f));

        public SequenceEditor(HudParentBase parent) : base(parent)
        {
            HeaderBuilder.Format = TerminalFormatting.HeaderFormat;
            HeaderBuilder.SetText("Logic Sequence Editor");

            header.Height = 60f;

            topDivider = new TexturedBox(header)
            {
                ParentAlignment = ParentAlignments.Bottom,
                DimAlignment = DimAlignments.Width,
                Padding = new Vector2(80f, 0f),
                Height = 1f,
            };

            sequenceList = new SequenceList()
            {
                Width = 270f
            };

            middleDivider = new TexturedBox()
            {
                Padding = new Vector2(24f, 0f),
                Width = 26f,
            };

            bodyChain = new HudChain(false, topDivider)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis | HudChainSizingModes.ClampChainBoth,
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.Left | ParentAlignments.InnerH,
                Padding = new Vector2(80f, 40f),
                Spacing = 12f,
                CollectionContainer = { sequenceList, middleDivider },
            };

            bottomDivider = new TexturedBox(this)
            {
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV,
                DimAlignment = DimAlignments.Width,
                Offset = new Vector2(0f, 40f),
                Padding = new Vector2(80f, 0f),
                Height = 1f,
            };

            closeButton = new Button(header)
            {
                Material = closeButtonMat,
                HighlightColor = Color.White,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Right | ParentAlignments.Inner,
                Size = new Vector2(30f),
                Offset = new Vector2(-18f, -14f),
                Color = new Color(173, 182, 189),
            };

            //sequenceList.SelectionChanged += HandleSelectionChange;
            closeButton.MouseInput.LeftClicked += (sender, args) => Close(false);
            SharedBinds.Escape.NewPressed += () => Close(false);

            BodyColor = new Color(37, 46, 53);
            BorderColor = new Color(84, 98, 107);

            Padding = new Vector2(80f, 40f);
            MinimumSize = new Vector2(1044f, 500f);

            Size = new Vector2(1044f, 850f);
            Vector2 normScreenSize = new Vector2(HudMain.ScreenWidth, HudMain.ScreenHeight) / HudMain.ResScale;

            if (normScreenSize.Y < 1080 || HudMain.AspectRatio < (16f / 9f))
                Height = MinimumSize.Y;

            Offset = (normScreenSize - Size) * .5f - new Vector2(40f);
        }

        public override Color BorderColor
        {
            set
            {
                base.BorderColor = value;
                topDivider.Color = value;
                bottomDivider.Color = value;
                middleDivider.Color = value;
            }
        }

/*
        public void SetSelection(SequencePart sequencePart, SequencePageBase newPage) =>
            sequenceList.SetSelection(sequencePart, newPage);

        private void HandleSelectionChange()
        {
            if (lastPage != null)
            {
                var pageElement = lastPage.AssocMember as HudElementBase;
                int index = bodyChain.FindIndex(x => x.Element == pageElement);

                bodyChain.RemoveAt(index);
            }

            if (SelectedPage != null)
                bodyChain.Add(SelectedPage.AssocMember as HudElementBase);

            lastPage = SelectedPage;
        }
*/
        protected override void Layout()
        {
            base.Layout();

/*
            // Update sizing
            if (SelectedPage != null)
            {
                var pageElement = SelectedPage.AssocMember as HudElementBase;
                pageElement.Width = Width - Padding.X - sequenceList.Width - bodyChain.Spacing;
            }
*/
            bodyChain.Height = Height - header.Height - topDivider.Height - Padding.Y - bottomDivider.Height;
            sequenceList.Width = 270f;

            // Bound window offset to keep it from being moved off screen
            Vector2 min = new Vector2(HudMain.ScreenWidth, HudMain.ScreenHeight) / (HudMain.ResScale * -2f), max = -min;
            Offset = Vector2.Clamp(Offset, min, max);

            // Update color opacity
            BodyColor = BodyColor.SetAlphaPct(HudMain.UiBkOpacity);
            header.Color = BodyColor;
        }

        void Load()
        {
            sequenceList.Load(Script);
        }

        void Save()
        {
            sequenceList.Save(Script);
        }

        public void Close(bool save)
        {
            if (save)
                Save();

            Session.Instance.HideEditor();
        }
    }
}
