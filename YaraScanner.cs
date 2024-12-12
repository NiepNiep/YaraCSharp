using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MYARAcons
{
    public class YaraScanner
    {
        private readonly string _yaraExecutable;
        private readonly DatabaseHelper _dbHelper; // Référence à la classe DatabaseHelper


        // Constructeur : Initialise le chemin de l'exécutable YARA.
        public YaraScanner(string yaraExecutable, DatabaseHelper dbHelper)
        {
            if (!File.Exists(yaraExecutable))
                throw new FileNotFoundException($"Le fichier '{yaraExecutable}' est introuvable.");

            _yaraExecutable = yaraExecutable;
            _dbHelper = dbHelper;
        }

        // Méthode pour exécuter YARA avec des règles et un fichier cible.
        // Renvoie la sortie standard et les éventuelles erreurs.
        public void ScanFile(string scanId, string rules, string targetFile)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _yaraExecutable,
                        Arguments = $"{rules} {targetFile}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                // Lecture des résultats
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                _dbHelper.InsertScanResult(scanId, targetFile, rules, output, errors);


                // Affichage des résultats
                Console.WriteLine($"[{timestamp}] Résultat de l'analyse pour {targetFile}:");
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine(output);
                }
                else
                {
                    Console.WriteLine("Aucun résultat trouvé pour ce fichier.");
                }

                if (!string.IsNullOrEmpty(errors))
                {
                    Console.WriteLine("Erreurs :");
                    Console.WriteLine(errors);
                }
            }
            catch (Exception ex)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"[{timestamp}] Erreur lors de l'analyse du fichier '{targetFile}': {ex.Message}");
            }
        }
    }
}