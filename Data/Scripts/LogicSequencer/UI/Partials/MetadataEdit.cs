using System;
using RichHudFramework.UI;
using VRageMath;

namespace LogicSequencer.UI
{
    public partial class SequenceEditor
    {
        class MetadataEdit : HudElementBase
        {
            public Script.ScriptSequence Script { get; set; }

            readonly HudChain layout;
            readonly TextBox nameEditor;
            readonly TextBox descriptionEditor;

            public MetadataEdit(HudParentBase parent = null) : base(parent)
            {
                var background = new TexturedBox(this) {
                    Color = new Color(41, 54, 62),
                    DimAlignment = DimAlignments.Width
                };

                var nameLabel = new Label() {
                    Text = "Name",
                    Padding = new Vector2(5, 0)
                };
                nameEditor = new TextBox() {
                    Text = "Name",
                };
                var nameGroup = new HudChain(false) {
                    DimAlignment = DimAlignments.Width,
                    CollectionContainer = { nameLabel, nameEditor }
                };

                var descriptionLabel = new Label() {
                    Text = "Description",
                    Padding = new Vector2(5, 0)
                };
                descriptionEditor = new TextBox() {
                    Text = "Description",
                };
                var descriptionGroup = new HudChain(false) {
                    DimAlignment = DimAlignments.Width,
                    CollectionContainer = { descriptionLabel, descriptionEditor }
                };

                layout = new HudChain(true, this) {
                    CollectionContainer = { nameGroup, descriptionGroup }
                };
            }

            protected override void Layout()
            {
                Width = Math.Max(Width, layout.Width + Padding.X);
            }

            public void Load()
            {
                if (Script == null)
                    return;

                nameEditor.Text = Script.Name;
                descriptionEditor.Text = Script.Description;
            }

            public void Save()
            {
                if (Script == null)
                    return;

                Script.Name = nameEditor.Text.ToString();
                Script.Description = descriptionEditor.Text.ToString();
            }
        }
    }
}
