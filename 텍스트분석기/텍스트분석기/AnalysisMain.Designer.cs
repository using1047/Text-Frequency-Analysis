
namespace 텍스트분석기
{
    partial class AnalysisForm
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_TextFilePath = new System.Windows.Forms.Label();
            this.btn_Load = new System.Windows.Forms.Button();
            this.dgv_AnalysisResult = new System.Windows.Forms.DataGridView();
            this.btn_StartAnalysis = new System.Windows.Forms.Button();
            this.btn_Export = new System.Windows.Forms.Button();
            this.btn_Import = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Pause = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tb_WordNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pb_Update = new System.Windows.Forms.ProgressBar();
            this.tb_WordsCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_LinesCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pn_RemoveWord = new System.Windows.Forms.Panel();
            this.pn_RemoveAddControl = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_SetSave = new System.Windows.Forms.Button();
            this.btn_SetLoad = new System.Windows.Forms.Button();
            this.btn_AddWords = new System.Windows.Forms.Button();
            this.tb_RemoveWord = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dgv_RemoveWordList = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_AnalysisResult)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pn_RemoveWord.SuspendLayout();
            this.pn_RemoveAddControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_RemoveWordList)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Path :";
            // 
            // lbl_TextFilePath
            // 
            this.lbl_TextFilePath.AutoSize = true;
            this.lbl_TextFilePath.Location = new System.Drawing.Point(92, 19);
            this.lbl_TextFilePath.Name = "lbl_TextFilePath";
            this.lbl_TextFilePath.Size = new System.Drawing.Size(40, 15);
            this.lbl_TextFilePath.TabIndex = 1;
            this.lbl_TextFilePath.Text = "none";
            // 
            // btn_Load
            // 
            this.btn_Load.Location = new System.Drawing.Point(371, 10);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(91, 32);
            this.btn_Load.TabIndex = 2;
            this.btn_Load.Text = "Find..";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // dgv_AnalysisResult
            // 
            this.dgv_AnalysisResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_AnalysisResult.Location = new System.Drawing.Point(15, 48);
            this.dgv_AnalysisResult.Name = "dgv_AnalysisResult";
            this.dgv_AnalysisResult.RowHeadersWidth = 51;
            this.dgv_AnalysisResult.RowTemplate.Height = 27;
            this.dgv_AnalysisResult.Size = new System.Drawing.Size(447, 355);
            this.dgv_AnalysisResult.TabIndex = 3;
            this.dgv_AnalysisResult.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgv_AnalysisResult_SortCompare);
            // 
            // btn_StartAnalysis
            // 
            this.btn_StartAnalysis.Location = new System.Drawing.Point(468, 9);
            this.btn_StartAnalysis.Name = "btn_StartAnalysis";
            this.btn_StartAnalysis.Size = new System.Drawing.Size(263, 58);
            this.btn_StartAnalysis.TabIndex = 4;
            this.btn_StartAnalysis.Text = "Start Frequency Analysis";
            this.btn_StartAnalysis.UseVisualStyleBackColor = true;
            this.btn_StartAnalysis.Click += new System.EventHandler(this.btn_StartAnalysis_Click);
            // 
            // btn_Export
            // 
            this.btn_Export.Location = new System.Drawing.Point(544, 3);
            this.btn_Export.Name = "btn_Export";
            this.btn_Export.Size = new System.Drawing.Size(187, 34);
            this.btn_Export.TabIndex = 5;
            this.btn_Export.Text = "DataGird Export...";
            this.btn_Export.UseVisualStyleBackColor = true;
            this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
            // 
            // btn_Import
            // 
            this.btn_Import.Location = new System.Drawing.Point(12, 3);
            this.btn_Import.Name = "btn_Import";
            this.btn_Import.Size = new System.Drawing.Size(187, 34);
            this.btn_Import.TabIndex = 6;
            this.btn_Import.Text = "DataGird Import...";
            this.btn_Import.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.btn_Pause);
            this.panel1.Controls.Add(this.btn_Export);
            this.panel1.Controls.Add(this.btn_Import);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 550);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(743, 40);
            this.panel1.TabIndex = 8;
            // 
            // btn_Pause
            // 
            this.btn_Pause.Location = new System.Drawing.Point(354, 3);
            this.btn_Pause.Name = "btn_Pause";
            this.btn_Pause.Size = new System.Drawing.Size(48, 34);
            this.btn_Pause.TabIndex = 7;
            this.btn_Pause.Text = " ▶";
            this.btn_Pause.UseVisualStyleBackColor = true;
            this.btn_Pause.Click += new System.EventHandler(this.btn_Pause_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tb_WordNumber);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.btn_StartAnalysis);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.pb_Update);
            this.panel2.Controls.Add(this.tb_WordsCount);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tb_LinesCount);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 409);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(743, 141);
            this.panel2.TabIndex = 9;
            // 
            // tb_WordNumber
            // 
            this.tb_WordNumber.BackColor = System.Drawing.SystemColors.HotTrack;
            this.tb_WordNumber.ForeColor = System.Drawing.Color.Transparent;
            this.tb_WordNumber.Location = new System.Drawing.Point(211, 112);
            this.tb_WordNumber.Name = "tb_WordNumber";
            this.tb_WordNumber.Size = new System.Drawing.Size(520, 25);
            this.tb_WordNumber.TabIndex = 15;
            this.tb_WordNumber.Text = "0";
            this.tb_WordNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(11, 116);
            this.label5.Margin = new System.Windows.Forms.Padding(100, 0, 100, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 19);
            this.label5.TabIndex = 14;
            this.label5.Text = "Searching for word.. :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(11, 85);
            this.label4.Margin = new System.Windows.Forms.Padding(100, 0, 100, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 19);
            this.label4.TabIndex = 13;
            this.label4.Text = "Update";
            // 
            // pb_Update
            // 
            this.pb_Update.Location = new System.Drawing.Point(83, 82);
            this.pb_Update.Name = "pb_Update";
            this.pb_Update.Size = new System.Drawing.Size(648, 22);
            this.pb_Update.TabIndex = 12;
            // 
            // tb_WordsCount
            // 
            this.tb_WordsCount.Location = new System.Drawing.Point(83, 40);
            this.tb_WordsCount.Name = "tb_WordsCount";
            this.tb_WordsCount.Size = new System.Drawing.Size(379, 25);
            this.tb_WordsCount.TabIndex = 11;
            this.tb_WordsCount.Text = "0";
            this.tb_WordsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(11, 46);
            this.label3.Margin = new System.Windows.Forms.Padding(100, 0, 100, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "Words :";
            // 
            // tb_LinesCount
            // 
            this.tb_LinesCount.Location = new System.Drawing.Point(83, 9);
            this.tb_LinesCount.Name = "tb_LinesCount";
            this.tb_LinesCount.Size = new System.Drawing.Size(379, 25);
            this.tb_LinesCount.TabIndex = 9;
            this.tb_LinesCount.Text = "0 / 0";
            this.tb_LinesCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(11, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(100, 0, 100, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 19);
            this.label2.TabIndex = 8;
            this.label2.Text = "Lines   : ";
            // 
            // pn_RemoveWord
            // 
            this.pn_RemoveWord.Controls.Add(this.label8);
            this.pn_RemoveWord.Controls.Add(this.pn_RemoveAddControl);
            this.pn_RemoveWord.Controls.Add(this.dgv_RemoveWordList);
            this.pn_RemoveWord.Dock = System.Windows.Forms.DockStyle.Right;
            this.pn_RemoveWord.Location = new System.Drawing.Point(468, 0);
            this.pn_RemoveWord.Name = "pn_RemoveWord";
            this.pn_RemoveWord.Size = new System.Drawing.Size(275, 409);
            this.pn_RemoveWord.TabIndex = 10;
            // 
            // pn_RemoveAddControl
            // 
            this.pn_RemoveAddControl.Controls.Add(this.label7);
            this.pn_RemoveAddControl.Controls.Add(this.btn_SetSave);
            this.pn_RemoveAddControl.Controls.Add(this.btn_SetLoad);
            this.pn_RemoveAddControl.Controls.Add(this.btn_AddWords);
            this.pn_RemoveAddControl.Controls.Add(this.tb_RemoveWord);
            this.pn_RemoveAddControl.Controls.Add(this.label6);
            this.pn_RemoveAddControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pn_RemoveAddControl.Location = new System.Drawing.Point(0, 0);
            this.pn_RemoveAddControl.Name = "pn_RemoveAddControl";
            this.pn_RemoveAddControl.Size = new System.Drawing.Size(275, 133);
            this.pn_RemoveAddControl.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 15);
            this.label7.TabIndex = 8;
            this.label7.Text = "Words Setting";
            // 
            // btn_SetSave
            // 
            this.btn_SetSave.Location = new System.Drawing.Point(147, 107);
            this.btn_SetSave.Name = "btn_SetSave";
            this.btn_SetSave.Size = new System.Drawing.Size(120, 23);
            this.btn_SetSave.TabIndex = 7;
            this.btn_SetSave.Text = "Save..";
            this.btn_SetSave.UseVisualStyleBackColor = true;
            this.btn_SetSave.Click += new System.EventHandler(this.btn_SetSave_Click);
            // 
            // btn_SetLoad
            // 
            this.btn_SetLoad.Location = new System.Drawing.Point(10, 107);
            this.btn_SetLoad.Name = "btn_SetLoad";
            this.btn_SetLoad.Size = new System.Drawing.Size(120, 23);
            this.btn_SetLoad.TabIndex = 6;
            this.btn_SetLoad.Text = "Load..";
            this.btn_SetLoad.UseVisualStyleBackColor = true;
            this.btn_SetLoad.Click += new System.EventHandler(this.btn_SetLoad_Click);
            // 
            // btn_AddWords
            // 
            this.btn_AddWords.Location = new System.Drawing.Point(10, 58);
            this.btn_AddWords.Name = "btn_AddWords";
            this.btn_AddWords.Size = new System.Drawing.Size(257, 23);
            this.btn_AddWords.TabIndex = 5;
            this.btn_AddWords.Text = "Add";
            this.btn_AddWords.UseVisualStyleBackColor = true;
            this.btn_AddWords.Click += new System.EventHandler(this.btn_AddWords_Click);
            // 
            // tb_RemoveWord
            // 
            this.tb_RemoveWord.Location = new System.Drawing.Point(10, 27);
            this.tb_RemoveWord.Name = "tb_RemoveWord";
            this.tb_RemoveWord.Size = new System.Drawing.Size(257, 25);
            this.tb_RemoveWord.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "Remove Word";
            // 
            // dgv_RemoveWordList
            // 
            this.dgv_RemoveWordList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_RemoveWordList.Location = new System.Drawing.Point(6, 154);
            this.dgv_RemoveWordList.Name = "dgv_RemoveWordList";
            this.dgv_RemoveWordList.RowHeadersWidth = 51;
            this.dgv_RemoveWordList.RowTemplate.Height = 27;
            this.dgv_RemoveWordList.Size = new System.Drawing.Size(257, 249);
            this.dgv_RemoveWordList.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(150, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "Words to be removed";
            // 
            // AnalysisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 590);
            this.Controls.Add(this.pn_RemoveWord);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgv_AnalysisResult);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.lbl_TextFilePath);
            this.Controls.Add(this.label1);
            this.Name = "AnalysisForm";
            this.Text = "Analysis Program";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_AnalysisResult)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pn_RemoveWord.ResumeLayout(false);
            this.pn_RemoveWord.PerformLayout();
            this.pn_RemoveAddControl.ResumeLayout(false);
            this.pn_RemoveAddControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_RemoveWordList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_TextFilePath;
        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.DataGridView dgv_AnalysisResult;
        private System.Windows.Forms.Button btn_StartAnalysis;
        private System.Windows.Forms.Button btn_Export;
        private System.Windows.Forms.Button btn_Import;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_WordsCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_LinesCount;
        private System.Windows.Forms.ProgressBar pb_Update;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_WordNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_Pause;
        private System.Windows.Forms.Panel pn_RemoveWord;
        private System.Windows.Forms.DataGridView dgv_RemoveWordList;
        private System.Windows.Forms.Panel pn_RemoveAddControl;
        private System.Windows.Forms.Button btn_AddWords;
        private System.Windows.Forms.TextBox tb_RemoveWord;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_SetSave;
        private System.Windows.Forms.Button btn_SetLoad;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

