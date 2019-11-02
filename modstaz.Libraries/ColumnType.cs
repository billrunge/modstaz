using System;
using System.Collections.Generic;
using System.Text;

namespace modstaz.Libraries
{
    class ColumnType
    {

        public List<Type> Types { get; set; }

        public ColumnType()
        {
            CreateTypesList();
        }

        private void CreateTypesList()
        {
            Types = new List<Type>()
            {
                new Type { Id = 1, Name = "Yes/No", SqlDataType = "BIT" },
                new Type { Id = 2, Name = "Integer", SqlDataType = "INT" },
                new Type { Id = 3, Name = "Decimal", SqlDataType = "FLOAT" },
                new Type { Id = 4, Name = "Small Text", SqlDataType = "NVARCHAR(255)" },
                new Type { Id = 5, Name = "Big Text", SqlDataType = "NVARCHAR(MAX)" },
                new Type { Id = 6, Name = "GUID", SqlDataType = "UNIQUEIDENTIFIER" },
                new Type { Id = 7, Name = "XML", SqlDataType = "XML" },
                new Type { Id = 8, Name = "Date/Time", SqlDataType = "DATETIME" }
            };
        }
    }

    class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SqlDataType { get; set; }
    }

}
