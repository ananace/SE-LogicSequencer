using RichHudFramework.UI;
using VRageMath;

namespace LogicSequencer.UI.Components
{
    public class SequenceComponent : SelectionBoxEntryTuple<LabelElementBase, object>, ISequenceContainer
    {
        // readonly List<TerminalPageBase> pages;
        readonly SequenceComponentNodeBox treeBox;

        public override bool Enabled
        {
            get { return base.Enabled && treeBox.EntryList.Count > 0; }
            set { base.Enabled = value; }
        }

        public SequenceComponent()
        {
            AllowHighlighting = false;

            treeBox = new SequenceComponentNodeBox();
            //pages = new List<TerminalPageBase>();
            SetElement(treeBox);
        }

        public void Load(Script.ScriptSequence script)
        {
        }

        public void Save(Script.ScriptSequence script)
        {
        }
    }

    public class SequenceComponentNodeBox : TreeBox<SelectionBoxEntryTuple<LabelElementBase, object>, LabelElementBase>
    {
        public SequenceComponentNodeBox(HudParentBase parent) : base(parent)
        {
            HeaderColor = new Color(40, 48, 55);
            MemberMinSize = new Vector2(0f, 34f);
        }

        public SequenceComponentNodeBox() : this(null)
        { }
    }
}
