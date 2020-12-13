using ICSharpCode.TextEditor;

namespace WinFormTestXmlEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbHighlight = new System.Windows.Forms.ComboBox();
            this.textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControlEx();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 536);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(695, 68);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 511);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "XML Folding errors:";
            // 
            // cmbHighlight
            // 
            this.cmbHighlight.FormattingEnabled = true;
            this.cmbHighlight.Items.AddRange(new object[] {
            "XML",
            "Lua",
            "SQL",
            "CSharp"});
            this.cmbHighlight.Location = new System.Drawing.Point(742, 46);
            this.cmbHighlight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbHighlight.Name = "cmbHighlight";
            this.cmbHighlight.Size = new System.Drawing.Size(140, 25);
            this.cmbHighlight.TabIndex = 3;
            this.cmbHighlight.Text = "XML";
            this.cmbHighlight.SelectedIndexChanged += new System.EventHandler(this.cmbHighlight_SelectedIndexChanged);
            // 
            // textEditorControl1
            // 
            this.textEditorControl1.ContextMenuEnabled = true;
            this.textEditorControl1.ContextMenuShowDefaultIcons = true;
            this.textEditorControl1.ContextMenuShowShortCutKeys = true;
            this.textEditorControl1.FoldingStrategy = "XML";
            this.textEditorControl1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textEditorControl1.HideVScrollBarIfPossible = true;
            this.textEditorControl1.Location = new System.Drawing.Point(15, 46);
            this.textEditorControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textEditorControl1.Name = "textEditorControl1";
            this.textEditorControl1.ShowVRuler = false;
            this.textEditorControl1.Size = new System.Drawing.Size(695, 462);
            this.textEditorControl1.SyntaxHighlighting = "XML";
            this.textEditorControl1.TabIndex = 0;
            this.textEditorControl1.Text = resources.GetString("textEditorControl1.Text");
            this.textEditorControl1.VRulerRow = 999;
            this.textEditorControl1.TextChanged += new System.EventHandler(this.textEditorControl1_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 621);
            this.Controls.Add(this.cmbHighlight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textEditorControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControlEx textEditorControl1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbHighlight;

    }
}

