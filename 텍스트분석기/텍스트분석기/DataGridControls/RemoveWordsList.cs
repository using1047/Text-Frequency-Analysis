using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.CustomColumns;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace 텍스트분석기
{
    public partial class RemoveWordsList : UserControl
    {
        DataGridSetupRemoveWords setup = new DataGridSetupRemoveWords();

        int RowsCount = 1;

        private string _FilePath = "";
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }

        public void Reload()
        {
            LoadData(_FilePath);
        }

        public RemoveWordsList()
        {
            InitializeComponent();
        }

        private void RemoveWordsList_Load(object sender, EventArgs e)
        {
            DataGrid.RegisterGroupBoxEvents();

            setup.SetupDataGridView(this.DataGrid, true);

            DataGrid.ShowLines = true;
            LoadData("Data/RemoveWordsListSampleData.xml");

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

        // 여기서부터 기능단

        public bool PersistModified = false;

        public void Add(object[] parameter)
        {
            if (!PersistModified) DataGrid.Rows.Clear();
            if (parameter.Length != setup.ColumnsCount - 1)
            {
                MessageBox.Show("데이터 개수와 Columns 개수가 맞지 않습니다.");
            }
            else
            {
                try
                {
                    string[] Values = new string[5];

                    Values[0] = parameter[0].ToString();
                    Values[1] = parameter[1].ToString();

                    foreach(DataGridViewRow Row in DataGrid.Rows)
                    {
                        if(Row.Cells[1].Value.ToString() == Values[0]) return;
                    }

                    DataGrid.Rows.Add(RowsCount, Values[0], Values[1]);
                    RowsCount++;
                    PersistModified = true;
                }
                catch { }
            }
        }
    }
}
