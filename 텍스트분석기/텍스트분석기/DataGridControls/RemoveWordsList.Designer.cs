
namespace 텍스트분석기
{
    partial class RemoveWordsList
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.OutlookGridGroupCollection outlookGridGroupCollection1 = new JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.OutlookGridGroupCollection();
            this.OuterPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.DataGrid = new JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.KryptonOutlookGrid();
            this.GroupBox = new JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.KryptonOutlookGridGroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.OuterPanel)).BeginInit();
            this.OuterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // OuterPanel
            // 
            this.OuterPanel.Controls.Add(this.DataGrid);
            this.OuterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterPanel.Location = new System.Drawing.Point(0, 0);
            this.OuterPanel.Name = "OuterPanel";
            this.OuterPanel.Size = new System.Drawing.Size(275, 306);
            this.OuterPanel.TabIndex = 0;
            // 
            // DataGrid
            // 
            this.DataGrid.AllowDrop = true;
            this.DataGrid.AllowUserToAddRows = false;
            this.DataGrid.AllowUserToResizeRows = false;
            this.DataGrid.ColumnHeadersHeight = 36;
            this.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGrid.FillMode = JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.FillMode.GroupsOnly;
            this.DataGrid.GridStyles.Style = ComponentFactory.Krypton.Toolkit.DataGridViewStyle.Mixed;
            this.DataGrid.GridStyles.StyleBackground = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
            this.DataGrid.GroupBox = this.GroupBox;
            this.DataGrid.GroupCollection = outlookGridGroupCollection1;
            this.DataGrid.HideOuterBorders = true;
            this.DataGrid.Location = new System.Drawing.Point(0, 0);
            this.DataGrid.Name = "DataGrid";
            this.DataGrid.PreviousSelectedGroupRow = -1;
            this.DataGrid.RowHeadersVisible = false;
            this.DataGrid.RowHeadersWidth = 51;
            this.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGrid.ShowLines = false;
            this.DataGrid.Size = new System.Drawing.Size(275, 306);
            this.DataGrid.TabIndex = 0;
            // 
            // GroupBox
            // 
            this.GroupBox.AllowDrop = true;
            this.GroupBox.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom;
            this.GroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBox.Location = new System.Drawing.Point(0, 0);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(100, 100);
            this.GroupBox.TabIndex = 0;
            // 
            // RemoveWordsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.OuterPanel);
            this.Name = "RemoveWordsList";
            this.Size = new System.Drawing.Size(275, 306);
            this.Load += new System.EventHandler(this.RemoveWordsList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OuterPanel)).EndInit();
            this.OuterPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel OuterPanel;
        private JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.KryptonOutlookGrid DataGrid;
        private JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.KryptonOutlookGridGroupBox GroupBox;
    }
}
