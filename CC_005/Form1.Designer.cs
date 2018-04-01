namespace CC_005 {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.b_01 = new System.Windows.Forms.Button();
            this.c_01 = new System.Windows.Forms.ComboBox();
            this.ss001 = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // b_01
            // 
            this.b_01.Location = new System.Drawing.Point(508, 74);
            this.b_01.Name = "b_01";
            this.b_01.Size = new System.Drawing.Size(75, 23);
            this.b_01.TabIndex = 0;
            this.b_01.Text = "确定";
            this.b_01.UseVisualStyleBackColor = true;
            this.b_01.Click += new System.EventHandler(this.button1_Click);
            // 
            // c_01
            // 
            this.c_01.FormattingEnabled = true;
            this.c_01.Location = new System.Drawing.Point(60, 74);
            this.c_01.Name = "c_01";
            this.c_01.Size = new System.Drawing.Size(355, 20);
            this.c_01.TabIndex = 1;
            this.c_01.Text = "dsfsdfsfsdfd";
            this.c_01.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // ss001
            // 
            this.ss001.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ss001.FormattingEnabled = true;
            this.ss001.Items.AddRange(new object[] {
            ""});
            this.ss001.Location = new System.Drawing.Point(40, 183);
            this.ss001.Name = "ss001";
            this.ss001.Size = new System.Drawing.Size(916, 612);
            this.ss001.TabIndex = 2;
            this.ss001.SelectedIndexChanged += new System.EventHandler(this.ss001_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1035, 882);
            this.Controls.Add(this.ss001);
            this.Controls.Add(this.c_01);
            this.Controls.Add(this.b_01);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button b_01;
        private System.Windows.Forms.ComboBox c_01;
        private System.Windows.Forms.CheckedListBox ss001;
    }
}

