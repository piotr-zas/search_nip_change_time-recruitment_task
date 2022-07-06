
namespace search_nip_change_time_recruitment_task
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.searchEmployerDataBtn = new System.Windows.Forms.Button();
            this.employerNIPTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.sqlSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.sqlDatabaseTextbox = new System.Windows.Forms.TextBox();
            this.sqlStateConnLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sqlConnectTestBtn = new System.Windows.Forms.Button();
            this.sqlServerTextbox = new System.Windows.Forms.TextBox();
            this.sqlUserPassTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sqlUserLoginTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.startSrtChangeTimeBtn = new System.Windows.Forms.Button();
            this.selectSrtFileBtn = new System.Windows.Forms.Button();
            this.srtFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.testConnectionBgw = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.mainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.sqlSettingsGroupBox.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Controls.Add(this.tabPage1);
            this.mainTabControl.Controls.Add(this.tabPage2);
            this.mainTabControl.Location = new System.Drawing.Point(12, 27);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1528, 847);
            this.mainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.searchEmployerDataBtn);
            this.tabPage1.Controls.Add(this.employerNIPTextbox);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.sqlSettingsGroupBox);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1512, 800);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Informacje o przedsiębiorcach";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // searchEmployerDataBtn
            // 
            this.searchEmployerDataBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchEmployerDataBtn.Location = new System.Drawing.Point(394, 294);
            this.searchEmployerDataBtn.Name = "searchEmployerDataBtn";
            this.searchEmployerDataBtn.Size = new System.Drawing.Size(183, 43);
            this.searchEmployerDataBtn.TabIndex = 12;
            this.searchEmployerDataBtn.Text = "Wyszukaj dane";
            this.searchEmployerDataBtn.UseVisualStyleBackColor = true;
            this.searchEmployerDataBtn.Click += new System.EventHandler(this.searchEmployerDataBtn_Click);
            // 
            // employerNIPTextbox
            // 
            this.employerNIPTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.employerNIPTextbox.Location = new System.Drawing.Point(79, 300);
            this.employerNIPTextbox.MaxLength = 10;
            this.employerNIPTextbox.Name = "employerNIPTextbox";
            this.employerNIPTextbox.Size = new System.Drawing.Size(288, 31);
            this.employerNIPTextbox.TabIndex = 12;
            this.employerNIPTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.employerNIPTextbox_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 303);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 25);
            this.label6.TabIndex = 11;
            this.label6.Text = "NIP:";
            // 
            // sqlSettingsGroupBox
            // 
            this.sqlSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlSettingsGroupBox.Controls.Add(this.sqlDatabaseTextbox);
            this.sqlSettingsGroupBox.Controls.Add(this.sqlStateConnLabel);
            this.sqlSettingsGroupBox.Controls.Add(this.label1);
            this.sqlSettingsGroupBox.Controls.Add(this.sqlConnectTestBtn);
            this.sqlSettingsGroupBox.Controls.Add(this.sqlServerTextbox);
            this.sqlSettingsGroupBox.Controls.Add(this.sqlUserPassTextbox);
            this.sqlSettingsGroupBox.Controls.Add(this.label2);
            this.sqlSettingsGroupBox.Controls.Add(this.sqlUserLoginTextbox);
            this.sqlSettingsGroupBox.Controls.Add(this.label3);
            this.sqlSettingsGroupBox.Controls.Add(this.label4);
            this.sqlSettingsGroupBox.Location = new System.Drawing.Point(15, 19);
            this.sqlSettingsGroupBox.Name = "sqlSettingsGroupBox";
            this.sqlSettingsGroupBox.Size = new System.Drawing.Size(1478, 250);
            this.sqlSettingsGroupBox.TabIndex = 10;
            this.sqlSettingsGroupBox.TabStop = false;
            this.sqlSettingsGroupBox.Text = "Ustawienia serwera SQL";
            // 
            // sqlDatabaseTextbox
            // 
            this.sqlDatabaseTextbox.Location = new System.Drawing.Point(248, 93);
            this.sqlDatabaseTextbox.Name = "sqlDatabaseTextbox";
            this.sqlDatabaseTextbox.Size = new System.Drawing.Size(508, 31);
            this.sqlDatabaseTextbox.TabIndex = 5;
            this.sqlDatabaseTextbox.TextChanged += new System.EventHandler(this.sqlDatabaseTextbox_TextChanged);
            // 
            // sqlStateConnLabel
            // 
            this.sqlStateConnLabel.AutoSize = true;
            this.sqlStateConnLabel.Location = new System.Drawing.Point(784, 43);
            this.sqlStateConnLabel.Name = "sqlStateConnLabel";
            this.sqlStateConnLabel.Size = new System.Drawing.Size(183, 25);
            this.sqlStateConnLabel.TabIndex = 9;
            this.sqlStateConnLabel.Text = "Status połączenia";
            this.sqlStateConnLabel.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serwer SQL:";
            // 
            // sqlConnectTestBtn
            // 
            this.sqlConnectTestBtn.Location = new System.Drawing.Point(784, 187);
            this.sqlConnectTestBtn.Name = "sqlConnectTestBtn";
            this.sqlConnectTestBtn.Size = new System.Drawing.Size(183, 43);
            this.sqlConnectTestBtn.TabIndex = 8;
            this.sqlConnectTestBtn.Text = "Test połączenia";
            this.sqlConnectTestBtn.UseVisualStyleBackColor = true;
            this.sqlConnectTestBtn.Click += new System.EventHandler(this.sqlConnectTestBtn_Click);
            // 
            // sqlServerTextbox
            // 
            this.sqlServerTextbox.Location = new System.Drawing.Point(248, 43);
            this.sqlServerTextbox.Name = "sqlServerTextbox";
            this.sqlServerTextbox.Size = new System.Drawing.Size(508, 31);
            this.sqlServerTextbox.TabIndex = 1;
            this.sqlServerTextbox.TextChanged += new System.EventHandler(this.sqlServerTextbox_TextChanged);
            // 
            // sqlUserPassTextbox
            // 
            this.sqlUserPassTextbox.Location = new System.Drawing.Point(248, 193);
            this.sqlUserPassTextbox.Name = "sqlUserPassTextbox";
            this.sqlUserPassTextbox.PasswordChar = '*';
            this.sqlUserPassTextbox.Size = new System.Drawing.Size(508, 31);
            this.sqlUserPassTextbox.TabIndex = 7;
            this.sqlUserPassTextbox.TextChanged += new System.EventHandler(this.sqlUserPassTextbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baza danych:";
            // 
            // sqlUserLoginTextbox
            // 
            this.sqlUserLoginTextbox.Location = new System.Drawing.Point(248, 143);
            this.sqlUserLoginTextbox.Name = "sqlUserLoginTextbox";
            this.sqlUserLoginTextbox.Size = new System.Drawing.Size(508, 31);
            this.sqlUserLoginTextbox.TabIndex = 6;
            this.sqlUserLoginTextbox.TextChanged += new System.EventHandler(this.sqlUserLoginTextbox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(207, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nazwa użytkownika:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "Hasło użytkownika:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.startSrtChangeTimeBtn);
            this.tabPage2.Controls.Add(this.selectSrtFileBtn);
            this.tabPage2.Controls.Add(this.srtFilePathTextBox);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(8, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1512, 800);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Zmiana czasu wyświetlania napisów";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // startSrtChangeTimeBtn
            // 
            this.startSrtChangeTimeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startSrtChangeTimeBtn.Enabled = false;
            this.startSrtChangeTimeBtn.Location = new System.Drawing.Point(1184, 150);
            this.startSrtChangeTimeBtn.Name = "startSrtChangeTimeBtn";
            this.startSrtChangeTimeBtn.Size = new System.Drawing.Size(311, 45);
            this.startSrtChangeTimeBtn.TabIndex = 3;
            this.startSrtChangeTimeBtn.Text = "Zmień czas i podziel plik";
            this.startSrtChangeTimeBtn.UseVisualStyleBackColor = true;
            this.startSrtChangeTimeBtn.Click += new System.EventHandler(this.startSrtChangeTimeBtn_Click);
            // 
            // selectSrtFileBtn
            // 
            this.selectSrtFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectSrtFileBtn.Location = new System.Drawing.Point(1334, 32);
            this.selectSrtFileBtn.Name = "selectSrtFileBtn";
            this.selectSrtFileBtn.Size = new System.Drawing.Size(161, 45);
            this.selectSrtFileBtn.TabIndex = 2;
            this.selectSrtFileBtn.Text = "Wybierz...";
            this.selectSrtFileBtn.UseVisualStyleBackColor = true;
            this.selectSrtFileBtn.Click += new System.EventHandler(this.selectSrtFileBtn_Click);
            // 
            // srtFilePathTextBox
            // 
            this.srtFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.srtFilePathTextBox.Location = new System.Drawing.Point(130, 39);
            this.srtFilePathTextBox.Name = "srtFilePathTextBox";
            this.srtFilePathTextBox.Size = new System.Drawing.Size(1198, 31);
            this.srtFilePathTextBox.TabIndex = 1;
            this.srtFilePathTextBox.TextChanged += new System.EventHandler(this.srtFilePath_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "Plik SRT:";
            // 
            // testConnectionBgw
            // 
            this.testConnectionBgw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.testConnectionBgw_DoWork);
            this.testConnectionBgw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.testConnectionBgw_RunWorkerCompleted);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "SRT files|*.srt";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 886);
            this.Controls.Add(this.mainTabControl);
            this.Name = "Form1";
            this.Text = "Zadanie rekrutacyjne";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.mainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.sqlSettingsGroupBox.ResumeLayout(false);
            this.sqlSettingsGroupBox.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox sqlServerTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox sqlUserPassTextbox;
        private System.Windows.Forms.TextBox sqlUserLoginTextbox;
        private System.Windows.Forms.TextBox sqlDatabaseTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label sqlStateConnLabel;
        private System.Windows.Forms.Button sqlConnectTestBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox sqlSettingsGroupBox;
        private System.Windows.Forms.TextBox employerNIPTextbox;
        private System.ComponentModel.BackgroundWorker testConnectionBgw;
        private System.Windows.Forms.Button searchEmployerDataBtn;
        private System.Windows.Forms.Button selectSrtFileBtn;
        private System.Windows.Forms.TextBox srtFilePathTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button startSrtChangeTimeBtn;
    }
}

