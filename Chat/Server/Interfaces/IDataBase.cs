using System.Data;

namespace Server.Interfaces
{
    public interface IDataBase
    {
        void bind(string field, string value);
        void bind(string[] fields);
        int executeNonQuery(string query);
        string singleSelect(string query);
        string[] tableToRow();
        DataTable manySelect(string query);
    }
}
