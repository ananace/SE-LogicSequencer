using LogicSequencer.Script;
using RichHudFramework.UI;
using VRageMath;

namespace LogicSequencer.UI.Components
{
    public class SequenceList : HudElementBase, ISequenceContainer
    {
        readonly LabelBox header;
        readonly ScrollBox<SequenceComponent, LabelElementBase> scrollBox;
        readonly ListInputElement<SequenceComponent, LabelElementBase> listInput;

        public SequenceList(HudParentBase parent = null) : base(parent)
        {
            scrollBox = new ScrollBox<SequenceComponent, LabelElementBase>(true, this)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis | HudChainSizingModes.ClampChainBoth,
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV,
                Color = TerminalFormatting.DarkSlateGrey,
                Padding = new Vector2(6f)
            };

            listInput = new ListInputElement<SequenceComponent, LabelElementBase>(scrollBox);

            header = new LabelBox(scrollBox)
            {
                AutoResize = false,
                ParentAlignment = ParentAlignments.Top,
                DimAlignment = DimAlignments.Width,
                Size = new Vector2(200f, 36f),
                Color = new Color(32, 39, 45),
                Format = TerminalFormatting.ControlFormat,
                Text = "Sequence",
                TextPadding = new Vector2(30f, 0f),
            };

            var listDivider = new TexturedBox(header)
            {
                ParentAlignment = ParentAlignments.Bottom,
                DimAlignment = DimAlignments.Width,
                Height = 1f,
                Color = new Color(53, 66, 75),
            };

            var listBorder = new BorderBox(this)
            {
                DimAlignment = DimAlignments.Both,
                Thickness = 1f,
                Color = new Color(53, 66, 75),
            };

            // var metadata =
            // var triggers =
            // var conditions =
            // var actions =
        }

        public void Load(ScriptSequence script)
        {
            // metadata.Load(script)
            // triggers.Clear()
            // foreach (var trigger in script.Triggers)
            //   triggers.Add(trigger)?
            // ...
        }

        public void Save(ScriptSequence script)
        {
        }
    }
}
