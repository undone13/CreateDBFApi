using CreateDBFApi.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CreateDBFApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateDBFController : ControllerBase
    {
        private readonly ILogger<CreateDBFController> _logger;

        private readonly IWebHostEnvironment _env;

        public CreateDBFController(IWebHostEnvironment env, ILogger<CreateDBFController> logger)
        {
            _env = env;
            _logger = logger;
        }

        protected OleDbType TypeReturn(string type)
        {
            if (type.ToLower() == "char")
                return OleDbType.Char;
            else if (type.ToLower() == "numeric")
                return OleDbType.Numeric;
            else if (type.ToLower() == "date")
                return OleDbType.Date;
            else if (type.ToLower() == "boolean")
                return OleDbType.Boolean;
            else if (type.ToLower() == "double")
                return OleDbType.Double;


            return OleDbType.Empty;
        }

        [HttpPost(Name = "CreateDBF")]
        public string CreateDBF([FromBody] CreateDBFViewModel model)
        {
            string errorMessage = "DataBaseName can't be empty or null";
            if (String.IsNullOrEmpty(model.DataBaseName))
            {
                return errorMessage;
            }

            List<string> commandsList = new List<string>();

            string commandTable = @"CREATE TABLE " + model.DataBaseName + @" (";
            string commandInsert = @"INSERT INTO " + model.DataBaseName + @" (";
            string valuesString = " VALUES ()";
            List<string> headersType = new List<string>();

            foreach (var header in model.Headers)
            {
                commandTable += header.ColumnName + " " + header.Type + "(" + header.Size.ToString() + "),";
                commandInsert += header.ColumnName + ", ";
                valuesString = valuesString.Insert(9, "?, ");

                headersType.Add(header.Type);
            }
            commandTable = commandTable.Remove(commandTable.Length - 1, 1);
            commandTable += ")";

            commandInsert = commandInsert.Remove(commandInsert.Length - 2, 2);
            commandInsert += ")";

            valuesString = valuesString.Remove(valuesString.Length - 3, 3);
            valuesString += ")";

            commandInsert = String.Concat(commandInsert, valuesString);

            ////////////////////////////////
            

            string dir = Path.Combine(_env.WebRootPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            string filepath = dir;


            ///////////////////////////////


            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder()
            {
                DataSource = filepath,
                Provider = "VFPOLEDB.1"
            };

            Builder.Add("Extended Properties", "dBase III");

            using (OleDbConnection cn = new OleDbConnection(Builder.ConnectionString))
            {
                cn.Open();
                new OleDbCommand("set null off", cn).ExecuteNonQuery();

                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.CommandText = commandTable;
                    cmd.Connection = cn;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return "ERROR: " + ex.Message;
                    }
                }

                foreach (var row in model.Values)
                {
                    string commandString = commandInsert;

                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        cmd.CommandText = commandString;
                        cmd.Connection = cn;

                        int index = 0;
                        foreach (var item in row)
                        {
                            Debug.WriteLine("comanda: " + TypeReturn(headersType[index]).ToString());
                            cmd.Parameters.Add(item.ColumnName, TypeReturn(headersType[index])).Value = item.Value.ToString();
                            index++;
                        }
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            return "ERROR: " + ex.Message;
                        }
                    }
                }


                cn.Close();
            }


            return commandTable + "\n" + commandInsert;
        }
    }
}