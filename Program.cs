using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        string yaraExe = @"yara64.exe";
        string rulesFile = @"rules.yar";
        string targetFile = "testfile.txt";

        if (!System.IO.File.Exists(yaraExe))
        {
            Console.WriteLine($"Le fichier '{yaraExe}' est introuvable.");
            return;
        }
        if (!System.IO.File.Exists(rulesFile))
        {
            Console.WriteLine($"Le fichier de règles '{rulesFile}' est introuvable.");
            return;
        }
        if (!System.IO.File.Exists(targetFile))
        {
            Console.WriteLine($"Le fichier cible '{targetFile}' est introuvable.");
            return;
        }

        string arguments = $"{rulesFile} {targetFile}";

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = yaraExe,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Résultat de l'analyse :");
                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine("Aucun résultat trouvé.");
            }

            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine("Erreurs :");
                Console.WriteLine(errors);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }
    }
}
