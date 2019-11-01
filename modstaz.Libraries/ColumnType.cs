using System;
using System.Collections.Generic;
using System.Text;

namespace modstaz.Libraries
{
    class ColumnType
    {

        enum ColumnTypes
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
    }
}
