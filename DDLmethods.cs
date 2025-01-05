namespace DBApplicatiom
{
    public class DDLmethods
    {
        public static void CreateTable(string connectionString)
        {
            Console.Clear();
            Console.Write("Enter the new table name: ");
            Console.WriteLine("\nPress 'Esc' at any time to cancel.");

            string tableName = ReadInputOrCancel();
            if (tableName == null)
            {
                Console.WriteLine("Table creation was canceled.");
                return;
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                Console.WriteLine("Table name cannot be empty. Please try again.");
                return;
            }

            List<string> columns = new List<string>();
            while (true)
            {
                Console.Write("Enter column name (or type 'done' to finish): ");
                string columnName = ReadInputOrCancel();
                if (columnName == null)
                {
                    Console.WriteLine("Table creation was canceled.");
                    return;
                }

                if (columnName.ToLower() == "done") break;

                if (string.IsNullOrWhiteSpace(columnName))
                {
                    Console.WriteLine("Column name cannot be empty. Please try again.");
                    continue;
                }

                if (columnName.ToLower() == "id")
                {
                    Console.WriteLine("Detected column 'Id', setting type to 'serial'.");
                    columns.Add($"{columnName} serial");
                    continue;
                }

                Console.Write("Enter column type (e.g., INT, VARCHAR): ");
                string columnType = ReadInputOrCancel();
                if (columnType == null)
                {
                    Console.WriteLine("Table creation was canceled.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(columnType))
                {
                    Console.WriteLine("Column type cannot be empty. Please try again.");
                    continue;
                }

                columns.Add($"{columnName} {columnType}");
            }

            if (columns.Count == 0)
            {
                Console.WriteLine("At least one column must be defined.");
                return;
            }

            string columnsDefinition = string.Join(", ", columns);
            string query = $"CREATE TABLE \"{tableName}\" ({columnsDefinition});";

            ExecuteNonQuery(connectionString, query);
            Console.WriteLine($"Table '{tableName}' created successfully.");
        }

        public static string ReadInputOrCancel()
        {
            string input = "";
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nOperation canceled.");
                    return null;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.Trim();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    Console.Write(key.KeyChar);
                    input += key.KeyChar;
                }
            }
        }

        public static void DeleteTable(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("Available tables: ");
            var tables = SELECTmethods.GetTables(connectionString);

            for (int i = 0; i < tables.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tables[i]}");
            }

            Console.Write("Enter the table number to delete: ");
            if (int.TryParse(Console.ReadLine(), out int tableChoice) && tableChoice > 0 && tableChoice <= tables.Count)
            {
                string tableName = tables[tableChoice - 1];

                Console.WriteLine($"Are you sure you want to delete the table '{tableName}'? This action cannot be undone. (yes/no)");
                string confirmation = Console.ReadLine()?.Trim().ToLower();

                if (confirmation == "yes")
                {
                    string query = $"drop table \"{tableName}\";";
                    ExecuteNonQuery(connectionString, query);
                    Console.WriteLine($"Table '{tableName}' has been deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Table deletion canceled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid table selection.");
            }

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }

        public static void TruncateTable(string connectionString, string tableName)
        {
            Console.WriteLine($"Are you sure you want to delete all values from the table '{tableName}'? This action cannot be undone. (yes/no)");
            string confirmation = Console.ReadLine()?.Trim().ToLower();
            if (confirmation == "yes")
            {
                string query = $"TRUNCATE table \"{tableName}\";";
                ExecuteNonQuery(connectionString, query);
                Console.WriteLine($"All values from {tableName} deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deleting values cancelled");
            }
        }

        public static void RenameTable(string connectionString, string selectedTable)
        {
            Console.Clear();
            Console.WriteLine("Enter the new table name:");
            string newTableName = Console.ReadLine();
            string queryRenameTable = $"ALTER TABLE {selectedTable} RENAME TO {newTableName}";
            ExecuteNonQuery(connectionString, queryRenameTable);
            Console.WriteLine($"{selectedTable} table has been renamed to {newTableName} successfully");
        }

        public static void RenameColumn(string connectionString, string selectedTable)
        {
            Console.Clear();
            var allColumns = SELECTmethods.GetColumns(connectionString, selectedTable);
            Console.WriteLine("Columns in the table:");
            if (allColumns.Count > 0)
            {
                Console.WriteLine(string.Join(" \n", allColumns));
            }
            else
            {
                Console.WriteLine("No columns found");
            }
            Console.WriteLine("Enter the column to rename:");
            string oldColumnName = Console.ReadLine();
            Console.WriteLine("\nEnter the new column name:");
            string newColumnName = Console.ReadLine();
            string queryChangeColName = $"ALTER TABLE {selectedTable} RENAME COLUMN {oldColumnName} TO {newColumnName};";
            ExecuteNonQuery(connectionString, queryChangeColName);
            Console.WriteLine($"{oldColumnName} column has been renamed to {newColumnName} successfully");
        }

        public static void AddColumn(string connectionString, string selectedTable)
        {
            Console.Clear();
            Console.WriteLine($"Adding columns to table: {selectedTable}");
            Console.WriteLine("Existing columns:");
            List<string> existingColumns = SELECTmethods.GetColumns(connectionString, selectedTable);
            existingColumns.ForEach(col => Console.WriteLine($"- {col}"));

            while (true)
            {
                Console.WriteLine("Enter the name of the new column (or type 'done' to finish):");
                string newColumn = ReadInputOrCancel();
                if (newColumn == null)
                {
                    Console.WriteLine("Column addition process canceled.");
                    break;
                }

                if (newColumn.ToLower() == "done")
                {
                    Console.WriteLine("Column addition process finished.");
                    break;
                }

                if (string.IsNullOrWhiteSpace(newColumn))
                {
                    Console.WriteLine("Column name cannot be empty. Please try again.");
                    continue;
                }

                Console.WriteLine("Enter the type of the new column (e.g., INT, VARCHAR(255)):");
                string dataType = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(dataType))
                {
                    Console.WriteLine("Data type cannot be empty. Please try again.");
                    continue;
                }

                try
                {
                    string addNewColumn = $"ALTER TABLE {selectedTable} ADD COLUMN {newColumn} {dataType};";
                    ExecuteNonQuery(connectionString, addNewColumn);
                    Console.WriteLine($"The column '{newColumn}' of type '{dataType}' was added successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while adding the column: {ex.Message}");
                }
            }
        }

        public static void DropColumn(string connectionString, string selectedTable)
        {
            Console.Clear();
            var columnsForDrop = SELECTmethods.GetColumns(connectionString, selectedTable);
            Console.WriteLine("Columns in the table:");
            if (columnsForDrop.Count > 0)
            {
                Console.WriteLine(string.Join(" \n", columnsForDrop));
            }
            else
            {
                Console.WriteLine("No columns found");
            }

            string dropColumn;
            do
            {
                Console.WriteLine("Enter the name of column to drop:");
                dropColumn = Console.ReadLine();

                if (SELECTmethods.ColumnExists(connectionString, selectedTable, dropColumn))
                {
                    Console.WriteLine($"Are you sure you want to drop the column {dropColumn} from the table '{selectedTable}'? This action cannot be undone. (yes/no)");
                    string confirmation = Console.ReadLine()?.Trim().ToLower();
                    if (confirmation == "yes")
                    {
                        string queryDropColumn = $"ALTER TABLE {selectedTable} DROP COLUMN {dropColumn};";
                        ExecuteNonQuery(connectionString, queryDropColumn);
                        Console.Clear();
                        Console.WriteLine($"{dropColumn} column has been dropped successfully.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Dropping column cancelled");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Column {dropColumn} does not exist in the table {selectedTable}. Please enter a valid column name.");
                }
            } while (true);
        }

        static void ExecuteNonQuery(string connectionString, string query)
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }
}

