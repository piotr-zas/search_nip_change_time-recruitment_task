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




            //server
            connectionString += $"server={sqlServerTextbox.Text};";

            //user login
            connectionString += $"user id={sqlUserLoginTextbox.Text};";

            //user pass
            connectionString += $"password={sqlUserPassTextbox.Text};";

            //database
            connectionString += $"database={sqlDatabaseTextbox.Text};";


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


            if (SendAllDataToSql(employerData, allTables)) return;

            DataTable tableFromMySql = GetDataFromSql(allTables);

            if (tableFromMySql == null) return;
            HtmlFileWithData fileHtml = new HtmlFileWithData("index.html", tableFromMySql);



            MessageBox.Show("Gotowe");
            File.WriteAllText(fileHtml.PathOfFile, fileHtml.HtmlCode);
            if (File.Exists(fileHtml.PathOfFile)) Process.Start(fileHtml.PathOfFile);
        }






        private DataTable GetDataFromSql(SetOfNeccessaryTables allTablesInfo)
        {
            try
            {
                using (MySqlConnection conncection = new MySqlConnection(GetSqlConnectionString()))
                {
                    conncection.Open();
                    string commandString = "SELECT ";

                    commandString += $"{allTablesInfo.EntityItemSqlTable.TableName}.*,";
                    commandString += $"{allTablesInfo.EntitySqlTable.TableName}.*,";
                    commandString += $"{allTablesInfo.AuthorizedClerksSqlTable.TableName}.*,";
                    commandString += $"{allTablesInfo.PartnersSqlTable.TableName}.*,";
                    commandString += $"{allTablesInfo.RepresentativesSqlTable.TableName}.*";

                    commandString += $" FROM {allTablesInfo.EntityItemSqlTable.TableName}";

                    commandString += $" LEFT JOIN {allTablesInfo.EntitySqlTable.TableName}";
                    commandString += $" ON {allTablesInfo.EntityItemSqlTable.TableName}.ID={allTablesInfo.EntitySqlTable.TableName}.EntityItemID";

                    commandString += $" LEFT JOIN {allTablesInfo.AuthorizedClerksSqlTable.TableName}";
                    commandString += $" ON {allTablesInfo.EntitySqlTable.TableName}.ID={allTablesInfo.AuthorizedClerksSqlTable.TableName}.EntityID";

                    commandString += $" LEFT JOIN {allTablesInfo.PartnersSqlTable.TableName}";
                    commandString += $" ON {allTablesInfo.EntitySqlTable.TableName}.ID={allTablesInfo.PartnersSqlTable.TableName}.EntityID";

                    commandString += $" LEFT JOIN {allTablesInfo.RepresentativesSqlTable.TableName}";
                    commandString += $" ON {allTablesInfo.EntitySqlTable.TableName}.ID={allTablesInfo.RepresentativesSqlTable.TableName}.EntityID";


                    commandString += ";";

                    using (MySqlCommand command = new MySqlCommand(commandString, conncection))
                    {
                        using (MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter(command))
                        {
                            DataTable readedData=new DataTable();

                            mySqlAdapter.Fill(readedData);
                            return readedData;
                        }
                    }


                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }



        }












        /// <summary>
        /// Send specific table to SQL
        /// </summary>
        /// <param name="tableSqlColumns">Object with table name, columns and types</param>
        /// <param name="valuesToSql"> List of values to insert</param>
        /// <returns>0 if error else last inserted id</returns>
        private long SendOneTableToSql(TableWithColumns tableSqlColumns, string[] valuesToSql)
        {
            try
            {
                using (MySqlConnection conncection = new MySqlConnection(GetSqlConnectionString()))
                {
                    conncection.Open();
                    string commandString = $"INSERT INTO {tableSqlColumns.TableName} " +
                        $"({String.Join(", ", tableSqlColumns.ListColumns)}) " +
                        $"VALUES ('{String.Join("', '", valuesToSql)}');".Replace("'null'", "null");

                    using (MySqlCommand command = new MySqlCommand(commandString, conncection))
                    {
                        command.ExecuteNonQuery();

                        return command.LastInsertedId;
                    }


                }
            } catch (Exception error)
            {
                MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

        }




        /// <summary>
        /// Send Table Entity Person to SQL
        /// </summary>
        /// <param name="entityPersonList"></param>
        /// <param name="tableColumns">Table name, columns with types</param>
        /// <param name="idParent"></param>
        /// <returns>True if error</returns>
        private bool SendTableEntityPerson(List<EntityPerson> entityPersonList, TableWithColumns tableColumns, long idParent)
        {
            foreach (var entityPerson in entityPersonList)
            {

                List<string> valuesEntityPerson = new List<string>();

                valuesEntityPerson.Add("null");//id column
                valuesEntityPerson.Add(entityPerson.companyName);
                valuesEntityPerson.Add(entityPerson.firstName);
                valuesEntityPerson.Add(entityPerson.lastName);
                valuesEntityPerson.Add(entityPerson.pesel);
                valuesEntityPerson.Add(entityPerson.nip);
                valuesEntityPerson.Add(idParent.ToString());

                ChangeQuoteInValuesToSql(valuesEntityPerson);

                long idParentTemp = SendOneTableToSql(tableColumns, valuesEntityPerson.ToArray());
                if (idParentTemp == 0) return true;//error

            }
            return false;


        }



        private void ChangeQuoteInValuesToSql(List<string> listValues)
        {
            for (int i = 0; i < listValues.Count; i++)
            {
                if (listValues[i] != null)
                    listValues[i] = listValues[i].Replace("'", "''");
            }

        }





        /// <summary>
        /// Get string "null" if DateTime is default
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>DateTime to string or null if DateTime is default</returns>
        private string GetNullIfDateTimeDefault(Nullable<DateTime> dateTime)
        {
            if (dateTime.HasValue) return dateTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
            else return "null";
        }




        /// <summary>
        /// Send all data to SQL
        /// </summary>
        /// <param name="employerData">Response from gov</param>
        /// <param name="allTablesInfo">Info about columns and table names</param>
        /// <returns>True if error</returns>
        private bool SendAllDataToSql(EntityResponse employerData, SetOfNeccessaryTables allTablesInfo)
        {
            List<string> valuesEntityItemSql = new List<string>();

            valuesEntityItemSql.Add("null");//id column
            valuesEntityItemSql.Add(employerData.result.requestDateTime);
            valuesEntityItemSql.Add(employerData.result.requestId);

            ChangeQuoteInValuesToSql(valuesEntityItemSql);

            long idParent = SendOneTableToSql(allTablesInfo.EntityItemSqlTable, valuesEntityItemSql.ToArray());
            if (idParent == 0) return true;//error




            List<string> valuesEntitySql = new List<string>();

            valuesEntitySql.Add("null");//id column
            valuesEntitySql.Add(employerData.result.subject.name);
            valuesEntitySql.Add(employerData.result.subject.nip);
            valuesEntitySql.Add(employerData.result.subject.statusVat);
            valuesEntitySql.Add(employerData.result.subject.regon);
            valuesEntitySql.Add(employerData.result.subject.pesel);
            valuesEntitySql.Add(employerData.result.subject.krs);
            valuesEntitySql.Add(employerData.result.subject.residenceAddress);
            valuesEntitySql.Add(employerData.result.subject.workingAddress);
            valuesEntitySql.Add(GetNullIfDateTimeDefault(employerData.result.subject.registrationLegalDate));
            valuesEntitySql.Add(GetNullIfDateTimeDefault(employerData.result.subject.registrationDenialDate));
            valuesEntitySql.Add(employerData.result.subject.registrationDenialBasis);
            valuesEntitySql.Add(GetNullIfDateTimeDefault(employerData.result.subject.restorationDate));
            valuesEntitySql.Add(employerData.result.subject.restorationBasis);
            valuesEntitySql.Add(GetNullIfDateTimeDefault(employerData.result.subject.removalDate));
            valuesEntitySql.Add(employerData.result.subject.removalBasis);
            valuesEntitySql.Add(string.Join("\r\n", employerData.result.subject.accountNumbers));
            valuesEntitySql.Add(idParent.ToString());

            ChangeQuoteInValuesToSql(valuesEntitySql);



            idParent = SendOneTableToSql(allTablesInfo.EntitySqlTable, valuesEntitySql.ToArray());
            if (idParent == 0) return true;//error




            if (SendTableEntityPerson
                (employerData.result.subject.authorizedClerks,
                allTablesInfo.AuthorizedClerksSqlTable,
                idParent)) return true;

            if (SendTableEntityPerson
               (employerData.result.subject.partners,
               allTablesInfo.PartnersSqlTable,
               idParent)) return true;

            if (SendTableEntityPerson
              (employerData.result.subject.representatives,
              allTablesInfo.RepresentativesSqlTable,
              idParent)) return true;


            return false;
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

            using (MySqlConnection connection = new MySqlConnection(GetSqlConnectionString()))
            {
                try
                {
                    connection.Open();

                    string commandString = $"CREATE TABLE IF NOT EXISTS {tableInfo.TableName} ({String.Join(", ", tableInfo.ListColumnsWithType)});";

                    using (MySqlCommand command = new MySqlCommand(commandString, connection))
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




    public class HtmlFileWithData
    {
        public string HtmlCode { get; set; }
        public DataTable DataTableFromSql { get; set; }
        public string PathOfFile { get; set; }
        public HtmlFileWithData(string path, DataTable tableFromSql)
        {
            PathOfFile = path;
            DataTableFromSql = tableFromSql;



            StringBuilder htmlCodeSB = new StringBuilder();
            htmlCodeSB.Append("<!DOCTYPE html>\r\n");
            htmlCodeSB.Append("<html>\r\n");
            htmlCodeSB.Append("<head>\r\n");
            htmlCodeSB.Append("<title>Dane przedsiębiorców</title>\r\n");
            htmlCodeSB.Append("</head>\r\n");
            htmlCodeSB.Append("<body>\r\n");
            htmlCodeSB.Append("<center>\r\n");
            htmlCodeSB.Append("<p>Dane przedsiębiorców<br></p>\r\n");
            htmlCodeSB.Append(PrepareDataTableToHtmlTable(tableFromSql)+"\r\n");
            htmlCodeSB.Append("</center>\r\n");
            htmlCodeSB.Append("</body>\r\n");
            htmlCodeSB.Append("</html>\r\n");

            HtmlCode = htmlCodeSB.ToString();

        }

        private string PrepareDataTableToHtmlTable(DataTable dataTable)
        {
            StringBuilder htmlCode = new StringBuilder();
            htmlCode.Append("<table>\r\n");

            htmlCode.Append("\t<tr>\r\n");
            foreach(DataColumn column in dataTable.Columns)
            {
                htmlCode.Append($"\t\t<th>{column.ColumnName}</th>\r\n");
            }
            htmlCode.Append("\t</tr>\r\n");


            foreach (DataRow row in dataTable.Rows)
            {
                htmlCode.Append("\t<tr>\r\n");
                foreach (DataColumn column in dataTable.Columns)
                {

                    htmlCode.Append($"\t\t<td>{row[column]}</td>\r\n");
                }
                htmlCode.Append("\t</tr>\r\n");
            }

            htmlCode.Append("</table>\r\n");


            return htmlCode.ToString();
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
            columnsSqlTable.Add("ID int NOT NULL AUTO_INCREMENT PRIMARY KEY");
            columnsSqlTable.Add("requestDateTime varchar(50)");
            columnsSqlTable.Add("requestId varchar(50)");


            EntityItemSqlTable = new TableWithColumns("EntityItem", columnsSqlTable.ToArray());




            columnsSqlTable.Clear();
            columnsSqlTable.Add("ID int NOT NULL AUTO_INCREMENT PRIMARY KEY");
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
            columnsSqlTable.Add("accountNumbers  varchar(5000)");
            columnsSqlTable.Add("EntityItemID int");
            columnsSqlTable.Add("FOREIGN KEY(EntityItemID) REFERENCES EntityItem(ID)");


            EntitySqlTable = new TableWithColumns("Entity", columnsSqlTable.ToArray());




            columnsSqlTable.Clear();
            columnsSqlTable.Add("ID int NOT NULL AUTO_INCREMENT PRIMARY KEY");
            columnsSqlTable.Add("companyName varchar(50)");
            columnsSqlTable.Add("firstName varchar(50)");
            columnsSqlTable.Add("lastName varchar(50)");
            columnsSqlTable.Add("pesel varchar(50)");
            columnsSqlTable.Add("nip varchar(50)");
            columnsSqlTable.Add("EntityID int ");
            columnsSqlTable.Add("FOREIGN KEY(EntityID) REFERENCES Entity(ID)");




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
                if (!column.Contains("FOREIGN"))
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
