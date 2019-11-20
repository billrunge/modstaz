using System;
using System.Collections.Generic;
using System.Text;

namespace modstaz.Libraries
{
    public class ColumnType
    {

        public List<Type> TypesList { get; set; }

        public ColumnType()
        {
            CreateTypesList();
        }

        private void CreateTypesList()
        {
            TypesList = new List<Type>()
            {
                new Type { Id = (int)Types.YesNo, Name = "Yes/No", SqlDataType = "BIT" },
                new Type { Id = (int)Types.Integer, Name = "Integer", SqlDataType = "INT" },
                new Type { Id = (int)Types.Decimal, Name = "Decimal", SqlDataType = "FLOAT" },
                new Type { Id = (int)Types.SmallText, Name = "Small Text", SqlDataType = "NVARCHAR(255)" },
                new Type { Id = (int)Types.BigText, Name = "Big Text", SqlDataType = "NVARCHAR(MAX)" },
                new Type { Id = (int)Types.GUID, Name = "GUID", SqlDataType = "UNIQUEIDENTIFIER" },
                new Type { Id = (int)Types.XML, Name = "XML", SqlDataType = "XML" },
                new Type { Id = (int)Types.DateTime, Name = "Date/Time", SqlDataType = "DATETIME" }
            };
        }

        public enum Types
        {
            YesNo = 1,
            Integer = 2,
            Decimal = 3,
            SmallText = 4,
            BigText = 5,
            GUID = 6,
            XML = 7,
            DateTime = 8
        }

        public class Type
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SqlDataType { get; set; }
        }
    }
}
