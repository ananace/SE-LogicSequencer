using System;
using RichHudFramework.UI;
using VRageMath;

namespace LogicSequencer.UI
{
    public partial class SequenceEditor
    {
        class MetadataEdit : HudElementBase, ISequenceContainer
        {
            readonly TextBox nameEditor;
            readonly TextBox descriptionEditor;

            public MetadataEdit(HudParentBase parent = null) : base(parent)
            {
                var nameLabel = new Label(this) {
                    Text = "Name",
                };
                nameEditor = new TextBox(this) {
                    DimAlignment = DimAlignments.Width,
                    Padding = new Vector2(0, 5),
                    Text = "Name",
                };

                var descriptionLabel = new Label(this) {
                    Text = "Description",
                };
                descriptionEditor = new TextBox(this) {
                    DimAlignment = DimAlignments.Width,
                    Height = 120f,
                    Text = "Description",
                };
            }

            protected override void Layout()
            {
            }

            public void Load(Script.ScriptSequence script)
            {
                nameEditor.Text = script.Name;
                descriptionEditor.Text = script.Description;
            }

            public void Save(Script.ScriptSequence script)
            {
                script.Name = nameEditor.Text.ToString();
                script.Description = descriptionEditor.Text.ToString();
            }
        }
    }
}
