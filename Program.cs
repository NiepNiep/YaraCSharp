using System;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public partial class MainForm : Form
{
    private string _currentRulePath;
    private Label label1;
    private ToolStripMenuItem saveRuleAsToolStripMenuItem;
    private Label label2;
    private Label lbl_is_saved;
    private string _directoryPath;
    public MainForm()
    {
        InitializeComponent();
    }

    private void openRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Filter = "YARA Rules (*.yar)|*.yar";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _currentRulePath = ofd.FileName;
                txtRuleContent.Text = File.ReadAllText(_currentRulePath);
                lbl_is_saved.Text = "✅";
            }
        }
    }

    private void btnSelectTarget_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog fbd = new FolderBrowserDialog())
        {
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _directoryPath = fbd.SelectedPath;
                txtTargetFile.Text = _directoryPath;
            }
        }
    }

    private void saveRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentRulePath))
        {
            File.WriteAllText(_currentRulePath, txtRuleContent.Text);
            lbl_is_saved.Text = "✅";
        }
        else
        {
            saveRuleAsToolStripMenuItem_Click(sender, e);
        }

    }

    private void saveRuleAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = "Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _currentRulePath = saveFileDialog.FileName;
                File.WriteAllText(_currentRulePath, txtRuleContent.Text);
                this.Text = $"Éditeur de texte - {Path.GetFileName(_currentRulePath)}";
                MessageBox.Show("Fichier sauvegardé avec succès.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lbl_is_saved.Text = "✅";
            }
        }

    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        // Vérifie si Ctrl + S est pressé
        if (e.Control && e.KeyCode == Keys.S)
        {
            saveRuleToolStripMenuItem_Click(sender, e);
        }
    }

    private void btnAnalyze_Click(object sender, EventArgs e)
    {
        string yaraExe = "yara64.exe";
        string rulesFile = _currentRulePath;
        string targetDirectory = _directoryPath;

        if (!System.IO.File.Exists(yaraExe) || !System.IO.File.Exists(rulesFile) || !System.IO.Directory.Exists(targetDirectory))
        {
            MessageBox.Show("Le fichier YARA, le fichier de règles ou le répertoire cible est introuvable.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            // Effacer les résultats précédents
            lstResults.Items.Clear();

            // Obtenir tous les fichiers dans le répertoire
            string[] files = Directory.GetFiles(targetDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string targetFile in files)
            {
                string arguments = $"{rulesFile} {targetFile}";

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
                    lstResults.Items.Add($"{Path.GetFileName(targetFile)}");
                }
                else if (!string.IsNullOrEmpty(errors))
                {
                    lstResults.Items.Add($"{Path.GetFileName(targetFile)}");
                }
            }

            if (lstResults.Items.Count == 0)
            {
                lstResults.Items.Add("Aucun résultat trouvé.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void InitializeComponent()
    {
        btnSelectTarget = new System.Windows.Forms.Button();
        txtTargetFile = new System.Windows.Forms.TextBox();
        btnAnalyze = new System.Windows.Forms.Button();
        txtRuleContent = new System.Windows.Forms.TextBox();
        menuStrip1 = new MenuStrip();
        fIleToolStripMenuItem = new ToolStripMenuItem();
        openRuleToolStripMenuItem = new ToolStripMenuItem();
        saveRuleToolStripMenuItem = new ToolStripMenuItem();
        saveRuleAsToolStripMenuItem = new ToolStripMenuItem();
        lstResults = new ListBox();
        label1 = new Label();
        label2 = new Label();
        lbl_is_saved = new Label();
        menuStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // btnSelectTarget
        // 
        btnSelectTarget.Location = new Point(14, 284);
        btnSelectTarget.Name = "btnSelectTarget";
        btnSelectTarget.Size = new Size(124, 27);
        btnSelectTarget.TabIndex = 1;
        btnSelectTarget.Text = "Select directory";
        btnSelectTarget.UseVisualStyleBackColor = true;
        btnSelectTarget.Click += btnSelectTarget_Click;
        // 
        // txtTargetFile
        // 
        txtTargetFile.Location = new Point(154, 284);
        txtTargetFile.Name = "txtTargetFile";
        txtTargetFile.Size = new Size(300, 27);
        txtTargetFile.TabIndex = 3;
        // 
        // btnAnalyze
        // 
        btnAnalyze.Location = new Point(14, 317);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(440, 34);
        btnAnalyze.TabIndex = 4;
        btnAnalyze.Text = "Analyser";
        btnAnalyze.UseVisualStyleBackColor = true;
        btnAnalyze.Click += btnAnalyze_Click;
        // 
        // txtRuleContent
        // 
        txtRuleContent.Dock = DockStyle.Top;
        txtRuleContent.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtRuleContent.Location = new Point(0, 28);
        txtRuleContent.Multiline = true;
        txtRuleContent.Name = "txtRuleContent";
        txtRuleContent.ScrollBars = ScrollBars.Both;
        txtRuleContent.Size = new Size(882, 154);
        txtRuleContent.TabIndex = 5;
        txtRuleContent.TextChanged += txtRuleContent_TextChanged;
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fIleToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(882, 28);
        menuStrip1.TabIndex = 6;
        menuStrip1.Text = "menuStrip1";
        // 
        // fIleToolStripMenuItem
        // 
        fIleToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openRuleToolStripMenuItem, saveRuleToolStripMenuItem, saveRuleAsToolStripMenuItem });
        fIleToolStripMenuItem.Name = "fIleToolStripMenuItem";
        fIleToolStripMenuItem.Size = new Size(52, 24);
        fIleToolStripMenuItem.Text = "Rule";
        fIleToolStripMenuItem.Click += fIleToolStripMenuItem_Click;
        // 
        // openRuleToolStripMenuItem
        // 
        openRuleToolStripMenuItem.Name = "openRuleToolStripMenuItem";
        openRuleToolStripMenuItem.Size = new Size(141, 26);
        openRuleToolStripMenuItem.Text = "Open";
        openRuleToolStripMenuItem.Click += openRuleToolStripMenuItem_Click;
        // 
        // saveRuleToolStripMenuItem
        // 
        saveRuleToolStripMenuItem.Name = "saveRuleToolStripMenuItem";
        saveRuleToolStripMenuItem.Size = new Size(141, 26);
        saveRuleToolStripMenuItem.Text = "Save";
        saveRuleToolStripMenuItem.Click += saveRuleToolStripMenuItem_Click;
        // 
        // saveRuleAsToolStripMenuItem
        // 
        saveRuleAsToolStripMenuItem.Name = "saveRuleAsToolStripMenuItem";
        saveRuleAsToolStripMenuItem.Size = new Size(141, 26);
        saveRuleAsToolStripMenuItem.Text = "Save as";
        saveRuleAsToolStripMenuItem.Click += saveRuleAsToolStripMenuItem_Click;
        // 
        // lstResults
        // 
        lstResults.AccessibleName = "";
        lstResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstResults.FormattingEnabled = true;
        lstResults.Location = new Point(507, 224);
        lstResults.Name = "lstResults";
        lstResults.Size = new Size(350, 164);
        lstResults.TabIndex = 7;
        lstResults.SelectedIndexChanged += lstResults_SelectedIndexChanged;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(507, 201);
        label1.Name = "label1";
        label1.Size = new Size(108, 20);
        label1.TabIndex = 8;
        label1.Text = "Detected files :";
        label1.Click += label1_Click;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(15, 189);
        label2.Name = "label2";
        label2.Size = new Size(56, 20);
        label2.TabIndex = 9;
        label2.Text = "Saved :";
        // 
        // lbl_is_saved
        // 
        lbl_is_saved.AutoSize = true;
        lbl_is_saved.Location = new Point(77, 189);
        lbl_is_saved.Name = "lbl_is_saved";
        lbl_is_saved.Size = new Size(0, 20);
        lbl_is_saved.TabIndex = 10;
        // 
        // MainForm
        // 
        ClientSize = new Size(882, 446);
        Controls.Add(lbl_is_saved);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(lstResults);
        Controls.Add(txtRuleContent);
        Controls.Add(btnAnalyze);
        Controls.Add(txtTargetFile);
        Controls.Add(btnSelectTarget);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "MainForm";
        Text = "Analyseur YARA";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
        //
        // Key ctrl + s
        //
        this.KeyPreview = true;   
        this.KeyDown += MainForm_KeyDown;
    }

    private System.Windows.Forms.Button btnSelectTarget;
    private System.Windows.Forms.TextBox txtTargetFile;
    private System.Windows.Forms.TextBox txtRuleContent;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fIleToolStripMenuItem;
    private ToolStripMenuItem openRuleToolStripMenuItem;
    private ToolStripMenuItem saveRuleToolStripMenuItem;
    private System.Windows.Forms.Button btnAnalyze;
    private ListBox lstResults;

    private void lstResults_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void fIleToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void txtRuleContent_TextChanged(object sender, EventArgs e)
    {
        lbl_is_saved.Text = "❌";
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
