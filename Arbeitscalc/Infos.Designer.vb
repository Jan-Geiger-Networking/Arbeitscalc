<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Infos
    Inherits System.Windows.Forms.Form

    Private WithEvents TabControl1 As TabControl
    Private WithEvents TabPage1 As TabPage
    Private WithEvents TabPage2 As TabPage
    Private WithEvents TabPage3 As TabPage
    Private WithEvents GroupBox1 As GroupBox
    Private WithEvents PictureBox1 As PictureBox
    Private WithEvents TextBox1 As TextBox
    Private WithEvents GroupBox5 As GroupBox
    Private WithEvents PictureBox5 As PictureBox
    Private WithEvents TextBox5 As TextBox
    Private WithEvents GroupBox4 As GroupBox
    Private WithEvents PictureBox4 As PictureBox
    Private WithEvents TextBox4 As TextBox
    Private WithEvents GroupBox3 As GroupBox
    Private WithEvents PictureBox3 As PictureBox
    Private WithEvents TextBox3 As TextBox
    Private WithEvents GroupBox2 As GroupBox
    Private WithEvents PictureBox2 As PictureBox
    Private WithEvents TextBox2 As TextBox
    Private WithEvents TextBox7 As TextBox

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Infos))
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        TextBox7 = New TextBox()
        GroupBox5 = New GroupBox()
        PictureBox5 = New PictureBox()
        TextBox5 = New TextBox()
        GroupBox4 = New GroupBox()
        PictureBox4 = New PictureBox()
        TextBox4 = New TextBox()
        GroupBox3 = New GroupBox()
        PictureBox3 = New PictureBox()
        TextBox3 = New TextBox()
        GroupBox2 = New GroupBox()
        PictureBox2 = New PictureBox()
        TextBox2 = New TextBox()
        GroupBox1 = New GroupBox()
        PictureBox1 = New PictureBox()
        TextBox1 = New TextBox()
        TabPage2 = New TabPage()
        dgvAnleitung = New DataGridView()
        TabPage3 = New TabPage()
        TextBox6 = New TextBox()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        GroupBox5.SuspendLayout()
        CType(PictureBox5, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox4.SuspendLayout()
        CType(PictureBox4, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox3.SuspendLayout()
        CType(PictureBox3, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox2.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        TabPage2.SuspendLayout()
        CType(dgvAnleitung, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Controls.Add(TabPage2)
        TabControl1.Controls.Add(TabPage3)
        TabControl1.Dock = DockStyle.Fill
        TabControl1.Location = New Point(0, 0)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1265, 796)
        TabControl1.TabIndex = 0
        ' 
        ' TabPage1
        ' 
        TabPage1.AutoScroll = True
        TabPage1.Controls.Add(TextBox7)
        TabPage1.Controls.Add(GroupBox5)
        TabPage1.Controls.Add(GroupBox4)
        TabPage1.Controls.Add(GroupBox3)
        TabPage1.Controls.Add(GroupBox2)
        TabPage1.Controls.Add(GroupBox1)
        TabPage1.Location = New Point(4, 24)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(1257, 768)
        TabPage1.TabIndex = 0
        TabPage1.Text = "Wie exportiere ich die CSV?" & vbLf & vbLf
        TabPage1.UseVisualStyleBackColor = True
        ' 
        ' TextBox7
        ' 
        TextBox7.BackColor = Color.DarkGray
        TextBox7.Font = New Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        TextBox7.ForeColor = Color.Red
        TextBox7.Location = New Point(418, 600)
        TextBox7.Multiline = True
        TextBox7.Name = "TextBox7"
        TextBox7.ReadOnly = True
        TextBox7.Size = New Size(406, 288)
        TextBox7.TabIndex = 6
        TextBox7.Text = resources.GetString("TextBox7.Text")
        ' 
        ' GroupBox5
        ' 
        GroupBox5.Controls.Add(PictureBox5)
        GroupBox5.Controls.Add(TextBox5)
        GroupBox5.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        GroupBox5.Location = New Point(830, 583)
        GroupBox5.Name = "GroupBox5"
        GroupBox5.Size = New Size(406, 571)
        GroupBox5.TabIndex = 5
        GroupBox5.TabStop = False
        GroupBox5.Text = "Schritt 5"
        ' 
        ' PictureBox5
        ' 
        PictureBox5.Image = My.Resources.Resources.Screenshot_2025_05_30_220626
        PictureBox5.Location = New Point(6, 21)
        PictureBox5.Name = "PictureBox5"
        PictureBox5.Size = New Size(394, 147)
        PictureBox5.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox5.TabIndex = 1
        PictureBox5.TabStop = False
        ' 
        ' TextBox5
        ' 
        TextBox5.BorderStyle = BorderStyle.FixedSingle
        TextBox5.Location = New Point(6, 174)
        TextBox5.Multiline = True
        TextBox5.Name = "TextBox5"
        TextBox5.ReadOnly = True
        TextBox5.Size = New Size(391, 391)
        TextBox5.TabIndex = 0
        TextBox5.Text = "Öffne dein Programm und wähle im Menü Datei → Importieren → CSV aus, um die exportierte CSV-Datei zu laden." & vbLf & vbLf
        ' 
        ' GroupBox4
        ' 
        GroupBox4.Controls.Add(PictureBox4)
        GroupBox4.Controls.Add(TextBox4)
        GroupBox4.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        GroupBox4.Location = New Point(6, 583)
        GroupBox4.Name = "GroupBox4"
        GroupBox4.Size = New Size(406, 571)
        GroupBox4.TabIndex = 4
        GroupBox4.TabStop = False
        GroupBox4.Text = "Schritt 4"
        ' 
        ' PictureBox4
        ' 
        PictureBox4.Image = My.Resources.Resources.Screenshot_20250530_213851_pds_Mitarbeiter
        PictureBox4.Location = New Point(6, 21)
        PictureBox4.Name = "PictureBox4"
        PictureBox4.Size = New Size(255, 550)
        PictureBox4.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox4.TabIndex = 1
        PictureBox4.TabStop = False
        ' 
        ' TextBox4
        ' 
        TextBox4.BorderStyle = BorderStyle.FixedSingle
        TextBox4.Location = New Point(267, 21)
        TextBox4.Multiline = True
        TextBox4.Name = "TextBox4"
        TextBox4.ReadOnly = True
        TextBox4.Size = New Size(130, 550)
        TextBox4.TabIndex = 0
        TextBox4.Text = "Wähle als Zeitraum immer vom 1. des Monats bis zum letzten Tag des Monats (siehe rote Pfeile im Screenshot)." & vbLf & "Tippe danach auf „CSV-Datei exportieren" & ChrW(8220) & "."
        ' 
        ' GroupBox3
        ' 
        GroupBox3.Controls.Add(PictureBox3)
        GroupBox3.Controls.Add(TextBox3)
        GroupBox3.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        GroupBox3.Location = New Point(830, 6)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(406, 571)
        GroupBox3.TabIndex = 3
        GroupBox3.TabStop = False
        GroupBox3.Text = "Schritt 3"
        ' 
        ' PictureBox3
        ' 
        PictureBox3.Image = My.Resources.Resources.Screenshot_20250530_213842_pds_Mitarbeiter
        PictureBox3.Location = New Point(6, 21)
        PictureBox3.Name = "PictureBox3"
        PictureBox3.Size = New Size(255, 550)
        PictureBox3.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox3.TabIndex = 1
        PictureBox3.TabStop = False
        ' 
        ' TextBox3
        ' 
        TextBox3.BorderStyle = BorderStyle.FixedSingle
        TextBox3.Location = New Point(267, 21)
        TextBox3.Multiline = True
        TextBox3.Name = "TextBox3"
        TextBox3.ReadOnly = True
        TextBox3.Size = New Size(130, 550)
        TextBox3.TabIndex = 0
        TextBox3.Text = "Tippe oben rechts auf das Export-Symbol."
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(PictureBox2)
        GroupBox2.Controls.Add(TextBox2)
        GroupBox2.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        GroupBox2.Location = New Point(418, 6)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(406, 571)
        GroupBox2.TabIndex = 2
        GroupBox2.TabStop = False
        GroupBox2.Text = "Schritt 2"
        ' 
        ' PictureBox2
        ' 
        PictureBox2.Image = My.Resources.Resources.Screenshot_20250530_213834_pds_Mitarbeiter
        PictureBox2.Location = New Point(6, 21)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(255, 550)
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox2.TabIndex = 1
        PictureBox2.TabStop = False
        ' 
        ' TextBox2
        ' 
        TextBox2.BorderStyle = BorderStyle.FixedSingle
        TextBox2.Location = New Point(267, 21)
        TextBox2.Multiline = True
        TextBox2.Name = "TextBox2"
        TextBox2.ReadOnly = True
        TextBox2.Size = New Size(130, 550)
        TextBox2.TabIndex = 0
        TextBox2.Text = "Tippe auf „Protokoll" & ChrW(8220) & " im Bereich „Zeit" & ChrW(8220)
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(PictureBox1)
        GroupBox1.Controls.Add(TextBox1)
        GroupBox1.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        GroupBox1.Location = New Point(6, 6)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(406, 571)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Schritt 1"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = My.Resources.Resources.Screenshot_20250530_213812_pds_Mitarbeiter
        PictureBox1.Location = New Point(6, 21)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(255, 550)
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.TabIndex = 1
        PictureBox1.TabStop = False
        ' 
        ' TextBox1
        ' 
        TextBox1.BorderStyle = BorderStyle.FixedSingle
        TextBox1.Location = New Point(267, 21)
        TextBox1.Multiline = True
        TextBox1.Name = "TextBox1"
        TextBox1.ReadOnly = True
        TextBox1.Size = New Size(130, 550)
        TextBox1.TabIndex = 0
        TextBox1.Text = "Tippe unten im Menü auf „Zeit" & ChrW(8220) & "."
        ' 
        ' TabPage2
        ' 
        TabPage2.Controls.Add(TextBox6)
        TabPage2.Controls.Add(dgvAnleitung)
        TabPage2.Location = New Point(4, 24)
        TabPage2.Name = "TabPage2"
        TabPage2.Padding = New Padding(3)
        TabPage2.Size = New Size(1257, 768)
        TabPage2.TabIndex = 1
        TabPage2.Text = "Wie genau muss ich stempeln?" & vbLf & vbLf
        TabPage2.UseVisualStyleBackColor = True
        ' 
        ' dgvAnleitung
        ' 
        dgvAnleitung.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvAnleitung.Location = New Point(6, 6)
        dgvAnleitung.Name = "dgvAnleitung"
        dgvAnleitung.Size = New Size(1243, 144)
        dgvAnleitung.TabIndex = 0
        ' 
        ' TabPage3
        ' 
        TabPage3.Location = New Point(4, 24)
        TabPage3.Name = "TabPage3"
        TabPage3.Size = New Size(1257, 768)
        TabPage3.TabIndex = 2
        TabPage3.Text = "Was muss ich machen, wenn ich einmal vergessen habe zu stempeln und die Zeit nicht stimmt?" & vbLf & vbLf
        TabPage3.UseVisualStyleBackColor = True
        ' 
        ' TextBox6
        ' 
        TextBox6.Font = New Font("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBox6.Location = New Point(8, 156)
        TextBox6.Multiline = True
        TextBox6.Name = "TextBox6"
        TextBox6.ReadOnly = True
        TextBox6.Size = New Size(1241, 609)
        TextBox6.TabIndex = 1
        TextBox6.Text = resources.GetString("TextBox6.Text")
        TextBox6.TextAlign = HorizontalAlignment.Center
        ' 
        ' Infos
        ' 
        ClientSize = New Size(1265, 796)
        Controls.Add(TabControl1)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "Infos"
        Text = "Infos"
        TabControl1.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage1.PerformLayout()
        GroupBox5.ResumeLayout(False)
        GroupBox5.PerformLayout()
        CType(PictureBox5, ComponentModel.ISupportInitialize).EndInit()
        GroupBox4.ResumeLayout(False)
        GroupBox4.PerformLayout()
        CType(PictureBox4, ComponentModel.ISupportInitialize).EndInit()
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        CType(PictureBox3, ComponentModel.ISupportInitialize).EndInit()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        TabPage2.ResumeLayout(False)
        TabPage2.PerformLayout()
        CType(dgvAnleitung, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Private Sub Infos_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim t As New DataTable()
        t.Columns.Add("Reihenfolge", GetType(Integer))
        t.Columns.Add("Buchung", GetType(String))
        t.Columns.Add("Aktion / Was du tust", GetType(String))
        t.Columns.Add("Beispiel", GetType(String))

        t.Rows.Add(1, "Anfahrt",
                   "Von der Firma losfahren (Material eingeladen)",
                   "07:00 – Material laden, losfahren")

        t.Rows.Add(2, "Arbeitsbeginn",
                   "Auf der Baustelle ankommen und anfangen",
                   "07:15 – erste Arbeiten")

        t.Rows.Add(3, "Automatische Pause",
                   "09 Uhr → 0,25 h · 12 Uhr → 0,50 h (automatisch, nichts buchen)",
                   "Wird immer Automatisch gebucht nicht umgehbar")

        t.Rows.Add(4, "Abfahrt",
                   "Baustelle verlassen, zurück zur Firma (o. neue Baustelle)",
                   "16:00 – Werkzeug einladen, losfahren")

        t.Rows.Add(5, "Arbeitsende",
                   "In der Firma ausladen, Feierabend (unbedingt buchen!)",
                   "16:40 – Bulli abstellen, Feierabend")

        ' === DataGridView binden ==================================================
        With dgvAnleitung
            .DataSource = t
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .RowHeadersVisible = False
            ' Optional: Zeilenhöhe für Mehrzeiligkeit
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        End With
    End Sub

    Friend WithEvents dgvAnleitung As DataGridView
    Friend WithEvents TextBox6 As TextBox

    'Designer Code kommt hier automatisch rein, wenn du die Form im Designer bearbeitest
End Class