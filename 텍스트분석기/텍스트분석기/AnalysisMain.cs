using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 텍스트분석기
{
    public partial class AnalysisForm : Form
    {
        string OriginalFilePath = "";

        Dictionary<string, int> AllText = new Dictionary<string, int>();

        bool Pause = false;
        ManualResetEvent PauseEvent = new ManualResetEvent(true);

        /// <summary>
        /// Form 생성시
        /// </summary>
        public AnalysisForm()
        {
            InitializeComponent();

            tb_LinesCount.Enabled = false;
            tb_WordsCount.Enabled = false;
            tb_WordNumber.Enabled = false;

            btn_Pause.Visible = false;
            btn_Export.Visible = false;

            dgv_AnalysisResult.Columns.Add("Word", "Word");
            dgv_AnalysisResult.Columns.Add("Count", "Count");

            dgv_AnalysisResult.Columns["Word"].Width = dgv_AnalysisResult.Width / 2 - 10;
            dgv_AnalysisResult.Columns["Count"].Width = dgv_AnalysisResult.Width / 2 - 42;

            dgv_RemoveWordList.Columns.Add("Word", "Word");
            dgv_RemoveWordList.Columns.Add("Deleted", "Deleted");

            dgv_RemoveWordList.Columns["Word"].Width = dgv_RemoveWordList.Width / 2 - 10;
            dgv_RemoveWordList.Columns["Deleted"].Width = dgv_RemoveWordList.Width / 2 - 42;
        }

        /// <summary>
        /// Read Text File
        /// 텍스트 파일 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Load_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = ".txt | *txt";

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] path = ofd.FileName.Split('\\');
                    lbl_TextFilePath.Text = path.Last();

                    OriginalFilePath = ofd.FileName;

                    if(OriginalFilePath == null)
                    {
                        MessageBox.Show("파일을 불러올 수 없었습니다.", "실패");
                        OriginalFilePath = "";
                    }
                }
            }
        }

        /// <summary>
        /// 분석 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartAnalysis_Click(object sender, EventArgs e)
        {
            if (OriginalFilePath != "")
            {
                pn_RemoveAddControl.Visible = false;
                ThreadNRead();
            }
            else
            {
                MessageBox.Show("파일이 지정되지 않았습니다.");
            }
        }

        /// <summary>
        /// Data Grid View 내용 내보내기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.InitialDirectory = @"C:\";
            sfd.Title = "Select Save Path...";
            sfd.Filter = "텍스트 문서(*.txt)|*.txt|모든 파일|*.*";
            sfd.DefaultExt = "txt";
            sfd.AddExtension = true;
            
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);

                for (int Row = 0; Row < dgv_AnalysisResult.Rows.Count; Row++)
                {
                    if (dgv_AnalysisResult.Rows[Row].Cells[0] != null) sw.WriteLine("단어 : " + dgv_AnalysisResult.Rows[Row].Cells[0] + " 빈도 : " + dgv_AnalysisResult.Rows[Row].Cells[1]);
                }

                sw.Close();
            }
        }

        /// <summary>
        /// 일시정지 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Pause_Click(object sender, EventArgs e)
        {
            if (Pause)
            {
                Pause = false;
                PauseEvent.Set();
                btn_Pause.Text = "■";
            }
            else
            {
                Pause = true;
                btn_Pause.Text = " ▶";
            }
        }

        private void btn_SetSave_Click(object sender, EventArgs e)
        {

        }

        private void btn_SetLoad_Click(object sender, EventArgs e)
        {

        }

        private void dgv_AnalysisResult_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            int a = int.Parse(e.CellValue1.ToString()), b = int.Parse(e.CellValue2.ToString());
            e.SortResult = a.CompareTo(b);
            e.Handled = true;
        }

        /// <summary>
        /// 삭제할 단어 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddWords_Click(object sender, EventArgs e)
        {
            if(tb_RemoveWord.Text != "")
            {
                bool check = false;
                foreach (DataGridViewRow Row in dgv_RemoveWordList.Rows)
                {
                    if (Row.Cells[0].Value != null)
                    {
                        if (Row.Cells[0].Value.ToString() == tb_RemoveWord.Text) check = true;
                    }
                }
                if (!check) dgv_RemoveWordList.Rows.Add(tb_RemoveWord.Text, false.ToString());
            }
        }

        void ThreadNRead()
        {
            try
            {
                using (StreamReader sr = new StreamReader(OriginalFilePath, Encoding.UTF8))
                {
                    int ALine = File.ReadLines(OriginalFilePath).Count();
                    int NLine = 1;

                    if (!Pause)
                    {
                        btn_Pause.Visible = true;
                        btn_Pause.Text = "■";

                        btn_Export.Visible = true;
                    }

                    btn_StartAnalysis.Enabled = false;

                    while (!sr.EndOfStream)
                    {
                        string StrLine = sr.ReadLine();

                        // 데이터 제거하기
                        foreach (DataGridViewRow Row in dgv_RemoveWordList.Rows)
                        {
                            if (Row.Cells[0].Value != null)
                            {
                                StrLine = StrLine.Replace(" " + Row.Cells[0].Value + " ", "");
                            }
                        }

                        string[] StrWords = StrLine.Split(' ');

                        NLine++;

                        // 단어 개수만큼 진행
                        if (StrWords.Length > 0)
                        {
                            for (int num = 0; num < StrWords.Length; num++)
                            {
                                if (AllText.ContainsKey(StrWords[num])) AllText[StrWords[num]]++;
                                else AllText.Add(StrWords[num], 1);
                                
                                tb_WordsCount.Text = AllText.Count.ToString();

                                string Status = (NLine.ToString() + " / " + ALine.ToString());
                                if (Status.Length > 17) Status = NLine.ToString() + "\n/ " + ALine.ToString();

                                tb_LinesCount.Text = Status;
                                
                                while (Pause)
                                {
                                    PauseEvent.WaitOne();
                                    Application.DoEvents();
                                }
                                Application.DoEvents();
                            }

                            if (NLine % 10000 == 0 && NLine != 0)
                            {
                                for (int Row = 0; Row < AllText.Count; Row++)
                                {
                                    if (AllText.ElementAt(Row).Value < 5)
                                    {
                                        AllText.Remove(AllText.ElementAt(Row).Key);
                                        Row = Row - 1;
                                    }
                                }
                            }
                            else if (NLine % 1000 == 0 && NLine != 0)
                            {
                                pb_Update.Value = 0;

                                // 삽입 구문
                                for (int Row = 0; Row < AllText.Count; Row++)
                                {
                                    tb_WordNumber.Text = Row.ToString();
                                    Application.DoEvents();

                                    if (AllText.ElementAt(Row).Value > 100)
                                    {
                                        bool Check = false;
                                        foreach (DataGridViewRow row in dgv_AnalysisResult.Rows)
                                        {
                                            if (row.Cells[0].Value != null)
                                            {
                                                if (row.Cells[0].Value.ToString() == AllText.ElementAt(Row).Key.ToString())
                                                {
                                                    row.Cells[1].Value = AllText.ElementAt(Row).Value.ToString();
                                                    Check = true;
                                                    break;
                                                }
                                            }
                                            Application.DoEvents();
                                        }

                                        if (Check == false)
                                        {
                                            dgv_AnalysisResult.Rows.Add(AllText.ElementAt(Row).Key, AllText.ElementAt(Row).Value.ToString());
                                        }

                                        while (Pause)
                                        {
                                            PauseEvent.WaitOne();
                                            Application.DoEvents();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                pb_Update.Value = NLine % 1000 / 10;
                            }
                        }
                    }
                }

                btn_StartAnalysis.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace.ToString());
            }
        }
    }
}
