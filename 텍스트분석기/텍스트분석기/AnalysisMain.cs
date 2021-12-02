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

namespace 텍스트분석기
{
    public partial class AnalysisForm : Form
    {
        string OriginalFilePath = "";

        SortedList<string, int> AllText = new SortedList<string, int>();

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
            tb_WordNumber.Visible = false;
            lbl_SearchText.Visible = false;

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

            btn_Pause.Left = (pn_Bottom1.Width - btn_Pause.Width) / 2;

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
                ThreadNRead();
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

                for (int Row = 0; Row < dgv_RemoveWordList.Rows.Count; Row++)
                {
                    if (dgv_RemoveWordList.Rows[Row].Cells[0].Value != null)
                    {
                        sw.WriteLine(dgv_RemoveWordList.Rows[Row].Cells[0].Value);
                    }
                }

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
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = ".txt | *txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName, Encoding.UTF8))
                    {
                        dgv_RemoveWordList.Rows.Clear();

                        while (!sr.EndOfStream)
                        {
                            string Word = sr.ReadLine();
                            if (Word != "") dgv_RemoveWordList.Rows.Add(Word, false.ToString());
                        }
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
            if (tb_RemoveWord.Text != "")
            {
                bool check = false;
                foreach (DataGridViewRow Row in dgv_RemoveWordList.Rows)
                {
                    if (Row.Cells[0].Value != null)
                    {
                        if (Row.Cells[0].Value.ToString() == tb_RemoveWord.Text) check = true;
                    }
                }
                if (!check)
                {
                    dgv_RemoveWordList.Rows.Add(tb_RemoveWord.Text, false.ToString());
                    tb_RemoveWord.Text = "";
                }
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
                        btn_Pause.Visible = true;
                        btn_Pause.Text = "■";

                        btn_Export.Visible = true;
                    }

                    btn_StartAnalysis.Enabled = false;

                    while (!sr.EndOfStream)
                    {
                        string StrLine = sr.ReadLine();
                        StrLine = RemoveStrInWords(StrLine);

                        string[] StrWords = StrLine.Split(' ');

                        NLine++;

                        // 단어가 1개 이상일 때 실행
                        if (StrWords.Length > 0)
                        {
                            // 단어 개수만큼
                            for (int num = 0; num < StrWords.Length; num++)
                            {
                                // 이미 있는 단어인지 탐색
                                if (AllText.ContainsKey(StrWords[num])) AllText[StrWords[num]]++;
                                else
                                {
                                    if (StrWords[num] != "") AllText.Add(StrWords[num], 1);
                                }
                                tb_WordsCount.Text = AllText.Count.ToString();

                                // 현재 라인 번호 / 전체 라인 개 수
                                string Status = (NLine.ToString() + " / " + ALine.ToString());
                                tb_LinesCount.Text = Status;

                                // Pause인지 확인하는 구문
                                while (Pause)
                                {
                                    PauseEvent.WaitOne();
                                    Application.DoEvents();
                                }
                                Application.DoEvents();
                            }
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
                        else if (NLine % 1000 == 0 && NLine != 0)
                        {
                            pb_Update.Value = 0;
                            tb_WordNumber.Visible = true;
                            lbl_SearchText.Visible = true;
                            dgv_AnalysisResult.Rows.Clear();

                            int CurrentNumber = 1;


                            // 업데이트 구문
                            foreach (var Text in AllText)
                            {
                                tb_WordNumber.Text = CurrentNumber.ToString();
                                Application.DoEvents();
                                if (Text.Value > 100)
                                {
                                    dgv_AnalysisResult.Rows.Add(Text.Key, Text.Value);
                                }
                                CurrentNumber++;
                            }
                            dgv_AnalysisResult.Sort(new RowComparer(SortOrder.Descending));

                            tb_WordNumber.Visible = false;
                            lbl_SearchText.Visible = false;
                        }

                        // 그 외에는 프로그래스 바 진행도 올리기
                        else if (NLine % 100 == 0 && NLine != 0)
                        {
                            pb_Update.Value = NLine % 1000 / 10;
                        }

                    }
                }

                btn_StartAnalysis.Visible = true;
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
                foreach (DataGridViewRow Row in dgv_RemoveWordList.Rows)
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
            btn_Pause.Left = (pn_Bottom1.Width - btn_Pause.Width) / 2;
            this.Refresh();
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
            pb_UMAPImage.Refresh();
            Application.DoEvents();
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
                    ofd.Filter = "텍스트 데이터|*.txt|탭 분리 데이터|*.tsv";
                    DialogResult dr = ofd.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        SavePath = ofd.FileName;
                        Current = Create_UMAP(SavePath);
                        pb_UMAPImage.BackgroundImage = Current;
                        pb_UMAPImage.Refresh();
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

        Dictionary<string, int> ExistWord = new Dictionary<string, int>();
        Bitmap Create_UMAP(string FilePath)
        {
            try
            {
                LabelledVector[] LV;

                int DataP = 0, S = 0, W = File.ReadAllLines(FilePath).Length;

                if (SaveDataCount > 0) LV = new LabelledVector[SaveDataCount];
                else LV = new LabelledVector[W];

                pb_ReadFile.Visible = true;
                lbl_ReadingStatus.Visible = true;
                lbl_ReadingStatus.Text = "Read the file...";

                // 메세지 팩 데이터 불러오기
                var data = MessagePackSerializer.Deserialize<LabelledVector[]>(File.ReadAllBytes(MessagePackPath));
                data = data.Take(10_0000).ToArray();

                using (StreamReader sr = new StreamReader(FilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        Application.DoEvents();
                        string[] Text = sr.ReadLine().Split('\t');
                        S = Text.Length - 1;
                        
                        if (Text[0] != null && Text[0] != "")
                        {
                            float[] Values = new float[S];

                            for (int CurrentCell = 1; CurrentCell < S; CurrentCell++)
                            {
                                var t = float.Parse(Text[CurrentCell]);
                                Values[CurrentCell] = t;
                                Application.DoEvents();
                            }

                            try
                            {
                                LV[DataP] = new LabelledVector();

                                LV[DataP].UID = Text[0];
                                LV[DataP].Vector = Values;

                                DataP++;
                                Application.DoEvents();
                            }
                            catch { MessageBox.Show(DataP.ToString() + "번 째 데이터가 이상합니다!"); }
                        }
                        Application.DoEvents();
                    }
                }

                lbl_AllSentencesCount.Text = "All of Sentences Count : " + (S-1);
                lbl_AllWordsCount.Text = "All of Words Count : " + W;

                
                lbl_ReadingStatus.Text = "Train...";

                var timer = Stopwatch.StartNew();
                var umap = new Umap(distance: Umap.DistanceFunctions.CosineForNormalizedVectors);
                var nEpochs = umap.InitializeFit(LV.Select(entry => entry.Vector).ToArray());
                
                pb_ReadFile.Visible = false;
                pb_UMAP.Visible = true;
                for (var i = 0; i < nEpochs; i++)
                {
                    umap.Step();
                    if ((i % 10) == 0)
                    {
                        pb_UMAP.Value = ValueTo100(nEpochs, i);
                        Application.DoEvents();
                    }
                    Application.DoEvents();
                }
                pb_UMAP.Value = 0;
                pb_UMAP.Visible = false;
                lbl_ReadingStatus.Text = "Status";
                lbl_ReadingStatus.Visible = false;

                var embeddings = umap.GetEmbedding()
                    .Select(vector => new { X = vector[0], Y = vector[1]})
                    .ToArray();

                Application.DoEvents();
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

                            Application.DoEvents();
                            
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
                MessageBox.Show("Exception!!\nValue don\'t not bigger than Max!");
                return result;
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

                pb_UMAPImage.Refresh();
                Application.DoEvents();
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


