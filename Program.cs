using Npgsql;
using Spectre.Console;


namespace DBApplicatiom
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=2233;Database=QuizApplication";
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("You are connected to database 'QuizApplication'");
                Console.WriteLine("Please choose an action: ");
                Console.WriteLine("1. Create table");
                Console.WriteLine("2. Manage table");
                Console.WriteLine("3. Display table");
                Console.WriteLine("4. Delete table");
                Console.WriteLine("5. Exit");

                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            DDLmethods.CreateTable(connectionString);
                            break;
                        case 2:
                            ManageTable(connectionString);
                            break;
                        case 3:
                            DMLmethods.DisplayTableMenu(connectionString);
                            break;
                        case 4:
                            DDLmethods.DeleteTable(connectionString);
                            break;
                        case 5:
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ManageTable(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("Please choose a table to manage: ");
            var tables = SELECTmethods.GetTables(connectionString);
            for (int i = 0; i < tables.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tables[i]}");
            }

            Console.Write("Enter the table number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= tables.Count)
            {
                string selectedTable = tables[choice - 1];
                Console.Clear();
                Console.WriteLine($"You selected table: {selectedTable}");

                Console.WriteLine("1. Insert values");
                Console.WriteLine("2. Update values");
                Console.WriteLine("3. Delete values");
                Console.WriteLine("4. Change table");
                Console.WriteLine("5. Go Back");

                Console.Write("Enter your choice: ");
                int subChoice = int.Parse(Console.ReadLine());

                switch (subChoice)
                {
                    case 1:
                        DMLmethods.InsertData(connectionString, selectedTable);
                        break;
                    case 2:
                        DMLmethods.UpdateData(connectionString, selectedTable);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Please choose the option: ");
                        Console.WriteLine("1. Delete all values from table");
                        Console.WriteLine("2. Delete spicific values");
                        Console.WriteLine("Enter your choice: ");

                        int deleteChoice = int.Parse(Console.ReadLine());

                        switch (deleteChoice)
                        {
                            case 1:
                                DDLmethods.TruncateTable(connectionString, selectedTable);
                                break;
                            case 2:
                                DMLmethods.DeleteData(connectionString, selectedTable);
                                break;
                            case 3:
                                return;
                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("What changes would you like to make to this table?\n");
                        Console.WriteLine("1. Rename a table");
                        Console.WriteLine("2. Rename a column");
                        Console.WriteLine("3. Add a new column");
                        Console.WriteLine("4. Drop a column");

                        int choiceIn = int.Parse(Console.ReadLine());
                        switch (choiceIn)
                        {
                            case 1:
                                DDLmethods.RenameTable(connectionString, selectedTable);
                                break;
                            case 2:
                                DDLmethods.RenameColumn(connectionString, selectedTable);
                                break;
                            case 3:
                                DDLmethods.AddColumn(connectionString, selectedTable);
                                break;
                            case 4:
                                Console.Clear();
                                DDLmethods.DropColumn(connectionString, selectedTable);
                                break;
                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid table selection.");
            }
        }
    }
}
