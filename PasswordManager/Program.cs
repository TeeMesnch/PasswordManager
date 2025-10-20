using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace PasswordManager
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            var commandList = new Dictionary<string, Action<string[]>>
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
                commandAction(args);
            }
            else
            {
                Console.WriteLine("Unknown command try --help or -H");
            }
        }
        
        public static string ReadPasswordFromConsole(string prompt = "Password: ")
        {
            Console.Write(prompt);
            var pwd = new StringBuilder();
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
        public static void InitializeDatabase(string[] placeholder)
        {
            const string dbFile = "database.db";

            if (!File.Exists(dbFile))
            {
                using (File.Create(dbFile));
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

            string master = MainClass.ReadPasswordFromConsole();

            if (!string.IsNullOrEmpty(master))
            {
                string hashedMaster = PasswordHasher.Hash(master);

                command.CommandText = """
                                          INSERT INTO MasterPassword (Id, Hash)
                                          VALUES (1, @hash)
                                          ON CONFLICT(Id) DO UPDATE SET Hash = @hash;
                                      """;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@hash", hashedMaster);
                command.ExecuteNonQuery();

                Console.WriteLine("Master password set.");
            }
            else
            {
                Console.WriteLine("No password entered. Master password was not set.");
            }
        }





        public static void DeleteDatabase(string[] placeholder)
        {
            if (File.Exists("database.db"))
            {
                Console.WriteLine("Before deleting the Database type in your master password.");
                string typedIn = MainClass.ReadPasswordFromConsole();

                if (PasswordHasher.Verify(typedIn, PasswordHasher.ReadHashedPassword()))
                {
                    File.Delete("database.db");
                    Console.WriteLine("Database file deleted.");
                }
            }
            else
            {
                Console.WriteLine("No Database file found. Try --help or -H");
            }
        }
    }


    static class CommandExecution
    {
        public static void Add(string[] args)
        {
            var username = args[1];
            var password = args[2];

            var hashedPassword = PasswordHasher.Hash(password);

            if (File.Exists("database.db"))
            {
                using var connection = new SqliteConnection("Data Source=database.db");
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = """
                                      INSERT INTO Data (Username, Password)
                                      VALUES (@username, @password);
                                      """;

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);

                command.ExecuteNonQuery();

                Console.WriteLine($"Successfully added new password (user : {username})");
            }
            else
            {
                Console.WriteLine("Database file not found. Try --help or -H");
            }
        }


        public static void Remove(string[] args)
        {
            if (File.Exists("database.db"))
            {
                var entryToRemove = args[1];

                Console.WriteLine("Before deleting the Password type in your master password.");
                string typedIn = MainClass.ReadPasswordFromConsole();
                
                using var connection = new SqliteConnection("Data Source=database.db");
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = """
                                      DELETE FROM Data WHERE Username = @username;
                                      """;

                command.Parameters.AddWithValue("@username", entryToRemove);

                if (PasswordHasher.Verify(typedIn, PasswordHasher.ReadHashedPassword()))
                {
                    command.ExecuteNonQuery();

                    Console.WriteLine($"Successfully removed the password (user : {entryToRemove})");
                }
            }
            else
            {
                Console.WriteLine("Database file not found. Try --help or -H");
            }
        }


        public static void Change(string[] args)
        {
            if (File.Exists("database.db"))
            {
                
            }
            else
            {
                Console.WriteLine("Database file not found. Try --help or -H");
            }
        }

        public static void Show(string[] args)
        {
            if (File.Exists("database.db"))
            {
                var entryToShow = args[1];

                var typedIn = MainClass.ReadPasswordFromConsole();
                
                if (PasswordHasher.Verify(typedIn, PasswordHasher.ReadHashedPassword()))
                {
                    using var connection = new SqliteConnection($"Data Source=database.db");
                    connection.Open();

                    using var command = connection.CreateCommand();

                    command.CommandText = $"""
                                          SELECT Username, Password FROM Data WHERE Username = @username;
                                          """;
                    
                    command.Parameters.AddWithValue("@username", entryToShow);
                    
                    using var reader = command.ExecuteReader();

                    string user = string.Empty;
                    string password = string.Empty;
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            user = reader.GetString(0);
                            password = reader.GetString(1);
                        }
                    }
                    
                    Console.WriteLine($"(user : {user}) (password : {password})");
                }
                else
                {
                    Console.WriteLine("Password cannot be verified");
                }
            }
            else
            {
                Console.WriteLine("Database file not found. Try --help or -H");
            }
        }

        public static void Help(string[] placeholder)
        {
            Console.WriteLine("To get started type INIT to create a new database");
            Console.WriteLine("Type DELETE to delete every stored password and the associated Database");
            Console.WriteLine("To add type --add or -A followed by {user} {password}");
            Console.WriteLine("To view your password type --show or -S followed by {user}");
            Console.WriteLine("To remove a single password type --remove or -R followed by {user} {password}");
            Console.WriteLine("To change type --change or -C followed by {originalUser} {originalPassword} {newUser} {newPassword}");
        }
    }
    
    public static class PasswordHasher
    {
        private const int SaltLength = 16;
        
        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltLength];
            RandomNumberGenerator.Fill(salt);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

            byte[] hash = SHA256.HashData(saltedPassword);

            byte[] result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

            return Convert.ToBase64String(result);
        }

        public static string ReadHashedPassword()
        {
            using var connection = new SqliteConnection($"Data Source=database.db");
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = """
                                  SELECT Hash FROM MasterPassword;
                                  """;

            string password = "password";
            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    password = reader.GetString(0);
                }
            }
            
            return password;
        }
        
        public static bool Verify(string password, string storedHash)
        {
            byte[] storedBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[SaltLength];
            byte[] storedHashBytes = new byte[storedBytes.Length - SaltLength];

            Buffer.BlockCopy(storedBytes, 0, salt, 0, SaltLength);
            Buffer.BlockCopy(storedBytes, SaltLength, storedHashBytes, 0, storedHashBytes.Length);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
            
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

            byte[] computedHash = SHA256.HashData(saltedPassword);

            return CryptographicOperations.FixedTimeEquals(storedHashBytes, computedHash);
        }
    }

}