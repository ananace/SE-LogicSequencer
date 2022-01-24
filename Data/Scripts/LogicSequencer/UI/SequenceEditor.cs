using RichHudFramework.UI;
using VRageMath;

namespace LogicSequencer.UI
{
    public partial class SequenceEditor : WindowBase
    {
        MetadataEdit metadataEdit;

        Script.ScriptSequence _Script;
        public Script.ScriptSequence Script { get { return _Script; } set { _Script = value; Load(); } }

        readonly HudChain layout;

        public SequenceEditor(HudParentBase parent) : base(parent)
        {
            metadataEdit = new MetadataEdit() {
                DimAlignment = DimAlignments.Width,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.InnerV
            };

            var closeButton = new LabelBoxButton() {
                //Color = VRageMath.Color.Black,
                Text = "Close",
            };
            var saveButton = new LabelBoxButton() {
                //Color = VRageMath.Color.Black,
                Text = "Save",
            };

            closeButton.MouseInput.LeftClicked += (s, ev) => Close(false);
            saveButton.MouseInput.LeftClicked += (s, ev) => Close(true);

            var statusBar = new HudChain(false) {
                DimAlignment = DimAlignments.Width,
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.Right | ParentAlignments.Inner,
                CollectionContainer = { closeButton, saveButton }
            };

            layout = new HudChain(true, body) {
                DimAlignment = DimAlignments.Both,
                CollectionContainer = { metadataEdit, statusBar }
            };

            BodyColor = new Color(41, 54, 62, 150);
            BorderColor = new Color(58, 68, 77);

            header.Format = new GlyphFormat(GlyphFormat.Blueish.Color, TextAlignment.Center, 1.08f);
            header.Height = 30f;
            HeaderText = "Logic Sequence Editor";

            AllowResizing = true;
            CanDrag = true;

            Size = new Vector2(500f, 300f);
        }

        void Load()
        {
            metadataEdit.Load();
        }

        void Save()
        {
            metadataEdit.Save();
        }

        public void Close(bool save)
        {
            if (save)
                Save();

            Session.Instance.HideEditor();
        }
    }
}
