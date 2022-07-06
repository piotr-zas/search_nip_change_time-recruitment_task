using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Diagnostics;



namespace search_nip_change_time_recruitment_task
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sqlServerTextbox_TextChanged(object sender, EventArgs e)
        {
            //hide connection status and save settings
            sqlStateConnLabel.Visible = false;
            Properties.Settings.Default.sqlServerTextboxSetting = sqlServerTextbox.Text;
            Properties.Settings.Default.Save();

        }

        private void sqlDatabaseTextbox_TextChanged(object sender, EventArgs e)
        {
            //hide connection status and save settings
            sqlStateConnLabel.Visible = false;
            Properties.Settings.Default.sqlDatabaseTextboxSetting = sqlDatabaseTextbox.Text;
            Properties.Settings.Default.Save();
        }

        private void sqlUserLoginTextbox_TextChanged(object sender, EventArgs e)
        {
            //hide connection status and save settings
            sqlStateConnLabel.Visible = false;
            Properties.Settings.Default.sqlUserLoginTextboxSetting = sqlUserLoginTextbox.Text;
            Properties.Settings.Default.Save();
        }

        private void sqlUserPassTextbox_TextChanged(object sender, EventArgs e)
        {
            //hide connection status and save settings
            sqlStateConnLabel.Visible = false;
            Properties.Settings.Default.sqlUserPassTextboxSetting = sqlUserPassTextbox.Text;
            Properties.Settings.Default.Save();
        }





        private void sqlConnectTestBtn_Click(object sender, EventArgs e)
        {
            testConnectionBgw.RunWorkerAsync();
        }





        string GetSqlConnectionString()
        {
            string connectionString = "";



            this.Invoke(new Action(() =>
            {
                //server
                connectionString += $"server={sqlServerTextbox.Text};";

                //user login
                connectionString += $"user id={sqlUserLoginTextbox.Text};";

                //user pass
                connectionString += $"password={sqlUserPassTextbox.Text};";

                //database
                connectionString += $"database={sqlDatabaseTextbox.Text};";
            }));

            return connectionString;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //get settings
            sqlServerTextbox.Text = Properties.Settings.Default.sqlServerTextboxSetting;
            sqlDatabaseTextbox.Text = Properties.Settings.Default.sqlDatabaseTextboxSetting;
            sqlUserLoginTextbox.Text = Properties.Settings.Default.sqlUserLoginTextboxSetting;
            sqlUserPassTextbox.Text = Properties.Settings.Default.sqlUserPassTextboxSetting;
            srtFilePathTextBox.Text = Properties.Settings.Default.srtFilePathSetting;


            if (File.Exists(srtFilePathTextBox.Text))
            {
                startSrtChangeTimeBtn.Enabled = true;
            }
            else startSrtChangeTimeBtn.Enabled = false;



        }

        private void testConnectionBgw_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke(new Action(() => {

                SetGuiStatus(false);
                sqlStateConnLabel.Text = "Trwa testowanie...";
                sqlStateConnLabel.ForeColor = Color.Blue;
                sqlStateConnLabel.Visible = true;

            }));




            try
            {

                using (MySqlConnection connection = new MySqlConnection(GetSqlConnectionString()))
                {
                    connection.Open();
                    this.Invoke(new Action(() =>
                    {
                        sqlStateConnLabel.ForeColor = Color.Green;

                        sqlStateConnLabel.Text = "Status połączenia: " + connection.State.ToString();
                        sqlStateConnLabel.Visible = true;
                    }));
                }
            }
            catch (Exception error)
            {
                this.Invoke(new Action(() =>
                {
                    sqlStateConnLabel.ForeColor = Color.Red;
                    sqlStateConnLabel.Text = "Błąd połączenia: " + error.Message;
                    sqlStateConnLabel.Visible = true;
                }));
            }
        }


        private void testConnectionBgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new Action(() => {

                SetGuiStatus(true);

            }));


        }




        void SetGuiStatus(bool status)
        {
            sqlSettingsGroupBox.Enabled = status;
            employerNIPTextbox.Enabled = status;
            searchEmployerDataBtn.Enabled = status;
        }





        //textbox only accept numbers
        private void employerNIPTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void searchEmployerDataBtn_Click(object sender, EventArgs e)
        {
            searchDataByNipBgw.RunWorkerAsync();
        }







        private void selectSrtFileBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            if (!File.Exists(openFileDialog1.FileName)) return;
            srtFilePathTextBox.Text = openFileDialog1.FileName;
        }








        private void startSrtChangeTimeBtn_Click(object sender, EventArgs e)
        {
            List<SrtFileElements> srtFileElementsList = new List<SrtFileElements>(ReadSrtFile(srtFilePathTextBox.Text).ToArray());


            AddTimeToSrtSubtitlesList(srtFileElementsList, 0, 5, 880);

            List<SrtFileElements> srtFileElementListZeroMilliseconds = GetSrtDataListIfStartMillisecondsIsZero(srtFileElementsList);


            string pathSubtitlesNoneZeroMill = Path.GetDirectoryName(srtFilePathTextBox.Text) + "\\"
                + Path.GetFileNameWithoutExtension(srtFilePathTextBox.Text)
                + "-non-zero-miliseconds"
                + Path.GetExtension(srtFilePathTextBox.Text);


            string pathSubtitlesZeroMill = Path.GetDirectoryName(srtFilePathTextBox.Text) + "\\"
                + Path.GetFileNameWithoutExtension(srtFilePathTextBox.Text)
               + "-zero-miliseconds"
               + Path.GetExtension(srtFilePathTextBox.Text);


            SaveListSrtElementsToFile(srtFileElementsList, pathSubtitlesNoneZeroMill);

            SaveListSrtElementsToFile(srtFileElementListZeroMilliseconds, pathSubtitlesZeroMill);

            MessageBox.Show("Gotowe!");

            Process.Start(Path.GetDirectoryName(srtFilePathTextBox.Text));
        }

        private void srtFilePath_TextChanged(object sender, EventArgs e)
        {

            if (File.Exists(srtFilePathTextBox.Text))
            {
                startSrtChangeTimeBtn.Enabled = true;
                Properties.Settings.Default.srtFilePathSetting = srtFilePathTextBox.Text;
                Properties.Settings.Default.Save();
            }
            else startSrtChangeTimeBtn.Enabled = false;
        }

        private void searchDataByNipBgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string typedNip = "";
            this.Invoke(new Action(() => {
                typedNip = employerNIPTextbox.Text;
                SetGuiStatus(false);
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            }));






            if (typedNip.Length != 10)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Podano zbyt krótki NIP", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }));
            }

            
                (EntityResponse employerData, HttpResponseMessage response) = GetDataFromNIP(typedNip);
          
            //check not found nip or response error
            if (CheckAndMessageErrorResponse(employerData, response)) return;




            SetOfNeccessaryTables allTables = new SetOfNeccessaryTables();


            //create tables
            if (CreateNecessaryTablesInSql(allTables.EntityItemSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.EntitySqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.PartnersSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.AuthorizedClerksSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.RepresentativesSqlTable)) return;

            
            if (SendAllDataToSql(employerData, allTables)) return;

            List<DataTable> tablesFromMySql = GetDataFromSql(allTables);

            if (tablesFromMySql == null) return;
            HtmlFileWithData fileHtml = new HtmlFileWithData("index.html", tablesFromMySql);


            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Gotowe!");
            }));
            File.WriteAllText(fileHtml.PathOfFile, fileHtml.HtmlCode);
            if (File.Exists(fileHtml.PathOfFile)) Process.Start(fileHtml.PathOfFile);


        }

        private void searchDataByNipBgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke(new Action(() => {
                progressBar1.Value = e.ProgressPercentage;
              
            }));
        }

        private void searchDataByNipBgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new Action(() => {

                SetGuiStatus(true);
                progressBar1.Visible = false;
            }));
        }
    }














}
