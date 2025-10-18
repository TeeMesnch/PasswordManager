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
            using var connection = new SqliteConnection("Filename = database.db");
            
            connection.Open();
            
            using var command = connection.CreateCommand();
            
            command.CommandText = """
                                      CREATE TABLE IF NOT EXISTS Passwords (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                          Service TEXT NOT NULL,
                                          Username TEXT,
                                          Password TEXT NOT NULL
                                      );
                                  """;
            command.ExecuteNonQuery();
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
            
        }
    }
}