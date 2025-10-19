using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace PasswordManager
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            var commandList = new Dictionary<string, Action>
            {
                { "INIT", Init.InitializeDatabase },
                { "DELETE", Init.DeleteDatabase },
                { "--add", CommandExecution.Add },
                { "-A", CommandExecution.Add },
                { "--remove", CommandExecution.Remove },
                { "-R", CommandExecution.Remove },
                { "--change", CommandExecution.Change },
                { "-C", CommandExecution.Change },
                { "--show", CommandExecution.Show },
                { "-S", CommandExecution.Show },
                { "--help", CommandExecution.Help },
                { "-H", CommandExecution.Help }
            };

            if (commandList.TryGetValue(args[0], out var commandAction))
            {
                commandAction();
            }
            else
            {
                Console.WriteLine("Unknown command try --help or -H");
            }
        }
        
        public static string ReadPasswordFromConsole(string prompt = "Password: ")
        {
            Console.Write(prompt);
            var pwd = new System.Text.StringBuilder();
            ConsoleKeyInfo key;

            while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pwd.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            Console.WriteLine();
            return pwd.ToString();
        }

    }

    static class Init
    {
        public static void InitializeDatabase()
        {
            const string dbFile = "database.db";

            if (!File.Exists(dbFile))
            {
                using (File.Create(dbFile))
                {
                }
            }
            else
            {
                Console.WriteLine("Database file already exists.");
                return;
            }

            using var connection = new SqliteConnection($"Data Source={dbFile}");

            Console.WriteLine("Initializing database...");

            connection.Open();

            Console.WriteLine("Opening database...");

            using var command = connection.CreateCommand();

            Console.WriteLine("Setting up database...");

            command.CommandText = """
                                      CREATE TABLE IF NOT EXISTS Data (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                          Username TEXT NOT NULL,
                                          Password TEXT NOT NULL
                                      );
                                  """;
            command.ExecuteNonQuery();

            command.CommandText = """
                                      CREATE TABLE IF NOT EXISTS MasterPassword (
                                          Id INTEGER PRIMARY KEY CHECK (Id = 1),
                                          Hash TEXT NOT NULL
                                      );
                                  """;
            command.ExecuteNonQuery();

            Console.WriteLine("Done initializing database. You may set your MASTER password now!");

            Console.Write("Master password: ");
            string master = MainClass.ReadPasswordFromConsole();

            if (!string.IsNullOrEmpty(master))
            {
                string hashedMaster = StorePassword.Hash(master); // CHANGE LATER

                command.CommandText = """
                                          INSERT INTO MasterPassword (Id, Hash)
                                          VALUES (1, $hash)
                                          ON CONFLICT(Id) DO UPDATE SET Hash = $hash;
                                      """;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$hash", hashedMaster);
                command.ExecuteNonQuery();

                Console.WriteLine("Master password set.");
            }
            else
            {
                Console.WriteLine("No password entered. Master password was not set.");
            }
        }





        public static void DeleteDatabase()
        {
            if (File.Exists("database.db"))
            {
                File.Delete("database.db");
                Console.WriteLine("successfully deleted database");
            }
            else
            {
                Console.WriteLine("Error: Database not found");
            }
        }
    }


    static class CommandExecution
    {
        public static void Add()
        {

        }

        public static void Remove()
        {

        }

        public static void Change()
        {

        }

        public static void Show()
        {

        }

        public static void Help()
        {
            Console.WriteLine("To get started type INIT to create a new database");
            Console.WriteLine("Type DELETE to delete every stored password");
            Console.WriteLine("To add type --add or -A followed by {user} {password}");
            Console.WriteLine("To view your password type --show or -S followed by {user}");
            Console.WriteLine("To remove a single password type --remove or -R followed by {user} {password}");
            Console.WriteLine("To change type --change or -C followed by {originalUser} {originalPassword} {newUser} {newPassword}");
        }
    }

    static class StorePassword
    {
        public static string Hash(string password)
        {
            static void AddSalt()
            {
                
            }

            return password;
        }

        public static void AddToDatabase()
        {
            
        }
    }
}