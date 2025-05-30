Imports System.Globalization
Imports System.Text.RegularExpressions
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.Net.Http
Imports System.Text.Json
Imports System.Diagnostics

Public Class Form1
    Private geladeneDatei As String = ""
    Private watcher As FileSystemWatcher

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForUpdateAsync()
    End Sub

    Private Sub ImportiereCSV(pfad As String)
        Dim entries As New List(Of Tuple(Of DateTime, String, String, String))()
        Using reader As New StreamReader(pfad, System.Text.Encoding.UTF8)
            Dim headerRead As Boolean = False
            While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()
                If Not headerRead Then
                    headerRead = True
                    Continue While
                End If
                If String.IsNullOrWhiteSpace(line) Then Continue While
                Dim columns = line.Split(";"c)
                If columns.Length >= 13 Then
                    Try
                        Dim datumUhrzeit = columns(0).Trim() & " " & columns(1).Trim()
                        Dim datum As DateTime = DateTime.ParseExact(datumUhrzeit, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        Dim buchungstyp = columns(2).Trim().ToLower()
                        Dim meldungstext = columns(3).Trim().ToLower()
                        Dim baustelle = columns(8).Trim()
                        Dim bemerkung = columns(12).Trim()
                        Dim typ As String = ""
                        If buchungstyp = "fahrt" And meldungstext = "anfahrt" Then
                            typ = "Anfahrt"
                        ElseIf buchungstyp = "beginn" And meldungstext = "arbeitsbeginn" Then
                            typ = "Arbeitsbeginn"
                        ElseIf buchungstyp = "fahrt" And meldungstext = "abfahrt" Then
                            typ = "Abfahrt"
                        ElseIf buchungstyp = "ende" And meldungstext = "arbeitsende" Then
                            typ = "Arbeitsende"
                        End If
                        If typ <> "" Then
                            entries.Add(Tuple.Create(datum, typ, baustelle, bemerkung))
                        End If
                    Catch ex As Exception
                        Debug.WriteLine("Fehler beim Parsen: " & ex.Message)
                    End Try
                End If
            End While
        End Using
        EintraegeZuTabelleMitBemerkung(entries)
        geladeneDatei = pfad
        StarteFileWatcher(geladeneDatei)
    End Sub

    ' =====================================================================

    Private Sub StarteFileWatcher(pfad As String)
        If watcher IsNot Nothing Then
            watcher.EnableRaisingEvents = False
            watcher.Dispose()
        End If

        watcher = New FileSystemWatcher()
        watcher.Path = System.IO.Path.GetDirectoryName(pfad)
        watcher.Filter = System.IO.Path.GetFileName(pfad)
        watcher.NotifyFilter = NotifyFilters.LastWrite

        AddHandler watcher.Changed, AddressOf DateiGeaendert
        watcher.EnableRaisingEvents = True
    End Sub

    Private Sub DateiGeaendert(sender As Object, e As FileSystemEventArgs)
        Threading.Thread.Sleep(500)
        Me.Invoke(Sub()
                      ImportStripMenuItem.PerformClick()
                  End Sub)
    End Sub

    Private Sub dgvTagesdaten_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTagesdaten.CellValueChanged
        AktualisiereSummen()
    End Sub

    Private Sub AktualisiereSummen()
        Dim summenTabelle As New DataTable()
        summenTabelle.Columns.Add("Zeitraumtyp")
        summenTabelle.Columns.Add("Zeitraum")
        summenTabelle.Columns.Add("Arbeitszeit (h)")
        summenTabelle.Columns.Add("Fahrzeit (h)")

        Dim summenProWoche As New Dictionary(Of Integer, Tuple(Of Double, Double))()
        Dim summenProMonat As New Dictionary(Of String, Tuple(Of Double, Double))()
        Dim summenProJahr As New Dictionary(Of Integer, Tuple(Of Double, Double))()

        For Each row As DataGridViewRow In dgvTagesdaten.Rows
            If Not row.IsNewRow Then
                Dim datumStr = row.Cells("Datum").Value?.ToString()
                Dim arbeitszeitStr = row.Cells("Arbeitszeit (h)").Value?.ToString()
                Dim fahrzeitStr = row.Cells("Fahrzeit (h)").Value?.ToString()

                If DateTime.TryParseExact(datumStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, Nothing) Then
                    Dim datum = DateTime.ParseExact(datumStr, "dd.MM.yyyy", CultureInfo.InvariantCulture)
                    Dim jahr = datum.Year
                    Dim monat = datum.ToString("MMMM yyyy", New CultureInfo("de-DE"))
                    Dim kw = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datum, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)

                    Dim aVal = If(IsNumeric(arbeitszeitStr), Convert.ToDouble(arbeitszeitStr), 0.0)
                    Dim fVal = If(IsNumeric(fahrzeitStr), Convert.ToDouble(fahrzeitStr), 0.0)

                    If Not summenProWoche.ContainsKey(kw) Then summenProWoche(kw) = Tuple.Create(0.0, 0.0)
                    summenProWoche(kw) = Tuple.Create(summenProWoche(kw).Item1 + aVal, summenProWoche(kw).Item2 + fVal)

                    If Not summenProMonat.ContainsKey(monat) Then summenProMonat(monat) = Tuple.Create(0.0, 0.0)
                    summenProMonat(monat) = Tuple.Create(summenProMonat(monat).Item1 + aVal, summenProMonat(monat).Item2 + fVal)

                    If Not summenProJahr.ContainsKey(jahr) Then summenProJahr(jahr) = Tuple.Create(0.0, 0.0)
                    summenProJahr(jahr) = Tuple.Create(summenProJahr(jahr).Item1 + aVal, summenProJahr(jahr).Item2 + fVal)
                End If
            End If
        Next

        For Each eintrag In summenProWoche
            summenTabelle.Rows.Add("Woche", $"KW {eintrag.Key}", Math.Round(eintrag.Value.Item1, 2).ToString("0.00"), Math.Round(eintrag.Value.Item2, 2).ToString("0.00"))
        Next
        For Each eintrag In summenProMonat
            summenTabelle.Rows.Add("Monat", eintrag.Key, Math.Round(eintrag.Value.Item1, 2).ToString("0.00"), Math.Round(eintrag.Value.Item2, 2).ToString("0.00"))
        Next
        For Each eintrag In summenProJahr
            summenTabelle.Rows.Add("Jahr", eintrag.Key.ToString(), Math.Round(eintrag.Value.Item1, 2).ToString("0.00"), Math.Round(eintrag.Value.Item2, 2).ToString("0.00"))
        Next

        dgvSummen.DataSource = summenTabelle
        dgvSummen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Sub EintraegeZuTabelleMitBemerkung(entries As List(Of Tuple(Of DateTime, String, String, String)))
        Dim sortedEntries = entries.OrderBy(Function(x) x.Item1).ToList()
        Dim tagesdaten As New DataTable()
        tagesdaten.Columns.Add("Tag")
        tagesdaten.Columns.Add("Datum")
        tagesdaten.Columns.Add("Baustelle")
        tagesdaten.Columns.Add("Bemerkung")
        tagesdaten.Columns.Add("Fahrzeit-Zeitraum")
        tagesdaten.Columns.Add("Arbeitszeit-Zeitraum")
        tagesdaten.Columns.Add("Arbeitszeit (h)")
        tagesdaten.Columns.Add("Fahrzeit (h)")
        tagesdaten.Columns.Add("Datenintegrität")

        Dim i As Integer = 0
        While i < sortedEntries.Count
            If sortedEntries(i).Item2 = "Anfahrt" Or sortedEntries(i).Item2 = "Arbeitsbeginn" Then
                Dim baustelle = sortedEntries(i).Item3
                Dim datum = sortedEntries(i).Item1.Date

                ' Alle Bemerkungen für diesen Block sammeln:
                Dim bemerkungenList As New List(Of String)
                Dim blockEndIndex = i
                For j = i To sortedEntries.Count - 1
                    If sortedEntries(j).Item1.Date <> datum OrElse sortedEntries(j).Item3 <> baustelle Then
                        Exit For
                    End If
                    Dim bemerkung = sortedEntries(j).Item4
                    If Not String.IsNullOrWhiteSpace(bemerkung) AndAlso Not bemerkungenList.Contains(bemerkung) Then
                        bemerkungenList.Add(bemerkung)
                    End If
                    blockEndIndex = j
                Next
                Dim bemerkungenText As String = String.Join(" | ", bemerkungenList)

                Dim startFahrzeit As DateTime? = Nothing
                Dim arbeitsbeginn As DateTime? = Nothing

                If sortedEntries(i).Item2 = "Anfahrt" Then
                    startFahrzeit = sortedEntries(i).Item1
                    If i + 1 < sortedEntries.Count AndAlso sortedEntries(i + 1).Item2 = "Arbeitsbeginn" AndAlso sortedEntries(i + 1).Item3 = baustelle Then
                        arbeitsbeginn = sortedEntries(i + 1).Item1
                        i += 1
                    Else
                        arbeitsbeginn = Nothing
                    End If
                Else
                    arbeitsbeginn = sortedEntries(i).Item1
                End If

                Dim blockEnd As DateTime? = Nothing
                Dim abfahrtZeit As DateTime? = Nothing
                Dim abfahrtGefunden As Boolean = False
                Dim arbeitsendeZeit As DateTime? = Nothing
                For j = i + 1 To sortedEntries.Count - 1
                    If sortedEntries(j).Item2 = "Abfahrt" AndAlso sortedEntries(j).Item3 = baustelle Then
                        abfahrtZeit = sortedEntries(j).Item1
                        abfahrtGefunden = True
                        blockEnd = abfahrtZeit
                        blockEndIndex = j
                        Exit For
                    End If
                    If (sortedEntries(j).Item2 = "Anfahrt" Or sortedEntries(j).Item2 = "Arbeitsbeginn") AndAlso sortedEntries(j).Item3 <> baustelle Then
                        blockEnd = sortedEntries(j).Item1
                        blockEndIndex = j - 1
                        Exit For
                    End If
                    If sortedEntries(j).Item2 = "Arbeitsende" AndAlso sortedEntries(j).Item3 = baustelle Then
                        arbeitsendeZeit = sortedEntries(j).Item1
                        blockEnd = arbeitsendeZeit
                        blockEndIndex = j
                        Exit For
                    End If
                Next

                Dim arbeitszeitBereich As String = ""
                Dim arbeitszeit As Double = 0
                If arbeitsbeginn.HasValue AndAlso blockEnd.HasValue Then
                    arbeitszeitBereich = $"{arbeitsbeginn.Value:HH:mm}–{blockEnd.Value:HH:mm}"
                    arbeitszeit = (blockEnd.Value - arbeitsbeginn.Value).TotalMinutes / 60 - 0.75
                    If arbeitszeit < 0 Then arbeitszeit = 0
                End If

                Dim fahrzeitBereich As String = ""
                Dim fahrzeitGesamt As Double = 0
                If startFahrzeit.HasValue AndAlso arbeitsbeginn.HasValue Then
                    fahrzeitBereich = $"{startFahrzeit.Value:HH:mm}–{arbeitsbeginn.Value:HH:mm}"
                    fahrzeitGesamt += (arbeitsbeginn.Value - startFahrzeit.Value).TotalMinutes / 60
                End If

                If abfahrtGefunden Then
                    Dim fahrzeitRueckStart = abfahrtZeit.Value
                    Dim fahrzeitRueckEnde As DateTime? = Nothing
                    For k = blockEndIndex + 1 To sortedEntries.Count - 1
                        If sortedEntries(k).Item2 = "Arbeitsende" AndAlso sortedEntries(k).Item3 = baustelle AndAlso sortedEntries(k).Item1.Date = datum Then
                            fahrzeitRueckEnde = sortedEntries(k).Item1
                            Exit For
                        End If
                    Next
                    If Not fahrzeitRueckEnde.HasValue Then
                        For k = blockEndIndex + 1 To sortedEntries.Count - 1
                            If sortedEntries(k).Item2 = "Arbeitsbeginn" Then
                                fahrzeitRueckEnde = sortedEntries(k).Item1
                                Exit For
                            End If
                        Next
                    End If
                    If fahrzeitRueckEnde.HasValue AndAlso fahrzeitRueckEnde.Value > fahrzeitRueckStart Then
                        fahrzeitBereich &= If(fahrzeitBereich <> "", ", ", "") & $"{fahrzeitRueckStart:HH:mm}–{fahrzeitRueckEnde.Value:HH:mm}"
                        fahrzeitGesamt += (fahrzeitRueckEnde.Value - fahrzeitRueckStart).TotalMinutes / 60
                    End If
                End If

                Dim status As String = "OK"
                If Not arbeitsbeginn.HasValue Or Not blockEnd.HasValue Then
                    status = "FEHLERHAFT"
                End If

                Dim tagName = datum.ToString("dddd", New CultureInfo("de-DE"))
                Dim datumStr = datum.ToString("dd.MM.yyyy")
                tagesdaten.Rows.Add(tagName, datumStr, baustelle, bemerkungenText, fahrzeitBereich, arbeitszeitBereich, arbeitszeit.ToString("0.00"), fahrzeitGesamt.ToString("0.00"), status)

                i = blockEndIndex + 1
            Else
                i += 1
            End If
        End While

        dgvTagesdaten.DataSource = tagesdaten
        dgvTagesdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvTagesdaten.ReadOnly = False

        AktualisiereSummen()
    End Sub

    ' ============================================================================

    Private Sub InfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InfoToolStripMenuItem.Click
        Dim about As New About()
        about.ShowDialog()
    End Sub

    Private Sub RechtlichesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RechtlichesToolStripMenuItem.Click
        Dim about As New Rechtliches()
        about.ShowDialog()
    End Sub

    Private Sub CSVImportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CSVImportToolStripMenuItem.Click
        openFileDialog1.Filter = "CSV-Dateien (*.csv)|*.csv"
        If openFileDialog1.ShowDialog = DialogResult.OK Then
            ImportiereCSV(openFileDialog1.FileName)
        End If
    End Sub

    Private Sub PDFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PDFToolStripMenuItem.Click
        ExportierePDF()
    End Sub

    Private Sub CSVToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CSVToolStripMenuItem.Click
        ExportiereCSV()
    End Sub
    Private Sub ExportierePDF()
        Dim monat = DateTime.Now.ToString("MMMM", New CultureInfo("de-DE"))
        Dim jahr = DateTime.Now.Year.ToString()
        Dim filename = $"Stundenbericht {monat} {jahr}.pdf"

        Dim sfd As New SaveFileDialog()
        sfd.FileName = filename
        sfd.Filter = "PDF-Datei (*.pdf)|*.pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim doc As New Document(PageSize.A4.Rotate, 20, 20, 20, 20)
            PdfWriter.GetInstance(doc, New System.IO.FileStream(sfd.FileName, System.IO.FileMode.Create))
            doc.Open()

            doc.Add(New Paragraph("Stundenbericht " & monat & " " & jahr, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)))
            doc.Add(New Paragraph("Erstellt am: " & DateTime.Now.ToString("dd.MM.yyyy")))
            doc.Add(New Paragraph(" "))

            ' --- Tagesdaten ---
            doc.Add(New Paragraph("Tagesdaten", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
            Dim table1 As New PdfPTable(dgvTagesdaten.Columns.Count)
            table1.WidthPercentage = 100
            ' Kopfzeile
            For Each col As DataGridViewColumn In dgvTagesdaten.Columns
                table1.AddCell(New Phrase(col.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
            Next
            ' Datenzeilen
            For Each row As DataGridViewRow In dgvTagesdaten.Rows
                If Not row.IsNewRow Then
                    For Each cell As DataGridViewCell In row.Cells
                        table1.AddCell(New Phrase(cell.Value?.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                    Next
                End If
            Next
            doc.Add(table1)

            doc.Add(New Paragraph(" "))
            doc.Add(New Paragraph("Summen", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
            Dim table2 As New PdfPTable(dgvSummen.Columns.Count)
            table2.WidthPercentage = 100
            For Each col As DataGridViewColumn In dgvSummen.Columns
                table2.AddCell(New Phrase(col.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
            Next
            For Each row As DataGridViewRow In dgvSummen.Rows
                If Not row.IsNewRow Then
                    For Each cell As DataGridViewCell In row.Cells
                        table2.AddCell(New Phrase(cell.Value?.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                    Next
                End If
            Next
            doc.Add(table2)

            doc.Close()
            MessageBox.Show("PDF exportiert: " & sfd.FileName)
        End If
    End Sub
    Private Sub ExportiereCSV()
        Dim monat = DateTime.Now.ToString("MMMM", New CultureInfo("de-DE"))
        Dim jahr = DateTime.Now.Year.ToString()
        Dim filename = $"Stundenbericht {monat} {jahr}.csv"

        Dim sfd As New SaveFileDialog()
        sfd.FileName = filename
        sfd.Filter = "CSV-Datei (*.csv)|*.csv"

        If sfd.ShowDialog() = DialogResult.OK Then
            Using writer As New StreamWriter(sfd.FileName, False, System.Text.Encoding.UTF8)
                ' Tagesdaten exportieren
                writer.WriteLine("Tagesdaten")
                For Each col As DataGridViewColumn In dgvTagesdaten.Columns
                    writer.Write(col.HeaderText & ";")
                Next
                writer.WriteLine()
                For Each row As DataGridViewRow In dgvTagesdaten.Rows
                    If Not row.IsNewRow Then
                        For Each cell As DataGridViewCell In row.Cells
                            writer.Write(cell.Value?.ToString().Replace(";", ",") & ";")
                        Next
                        writer.WriteLine()
                    End If
                Next

                writer.WriteLine()
                writer.WriteLine("Summen")
                For Each col As DataGridViewColumn In dgvSummen.Columns
                    writer.Write(col.HeaderText & ";")
                Next
                writer.WriteLine()
                For Each row As DataGridViewRow In dgvSummen.Rows
                    If Not row.IsNewRow Then
                        For Each cell As DataGridViewCell In row.Cells
                            writer.Write(cell.Value?.ToString().Replace(";", ",") & ";")
                        Next
                        writer.WriteLine()
                    End If
                Next
            End Using
            MessageBox.Show("CSV exportiert: " & sfd.FileName)
        End If
    End Sub

    Private Sub PDSOnlineToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PDSOnlineToolStripMenuItem1.Click
        Process.Start(New ProcessStartInfo("https://11427-01.pdscloud.de/pds/portal/") With {.UseShellExecute = True})
    End Sub

    Private Sub GithubToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GithubToolStripMenuItem.Click
        Process.Start(New ProcessStartInfo("https://github.com/Jan-Geiger-Networking/Arbeitscalc") With {.UseShellExecute = True})
    End Sub
    Private Async Sub CheckForUpdateAsync()
        Try
            Dim currentVersion As String = Application.ProductVersion
            Dim repoOwner As String = "Jan-Geiger-Networking"
            Dim repoName As String = "Arbeitscalc"
            Dim url As String = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest"

            Using client As New HttpClient()
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request")
                Dim response As HttpResponseMessage = Await client.GetAsync(url)
                If response.IsSuccessStatusCode Then
                    Dim content As String = Await response.Content.ReadAsStringAsync()
                    Using json = JsonDocument.Parse(content)
                        Dim latestTag As String = json.RootElement.GetProperty("tag_name").GetString()
                        Dim releaseUrl As String = json.RootElement.GetProperty("html_url").GetString()
                        If Not String.IsNullOrWhiteSpace(latestTag) Then
                            Debug.WriteLine("Aktuelle Version: " & currentVersion)
                            Debug.WriteLine("GitHub-Tag: " & latestTag)
                            Dim latestVersion As New System.Version(latestTag.TrimStart("v"c).Trim())
                            Dim versionString As String = currentVersion.Split("+"c)(0).Trim()
                            Dim currentVersionObj As New System.Version(versionString)

                            If latestVersion.CompareTo(currentVersionObj) > 0 Then
                                ShowUpdateDialog(releaseUrl, latestTag)
                            End If
                        End If
                    End Using
                End If
            End Using
        Catch ex As Exception
            ' Fehler ignorieren oder loggen
        End Try
    End Sub

    Private Sub ShowUpdateDialog(releaseUrl As String, latestTag As String)
        Dim result = MessageBox.Show(
        $"Es ist eine neue Version verfügbar: {latestTag}{vbCrLf}{vbCrLf}Möchtest du jetzt den Download öffnen?",
        "Update verfügbar",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Information
    )
        If result = DialogResult.Yes Then
            Process.Start(New ProcessStartInfo(releaseUrl) With {.UseShellExecute = True})
        End If
    End Sub
End Class