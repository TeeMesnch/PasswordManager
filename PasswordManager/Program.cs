using System.IO;
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
    }

    static class Init
    {
        public static void InitializeDatabase()
        {
            const string dbFile = "database.db";
            
            if (!File.Exists(dbFile))
            {
                using (File.Create(dbFile)) {} 
            }

            using var connection = new SqliteConnection($"Data Source={dbFile}");

            Console.WriteLine("Initializing database...");

            connection.Open();

            Console.WriteLine("Opening database...");

            using var command = connection.CreateCommand();

            Console.WriteLine("setting up database...");

            command.CommandText = """
                                      CREATE TABLE IF NOT EXISTS Data (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                          Username TEXT NOT NULL,
                                          Password TEXT NOT NULL
                                      );
                                  """;
            command.ExecuteNonQuery();

            Console.WriteLine("Done initializing database");
        }


        public static void DeleteDatabase()
        {
            
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
}