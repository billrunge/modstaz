using System;
using System.Collections.Generic;
using System.Text;

namespace modstaz.Libraries
{
    class ColumnType
    {

        public List<Type> Types { get; set; } = new List<Type>();

        public ColumnType()
        {
            CreateTypesList();
        }

        private void CreateTypesList()
        {
            Types.Add(new Type { Id = 1, Name = "Yes/No", SqlDataType = "BIT" });
            Types.Add(new Type { Id = 2, Name = "Integer", SqlDataType = "INT" });
            Types.Add(new Type { Id = 3, Name = "Decimal", SqlDataType = "FLOAT" });
            Types.Add(new Type { Id = 4, Name = "Small Text", SqlDataType = "NVARCHAR(255)" });
            Types.Add(new Type { Id = 5, Name = "Big Text", SqlDataType = "NVARCHAR(MAX)" });
            Types.Add(new Type { Id = 6, Name = "GUID", SqlDataType = "UNIQUEIDENTIFIER" });
            Types.Add(new Type { Id = 7, Name = "XML", SqlDataType = "XML" });
            Types.Add(new Type { Id = 8, Name = "Date/Time", SqlDataType = "DATETIME" });
        }
    }

    class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SqlDataType { get; set; }
    }

}
