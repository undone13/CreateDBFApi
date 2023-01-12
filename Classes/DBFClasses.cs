using System.Data.OleDb;

namespace CreateDBFApi.Classes
{
    public class CreateDBFViewModel
    {
        public string DataBaseName { get; set; }

        public CreateDBFViewModel()
        {
            Headers = new List<Header>();
            Values = new List<RowValues>();
        }

        public List<Header> Headers { get; set; }

        public List<RowValues> Values { get; set; }
    }

    public class RowValues
    {
        public Object Fc { get; set; }
        public Object Ser_fac { get; set; }
        public Object Nr_fac { get; set; }
    }

    public class Header {
        public string ColumnName { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
    }

}
