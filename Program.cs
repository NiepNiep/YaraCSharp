using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YARA01;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnSelectTarget_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
        {
            Description = "Sélectionnez le dossier à analyser"
        };
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            txtTargetFile.Text = folderBrowserDialog.SelectedPath;
        }
    }

    private void btnAnalyze_Click(object sender, EventArgs e)
    {
        string yaraExe = "yara64.exe";
        string rulesFolder = @"rules";
        string targetDirectory = txtTargetFile.Text;
        string dbFilePath = "results.db";
        string scanId = Guid.NewGuid().ToString();

        var dbHelper = new DatabaseHelper(dbFilePath);
        var yaraScanner = new YaraScanner(yaraExe, dbHelper);

        if (string.IsNullOrWhiteSpace(txtTargetFile.Text))
        {
            MessageBox.Show(
                "Erreur : Aucun fichier trouvé dans le répertoire spécifié.",
                "Aucun fichier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        string[] targetFiles = Directory.GetFiles(targetDirectory);
        string rulesArguments = string.Join(" ", Directory.GetFiles(rulesFolder, "*.yar"));

        foreach (var targetFile in targetFiles)
        {
            yaraScanner.ScanFile(scanId, rulesArguments, targetFile);
        }

        int totalFiles = dbHelper.GetTotalFilesByScanId(scanId);
        int suspiciousFiles = dbHelper.GetSuspiciousFilesByScanId(scanId);

        lblTotalFiles.Text = totalFiles.ToString();
        lblSuspiciousFiles.Text = suspiciousFiles.ToString();

        string scanResults = dbHelper.GetResultsAsStringByScanId(scanId);
        txtResults.Text = scanResults;
    }

    private void InitializeComponent()
    {
        btnSelectTarget = new Button();
        txtTargetFile = new TextBox();
        btnAnalyze = new Button();
        txtResults = new TextBox();
        label1 = new Label();
        lblTotalFiles = new Label();
        label3 = new Label();
        lblSuspiciousFiles = new Label();
        SuspendLayout();

        // Couleurs simples
        var backgroundColor = Color.WhiteSmoke;
        var buttonColor = Color.LightGray;
        var labelColor = Color.Black;
        var resultBackgroundColor = Color.White;
        var resultTextColor = Color.Black;

        BackColor = backgroundColor;

        // btnSelectTarget
        btnSelectTarget.BackColor = buttonColor;
        btnSelectTarget.ForeColor = labelColor;
        btnSelectTarget.FlatStyle = FlatStyle.Flat;
        btnSelectTarget.Location = new Point(50, 30);
        btnSelectTarget.Name = "btnSelectTarget";
        btnSelectTarget.Size = new Size(200, 35);
        btnSelectTarget.TabIndex = 1;
        btnSelectTarget.Text = "Sélectionner un dossier";
        btnSelectTarget.UseVisualStyleBackColor = true;
        btnSelectTarget.Click += btnSelectTarget_Click;

        // txtTargetFile
        txtTargetFile.Location = new Point(270, 35);
        txtTargetFile.Name = "txtTargetFile";
        txtTargetFile.Size = new Size(500, 23);
        txtTargetFile.TabIndex = 2;
        txtTargetFile.BackColor = resultBackgroundColor;
        txtTargetFile.ForeColor = resultTextColor;

        // btnAnalyze
        btnAnalyze.BackColor = buttonColor;
        btnAnalyze.FlatStyle = FlatStyle.Flat;
        btnAnalyze.ForeColor = labelColor;
        btnAnalyze.Location = new Point(50, 80);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(200, 40);
        btnAnalyze.TabIndex = 3;
        btnAnalyze.Text = "Lancer l'analyse";
        btnAnalyze.UseVisualStyleBackColor = true;
        btnAnalyze.Click += btnAnalyze_Click;

        // txtResults
        txtResults.Location = new Point(50, 140);
        txtResults.Multiline = true;
        txtResults.ScrollBars = ScrollBars.Vertical;
        txtResults.Name = "txtResults";
        txtResults.ReadOnly = true;
        txtResults.Size = new Size(720, 300);
        txtResults.BackColor = resultBackgroundColor;
        txtResults.ForeColor = resultTextColor;

        // label1
        label1.AutoSize = true;
        label1.Location = new Point(50, 460);
        label1.ForeColor = labelColor;
        label1.Text = "Fichiers analysés :";
        label1.Font = new Font(label1.Font.FontFamily, 12, FontStyle.Bold);
        label1.Name = "label1";

        // lblTotalFiles
        lblTotalFiles.AutoSize = true;
        lblTotalFiles.Location = new Point(200, 460);
        lblTotalFiles.ForeColor = labelColor;
        lblTotalFiles.Text = "0";
        lblTotalFiles.Font = new Font(lblTotalFiles.Font.FontFamily, 14, FontStyle.Bold);
        lblTotalFiles.Name = "lblTotalFiles";

        // label3
        label3.AutoSize = true;
        label3.Location = new Point(50, 500);
        label3.ForeColor = labelColor;
        label3.Text = "Fichiers suspicieux :";
        label3.Font = new Font(label3.Font.FontFamily, 12, FontStyle.Bold);
        label3.Name = "label3";

        // lblSuspiciousFiles
        lblSuspiciousFiles.AutoSize = true;
        lblSuspiciousFiles.Location = new Point(220, 500);
        lblSuspiciousFiles.ForeColor = labelColor;
        lblSuspiciousFiles.Text = "0";
        lblSuspiciousFiles.Font = new Font(lblSuspiciousFiles.Font.FontFamily, 14, FontStyle.Bold);
        lblSuspiciousFiles.Name = "lblSuspiciousFiles";

        // MainForm
        ClientSize = new Size(850, 600);
        Controls.Add(btnSelectTarget);
        Controls.Add(txtTargetFile);
        Controls.Add(btnAnalyze);
        Controls.Add(txtResults);
        Controls.Add(label1);
        Controls.Add(lblTotalFiles);
        Controls.Add(label3);
        Controls.Add(lblSuspiciousFiles);
        Text = "Analyseur YARA";
        ResumeLayout(false);
        PerformLayout();
    }

    private Button btnSelectTarget;
    private TextBox txtTargetFile;
    private Button btnAnalyze;
    private TextBox txtResults;
    private Label label1;
    private Label lblTotalFiles;
    private Label label3;
    private Label lblSuspiciousFiles;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
