using Microsoft.Data.Sqlite;

namespace PasswordManager
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            var commandList = new Dictionary<string, Action>
            {
                { "INIT", Init.Initialize},
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
        public static void Initialize()
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