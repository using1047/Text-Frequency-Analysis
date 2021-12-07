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
using UMAP;
using MessagePack;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using ComponentFactory.Krypton.Toolkit;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid;
using System.Xml;
using JDHSoftware.Krypton.Toolkit.KryptonOutlookGrid.CustomColumns;

namespace 텍스트분석기
{
    public partial class AnalysisForm : Form
    {
        string OriginalFilePath = "";

        SortedList<string, int> AllText = new SortedList<string, int>();

        bool Pause = false;
        ManualResetEvent PauseEvent = new ManualResetEvent(true);

        List<DataGridViewRow> SaveAnalysisList = new List<DataGridViewRow>();
        List<DataGridViewRow> SaveGraphList = new List<DataGridViewRow>();

        RemoveWordsList RemoveWordsList = new RemoveWordsList();
        ViewWordsList ViewWordsList = new ViewWordsList();

        /// <summary>
        /// Form 생성시
        /// </summary>
        public AnalysisForm()
        {
            InitializeComponent();

            pn_Bottom_Mid.Enabled = false;

            btn_Pause.Visible = false;
            btn_Export.Visible = false;

            dgv_AnalysisResult.Columns.Add("Word", "Word");
            dgv_AnalysisResult.Columns.Add("Count", "Count");

            dgv_AnalysisResult.Columns["Word"].Width = dgv_AnalysisResult.Width / 2 - 10;
            dgv_AnalysisResult.Columns["Count"].Width = dgv_AnalysisResult.Width / 2 - 42
                ;
            dgv_AnalysisResult.AllowUserToAddRows = false;
            dgv_AnalysisResult.RowHeadersVisible = false;

            btn_Pause.Left = (pn_Bottom_Bottom.Width - btn_Pause.Width) / 2;

            pb_UMAP.Visible = false;
            pb_ReadFile.Visible = false;
            lbl_ReadingStatus.Visible = false;

            Current = (Bitmap)Bitmap.FromFile("Waiting.jpg");
            pb_UMAPImage.BackgroundImage = Current;

            pn_OuterPB.MouseWheel += pb_UMAPImage_MouseWheel;
        }

        /// <summary>
        /// Read Text File
        /// 텍스트 파일 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = ".txt | *txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] path = ofd.FileName.Split('\\');
                    lbl_TextFilePath.Text = path.Last();

                    OriginalFilePath = ofd.FileName;

                    if (OriginalFilePath == null)
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
            dgv_AnalysisResult.Rows.Clear();
            if (OriginalFilePath != "")
            {
                pn_RemoveAddControl.Enabled = false;
                Thread ThreadRead = new Thread(() => ThreadNRead());
                ThreadRead.Start();
            }
            else
            {
                MessageBox.Show("파일이 지정되지 않았습니다.");
            }

            pn_RemoveAddControl.Enabled = true;
            OriginalFilePath = "";
            lbl_TextFilePath.Text = "";
            pb_Update.Value = 0;

            dgv_AnalysisResult.Rows.Clear();
            foreach (var Text in AllText)
            {
                dgv_AnalysisResult.Rows.Add(Text.Key, Text.Value);
            }

            Application.DoEvents();
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

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);

