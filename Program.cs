using System;
using System.Diagnostics;
using System.Windows.Forms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnSelectRules_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Sélectionnez le fichier de règles YARA";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            txtRulesFile.Text = openFileDialog.FileName;
        }
    }

    private void btnSelectTarget_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Sélectionnez le fichier à analyser";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            txtTargetFile.Text = openFileDialog.FileName;
        }
    }

    private void btnAnalyze_Click(object sender, EventArgs e)
    {
        string yaraExe = "yara64.exe";
        string rulesFile = txtRulesFile.Text;
        string targetFile = txtTargetFile.Text;

        if (!System.IO.File.Exists(yaraExe) || !System.IO.File.Exists(rulesFile) || !System.IO.File.Exists(targetFile))
        {
            MessageBox.Show("Un ou plusieurs fichiers sont introuvables.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(output, "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (!string.IsNullOrEmpty(errors))
                MessageBox.Show(errors, "Erreurs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Aucun résultat trouvé.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeComponent()
    {
        this.btnSelectRules = new System.Windows.Forms.Button();
        this.btnSelectTarget = new System.Windows.Forms.Button();
        this.txtRulesFile = new System.Windows.Forms.TextBox();
        this.txtTargetFile = new System.Windows.Forms.TextBox();
        this.btnAnalyze = new System.Windows.Forms.Button();
        this.SuspendLayout();

        // 
        // btnSelectRules
        // 
        this.btnSelectRules.Location = new System.Drawing.Point(12, 12);
        this.btnSelectRules.Name = "btnSelectRules";
        this.btnSelectRules.Size = new System.Drawing.Size(200, 23);
        this.btnSelectRules.TabIndex = 0;
        this.btnSelectRules.Text = "Sélectionner le fichier de règles YARA";
        this.btnSelectRules.UseVisualStyleBackColor = true;
        this.btnSelectRules.Click += new System.EventHandler(this.btnSelectRules_Click);

        // 
        // btnSelectTarget
        // 
        this.btnSelectTarget.Location = new System.Drawing.Point(12, 41);
        this.btnSelectTarget.Name = "btnSelectTarget";
        this.btnSelectTarget.Size = new System.Drawing.Size(200, 23);
        this.btnSelectTarget.TabIndex = 1;
        this.btnSelectTarget.Text = "Sélectionner le fichier à analyser";
        this.btnSelectTarget.UseVisualStyleBackColor = true;
        this.btnSelectTarget.Click += new System.EventHandler(this.btnSelectTarget_Click);

        // 
        // txtRulesFile
        // 
        this.txtRulesFile.Location = new System.Drawing.Point(218, 14);
        this.txtRulesFile.Name = "txtRulesFile";
        this.txtRulesFile.Size = new System.Drawing.Size(300, 20);
        this.txtRulesFile.TabIndex = 2;

        // 
        // txtTargetFile
        // 
        this.txtTargetFile.Location = new System.Drawing.Point(218, 43);
        this.txtTargetFile.Name = "txtTargetFile";
        this.txtTargetFile.Size = new System.Drawing.Size(300, 20);
        this.txtTargetFile.TabIndex = 3;

        // 
        // btnAnalyze
        // 
        this.btnAnalyze.Location = new System.Drawing.Point(12, 70);
        this.btnAnalyze.Name = "btnAnalyze";
        this.btnAnalyze.Size = new System.Drawing.Size(506, 23);
        this.btnAnalyze.TabIndex = 4;
        this.btnAnalyze.Text = "Analyser";
        this.btnAnalyze.UseVisualStyleBackColor = true;
        this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);

        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(530, 105);
        this.Controls.Add(this.btnAnalyze);
        this.Controls.Add(this.txtTargetFile);
        this.Controls.Add(this.txtRulesFile);
        this.Controls.Add(this.btnSelectTarget);
        this.Controls.Add(this.btnSelectRules);
        this.Name = "MainForm";
        this.Text = "Analyseur YARA";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Button btnSelectRules;
    private System.Windows.Forms.Button btnSelectTarget;
    private System.Windows.Forms.TextBox txtRulesFile;
    private System.Windows.Forms.TextBox txtTargetFile;
    private System.Windows.Forms.Button btnAnalyze;
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
