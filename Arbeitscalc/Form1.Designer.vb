<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        openFileDialog1 = New OpenFileDialog()
        dgvTagesdaten = New DataGridView()
        dgvSummen = New DataGridView()
        LabelTagesdaten = New Label()
        LabelSummen = New Label()
        MenuStrip1 = New MenuStrip()
        ToolStripMenuItem1 = New ToolStripMenuItem()
        ImportStripMenuItem = New ToolStripMenuItem()
        CSVImportToolStripMenuItem = New ToolStripMenuItem()
        ÖffnenToolStripMenuItem = New ToolStripMenuItem()
        ExportierenToolStripMenuItem = New ToolStripMenuItem()
        PDFToolStripMenuItem = New ToolStripMenuItem()
        CSVToolStripMenuItem = New ToolStripMenuItem()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        PDSOnlineToolStripMenuItem1 = New ToolStripMenuItem()
        GithubToolStripMenuItem = New ToolStripMenuItem()
        JGNSchlaubereichToolStripMenuItem = New ToolStripMenuItem()
        InfoToolStripMenuItem = New ToolStripMenuItem()
        RechtlichesToolStripMenuItem = New ToolStripMenuItem()
        SplitContainer1 = New SplitContainer()
        SupportUndBugReportToolStripMenuItem = New ToolStripMenuItem()
        CType(dgvTagesdaten, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgvSummen, ComponentModel.ISupportInitialize).BeginInit()
        MenuStrip1.SuspendLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        SuspendLayout()
        ' 
        ' openFileDialog1
        ' 
        openFileDialog1.FileName = "OpenFileDialog1"
        ' 
        ' dgvTagesdaten
        ' 
        dgvTagesdaten.AllowDrop = True
        dgvTagesdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvTagesdaten.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvTagesdaten.Dock = DockStyle.Fill
        dgvTagesdaten.Location = New Point(0, 0)
        dgvTagesdaten.Name = "dgvTagesdaten"
        dgvTagesdaten.Size = New Size(1077, 489)
        dgvTagesdaten.TabIndex = 1
        ' 
        ' dgvSummen
        ' 
        dgvSummen.AllowDrop = True
        dgvSummen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSummen.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvSummen.Dock = DockStyle.Fill
        dgvSummen.Location = New Point(0, 0)
        dgvSummen.Name = "dgvSummen"
        dgvSummen.Size = New Size(1077, 262)
        dgvSummen.TabIndex = 2
        ' 
        ' LabelTagesdaten
        ' 
        LabelTagesdaten.AutoSize = True
        LabelTagesdaten.Location = New Point(0, 0)
        LabelTagesdaten.Name = "LabelTagesdaten"
        LabelTagesdaten.Size = New Size(67, 15)
        LabelTagesdaten.TabIndex = 3
        LabelTagesdaten.Text = "Tagesdaten"
        ' 
        ' LabelSummen
        ' 
        LabelSummen.AutoSize = True
        LabelSummen.Location = New Point(0, 0)
        LabelSummen.Name = "LabelSummen"
        LabelSummen.Size = New Size(55, 15)
        LabelSummen.TabIndex = 4
        LabelSummen.Text = "Summen"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.Items.AddRange(New ToolStripItem() {ToolStripMenuItem1, ToolsToolStripMenuItem, InfoToolStripMenuItem, RechtlichesToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1101, 24)
        MenuStrip1.TabIndex = 6
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' ToolStripMenuItem1
        ' 
        ToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {ImportStripMenuItem, ExportierenToolStripMenuItem})
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New Size(46, 20)
        ToolStripMenuItem1.Text = "Datei"
        ' 
        ' ImportStripMenuItem
        ' 
        ImportStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {CSVImportToolStripMenuItem, ÖffnenToolStripMenuItem})
        ImportStripMenuItem.Name = "ImportStripMenuItem"
        ImportStripMenuItem.Size = New Size(136, 22)
        ImportStripMenuItem.Text = "Importieren"
        ' 
        ' CSVImportToolStripMenuItem
        ' 
        CSVImportToolStripMenuItem.Name = "CSVImportToolStripMenuItem"
        CSVImportToolStripMenuItem.Size = New Size(174, 22)
        CSVImportToolStripMenuItem.Text = "CSV"
        ' 
        ' ÖffnenToolStripMenuItem
        ' 
        ÖffnenToolStripMenuItem.Name = "ÖffnenToolStripMenuItem"
        ÖffnenToolStripMenuItem.Size = New Size(174, 22)
        ÖffnenToolStripMenuItem.Text = "TXT (Experimental)"
        ' 
        ' ExportierenToolStripMenuItem
        ' 
        ExportierenToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {PDFToolStripMenuItem, CSVToolStripMenuItem})
        ExportierenToolStripMenuItem.Name = "ExportierenToolStripMenuItem"
        ExportierenToolStripMenuItem.Size = New Size(136, 22)
        ExportierenToolStripMenuItem.Text = "Exportieren"
        ' 
        ' PDFToolStripMenuItem
        ' 
        PDFToolStripMenuItem.Name = "PDFToolStripMenuItem"
        PDFToolStripMenuItem.Size = New Size(95, 22)
        PDFToolStripMenuItem.Text = "PDF"
        ' 
        ' CSVToolStripMenuItem
        ' 
        CSVToolStripMenuItem.Name = "CSVToolStripMenuItem"
        CSVToolStripMenuItem.Size = New Size(95, 22)
        CSVToolStripMenuItem.Text = "CSV"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {PDSOnlineToolStripMenuItem1, GithubToolStripMenuItem, JGNSchlaubereichToolStripMenuItem, SupportUndBugReportToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(47, 20)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' PDSOnlineToolStripMenuItem1
        ' 
        PDSOnlineToolStripMenuItem1.Name = "PDSOnlineToolStripMenuItem1"
        PDSOnlineToolStripMenuItem1.Size = New Size(202, 22)
        PDSOnlineToolStripMenuItem1.Text = "PDS Online"
        ' 
        ' GithubToolStripMenuItem
        ' 
        GithubToolStripMenuItem.Name = "GithubToolStripMenuItem"
        GithubToolStripMenuItem.Size = New Size(202, 22)
        GithubToolStripMenuItem.Text = "Github"
        ' 
        ' JGNSchlaubereichToolStripMenuItem
        ' 
        JGNSchlaubereichToolStripMenuItem.Name = "JGNSchlaubereichToolStripMenuItem"
        JGNSchlaubereichToolStripMenuItem.Size = New Size(202, 22)
        JGNSchlaubereichToolStripMenuItem.Text = "JGN Schlau"
        ' 
        ' InfoToolStripMenuItem
        ' 
        InfoToolStripMenuItem.Name = "InfoToolStripMenuItem"
        InfoToolStripMenuItem.Size = New Size(40, 20)
        InfoToolStripMenuItem.Text = "Info"
        ' 
        ' RechtlichesToolStripMenuItem
        ' 
        RechtlichesToolStripMenuItem.Name = "RechtlichesToolStripMenuItem"
        RechtlichesToolStripMenuItem.Size = New Size(79, 20)
        RechtlichesToolStripMenuItem.Text = "Rechtliches"
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.AllowDrop = True
        SplitContainer1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        SplitContainer1.Location = New Point(12, 27)
        SplitContainer1.Name = "SplitContainer1"
        SplitContainer1.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(LabelTagesdaten)
        SplitContainer1.Panel1.Controls.Add(dgvTagesdaten)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(LabelSummen)
        SplitContainer1.Panel2.Controls.Add(dgvSummen)
        SplitContainer1.Size = New Size(1077, 755)
        SplitContainer1.SplitterDistance = 489
        SplitContainer1.TabIndex = 7
        ' 
        ' SupportUndBugReportToolStripMenuItem
        ' 
        SupportUndBugReportToolStripMenuItem.Name = "SupportUndBugReportToolStripMenuItem"
        SupportUndBugReportToolStripMenuItem.Size = New Size(202, 22)
        SupportUndBugReportToolStripMenuItem.Text = "Support und Bug Report"
        ' 
        ' Form1
        ' 
        AllowDrop = True
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        ClientSize = New Size(1101, 794)
        Controls.Add(MenuStrip1)
        Controls.Add(SplitContainer1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Arbeitscalc by Jan Geiger Networking"
        CType(dgvTagesdaten, ComponentModel.ISupportInitialize).EndInit()
        CType(dgvSummen, ComponentModel.ISupportInitialize).EndInit()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel1.PerformLayout()
        SplitContainer1.Panel2.ResumeLayout(False)
        SplitContainer1.Panel2.PerformLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents openFileDialog1 As OpenFileDialog
    Friend WithEvents dgvTagesdaten As DataGridView
    Friend WithEvents dgvSummen As DataGridView
    Friend WithEvents LabelTagesdaten As Label
    Friend WithEvents LabelSummen As Label
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ImportStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportierenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InfoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RechtlichesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents CSVImportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ÖffnenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PDFToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CSVToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PDSOnlineToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents GithubToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JGNSchlaubereichToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SupportUndBugReportToolStripMenuItem As ToolStripMenuItem

End Class
