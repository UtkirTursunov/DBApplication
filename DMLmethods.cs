using Npgsql;

namespace DBApplicatiom
{
    public class DMLmethods
    {
        static void DisplayTable(string connectionString, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Displaying data from table '{tableName}'");

            var columns = SELECTmethods.GetColumnsWithTypes(connectionString, tableName);
            SELECTmethods.PrintFormattedColumns(columns);
            var query = $"SELECT * from \"{tableName}\";";

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            int firstColumnWidth = 4; 
            int otherColumnWidth = 15; 

            foreach (var column in columns.Keys)
            {
                if (column == "id") 
                {
                    Console.Write(column.PadRight(firstColumnWidth));
                }
                else
                {
                    Console.Write(column.PadRight(otherColumnWidth));
                }
            }
            Console.WriteLine();

            while (reader.Read())
            {
                foreach (var column in columns.Keys)
                {
                    if (column == "id") 
                    {
                        Console.Write(reader[column].ToString().PadRight(firstColumnWidth));
                    }
                    else
                    {
                        Console.Write(reader[column].ToString().PadRight(otherColumnWidth));
                    }
                }
                Console.WriteLine();
            }
        }

        public static void DisplayTableMenu(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("Please choose a table to display: ");
            var tables = SELECTmethods.GetTables(connectionString);
            for (int i = 0; i < tables.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tables[i]}");
            }

            Console.Write("Enter the table number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= tables.Count)
            {
                string selectedTable = tables[choice - 1];
                DisplayTable(connectionString, selectedTable);
            }
            else
            {
                Console.WriteLine("Invalid table selection.");
            }
        }

        public static void InsertData(string connectionString, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Inserting data into table '{tableName}'.");
            Console.WriteLine("Press 'Esc' at any time to cancel.");

            var columnsWithTypes = SELECTmethods.GetColumnsWithTypes(connectionString, tableName);

            columnsWithTypes.Remove("id");

            var commandText = $"INSERT INTO \"{tableName}\" ({string.Join(", ", columnsWithTypes.Keys)}) VALUES ({string.Join(", ", columnsWithTypes.Keys.Select(c => "@" + c))});";

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(commandText, connection);

            foreach (var column in columnsWithTypes)
            {
                Console.Write($"Enter value for column '{column.Key}' ({column.Value}): ");
                string value = "";

                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);

                        if (key.Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("\nInsert operation cancelled.");
                            return; 
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            Console.WriteLine();
                            break;
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (value.Length > 0)
                            {
                                value = value.Remove(value.Length - 1);
                                Console.Write("\b \b");
                            }
                        }
                        else
                        {
                            Console.Write(key.KeyChar);
                            value += key.KeyChar;
                        }
                    }
                }

                if (string.IsNullOrEmpty(value))
                {
                    command.Parameters.AddWithValue("@" + column.Key, DBNull.Value);
                }
                else
                {
                    if (column.Value == "integer")
                    {
                        command.Parameters.AddWithValue("@" + column.Key, int.Parse(value));
                    }
                    else if (column.Value == "varchar" || column.Value == "text")
                    {
                        command.Parameters.AddWithValue("@" + column.Key, value);
                    }
                    else if (column.Value == "boolean")
                    {
                        command.Parameters.AddWithValue("@" + column.Key, bool.Parse(value));
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@" + column.Key, value);
                    }
                }

            }

            command.ExecuteNonQuery();
            Console.WriteLine("Data inserted successfully.");
        }
       
        public static void UpdateData(string connectionString, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Updating data in table '{tableName}'.");

            Console.Write("Enter the ID of the record to update: ");
            string id = Console.ReadLine();

            var columns = SELECTmethods.GetColumns(connectionString, tableName);
            List<string> setClauses = new List<string>();

            foreach (var column in columns)
            {
                Console.Write($"Enter new value for column '{column}' (leave empty to skip): ");
                string newValue = Console.ReadLine();
                if (!string.IsNullOrEmpty(newValue))
                {
                    setClauses.Add($"\"{column}\" = '{newValue}'");
                }
            }

            string setClause = string.Join(", ", setClauses);
            string query = $"UPDATE \"{tableName}\" SET {setClause} WHERE \"id\" = {id};";
            ExecuteNonQuery(connectionString, query);
            Console.WriteLine($"Data with ID {id} updated successfully.");
        }

        public static void DeleteData(string connectionString, string tableName)
        {
            Console.Clear();
            Console.WriteLine($"Deleting data from table '{tableName}'.");

            Console.Write("Enter the ID of the record to delete: ");
            string id = Console.ReadLine();

            string query = $"DELETE FROM \"{tableName}\" WHERE \"id\" = {id};";
            ExecuteNonQuery(connectionString, query);
            Console.WriteLine($"Data with ID {id} deleted successfully.");
        }

        private static void ExecuteNonQuery(string connectionString, string query)
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }
}
