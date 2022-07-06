using System;
using System.Collections.Generic;
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
using System.Data;



namespace search_nip_change_time_recruitment_task
{
   public partial class Form1
    {


        private void GetTableSqlByCommand(string commandString, MySqlConnection conncection, List<DataTable> tableFromSql)
        {
            using (MySqlCommand command = new MySqlCommand(commandString, conncection))
            {
                using (MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter(command))
                {
                    DataTable readedData = new DataTable();

                    mySqlAdapter.Fill(readedData);
                    tableFromSql.Add(readedData);

                }
            }

        }



        private List<DataTable> GetDataFromSql(SetOfNeccessaryTables allTablesInfo)
        {
            List<DataTable> tableFromSql = new List<DataTable>();

            try
            {
                using (MySqlConnection conncection = new MySqlConnection(GetSqlConnectionString()))
                {
                    conncection.Open();
                    string commandString = $"SELECT * FROM {allTablesInfo.EntityItemSqlTable.TableName};";
                    GetTableSqlByCommand(commandString, conncection, tableFromSql);

                    commandString = $"SELECT * FROM {allTablesInfo.EntitySqlTable.TableName};";
                    GetTableSqlByCommand(commandString, conncection, tableFromSql);

                    commandString = $"SELECT * FROM {allTablesInfo.AuthorizedClerksSqlTable.TableName};";
                    GetTableSqlByCommand(commandString, conncection, tableFromSql);

                    commandString = $"SELECT * FROM {allTablesInfo.PartnersSqlTable.TableName};";
                    GetTableSqlByCommand(commandString, conncection, tableFromSql);

                    commandString = $"SELECT * FROM {allTablesInfo.RepresentativesSqlTable.TableName};";
                    GetTableSqlByCommand(commandString, conncection, tableFromSql);




                    return tableFromSql;
                }
            }
            catch (Exception error)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
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
            }
            catch (Exception error)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
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


        /// <summary>
        /// Replace ' to ''
        /// </summary>
        /// <param name="listValues"></param>
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
            if (dateTime.HasValue) return dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
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
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(response.StatusCode.ToString(), "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
                return true;
            }

            if (employerData.result.subject == null)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Nie znaleziono takiego numeru NIP w bazie", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
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
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                    return true;
                }

            }
        }


    }
}
