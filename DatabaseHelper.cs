using System.Data;
using System.Data.SQLite;

namespace YARA01
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        // Constructeur : Spécifie la chaîne de connexion à la base de données SQLite.
        public DatabaseHelper(string dbFilePath)
        {
            _connectionString = $"Data Source={dbFilePath};Version=3;";

            // Créer la base de données et les tables si elles n'existent pas.
            InitializeDatabase();
        }

        // Méthode pour initialiser la base de données, créant les tables nécessaires si elles n'existent pas.
        private void InitializeDatabase()
        {
            if (!File.Exists("results.db"))
            {
                SQLiteConnection.CreateFile("results.db"); // Crée le fichier de base de données SQLite
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Créer la table des résultats si elle n'existe pas déjà.
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS ScanResults (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ScanId TEXT NOT NULL,
                    FileName TEXT,
                    FilePath TEXT,
                    ScanDate DATETIME,
                    ScanResult TEXT,
                    Error TEXT
                );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Méthode pour insérer un résultat de scan dans la base de données.
        public void InsertScanResult(string scanId, string fileName, string filePath, string scanResult, string error = null)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //modif

                string insertQuery = @"
                INSERT INTO ScanResults (ScanId, FileName, FilePath, ScanDate, ScanResult, Error)
                VALUES (@ScanId, @FileName, @FilePath, @ScanDate, @ScanResult, @Error);";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    // modif
                    command.Parameters.AddWithValue("@ScanId", scanId);
                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.Parameters.AddWithValue("@ScanDate", timestamp);
                    command.Parameters.AddWithValue("@ScanResult", scanResult);
                    command.Parameters.AddWithValue("@Error", error ?? string.Empty); // Si pas d'erreur, on insère une chaîne vide

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }


        //===========% CMD SQL %==========/

        // Méthode pour récupérer tous les résultats des scans, par exemple pour générer des rapports.
        
       
        public string GetResultsAsStringByScanId(string scanId)
        {
            using (var connection = new SQLiteConnection(_connectionString)) // Crée une connexion à la base de données SQLite
            {
                connection.Open(); // Ouvre la connexion à la base de données

                // Requête SQL pour sélectionner tous les résultats d'un scan donné, triés par date
                string query = "SELECT * FROM ScanResults WHERE ScanId = @ScanId ORDER BY ScanDate;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ScanId", scanId);

                    using (var reader = command.ExecuteReader())
                    {
                        var results = new System.Text.StringBuilder();
                        results.AppendLine("Résultats du scan :" + scanId);
                        results.AppendLine(new string('-', 50));

                        while (reader.Read())
                        {
                            results.AppendLine($"Nom du fichier : {reader["FileName"]}");
                            results.AppendLine($"Date du scan : {reader["ScanDate"]}");
                            results.AppendLine($"Résultat : {reader["ScanResult"]}");
                            results.AppendLine(new string('-', 50));
                        }

                        return results.ToString();
                    }
                }
            }
        }

        public int GetTotalFilesByScanId(string scanId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM ScanResults WHERE ScanId = @ScanId;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ScanId", scanId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int GetSuspiciousFilesByScanId(string scanId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM ScanResults WHERE ScanId = @ScanId AND ScanResult IS NOT NULL AND TRIM(ScanResult) != '';";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ScanId", scanId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public List<string> GetSuspiciousFileNamesByScanId(string scanId)
        {
            List<string> suspiciousFileNames = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT FileName FROM ScanResults WHERE ScanId = @ScanId AND ScanResult IS NOT NULL AND TRIM(ScanResult) != '';";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ScanId", scanId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suspiciousFileNames.Add(reader["FileName"].ToString());
                        }
                    }
                }
            }

            return suspiciousFileNames;
        }


    }
}