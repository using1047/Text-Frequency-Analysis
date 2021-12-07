using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.CustomColumns;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace 텍스트분석기
{
    public partial class ViewWordsList : UserControl
    {
        DataGridSetupViewWords setup = new DataGridSetupViewWords();

        int RowsCount = 1;

        private string _FilePath = "";
        public string FilePath
        {
            get { return FilePath; }
            set { _FilePath = value; }
        }

        public void Reload()
        {
            LoadData(_FilePath);
        }

        public ViewWordsList()
        {
            InitializeComponent();
        }

        private void ViewWordsList_Load(object sender, EventArgs e)
        {
            DataGrid.RegisterGroupBoxEvents();

            setup.SetupDataGridView(this.DataGrid, true);

            DataGrid.ShowLines = true;
            LoadData("Data/ViewWordsListSampleData.xml");

            DataGrid.AutoResizeColumns();
        }

        private void LoadData(string file)
        {
            OutlookGridRow row = new OutlookGridRow();
            List<OutlookGridRow> I = new List<OutlookGridRow>();

            DataGrid.SuspendLayout();
            DataGrid.ClearInternalRows();
            DataGrid.FillMode = FillMode.GroupsAndNodes;

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

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

        // 여기서부터 기능단

        public void Save()
        {
            DataGrid.PersistConfiguration(_FilePath, 1.ToString());
        }

        public bool PersistModified = false;

        public void Add(object[] parameter)
        {
            if(!PersistModified) DataGrid.Rows.Clear();
            if (parameter.Length != setup.ColumnsCount)
            {
                MessageBox.Show("데이터 개수와 Columns 개수가 맞지 않습니다.");
            }
            else
            {
                try
                {
                    string[] Values = new string[4];

                    // Word
                    Values[0] = parameter[0].ToString();
                    // Status
                    Values[1] = parameter[1].ToString();
                    // Vector X
                    Values[2] = parameter[2].ToString();
                    // Vector Y
                    Values[3] = parameter[3].ToString();

                    if(DataGrid.InvokeRequired)
                    {
                        DataGrid.Invoke(new Action(delegate
                        {
                            DataGrid.Rows.Add(RowsCount, Values[0], Values[1], "None", "None");
                            OutlookGridRow row = (OutlookGridRow)DataGrid.Rows[0];
                            DataGrid.InternalRows.Add(row);
                            DataGrid.Rows.RemoveAt(0);
                            Save();
                        }));
                    }
                    else
                    {
                        DataGrid.Rows.Add(RowsCount, Values[0], Values[1], "None", "None");
                        Save();
                    }
                    
                    RowsCount++;
                    PersistModified = true;
                }
                catch { }
            }
        }
    }
}
