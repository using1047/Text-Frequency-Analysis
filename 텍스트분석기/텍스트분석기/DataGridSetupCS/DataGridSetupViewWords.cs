using ComponentFactory.Krypton.Toolkit;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.CustomColumns;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace 텍스트분석기
{
    public class DataGridSetupViewWords
    {
        public readonly int ColumnsCount = 4;

        private ViewWordListGridColumn[] activateColumns;

        public enum ViewWordListGridColumn
        {
            ColumnNumber = 0,
            ColumnWord = 1,
            ColumnStatus = 2,
            ColumnVector = 3
        }

        public enum State
        {
            Not_View,
            View
        }

        public void SetupDataGridView(KryptonOutlookGrid Grid, bool RestoreIfPossible)
        {
            if (File.Exists(Application.StartupPath + @"\Data\ViewWords.xml") & RestoreIfPossible)
            {
                try
                {
                    LoadConfigFromFile(Application.StartupPath + @"\Data\ViewWords.xml", Grid);
                }
                catch (Exception ex)
                {
                    KryptonMessageBox.Show("Config 파일을 불러오면서 오류가 일어났습니다.\n" + ex.ToString(),
                                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Grid.ClearEverything();
                    LoadDefaultConfiguration(Grid);
                }
            }
            else
            {
                LoadDefaultConfiguration(Grid);
            }
        }

        private void LoadDefaultConfiguration(KryptonOutlookGrid Grid)
        {
            Grid.ClearEverything();
            Grid.GroupBox.Visible = true;
            Grid.HideColumnOnGrouping = false;

            // Tree 활성화
            Grid.FillMode = FillMode.GroupsAndNodes;
            Grid.ShowLines = true;

            activateColumns = new ViewWordListGridColumn[]
            {
                ViewWordListGridColumn.ColumnNumber,
                ViewWordListGridColumn.ColumnWord,
                ViewWordListGridColumn.ColumnStatus,
                ViewWordListGridColumn.ColumnVector
            };

            DataGridViewColumn[] columnsToAdd = new DataGridViewColumn[4]
            {
                SetupColumn(ViewWordListGridColumn.ColumnNumber),
                SetupColumn(ViewWordListGridColumn.ColumnWord),
                SetupColumn(ViewWordListGridColumn.ColumnStatus),
                SetupColumn(ViewWordListGridColumn.ColumnVector)
            };

            Grid.Columns.AddRange(columnsToAdd);

            Grid.AddInternalColumn(columnsToAdd[0], new OutlookGridDefaultGroup(null), SortOrder.Ascending, -1, -1);
            Grid.AddInternalColumn(columnsToAdd[1], new OutlookGridAlphabeticGroup(null), SortOrder.None, -1, -1);
            Grid.AddInternalColumn(columnsToAdd[2], new OutlookGridDefaultGroup(null), SortOrder.None, -1, -1);
            Grid.AddInternalColumn(columnsToAdd[3], new OutlookGridAlphabeticGroup(null), SortOrder.None, -1, -1);
        }

        private void LoadConfigFromFile(string file, KryptonOutlookGrid Grid)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("Config 파일을 찾을 수 없습니다");

            XDocument doc = XDocument.Load(file);

            Grid.ClearEverything();
            Grid.GroupBox.Visible = CommonHelper.StringToBool(doc.XPathSelectElement("OutlookGrid/GroupBox").Value);
            Grid.HideColumnOnGrouping = CommonHelper.StringToBool(doc.XPathSelectElement("OutlookGrid/HideColumnOnGrouping").Value);

            // 컬럼 총 개수 불러오기
            int ColCount = doc.XPathSelectElements("//Column").Count();

            // 컬럼 개수 만큼
            DataGridViewColumn[] columnsToAdd = new DataGridViewColumn[ColCount];
            ViewWordListGridColumn[] enumCols = new ViewWordListGridColumn[ColCount];
            OutlookGridColumn[] OutlookColumnsToAdd = new OutlookGridColumn[columnsToAdd.Length];

            SortedList<int, int> hash = new SortedList<int, int>();

            int i = 0;
            IOutlookGridGroup group;
            XElement node2;

            foreach (XElement node in doc.XPathSelectElement("Outlookgrid/Columns").Nodes())
            {
                // 데이터 타입으로 변환
                enumCols[i] = (ViewWordListGridColumn)Enum.Parse(typeof(ViewWordListGridColumn), node.Element("Name").Value);

                // 컬럼 삽입
                columnsToAdd[i] = SetupColumn(enumCols[i]);
                // 컬럼 크기 지정
                columnsToAdd[i].Width = int.Parse(node.Element("Width").Value);
                // 컬럼 Visible 설정
                columnsToAdd[i].Visible = CommonHelper.StringToBool(node.Element("Visible").Value);

                // 보여지는 Index를 hashTable로 설정
                hash.Add(int.Parse(node.Element("DisplayIndex").Value), i);
                group = null;

                if (!node.Element("GroupingType").IsEmpty && node.Element("GroupingType").HasElements)
                {
                    node2 = node.Element("GroupingType");

                    group = (IOutlookGridGroup)Activator.CreateInstance(
                                                                        Type.GetType(
                                                                            TypeConverter.ProcessType(
                                                                                node2.Element("Word").Value), true));
                    group.OneItemText = node2.Element("OneItemText").Value;
                    group.XXXItemsText = node2.Element("XXXItemsText").Value;
                    group.SortBySummaryCount = CommonHelper.StringToBool(node2.Element("SortBySummaryCount").Value);

                    if (!string.IsNullOrEmpty((node2.Element("ItemsComparer").Value)))
                    {
                        Object comparer = Activator.CreateInstance(
                                                            Type.GetType(
                                                                TypeConverter.ProcessType(
                                                                    node2.Element("ItemsComparer").Value), true));
                        group.ItemsComparer = (IComparer)comparer;
                    }
                }
                OutlookColumnsToAdd[i] = new OutlookGridColumn(
                                                                            columnsToAdd[i],
                                                                            group,
                                                                            (SortOrder)Enum.Parse(typeof(SortOrder), node.Element("SortDirection").Value),
                                                                            int.Parse(node.Element("GroupIndex").Value),
                                                                            int.Parse(node.Element("SortIndex").Value));
            }
            i++;
        }

        /// <summary>
        /// 초기 컬럼 설정
        /// </summary>
        /// <param name="colType"></param>
        /// <returns></returns>
        private DataGridViewColumn SetupColumn(ViewWordListGridColumn colType)
        {
            DataGridViewColumn column = null;
            switch (colType)
            {
                // 컬럼이 Number 라면
                case ViewWordListGridColumn.ColumnNumber:
                    column = new KryptonDataGridViewTextBoxColumn();
                    column.HeaderText = "Number";
                    column.Name = "ColumnNumber";
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    column.Width = 50;
                    return column;

                // 컬럼이 Word 라면
                case ViewWordListGridColumn.ColumnWord:
                    column = new KryptonDataGridViewTreeTextColumn();
                    column.HeaderText = "Word";
                    column.Name = "ColumnWord";
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    column.Width = 50;
                    return column;

                // 컬럼이 Status 라면
                case ViewWordListGridColumn.ColumnStatus:
                    column = new DataGridViewCheckBoxColumn();
                    column.HeaderText = "Status";
                    column.Name = "ColumnStatus";
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    column.Width = 50;
                    return column;

                // 컬럼이 Vector 라면
                case ViewWordListGridColumn.ColumnVector:
                    column = new KryptonDataGridViewTreeTextColumn();
                    column.HeaderText = "Vector";
                    column.Name = "ColumnVector";
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    column.Width = 50;
                    return column;

                default:
                    throw new Exception("정의되지 않은 컬럼 타입입니다.\n지원되는 컬럼 목록"
                                                       + "\nNumber"
                                                       + "\nWord"
                                                       + "\nStatus"
                                                       + "\nVector");
            }
        }
    }
}
