using Npgsql;

namespace DBApplicatiom
{
    public class SELECTmethods
    {
        public static List<string> GetTables(string connectionString)
        {
            var tables = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";

                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            return tables;
        }

        public static List<string> GetColumns(string connectionString, string tableName)
        {
            var columns = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tableName}'";

                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(reader.GetString(0));
                }
            }

            return columns;
        }

        public static Dictionary<string, string> GetColumnsWithTypes(string connectionString, string tableName)
        {
            var columnsWithTypes = new Dictionary<string, string>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '{tableName}'";

                using var command = new NpgsqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnsWithTypes.Add(reader.GetString(0), reader.GetString(1));
                    }
                }
            }

            return columnsWithTypes;
        }

        public static bool ColumnExists(string connectionString, string tableName, string columnName)
        {
            string query = $"SELECT COUNT(*) FROM information_schema.columns WHERE table_name = @tableName AND column_name = @columnName;";

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@tableName", tableName);
            command.Parameters.AddWithValue("@columnName", columnName);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }
}
