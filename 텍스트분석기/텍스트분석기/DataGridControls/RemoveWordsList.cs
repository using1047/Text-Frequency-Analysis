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
    public partial class RemoveWordsList : UserControl
    {
        DataGridSetupRemoveWords setup = new DataGridSetupRemoveWords();

        public RemoveWordsList()
        {
            InitializeComponent();
        }

        private void RemoveWordsList_Load(object sender, EventArgs e)
        {
            DataGrid.RegisterGroupBoxEvents();

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
            doc.Load("Data/RemoveWordsListData.xml");

            foreach (XmlNode Word in doc.SelectNodes("//RemoveWordsListData"))
            {
                try
                {
                    row = new OutlookGridRow();
                    row.CreateCells(DataGrid, new object[]
                    {
                        Word["Number"].InnerText,
                        Word["Word"].InnerText,
                        Word["Status"].InnerText
                    });

                    I.Add(row);
                    ((KryptonDataGridViewTreeTextCell)row.Cells[1]).UpdateStyle();
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
    }
}
