using CreateDBFApi.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.OleDb;
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


            return OleDbType.Empty;
        }

        [HttpPost(Name = "CreateDBF")]
        public string CreateDBF([FromBody] CreateDBFViewModel model)
        {
            string errorMessage = "DataBaseName can't be empty or null";
            if(String.IsNullOrEmpty(model.DataBaseName))
            {
                return errorMessage;
            }

            List<string> commandsList = new List<string>();
            string commands = "";
            foreach(var row in model.Values)
            {
                string command = @"INSERT INTO " + model.DataBaseName + @" (";

                foreach (var header in model.Headers)
                {
                    command += (header + ", ");
                }
                command = command.Remove(command.Length - 2, 2);
                command += ") VALUES(";

                //foreach(var item in row)
                //{
                //    command += (item.Value.ToString() + ", ");
                //}
                //command = command.Remove(command.Length - 2, 2);
                //command += ")";

                commandsList.Add(command);
                commands += command + "\n";
            }
            string dir = Path.Combine(_env.WebRootPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            string filepath = dir + "/" + model.DataBaseName;

            //OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder()
            //{
            //    DataSource = "C:\\Ceva",
            //    Provider = "VFPOLEDB.1"
            //};

            //Builder.Add("Extended Properties", "dBase III");

            //foreach (var com in commandsList)
            //{
            //    using (OleDbConnection cn = new OleDbConnection(Builder.ConnectionString))
            //    {
            //        cn.Open();

            //        using (OleDbCommand cmd = new OleDbCommand())
            //        {
            //            cmd.CommandText = @"create table facemi (FC , SER_FAC)";
            //            cmd.Connection = cn;

            //            try
            //            {
            //                cmd.ExecuteNonQuery();
            //            }
            //            catch (Exception ex)
            //            {
            //                return "ERROR: " + ex.Message;
            //            }
            //        }

            //        using (OleDbCommand cmd = new OleDbCommand())
            //        {
            //            cmd.CommandText = com;
            //            cmd.Connection = cn;

            //            try
            //            {
            //                cmd.ExecuteNonQuery();
            //            }
            //            catch (Exception ex)
            //            {
            //                return "ERROR: " + ex.Message;
            //            }
            //        }
            //        cn.Close();
            //    }
            //}

            //foreach (var row in model.Values)
            //{
            //    string command = @"create table " + model.DataBaseName + @" (";
            //    foreach (var header in model.Headers)
            //    {
            //        command += (header + ", ");
            //    }
            //}



            return commands;
        }
    }
}