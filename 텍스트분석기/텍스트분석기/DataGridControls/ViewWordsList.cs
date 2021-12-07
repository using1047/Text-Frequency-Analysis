using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.CustomColumns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace 텍스트분석기
{
    public partial class ViewWordsList : UserControl
    {
        public ViewWordsList()
        {
            InitializeComponent();
        }

        private void ViewWordsList_Load(object sender, EventArgs e)
        {
            DataGrid.RegisterGroupBoxEvents();

            DataGridSetupViewWords setup = new DataGridSetupViewWords();
            setup.SetupDataGridView(this.DataGrid, true);

            DataGrid.ShowLines = true;
            LoadData();
        }
        private void LoadData()
        {
            OutlookGridRow row = new OutlookGridRow();
            List<OutlookGridRow> I = new List<OutlookGridRow>();

            DataGrid.SuspendLayout();
            DataGrid.ClearInternalRows();
            DataGrid.FillMode = FillMode.GroupsAndNodes;

            XmlDocument doc = new XmlDocument();
            doc.Load("Data/ViewWordsListData.xml");

            foreach (XmlNode Word in doc.SelectNodes("//ViewWordsListData"))
            {
                try
                {
                    row = new OutlookGridRow();
                    row.CreateCells(DataGrid, new object[]
                    {
                        Word["Number"].InnerText,
                        Word["Word"].InnerText,
                        Word["Status"].InnerText,
                        Word["Vector"].InnerText
                    });

                    I.Add(row);
                    ((KryptonDataGridViewTreeTextCell)row.Cells[1]).UpdateStyle();
                    ((KryptonDataGridViewTreeTextCell)row.Cells[3]).UpdateStyle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            DataGrid.ResumeLayout();
            DataGrid.AssignRows(I);
            DataGrid.ForceRefreshGroupBox();
            DataGrid.Fill();
        }

        private void OuterPanel_Resize(object sender, EventArgs e)
        {
            DataGrid.Width = this.OuterPanel.Width;
            DataGrid.Height = this.OuterPanel.Height;
        }

        private void DataGrid_Resize(object sender, EventArgs e)
        {
            MessageBox.Show(DataGrid.Width.ToString());
        }
    }
}
