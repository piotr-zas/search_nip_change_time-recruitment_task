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

    public class HtmlFileWithData
    {
        public string HtmlCode { get; set; }
        public List<DataTable> DataTablesFromSql { get; set; }
        public string PathOfFile { get; set; }
        public HtmlFileWithData(string path, List<DataTable> tablesFromSql)
        {
            PathOfFile = path;
            DataTablesFromSql = tablesFromSql;
           


            StringBuilder htmlCodeSB = new StringBuilder();
            htmlCodeSB.Append("<!DOCTYPE html>\r\n");
            htmlCodeSB.Append("<html>\r\n");
            htmlCodeSB.Append("<head>\r\n");
            htmlCodeSB.Append("<style>\r\n");
            htmlCodeSB.Append("table th{position: -webkit-sticky; position: sticky;}\r\n");
            htmlCodeSB.Append("</style>\r\n");
            htmlCodeSB.Append("<title>Dane przedsiębiorców</title>\r\n");
            htmlCodeSB.Append("</head>\r\n");
            htmlCodeSB.Append("<body>\r\n");
            htmlCodeSB.Append("<center>\r\n");
            htmlCodeSB.Append("<p><h1>Dane przedsiębiorców</h1><br></p>\r\n");
            htmlCodeSB.Append(PrepareDataTableToHtmlTable(tablesFromSql) + "\r\n");
            htmlCodeSB.Append("</center>\r\n");
            htmlCodeSB.Append("</body>\r\n");
            htmlCodeSB.Append("</html>\r\n");

            HtmlCode = htmlCodeSB.ToString();

        }

        private string PrepareDataTableToHtmlTable(List<DataTable> dataTables)
        {
            StringBuilder htmlCode = new StringBuilder();

            foreach (DataTable dataTable in dataTables)
            {
                htmlCode.Append("<table style='font-size:80%' border=\"1\">\r\n");

                htmlCode.Append("\t<tr>\r\n");
                foreach (DataColumn column in dataTable.Columns)
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
            }

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






    public class TableWithColumns
    {
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