                foreach (var Text in AllText)
                {
                    sw.WriteLine(Text.Key + "\t" + Text.Value);
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
                pn_RemoveAddControl.Enabled = false;
            }
            else
            {
                Pause = true;
                btn_Pause.Text = " ▶";
                pn_RemoveAddControl.Enabled = true;
            }
        }

        /// <summary>
        /// 삭제 단어 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.InitialDirectory = @"C:\";
            sfd.Title = "Select Save Path...";
            sfd.Filter = "텍스트 문서(*.txt)|*.txt|모든 파일|*.*";
            sfd.DefaultExt = "txt";
            sfd.AddExtension = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);

                /*
                for(int Row = 0;  Row < OutlookGrid_1.Rows.Count; Row++)
                {
                    if(OutlookGrid_1.Rows[Row].Cells[1].Value != null)
                    {
                        sw.WriteLine(OutlookGrid_1.Rows[Row].Cells[1].Value);
                    }
                }
                */
                sw.Close();
            }
        }

        /// <summary>
        /// 삭제 단어들 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetLoad_Click(object sender, EventArgs e)
        {
            if (tc_Pages.SelectedIndex == 0)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "XML 데이터 파일|*.xml";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        RemoveWordsList.FilePath = ofd.FileName;
                        RemoveWordsList.Reload();
                    }
                }
            }
        }

        /// <summary>
        /// DataGridView 숫자 정렬을 위해 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_AnalysisResult_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name.Equals("Count"))
            {
                int a = int.Parse(e.CellValue1.ToString()), b = int.Parse(e.CellValue2.ToString());
                e.SortResult = a.CompareTo(b);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 삭제할 단어 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddWords_Click(object sender, EventArgs e)
        {
            if (tb_Word.Text != "" && tc_Pages.SelectedIndex == 0)
            {
                string[] Values = new string[2];
                Values[0] = tb_Word.Text;
                Values[1] = "False";
                RemoveWordsList.Add(Values);
            }
        }

        /// <summary>
        /// 분석 작업
        /// </summary>
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
                        Update_Visible(btn_Pause, true);
                        Update_Text(btn_Pause, "■");
                        Update_Visible(btn_Export, true);
                    }

                    Update_Enable(btn_StartAnalysis, false);
                    if (UpdateEpochs.Value == 0) UpdateNUD_Value(UpdateEpochs, 1000);
                    while (!sr.EndOfStream)
                    {
                        string StrLine = sr.ReadLine();
                        StrLine = RemoveStrInWords(StrLine);

                        string[] StrWords = StrLine.Split(' ');

                        NLine++;

                        // 단어가 1개 이상일 때 실행
                        if (StrWords.Length > 0)
                        {
                            string Status = "";
                            // 단어 개수만큼
                            for (int num = 0; num < StrWords.Length; num++)
                            {
                                // 이미 있는 단어인지 탐색
                                if (AllText.ContainsKey(StrWords[num])) AllText[StrWords[num]]++;
                                else
                                {
                                    if (StrWords[num] != "") AllText.Add(StrWords[num], 1);
                                }
                               
                                // 현재 라인 번호 / 전체 라인 개 수
                               Status = (NLine.ToString() + " / " + ALine.ToString());

                                // Pause인지 확인하는 구문
                                while (Pause)
                                {
                                    PauseEvent.WaitOne();
                                    Application.DoEvents();
                                }
                            }
                            Thread.Sleep(1);
                            Update_Text(lbl_WordsCount, AllText.Count.ToString());
                            Update_Text(lbl_LinesCount, Status);
                            Thread.Sleep(1);
                        }

                        // 10000번째 줄 마다 5개 미만인 단어 없애기
                        if (NLine % 10000 == 0 && NLine != 0)
                        {
                            if (cb_Delete.Checked)
                            {
                                List<string> TempDic = new List<string>();
                                // 개 수가 5 미만인 경우 없애기
                                foreach (var Dictionary in AllText)
                                {
                                    if (Dictionary.Value < 5) TempDic.Add(Dictionary.Key);
                                }
                                foreach (var Dictionary in TempDic)
                                {
                                    AllText.Remove(Dictionary);
                                }
                            }
                        }

                        // 1000번째 줄 마다 업데이트
                        else if (UpdateEpochs.Value != 0 && NLine % UpdateEpochs.Value == 0)
                        {
                            UpdatePB_Value(pb_Update, 0);
                            Update_Enable(pn_Bottom_Mid, true);
                            dgv_AnalysisResult.Rows.Clear();

                            int CurrentNumber = 1;

                            // 업데이트 구문
                            foreach (var Text in AllText)
                            {
                                Update_Text(lbl_WordNumber, CurrentNumber.ToString());
                                UpdatePB_Value(pb_Update, ValueTo100(AllText.Count, CurrentNumber));
                                Thread.Sleep(1);
                                if (Text.Value > 100)
                                {
                                    dgv_AnalysisResult.Rows.Add(Text.Key, Text.Value);
                                }
                                CurrentNumber++;
                            }
                            dgv_AnalysisResult.Sort(new RowComparer(SortOrder.Descending));

                            Update_Enable(pn_Bottom_Mid, false);
                        }

                    }
                }
                Update_Enable(btn_StartAnalysis, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// 문장에서 삭제 단어들 지우기
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        string RemoveStrInWords(string Line)
        {
            if (Line.Length > 0)
            {
                /*
                foreach (DataGridViewRow Row in dgv_WordList.Rows)
                {
                    if (Row.Cells[0].Value != null)
                    {
                        Line = Line.Replace("\n", "");
                        Line = Line.Replace(" " + Row.Cells[0].Value + "\n", "");
                        Line = Line.Replace(" " + Row.Cells[0].Value + " ", "");

                        if (Line.Split(' ').Contains(Row.Cells[0].Value)) Line = Line.Replace(Row.Cells[0].Value.ToString(), "");
                    }

                    if (Row.Cells[1].Value != null)
                    {
                        if (Row.Cells[1].Value.ToString() != true.ToString()) Row.Cells[1].Value = true.ToString();
                    }
                }
                */
                return Line;
            }
            else return "";
        }

        /// <summary>
        /// 폼 크기 변경 시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisForm_Resize(object sender, EventArgs e)
        {
            Ctl_PausePostionChange();
        }

        /// <summary>
        /// Pause 버튼 위치 수정
        /// </summary>
        void Ctl_PausePostionChange()
        {
            btn_Pause.Left = (pn_Bottom_Bottom.Width - btn_Pause.Width) / 2;
        }

        // 전체 문장 목록
        List<string> Sentences = new List<string>();
        // 전체 단어 One-Hot Encoding
        Dictionary<string, int> WordsList = new Dictionary<string, int>();
        private void btn_LoadNewText_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "텍스트 문서|*.txt|탭 분리 문서|*.tsv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        int KeyNValue = 0;
                        while (!sr.EndOfStream)
                        {
                            string Text = sr.ReadLine();

                            Text = Text.Replace(".", "");
                            Text = Text.Replace(",", "");
                            Text = Text.Replace("?", "");
                            Text = Text.Replace("\'", "");
                            Text = Text.Replace("’", "");

                            Sentences.Add(Text);
                            string[] Words = Text.Split(' ');

                            foreach (var Word in Words)
                            {
                                if (!WordsList.ContainsKey(Word) && Word != null && Word != "")
                                {
                                    WordsList.Add(Word, KeyNValue);
                                    KeyNValue++;
                                }
                            }
                        }
                    }
                    lbl_AllWordsCount.Text = "전체 단어 수 : " + WordsList.Count.ToString();
                    lbl_AllSentencesCount.Text = "전체 문장 수 : " + Sentences.Count;

                    WordListUpdate();
                }
            }
        }

        string SavePath = "";
        int SaveDataCount = 0;
        string MessagePackPath = "MNIST-LabelledVectorArray-60000x100.msgpack";

        Bitmap Current;
        private void btn_SaveData_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "TSV 파일|*.tsv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SavePath = sfd.FileName;
                    FileStream fs = new FileStream(sfd.FileName, FileMode.Open);
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        /*
                        sw.Write("W/S\t");

                        foreach (var Sentence in Sentences)
                        {
                            sw.Write(Sentence + "\t");
                        }
                        sw.WriteLine();
                        */

                        for (int CurrentSentence = 0; CurrentSentence < Sentences.Count; CurrentSentence++)
                        {
                            // 분리할 문장
                            string[] Words = Sentences[CurrentSentence].Split(' ');

                            foreach (var Word in Words)
                            {
                                sw.Write(Word + "\t");
                                SaveDataCount++;
                                // 사이즈 맞춰주기
                                for (int Cur = 0; Cur < Sentences.Count; Cur++)
                                {
                                    if (Sentences[Cur].Contains(Word))
                                    {
                                        var KeyValue = WordsList[Word] + 1;
                                        var Value = (double)1 / KeyValue;
                                        sw.Write(Value + "\t");
                                    }
                                    else
                                    {
                                        sw.Write("0\t");
                                    }
                                    Application.DoEvents();
                                }
                                sw.WriteLine();
                                Application.DoEvents();
                            }
                        }
                    }
                }
            }
            Current = Create_UMAP(SavePath);
            pb_UMAPImage.BackgroundImage = Current;
        }

        /// <summary>
        /// UMAP 이미지만 그리기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OnlyImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "텍스트 데이터|*.txt|탭 분리 데이터|*.tsv|쉼표 분리 데이터|*.csv";
                    DialogResult dr = ofd.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        SavePath = ofd.FileName;

                        var ThreadCreateUmap = new Thread(() =>
                        {
                            Current = Create_UMAP(SavePath);
                            pb_UMAPImage.BackgroundImage = Current;
                        });

                        ThreadCreateUmap.Start();
                    }
                    else if (dr == DialogResult.Cancel) { }
                    else { }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        const int bitwidth = 1600;
        const int bitheight = 1200;

        void Update_Text(Control ctl, string message)
        {
            if(ctl.InvokeRequired)
            {
                ctl.Invoke(new Action(delegate ()
                {
                    ctl.Text = message;
                }));
            }
            else
            {
                ctl.Text = message;
            }
        }
        void Update_Visible(Control ctl, bool Power)
        {
            if(ctl.InvokeRequired)
            {
                ctl.Invoke(new Action(delegate ()
                {
                    ctl.Visible = Power;
                }));
            }
            else
            {
                ctl.Visible = Power;
            }
        }
        void UpdatePB_Value(ProgressBar ctl, int Value)
        {
            if(ctl.InvokeRequired)
            {
                ctl.Invoke(new Action(delegate ()
                {
                    ctl.Value = Value;
                    ctl.Update();
                }));
            }
            else
            {
                ctl.Value = Value;
                ctl.Update();
            }
        }
        void UpdateNUD_Value(NumericUpDown ctl, int Value)
        {
            if (ctl.InvokeRequired)
            {
                ctl.Invoke(new Action(delegate ()
                {
                    ctl.Value = Value;
                    ctl.Update();
                }));
            }
            else
            {
                ctl.Value = Value;
                ctl.Update();
            }
        }
        void Update_Enable(Control ctl, bool Enable)
        {
            if(ctl.InvokeRequired)
            {
                ctl.Invoke(new Action(delegate
                {
                    ctl.Enabled = Enable;
                }));
            }
            else
            {
                ctl.Enabled = Enable;
            }
        }

        Dictionary<string, int> ExistWord = new Dictionary<string, int>();
        LabelledVector[] LV;
        Umap umap;
        Bitmap Create_UMAP(string FilePath)
        {
            try
            {
                int DataP = 0, S = 0, W = File.ReadAllLines(FilePath).Length;

                if (SaveDataCount > 0) LV = new LabelledVector[SaveDataCount];
                else LV = new LabelledVector[W];

                Update_Visible(pb_ReadFile, true);
                Update_Visible(lbl_ReadingStatus, true);
                Update_Text(lbl_ReadingStatus, "Read the file...");

                // 메세지 팩 데이터 불러오기
                var data = MessagePackSerializer.Deserialize<LabelledVector[]>(File.ReadAllBytes(MessagePackPath));
                data = data.Take(10_000).ToArray();

                using (StreamReader sr = new StreamReader(FilePath))
                {
                    ViewWordsList.FilePath = Application.StartupPath + "/Data/" +FilePath.Split('\\').Last().Split('.')[0] + ".xml";
                    
                    while (!sr.EndOfStream)
                    {
                        string Line = sr.ReadLine();
                        string[] Text;
                        if (Line.Contains('\t'))
                        {
                            Text = Line.Split('\t');
                            S = Text.Length - 1;
                        }
                        else if (Line.Contains(','))
                        {
                            Text = Line.Split(',');
                            S = Text.Length - 1;
                        }
                        else
                        {
                            MessageBox.Show("Data Anomaly!!");
                            return null;
                        }
                       
                        if (Text[0] != null && Text[0] != "")
                        {
                            if (!WordsList.ContainsKey(Text[0])) WordsList.Add(Text[0], 1);

                            float[] Values = new float[S];

                            for (int CurrentCell = 1; CurrentCell < S; CurrentCell++)
                            {
                                var t = float.Parse(Text[CurrentCell]);
                                Values[CurrentCell] = t;
                            }

                            try
                            {
                                LV[DataP] = new LabelledVector();

                                LV[DataP].UID = Text[0];
                                LV[DataP].Vector = Values;

                                string[] ViewData = new string[4];
                                ViewData[0] = LV[DataP].UID;
                                ViewData[1] = "False";
                                ViewData[2] = "0";
                                ViewData[3] = "0";

                                ViewWordsList.Add(ViewData);

                                DataP++;
                            }
                            catch { MessageBox.Show(DataP.ToString() + "번 째 데이터가 이상합니다!"); }
                        }
                    }
                }

                Update_Text(lbl_AllSentencesCount, "All of Sentences Count : " + (S-1));
                Update_Text(lbl_AllWordsCount, "All of Words Count : " + W);

                var timer = Stopwatch.StartNew();
                umap = new Umap(distance: Umap.DistanceFunctions.Cosine, numberOfNeighbors: 200, customNumberOfEpochs: 100);
                Update_Text(lbl_ReadingStatus, "Start Fit...");
                var nEpochs = umap.InitializeFit(LV.Select(entry => entry.Vector).ToArray());

                Update_Visible(pb_ReadFile, false);
                Update_Visible(pb_UMAP, true);
                Update_Text(lbl_ReadingStatus, "Start Train...");
                for (var i = 0; i < nEpochs; i++)
                {
                    umap.Step();
                    if ((i % 10) == 0)
                    {
                        UpdatePB_Value(pb_UMAP, ValueTo100(nEpochs, i));
                    }
                }
                UpdatePB_Value(pb_UMAP, 0);
                Update_Visible(pb_UMAP, false);
                Update_Text(lbl_ReadingStatus, "Status");
                Update_Visible(lbl_ReadingStatus, false);

                var embeddings = umap.GetEmbedding()
                    .Select(vector => new { X = vector[0], Y = vector[1]})
                    .ToArray();

                timer.Stop();

                var minX = embeddings.Min(vector => vector.X);
                var rangeX = embeddings.Max(vector => vector.X) - minX;

                var minY = embeddings.Min(vector => vector.Y);
                var rangeY = embeddings.Max(vector => vector.Y) - minY;

                var scaledEmbeddings = embeddings
                    .Select(vector => new { X = (vector.X - minX) / rangeX, Y = (vector.Y - minY) / rangeY })
                    .ToArray();



                var bitmap = new Bitmap(bitwidth, bitheight);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.FillRectangle(Brushes.DarkBlue, 0, 0, bitwidth, bitheight);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var font = new Font("Tahoma", 8))
                    {
                        int Count = 0;
                        foreach (var (vector, uid) in scaledEmbeddings.Zip(LV, (vector, entry) => (vector, entry.UID)).OrderByDescending(x => x.UID))
                        {
                            g.DrawString(uid, font, Brushes.White, vector.X * bitwidth, vector.Y * bitheight);
                            Count++;                            
                        }
                    }
                }

                bitmap.Save("LabelName-UMAP.png");

                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new Bitmap(bitwidth, bitheight);
            }
        }

        void WordListUpdate()
        {
            foreach (var Word in WordsList)
            {
              
            }
        }

        /// <summary>
        /// 백분율 구하기
        /// </summary>
        /// <param name="Max"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        int ValueTo100(int Max, int Value)
        {
            int result = 0;
            if (Value > Max)
            {
                MessageBox.Show("Exception!!\nValue don\'t not bigger than Max!" + $"\n{Max} : {Value}");
                return 100;
            }

            else
            {
                double Set = (double)Value / (double)Max;
                result = (int)(Set * 100);
            }

            if (result > 100)
            {
                MessageBox.Show(result.ToString());
                return 0;
            }
            else return result;
        }

        Point ZOOM_POINT = new Point(0, 0);
        /// <summary>
        /// 마우스 휠 작동 시 처리 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_UMAPImage_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                ZOOM_POINT = new Point(e.X, e.Y);

                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }

                int X = ValueTo100(pn_OuterPB.HorizontalScroll.Maximum, ZOOM_POINT.X);
                int Y = ValueTo100(pn_OuterPB.VerticalScroll.Maximum, ZOOM_POINT.Y);

                pn_OuterPB.HorizontalScroll.Value = (pn_OuterPB.HorizontalScroll.Maximum / 100) * X;
                pn_OuterPB.VerticalScroll.Value = (pn_OuterPB.VerticalScroll.Maximum / 100) * Y;

                {
                    /* 테스트 문구
                    lbl_AllSentencesCount.Text = $"ScrollPoint       X : {ZOOM_POINT.X,-5} Y : {ZOOM_POINT.Y,-5}\n\n" +
                                                                    $"ScrollSize        X : {pn_OuterPB.HorizontalScroll.Maximum,-5} Y : {pn_OuterPB.VerticalScroll.Maximum,-5}\n\n" +
                                                                    $"ScrollPoint       X : {X,-5} Y : {Y,-5}\n\n";
                    */
                }
            }
            catch { }
        }

        /// <summary>
        /// 줌 크기
        /// </summary>
        double ZOOM_FACTOR = 1.5;
        /// <summary>
        /// 줌 인
        /// </summary>
        void ZoomIn()
        {
            // 최대 배율
            int MAX = 5;

            if ((pb_UMAPImage.Width < (MAX * pn_OuterPB.Width)) &&
                (pb_UMAPImage.Height < (MAX * pn_OuterPB.Height)))
            {
                pb_UMAPImage.Width = Convert.ToInt32(pb_UMAPImage.Width * ZOOM_FACTOR);
                pb_UMAPImage.Height = Convert.ToInt32(pb_UMAPImage.Height * ZOOM_FACTOR);

                pb_UMAPImage.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        /// <summary>
        /// 줌 아웃
        /// </summary>
        void ZoomOut()
        {
            if ((pb_UMAPImage.Width > (pn_OuterPB.Width)) &&
                (pb_UMAPImage.Height > (pn_OuterPB.Height)))
            {
                pb_UMAPImage.SizeMode = PictureBoxSizeMode.StretchImage;
                pb_UMAPImage.Width = Convert.ToInt32(pb_UMAPImage.Width / ZOOM_FACTOR);
                pb_UMAPImage.Height = Convert.ToInt32(pb_UMAPImage.Height / ZOOM_FACTOR);
            }
            else
            {
                pb_UMAPImage.SizeMode = PictureBoxSizeMode.StretchImage;
                pb_UMAPImage.Width = pn_OuterPB.Width;
                pb_UMAPImage.Height = pn_OuterPB.Height;
            }
        }

        /// <summary>
        /// 사이즈 조정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pn_OuterPB_Resize(object sender, EventArgs e)
        {
            try
            {
                pb_UMAPImage.Width = pn_OuterPB.Width;
                pb_UMAPImage.Height = pn_OuterPB.Height;
            }
            catch { }
        }

        bool ScrollClick = false;
        Point ClickPoint = new Point();

        /// <summary>
        /// 이미지 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_UMAPImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ScrollClick)
            {
                ScrollClick = true;

                ClickPoint.X = e.X;
                ClickPoint.Y = e.Y;
            }
            else ScrollClick = true;
        }

        /// <summary>
        /// 이미지 클릭 해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_UMAPImage_MouseUp(object sender, MouseEventArgs e)
        {
            ScrollClick = false;
        }

        /// <summary>
        /// 클릭 후 움직이면 이벤트 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_UMAPImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (ScrollClick)
                {
                    if (e.X > ClickPoint.X) pn_OuterPB.HorizontalScroll.Value = CalScrollPoint(pn_OuterPB.HorizontalScroll.Maximum, pn_OuterPB.HorizontalScroll.Value + 5);
                    else if (e.X < ClickPoint.X) pn_OuterPB.HorizontalScroll.Value = CalScrollPoint(pn_OuterPB.HorizontalScroll.Maximum, pn_OuterPB.HorizontalScroll.Value - 5);

                    if (e.Y > ClickPoint.Y) pn_OuterPB.VerticalScroll.Value = CalScrollPoint(pn_OuterPB.VerticalScroll.Maximum, pn_OuterPB.VerticalScroll.Value + 5);
                    else if (e.Y < ClickPoint.Y) pn_OuterPB.VerticalScroll.Value = CalScrollPoint(pn_OuterPB.VerticalScroll.Maximum, pn_OuterPB.VerticalScroll.Value - 5);
                }
                {
                    /* 테스트 문구
                    lbl_AllSentencesCount.Text = $"ScrollPoint       X : {ScrollPoint.X,-5} Y : {ScrollPoint.Y,-5}\n\n" +
                                                                        $"pnScrollPoint  X : {pn.X,-5} Y : {pn.Y,-5}\n\n" +
                                                                        $"ClickPoint        X : {ClickPoint.X,-5} Y : {ClickPoint.Y,-5}\n" +
                                                                        $"PictureBoxsize X : {pb_UMAPImage.Width,-5} Y : {pb_UMAPImage.Height,-5}";
                    */
                }
            }
            catch { }
        }

        int CalScrollPoint(int max, int point)
        {
            if (point < 0) return 0;
            else if (point > max) return max - 1;
            else return point;
        }

        /// <summary>
        /// 페이지 변경 시 이벤트 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tc_Pages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tc_Pages.SelectedIndex == 0)
            {
                lbl_StatusWords.Text = "Remove Words";
                gb_WordsData.Text = "Words to be removed";

                this.gb_WordsData.Controls.Clear();
                this.gb_WordsData.Controls.Add(RemoveWordsList);
            }
            else if(tc_Pages.SelectedIndex == 1)
            {
                lbl_StatusWords.Text = "Display Words";
                gb_WordsData.Text = "Words to be Displayed";

                this.gb_WordsData.Controls.Clear();
                this.gb_WordsData.Controls.Add(ViewWordsList);
            }
        }

        void InsertData(List<DataGridViewRow> data)
        {
            /*
            dgv_WordList.Rows.Clear();
            foreach(DataGridViewRow Row in data)
            {
                dgv_WordList.Rows.Add(Row);
            }
            */
        }

        private void dgv_WordList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if(tc_Pages.SelectedIndex == 1 && e.ColumnIndex == 1)
            {
                if (dgv_WordList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Not View")
                {
                    dgv_WordList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "View";
                }
                else dgv_WordList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Not View";
            }
            */
        }

        void UmapUpdate(Bitmap bitmap)
        {
            if(pb_UMAPImage.InvokeRequired)
            {
               pb_UMAPImage.Invoke(new Action(delegate
               {
                   pb_UMAPImage.BackgroundImage = bitmap;
               }));
            }
            else
            {
                pb_UMAPImage.BackgroundImage = bitmap;
            }
        }

        private void rb_AllWords_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AllWords.Checked)
            {
                var embeddings = umap.GetEmbedding()
                       .Select(vector => new { X = vector[0], Y = vector[1] })
                       .ToArray();

                var minX = embeddings.Min(vector => vector.X);
                var rangeX = embeddings.Max(vector => vector.X) - minX;

                var minY = embeddings.Min(vector => vector.Y);
                var rangeY = embeddings.Max(vector => vector.Y) - minY;

                var scaledEmbeddings = embeddings
                    .Select(vector => new { X = (vector.X - minX) / rangeX, Y = (vector.Y - minY) / rangeY })
                    .ToArray();



                var bitmap = new Bitmap(bitwidth, bitheight);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.FillRectangle(Brushes.DarkBlue, 0, 0, bitwidth, bitheight);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var font = new Font("Tahoma", 8))
                    {
                        int Count = 0;
                        foreach (var (vector, uid) in scaledEmbeddings.Zip(LV, (vector, entry) => (vector, entry.UID)).OrderByDescending(x => x.UID))
                        {
                            g.DrawString(uid, font, Brushes.White, vector.X * bitwidth, vector.Y * bitheight);
                            Count++;
                        }
                    }
                }

                UmapUpdate(bitmap);
            }
        }

        private void rb_SelectedWords_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SelectedWords.Checked)
            {
                var embeddings = umap.GetEmbedding()
                     .Select(vector => new { X = vector[0], Y = vector[1] })
                     .ToArray();

                var minX = embeddings.Min(vector => vector.X);
                var rangeX = embeddings.Max(vector => vector.X) - minX;

                var minY = embeddings.Min(vector => vector.Y);
                var rangeY = embeddings.Max(vector => vector.Y) - minY;

                var scaledEmbeddings = embeddings
                    .Select(vector => new { X = (vector.X - minX) / rangeX, Y = (vector.Y - minY) / rangeY })
                    .ToArray();



                var bitmap = new Bitmap(bitwidth, bitheight);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.FillRectangle(Brushes.DarkBlue, 0, 0, bitwidth, bitheight);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var font = new Font("Tahoma", 8))
                    {
                        /*
                        foreach (var (vector, uid) in scaledEmbeddings.Zip(LV, (vector, entry) => (vector, entry.UID)).OrderByDescending(x => x.UID))
                        {
                            for (int i = 0; i < dgv_WordList.Rows.Count; i++)
                            {
                                if (dgv_WordList.Rows[i].Cells[1].Value.ToString() == "View" && dgv_WordList.Rows[i].Cells[0].Value.ToString() == uid)
                                {
                                    g.DrawString(uid, font, Brushes.White, vector.X * bitwidth, vector.Y * bitheight);
                                }
                            }
                        }*/
                    }

                    UmapUpdate(bitmap);
                }
            }
        }

        
        private void AnalysisForm_Load(object sender, EventArgs e)
        {
            RemoveWordsList.Dock = DockStyle.Fill;
            ViewWordsList.Dock = DockStyle.Fill;

            this.gb_WordsData.Controls.Add(RemoveWordsList);
        }

        private void splitCon_TOP_Resize(object sender, EventArgs e)
        {
            if(tc_Pages.InvokeRequired)
            {
                tc_Pages.Invoke(new Action(delegate
                {
                    tc_Pages.Width = splitCon_TOP.Panel1.Width;
                }));
            }
            else
            {
                tc_Pages.Width = splitCon_TOP.Panel1.Width;
            }
        }

        private void sc_Umap_Resize(object sender, EventArgs e)
        {
            if (sc_Umap.SplitterDistance < 320) sc_Umap.SplitterDistance = 320;
        }

        int eraPanel1Width = 0;
        private void btn_AnalyzedInformationClose_Click(object sender, EventArgs e)
        {
            Button btn = new Button();
            btn.BackColor = Color.DimGray;
            btn.ForeColor = Color.White;
            btn.Text = "O\nP\nE\nN";
            btn.AutoSize = true;
            btn.Width = 20;
            btn.Dock = DockStyle.Left;
            btn.Click += Btn_AnalyzedInformationOpenClick;

            eraPanel1Width = sc_Umap.SplitterDistance;
            sc_Umap.SplitterDistance = 25;
            sc_Umap.Panel1.Controls.Clear();
            sc_Umap.Panel1.Controls.Add(btn);
        }

        private void Btn_AnalyzedInformationOpenClick(object sender, EventArgs e)
        {
            sc_Umap.Panel1.Controls.Clear();
            sc_Umap.Panel1.Controls.Add(gb_Information);
            sc_Umap.SplitterDistance = eraPanel1Width;
        }
    }

    /// <summary>
    /// 정렬
    /// </summary>
    class RowComparer : System.Collections.IComparer
    {
        private static int sortOrderModifier = 1;

        public RowComparer(SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Descending)
            {
                sortOrderModifier = -1;
            }
            else if (sortOrder == SortOrder.Ascending)
            {
                sortOrderModifier = 1;
            }
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

            // Try to sort based on the Last Name column.
            int CompareResult = System.String.Compare(
                DataGridViewRow1.Cells[1].Value.ToString(),
                DataGridViewRow2.Cells[1].Value.ToString());

            // If the Last Names are equal, sort based on the First Name.
            if (CompareResult == 0)
            {
                CompareResult = System.String.Compare(
                    DataGridViewRow1.Cells[0].Value.ToString(),
                    DataGridViewRow2.Cells[0].Value.ToString());
            }
            return CompareResult * sortOrderModifier;
        }
    }

    /// <summary>
    /// 메세지팩 클래스
    /// </summary>
    [MessagePackObject]
    public sealed class LabelledVector
    {
        [Key(0)] public string UID;
        [Key(1)] public float[] Vector;
    }
}


