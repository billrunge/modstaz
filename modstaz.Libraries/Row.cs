using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class Row
    {
       public int StorageAreaId { get; set; }
        public async Task<string> InsertRowAsync()
        {
            //string columns = "";
            //string values = "";
            Column column = new Column() { StorageAreaId = StorageAreaId };

            return  await column.GetStorageAreaColumnsAsync();



            //string sql = $@"
            //    INSERT INTO [{ StorageAreaId }Rows] 
            //                ({ columns }) 
            //    VALUES      ({ values })";



        }
    }
}
