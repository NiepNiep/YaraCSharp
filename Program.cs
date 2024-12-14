using YARA01;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnSelectTarget_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.Description = "Sélectionnez le dossier à analyser";
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
        string dbFilePath = "results.db"; // Chemin de la base de données SQLite
        string scanId = Guid.NewGuid().ToString(); // scan id avec num aléatoire GUID

        // création base de donnée
        var dbHelper = new DatabaseHelper(dbFilePath);
        //Initialise le chemin de l'exécutable YARA
        var yaraScanner = new YaraScanner(yaraExe, dbHelper);


        // Charger et analyser les fichiers...

        string[] targetFiles = Directory.GetFiles(targetDirectory);
        string rulesArguments = string.Join(" ", Directory.GetFiles(rulesFolder, "*.yar"));

        Console.WriteLine($"Début du scan avec ScanId: {scanId}");
        foreach (var targetFile in targetFiles)
        {
            yaraScanner.ScanFile(scanId, rulesArguments, targetFile);

        }
        Console.WriteLine($"Scan terminé avec ScanId: {scanId}");

        int totalFiles = dbHelper.GetTotalFilesByScanId(scanId);
        int suspiciousFiles = dbHelper.GetSuspiciousFilesByScanId(scanId);

        lblTotalFiles.Text = totalFiles.ToString();
        lblSuspiciousFiles.Text = suspiciousFiles.ToString();

        // Récupérer et afficher les résultats
        string scanResults = dbHelper.GetResultsAsStringByScanId(scanId);
        txtResults.Text = scanResults; // txtResults est la TextBox multi-lignes

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
        // 
        // btnSelectTarget
        // 
        btnSelectTarget.Location = new Point(217, 63);
        btnSelectTarget.Name = "btnSelectTarget";
        btnSelectTarget.Size = new Size(200, 23);
        btnSelectTarget.TabIndex = 1;
        btnSelectTarget.Text = "Sélectionner le dossier à analyser";
        btnSelectTarget.UseVisualStyleBackColor = true;
        btnSelectTarget.Click += btnSelectTarget_Click;
        // 
        // txtTargetFile
        // 
        txtTargetFile.Location = new Point(423, 64);
        txtTargetFile.Name = "txtTargetFile";
        txtTargetFile.Size = new Size(605, 23);
        txtTargetFile.TabIndex = 3;
        // 
        // btnAnalyze
        // 
        btnAnalyze.Location = new Point(217, 117);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(200, 42);
        btnAnalyze.TabIndex = 4;
        btnAnalyze.Text = "Analyser";
        btnAnalyze.UseVisualStyleBackColor = true;
        btnAnalyze.Click += btnAnalyze_Click;
        // 
        // txtResults
        // 
        txtResults.ForeColor = SystemColors.WindowText;
        txtResults.Location = new Point(217, 185);
        txtResults.Multiline = true;
        txtResults.Name = "txtResults";
        txtResults.ReadOnly = true;
        txtResults.ScrollBars = ScrollBars.Vertical;
        txtResults.Size = new Size(811, 453);
        txtResults.TabIndex = 5;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(467, 131);
        label1.Name = "label1";
        label1.Size = new Size(156, 15);
        label1.TabIndex = 6;
        label1.Text = "Nombre de fichiers analysé :";
        // 
        // lblTotalFiles
        // 
        lblTotalFiles.AutoSize = true;
        lblTotalFiles.Location = new Point(629, 131);
        lblTotalFiles.Name = "lblTotalFiles";
        lblTotalFiles.Size = new Size(13, 15);
        lblTotalFiles.TabIndex = 7;
        lblTotalFiles.Text = "0";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(793, 131);
        label3.Name = "label3";
        label3.Size = new Size(172, 15);
        label3.TabIndex = 8;
        label3.Text = "Nombre de fichiers suspicieux :";
        // 
        // lblSuspiciousFiles
        // 
        lblSuspiciousFiles.AutoSize = true;
        lblSuspiciousFiles.Location = new Point(971, 131);
        lblSuspiciousFiles.Name = "lblSuspiciousFiles";
        lblSuspiciousFiles.Size = new Size(13, 15);
        lblSuspiciousFiles.TabIndex = 9;
        lblSuspiciousFiles.Text = "0";
        // 
        // MainForm
        // 
        ClientSize = new Size(1264, 722);
        Controls.Add(lblSuspiciousFiles);
        Controls.Add(label3);
        Controls.Add(lblTotalFiles);
        Controls.Add(label1);
        Controls.Add(txtResults);
        Controls.Add(btnAnalyze);
        Controls.Add(txtTargetFile);
        Controls.Add(btnSelectTarget);
        Name = "MainForm";
        Text = "Analyseur YARA";
        Load += MainForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button btnSelectTarget;
    private System.Windows.Forms.TextBox txtTargetFile;
    private TextBox txtResults;
    private Label label1;
    private Label lblTotalFiles;
    private Label label3;
    private Label lblSuspiciousFiles;
    private System.Windows.Forms.Button btnAnalyze;

    private void MainForm_Load(object sender, EventArgs e)
    {

    }
}

public static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
