using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace FO.CLS.UTIL
{
    public class SQLITEINI
    {
        string sqlitePath;
        SQLiteConnection connection;

        string prefix = "";

        public SQLITEINI(string prefix = "")
        {
            sqlitePath = Application.StartupPath + "\\ini.sqlite";

            connection = new SQLiteConnection("Data Source=" + sqlitePath + ";Version=3;");

            if(!File.Exists(sqlitePath))
            {
                SQLiteConnection.CreateFile(sqlitePath);

                connection.Open();

                string sql = "create table ini (key varchar(200), value varchar(200))";
                SQLiteCommand command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();

                connection.Close();
            }

            if (prefix != string.Empty)
                prefix = prefix + "_";

            this.prefix = prefix;
        }

        //~SQLITEINI()
        //{
        //    try
        //    {
        //        connection?.Close();
        //        GC.Collect();
        //    }
        //    catch
        //    {

        //    }
        //}

        public string ReadValue(string key, string defaultValue = "")
        {
            key = this.prefix + key;

            connection.Open();

            string sql = @"
                            SELECT value
                              FROM ini
                             WHERE key = ':key'
                            ";

            sql = sql.Replace(":key", key);

            SQLiteDataAdapter command = new SQLiteDataAdapter(sql, connection);

            DataSet dataSet = new DataSet();

            command.Fill(dataSet);
            DataTable dataTable = dataSet.Tables[0];

            connection.Close();

            string r = defaultValue;

            if(dataTable.Rows.Count > 0)
                r = dataTable.Rows[0]["value"].ToString();

            return r;
        }

        public void WriteValue(string key, string value)
        {
            key = this.prefix + key;

            connection.Open();

            string sql = @"
                            UPDATE ini
                               SET value = ':value'
                             WHERE key = ':key'
                            ";

            sql = sql.Replace(":value", value);
            sql = sql.Replace(":key", key);

            SQLiteCommand sqliteCommand = new SQLiteCommand(sql, connection);
            int affectedRow = sqliteCommand.ExecuteNonQuery();

            if(affectedRow == 0)
            {
                sql = @"
                        INSERT
                          INTO ini
                        VALUES (':key', ':value')
                        ";

                sql = sql.Replace(":value", value);
                sql = sql.Replace(":key", key);

                sqliteCommand = new SQLiteCommand(sql, connection);
                sqliteCommand.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
