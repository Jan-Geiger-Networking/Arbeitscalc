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
        If dgvTagesdaten.Columns(e.ColumnIndex).HeaderText = "Pausenzeit (h)" Then
            Dim row = dgvTagesdaten.Rows(e.RowIndex)
            Dim bereich = row.Cells("Arbeitszeit-Zeitraum").Value?.ToString()
            Dim pausenStr = row.Cells("Pausenzeit (h)").Value?.ToString()
            Dim neueArbeitszeit As Double = 0

            If Not String.IsNullOrWhiteSpace(bereich) AndAlso bereich.Contains("–") Then
                Dim teile = bereich.Split("–"c)
                Dim t1, t2 As DateTime
                If DateTime.TryParseExact(teile(0).Trim(), "HH:mm", Nothing, Globalization.DateTimeStyles.None, t1) AndAlso
                   DateTime.TryParseExact(teile(1).Trim(), "HH:mm", Nothing, Globalization.DateTimeStyles.None, t2) Then
                    Dim pausen As Double = 0
                    Double.TryParse(pausenStr.Replace(",", "."), Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, pausen)
                    neueArbeitszeit = (t2 - t1).TotalMinutes / 60 - pausen
                    If neueArbeitszeit < 0 Then neueArbeitszeit = 0
                    row.Cells("Arbeitszeit (h)").Value = neueArbeitszeit.ToString("0.00")
                End If
            End If

            ' Überstunden und Vergütung ebenfalls neu berechnen
            AktualisiereTageszeile(row)
            AktualisiereSummen()
        End If
    End Sub

    Private Sub AktualisiereTageszeile(row As DataGridViewRow)
        Try
            Dim tagName = row.Cells("Tag").Value?.ToString()
            Dim pausenzeitStr = row.Cells("Pausenzeit (h)").Value?.ToString()
            Dim arbeitszeitStr = row.Cells("Arbeitszeit (h)").Value?.ToString()

            Dim arbeitszeit As Double = 0
            Dim pausenzeit As Double = 0
            Double.TryParse(arbeitszeitStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, arbeitszeit)
            Double.TryParse(pausenzeitStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, pausenzeit)
            Dim soll As Double = If(tagName.ToLower() = "freitag", 6, 8)
            Dim uebersoll As Double = 0
            Dim ueb25 As Double = 0
            Dim ueb50 As Double = 0
            If arbeitszeit > soll Then
                uebersoll = arbeitszeit - soll
                ueb25 = Math.Min(2, uebersoll)
                ueb50 = Math.Max(0, uebersoll - 2)
            Else
                uebersoll = 0
                ueb25 = 0
                ueb50 = 0
            End If

            Dim vergütet As Double
            If arbeitszeit <= soll Then
                vergütet = arbeitszeit
            Else
                vergütet = soll + ueb25 * 1.25 + ueb50 * 1.5
            End If

            row.Cells("Überstunden (h)").Value = uebersoll.ToString("0.00")
            row.Cells("Überstd. 25% (h)").Value = ueb25.ToString("0.00")
            row.Cells("Überstd. 50% (h)").Value = ueb50.ToString("0.00")
            row.Cells("Vergütete Arbeitszeit (h)").Value = vergütet.ToString("0.00")
        Catch ex As Exception
            ' Fehlerbehandlung
        End Try
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
                ' Neu: Vergütete Arbeitszeit (h) summieren!
                Dim vergZeitStr = row.Cells("Vergütete Arbeitszeit (h)").Value?.ToString()
                Dim fahrzeitStr = row.Cells("Fahrzeit (h)").Value?.ToString()

                If DateTime.TryParseExact(datumStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, Nothing) Then
                    Dim datum = DateTime.ParseExact(datumStr, "dd.MM.yyyy", CultureInfo.InvariantCulture)
                    Dim jahr = datum.Year
                    Dim monat = datum.ToString("MMMM yyyy", New CultureInfo("de-DE"))
                    Dim kw = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datum, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)

                    Dim aVal = If(IsNumeric(vergZeitStr), Convert.ToDouble(vergZeitStr), 0.0)
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
        tagesdaten.Columns.Add("Pausenzeit (h)")
        tagesdaten.Columns.Add("Arbeitszeit (h)")
        tagesdaten.Columns.Add("Fahrzeit (h)")
        tagesdaten.Columns.Add("Überstunden (h)")
        tagesdaten.Columns.Add("Überstd. 25% (h)")
        tagesdaten.Columns.Add("Überstd. 50% (h)")
        tagesdaten.Columns.Add("Vergütete Arbeitszeit (h)")
        tagesdaten.Columns.Add("Datenintegrität")

        ' Blöcke bauen: Jeder Block = Arbeitsbeginn...Arbeitsende/Abfahrt/Arbeitsbeginn (nächster)
        Dim blocks As New List(Of (Datum As Date, TagName As String, Baustelle As String, Bemerkung As String, Anfahrt As (Von As DateTime, Bis As DateTime)?, Arbeitszeit As (Von As DateTime, Bis As DateTime), Abfahrt As (Von As DateTime, Bis As DateTime)?))
        For i = 0 To sortedEntries.Count - 1
            If sortedEntries(i).Item2 = "Arbeitsbeginn" Then
                Dim baustelle = sortedEntries(i).Item3
                Dim bemerkung = sortedEntries(i).Item4
                Dim datum = sortedEntries(i).Item1.Date
                Dim arbeitsStart = sortedEntries(i).Item1

                ' Suche passende Anfahrt (sofort davor, gleiche Baustelle)
                Dim anfahrt As (Von As DateTime, Bis As DateTime)? = Nothing
                If i > 0 AndAlso sortedEntries(i - 1).Item2 = "Anfahrt" AndAlso sortedEntries(i - 1).Item3 = baustelle Then
                    anfahrt = (sortedEntries(i - 1).Item1, sortedEntries(i).Item1)
                End If

                ' Arbeitsende: nächster "Abfahrt" oder "Arbeitsende" oder "Arbeitsbeginn" einer anderen Baustelle
                Dim arbeitsEnde = arbeitsStart
                Dim abfahrt As (Von As DateTime, Bis As DateTime)? = Nothing
                Dim j = i + 1
                While j < sortedEntries.Count
                    If sortedEntries(j).Item2 = "Abfahrt" AndAlso sortedEntries(j).Item3 = baustelle Then
                        arbeitsEnde = sortedEntries(j).Item1
                        ' Guck, ob danach (sofort!) ein Arbeitsende kommt (selbe Baustelle)
                        If j + 1 < sortedEntries.Count AndAlso sortedEntries(j + 1).Item2 = "Arbeitsende" AndAlso sortedEntries(j + 1).Item3 = baustelle Then
                            abfahrt = (sortedEntries(j).Item1, sortedEntries(j + 1).Item1)
                        Else
                            ' Suche die nächste Arbeitsbeginn-Zeit (egal welche Baustelle) nach der Abfahrt
                            Dim abfahrtBis As DateTime = sortedEntries(j).Item1
                            For k = j + 1 To sortedEntries.Count - 1
                                If sortedEntries(k).Item2 = "Arbeitsbeginn" Then
                                    abfahrtBis = sortedEntries(k).Item1
                                    Exit For
                                End If
                            Next
                            abfahrt = (sortedEntries(j).Item1, abfahrtBis)
                        End If
                        Exit While
                    ElseIf sortedEntries(j).Item2 = "Arbeitsbeginn" Then
                        arbeitsEnde = sortedEntries(j).Item1
                        Exit While
                    ElseIf sortedEntries(j).Item2 = "Arbeitsende" AndAlso sortedEntries(j).Item3 = baustelle Then
                        arbeitsEnde = sortedEntries(j).Item1
                        Exit While
                    End If
                    j += 1
                End While

                blocks.Add((datum, datum.ToString("dddd", New CultureInfo("de-DE")), baustelle, bemerkung, anfahrt, (arbeitsStart, arbeitsEnde), abfahrt))
            End If
        Next

        ' Bemerkungen pro Tag sammeln
        Dim bemerkungenProTag As New Dictionary(Of Date, String)
        For Each e In sortedEntries
            If Not String.IsNullOrWhiteSpace(e.Item4) Then
                Dim d = e.Item1.Date
                If Not bemerkungenProTag.ContainsKey(d) Then
                    bemerkungenProTag(d) = e.Item4
                ElseIf Not bemerkungenProTag(d).Contains(e.Item4) Then
                    bemerkungenProTag(d) &= ", " & e.Item4
                End If
            End If
        Next

        ' Nach Tagen gruppieren, Überstunden aufteilen, Pausenregel: Nur erster Block bekommt Pause
        For Each taggruppe In blocks.GroupBy(Function(b) b.Datum).OrderBy(Function(g) g.Key)
            Dim blocksThisDay = taggruppe.ToList()
            Dim tagName = blocksThisDay.First().TagName
            Dim datum = blocksThisDay.First().Datum
            Dim soll = If(tagName.ToLower() = "freitag", 6.0, 8.0)
            Dim bemerkungenText As String = If(bemerkungenProTag.ContainsKey(datum), bemerkungenProTag(datum), "")

            Dim pausenList As New List(Of Double)
            For idx = 0 To blocksThisDay.Count - 1
                pausenList.Add(If(idx = 0, If(tagName.ToLower() = "freitag", 0.25, 0.75), 0))
            Next

            ' Alle Arbeitszeiten und Fahrzeiten für Überstundenverteilung
            Dim arbeitszeiten = blocksThisDay.Select(Function(b, idx) Math.Max(0, (b.Arbeitszeit.Bis - b.Arbeitszeit.Von).TotalMinutes / 60 - pausenList(idx))).ToList()
            Dim fahrzeiten = blocksThisDay.Select(Function(b)
                                                      Dim ft As Double = 0
                                                      If b.Anfahrt.HasValue Then ft += (b.Anfahrt.Value.Bis - b.Anfahrt.Value.Von).TotalMinutes / 60
                                                      If b.Abfahrt.HasValue Then ft += (b.Abfahrt.Value.Bis - b.Abfahrt.Value.Von).TotalMinutes / 60
                                                      Return ft
                                                  End Function).ToList()
            Dim sumArbeitszeit = arbeitszeiten.Sum()
            Dim sumFahrzeit = fahrzeiten.Sum()
            Dim uebersoll = Math.Max(0, sumArbeitszeit - soll)
            Dim ueb25 = Math.Min(2, uebersoll)
            Dim ueb50 = Math.Max(0, uebersoll - 2)
            Dim verguetung = Math.Min(sumArbeitszeit, soll) + ueb25 * 1.25 + ueb50 * 1.5

            For idx = 0 To blocksThisDay.Count - 1
                Dim b = blocksThisDay(idx)
                Dim arbeitszeit = arbeitszeiten(idx)
                Dim fahrzeit = fahrzeiten(idx)
                Dim pause = pausenList(idx)
                Dim anteil = If(sumArbeitszeit > 0, arbeitszeit / sumArbeitszeit, 0)
                Dim ueber = uebersoll * anteil
                Dim u25 = ueb25 * anteil
                Dim u50 = ueb50 * anteil
                Dim verg = verguetung * anteil

                ' Korrekte Fahrzeit-Zeiträume: Nur was zu diesem Block gehört!
                Dim fahrzeitStr As String = ""
                If b.Anfahrt.HasValue Then
                    fahrzeitStr = $"{b.Anfahrt.Value.Von:HH:mm}–{b.Anfahrt.Value.Bis:HH:mm}"
                End If
                If b.Abfahrt.HasValue Then
                    If fahrzeitStr <> "" Then fahrzeitStr &= ", "
                    fahrzeitStr &= $"{b.Abfahrt.Value.Von:HH:mm}–{b.Abfahrt.Value.Bis:HH:mm}"
                End If

                tagesdaten.Rows.Add(
            tagName,
            datum.ToString("dd.MM.yyyy"),
            b.Baustelle,
            bemerkungenText,
            fahrzeitStr,
            $"{b.Arbeitszeit.Von:HH:mm}–{b.Arbeitszeit.Bis:HH:mm}",
            pause.ToString("0.00"),
            arbeitszeit.ToString("0.00"),
            fahrzeit.ToString("0.00"),
            ueber.ToString("0.00"),
            u25.ToString("0.00"),
            u50.ToString("0.00"),
            verg.ToString("0.00"),
            "OK"
        )
            Next
        Next

        dgvTagesdaten.DataSource = tagesdaten
        dgvTagesdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvTagesdaten.ReadOnly = False
        dgvTagesdaten.Columns("Pausenzeit (h)").ReadOnly = False

        AktualisiereSummen()
    End Sub




    ' ===== Menü- und Export/Import =======================================

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

    Private Sub JGNSchlaubereichToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JGNSchlaubereichToolStripMenuItem.Click
        Process.Start(New ProcessStartInfo("https://jgnet.eu/schlau") With {.UseShellExecute = True})
    End Sub

    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files.Length > 0 Then
            Dim filePath = files(0)
            If filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                ImportiereCSV(filePath)
            ElseIf filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) Then
                ' TXT-Import analog wie im Menü
                Dim lines = System.IO.File.ReadAllLines(filePath)
                ' (Hier ggf. eigenen TXT-Import aufrufen, falls du willst)
            Else
                MessageBox.Show("Nur CSV oder TXT Dateien können importiert werden!")
            End If
        End If
    End Sub

    Private Sub dgvTagesdaten_DragEnter(sender As Object, e As DragEventArgs) Handles dgvTagesdaten.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub dgvTagesdaten_DragDrop(sender As Object, e As DragEventArgs) Handles dgvTagesdaten.DragDrop
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files.Length > 0 Then
            Dim filePath = files(0)
            If filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                ImportiereCSV(filePath)
            ElseIf filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) Then
                ' TXT-Import analog wie im Menü
                Dim lines = System.IO.File.ReadAllLines(filePath)
                ' (Hier ggf. eigenen TXT-Import aufrufen)
            Else
                MessageBox.Show("Nur CSV oder TXT Dateien können importiert werden!")
            End If
        End If
    End Sub

    Private Sub dgvSummen_DragEnter(sender As Object, e As DragEventArgs) Handles dgvTagesdaten.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub dgvSummen_DragDrop(sender As Object, e As DragEventArgs) Handles dgvTagesdaten.DragDrop
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files.Length > 0 Then
            Dim filePath = files(0)
            If filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                ImportiereCSV(filePath)
            ElseIf filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) Then
                ' TXT-Import analog wie im Menü
                Dim lines = System.IO.File.ReadAllLines(filePath)
                ' (Hier ggf. eigenen TXT-Import aufrufen)
            Else
                MessageBox.Show("Nur CSV oder TXT Dateien können importiert werden!")
            End If
        End If
    End Sub

    Private Sub SupportUndBugReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SupportUndBugReportToolStripMenuItem.Click
        Process.Start(New ProcessStartInfo("https://jgnet.eu/arbeitscalc-support") With {.UseShellExecute = True})
    End Sub
End Class
