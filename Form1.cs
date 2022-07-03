using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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

            //server
            connectionString += $"Data Source={sqlServerTextbox.Text};";

            //database
            connectionString += $"Initial Catalog={sqlDatabaseTextbox.Text};";

            //user login
            connectionString += $"User ID={sqlUserLoginTextbox.Text};";

            //user pass
            connectionString += $"Password={sqlUserPassTextbox.Text};";

            return connectionString;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //get settings
            sqlServerTextbox.Text = Properties.Settings.Default.sqlServerTextboxSetting;
            sqlDatabaseTextbox.Text = Properties.Settings.Default.sqlDatabaseTextboxSetting;
            sqlUserLoginTextbox.Text = Properties.Settings.Default.sqlUserLoginTextboxSetting;
            sqlUserPassTextbox.Text = Properties.Settings.Default.sqlUserPassTextboxSetting;
         
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
                using (SqlConnection connection = new SqlConnection(GetSqlConnectionString()))
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
            
            if (employerNIPTextbox.Text.Length != 10)
            {
                MessageBox.Show("Podano zbyt krótki NIP", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            (EntityResponse employerData, HttpResponseMessage response) = GetDataFromNIP(employerNIPTextbox.Text);
            //check not found nip or response error
            if (CheckAndMessageErrorResponse(employerData, response)) return;




            SetOfNeccessaryTables allTables = new SetOfNeccessaryTables();


            //create tables
            if (CreateNecessaryTablesInSql(allTables.EntityItemSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.EntitySqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.PartnersSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.AuthorizedClerksSqlTable)) return;
            if (CreateNecessaryTablesInSql(allTables.RepresentativesSqlTable)) return;

        }








        /// <summary>
        /// Get data from gov database
        /// </summary>
        /// <param name="nip"> Employer NIP</param>
        /// <returns>Object with data employer and response </returns>
        private (EntityResponse dataObject, HttpResponseMessage response) GetDataFromNIP(string nip)
        {
            using (HttpClient client = new HttpClient())
            {
                //date
                string urlParameters = $"?date={DateTime.Now.ToString("yyyy-MM-dd")}";

                //base address
                client.BaseAddress = new Uri($"https://wl-api.mf.gov.pl/api/search/nip/{nip}");

                //json format
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                string responseJson = response.Content.ReadAsStringAsync().Result;


                return (JsonConvert.DeserializeObject<EntityResponse>(responseJson), response);

            }
        }





        /// <summary>
        /// Check errors in response from gov database
        /// </summary>
        /// <param name="employerData"></param>
        /// <param name="response"></param>
        /// <returns>True if error exist</returns>
        private bool CheckAndMessageErrorResponse(EntityResponse employerData, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString(), "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            if (employerData.result.subject == null)
            {
                MessageBox.Show("Nie znaleziono takiego numeru NIP w bazie", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }




        /// <summary>
        /// Create necessary tables in SQL
        /// </summary>
        /// <param name="tableInfo">Table with name, column with type and only columns names</param>
        /// <returns>Return True if error</returns>
        private bool CreateNecessaryTablesInSql(TableWithColumns tableInfo)
        {

            using (SqlConnection connection = new SqlConnection(GetSqlConnectionString()))
            {
                try
                {
                    connection.Open();

                    string commandString = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableInfo.TableName}') CREATE TABLE {tableInfo.TableName} ({String.Join(", ", tableInfo.ListColumnsWithType)});";

                    using (SqlCommand command = new SqlCommand(commandString, connection))
                    {

                        command.ExecuteNonQuery();
                    }


                    return false;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }

            }
        }







    }






    public class SetOfNeccessaryTables
    {
        public TableWithColumns EntityItemSqlTable { get; set; }
        public TableWithColumns EntitySqlTable { get; set; }
        public TableWithColumns RepresentativesSqlTable { get; set; }
        public TableWithColumns AuthorizedClerksSqlTable { get; set; }
        public TableWithColumns PartnersSqlTable { get; set; }


        public SetOfNeccessaryTables()
        {

            List<string> columnsSqlTable = new List<string>();
            columnsSqlTable.Add("ID int IDENTITY(1,1) PRIMARY KEY");
            columnsSqlTable.Add("requestDateTime varchar(50)");
            columnsSqlTable.Add("requestId varchar(50)");


            EntityItemSqlTable = new TableWithColumns("EntityItem", columnsSqlTable.ToArray());




            columnsSqlTable.Clear();
            columnsSqlTable.Add("ID int IDENTITY(1,1) PRIMARY KEY");
            columnsSqlTable.Add("name varchar(100)");
            columnsSqlTable.Add("nip varchar(100)");
            columnsSqlTable.Add("statusVat varchar(100)");
            columnsSqlTable.Add("regon varchar(100)");
            columnsSqlTable.Add("pesel varchar(100)");
            columnsSqlTable.Add("krs varchar(100)");
            columnsSqlTable.Add("residenceAddress varchar(100)");
            columnsSqlTable.Add("workingAddress varchar(100)");
            columnsSqlTable.Add("registrationLegalDate DATETIME");
            columnsSqlTable.Add("registrationDenialDate  DATETIME");
            columnsSqlTable.Add("registrationDenialBasis varchar(100)");
            columnsSqlTable.Add("restorationDate DATETIME");
            columnsSqlTable.Add("restorationBasis varchar(100)");
            columnsSqlTable.Add("removalDate DATETIME");
            columnsSqlTable.Add("removalBasis varchar(100)");
            columnsSqlTable.Add("accountNumbers  varchar(100)");
            columnsSqlTable.Add("EntityItemID int FOREIGN KEY REFERENCES EntityItem(ID)");



            EntitySqlTable = new TableWithColumns("Entity", columnsSqlTable.ToArray());




            columnsSqlTable.Clear();
            columnsSqlTable.Add("ID int IDENTITY(1,1) PRIMARY KEY");
            columnsSqlTable.Add("companyName varchar(50)");
            columnsSqlTable.Add("firstName varchar(50)");
            columnsSqlTable.Add("lastName varchar(50)");
            columnsSqlTable.Add("pesel varchar(50)");
            columnsSqlTable.Add("nip varchar(50)");
            columnsSqlTable.Add("EntityID int FOREIGN KEY REFERENCES Entity(ID)");



            RepresentativesSqlTable = new TableWithColumns("representatives", columnsSqlTable.ToArray());
            AuthorizedClerksSqlTable = new TableWithColumns("authorizedClerks", columnsSqlTable.ToArray());
            PartnersSqlTable = new TableWithColumns("partners", columnsSqlTable.ToArray());

        }


    }






    public class TableWithColumns {
        public string TableName { get; set; }
        public List<string> ListColumnsWithType { get; set; }
        public List<string> ListColumns { get; set; }

        public TableWithColumns(string table, string[] columns)
        {
            TableName = table;
            ListColumnsWithType = new List<string>(columns);
            ListColumns = new List<string>();


            foreach (string column in columns)
            {
                ListColumns.Add(Regex.Replace(column, " .*", ""));
            }
        }

    }


    public class EntityItem
    {
        public Entity subject { get; set; }
        public string requestDateTime { get; set; }
        public string requestId { get; set; }
    }


    public class EntityPerson
    {
        public string companyName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string pesel { get; set; }
        public string nip { get; set; }
    }



    public class EntityResponse
    {
        public EntityItem result { get; set; }
    }

    public class Entity
    {
        public string name { get; set; }
        public string nip { get; set; }
        public string statusVat { get; set; }
        public string regon { get; set; }
        public string pesel { get; set; }
        public string krs { get; set; }
        public string residenceAddress { get; set; }
        public string workingAddress { get; set; }
        public List<EntityPerson> representatives { get; set; }
        public List<EntityPerson> authorizedClerks { get; set; }
        public List<EntityPerson> partners { get; set; }
      
        public Nullable<DateTime> registrationLegalDate { get; set; }
        public Nullable<DateTime> registrationDenialDate { get; set; }
        public string registrationDenialBasis { get; set; }
        public Nullable<DateTime> restorationDate { get; set; }
        public string restorationBasis { get; set; }
        public Nullable<DateTime> removalDate { get; set; }
        public string removalBasis { get; set; }
        public List<string> accountNumbers { get; set; }
        public bool hasVirtualAccounts { get; set; }


    }


}
