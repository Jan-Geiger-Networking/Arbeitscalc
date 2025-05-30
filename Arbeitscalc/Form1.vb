Imports System.Globalization
Imports System.Text.RegularExpressions
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class Form1
    Private geladeneDatei As String = ""
    Private watcher As FileSystemWatcher

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
                      ÖffnenToolStripMenuItem.PerformClick()
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

            ' Tagesdaten
            doc.Add(New Paragraph("Tagesdaten", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
            Dim table1 As New PdfPTable(dgvTagesdaten.Columns.Count)
            table1.WidthPercentage = 100
            For Each col As DataGridViewColumn In dgvTagesdaten.Columns
                table1.AddCell(New Phrase(col.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
            Next
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

    Private Sub ÖffnenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ÖffnenToolStripMenuItem.Click
        openFileDialog1.Filter = "Textdateien (*.txt)|*.txt"
        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim lines As String() = System.IO.File.ReadAllLines(openFileDialog1.FileName)
            Dim entries As New List(Of Tuple(Of DateTime, String, String))() ' Datum, Typ, Baustelle


            Dim regex As New Regex("(?<datum>\d{2}\.\d{2}\.\d{4})(?<zeit>\d{2}:\d{2})(?<typ>Anfahrt|Arbeitsbeginn|Abfahrt|Arbeitsende)(?<ort>.+)")
            For Each line In lines.Distinct()
                Dim match = regex.Match(line)
                If match.Success Then
                    Try
                        Dim datum = DateTime.ParseExact(match.Groups("datum").Value & match.Groups("zeit").Value, "dd.MM.yyyyHH:mm", CultureInfo.InvariantCulture)
                        Dim typ = match.Groups("typ").Value
                        Dim ort = match.Groups("ort").Value.Trim()
                        entries.Add(Tuple.Create(datum, typ, ort))
                    Catch ex As Exception
                        Debug.WriteLine("Fehler beim Parsen: " & line)
                    End Try
                End If
            Next


            ' Alle Einträge sortiert durchgehen (Datum + Zeit)
            Dim sortedEntries = entries.OrderBy(Function(x) x.Item1).ToList()
            Dim tagesdaten As New DataTable()
            tagesdaten.Columns.Add("Tag")
            tagesdaten.Columns.Add("Datum")
            tagesdaten.Columns.Add("Baustelle")
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
                    Dim startFahrzeit As DateTime? = Nothing
                    Dim arbeitsbeginn As DateTime? = Nothing

                    ' Blockstart bestimmen
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

                    ' Blockende bestimmen
                    Dim blockEnd As DateTime? = Nothing
                    Dim abfahrtZeit As DateTime? = Nothing
                    Dim abfahrtGefunden As Boolean = False
                    Dim arbeitsendeZeit As DateTime? = Nothing
                    Dim blockEndIndex = i + 1
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

                    ' --- Arbeitszeit berechnen ---
                    Dim arbeitszeitBereich As String = ""
                    Dim arbeitszeit As Double = 0
                    If arbeitsbeginn.HasValue AndAlso blockEnd.HasValue Then
                        arbeitszeitBereich = $"{arbeitsbeginn.Value:HH:mm}–{blockEnd.Value:HH:mm}"
                        arbeitszeit = (blockEnd.Value - arbeitsbeginn.Value).TotalMinutes / 60 - 0.75
                        If arbeitszeit < 0 Then arbeitszeit = 0
                    End If

                    ' --- Fahrzeit hin berechnen ---
                    Dim fahrzeitBereich As String = ""
                    Dim fahrzeitGesamt As Double = 0
                    If startFahrzeit.HasValue AndAlso arbeitsbeginn.HasValue Then
                        fahrzeitBereich = $"{startFahrzeit.Value:HH:mm}–{arbeitsbeginn.Value:HH:mm}"
                        fahrzeitGesamt += (arbeitsbeginn.Value - startFahrzeit.Value).TotalMinutes / 60
                    End If

                    ' --- Fahrzeit zurück berechnen (NUR wenn ein Abfahrt gefunden wurde) ---
                    ' --- Fahrzeit zurück berechnen (NUR wenn ein Abfahrt gefunden wurde) ---
                    If abfahrtGefunden Then
                        ' Erstes: Suche Arbeitsende auf GLEICHER Baustelle und GLEICHEM TAG
                        Dim fahrzeitRueckStart = abfahrtZeit.Value
                        Dim fahrzeitRueckEnde As DateTime? = Nothing
                        For k = blockEndIndex + 1 To sortedEntries.Count - 1
                            If sortedEntries(k).Item2 = "Arbeitsende" AndAlso sortedEntries(k).Item3 = baustelle AndAlso sortedEntries(k).Item1.Date = datum Then
                                fahrzeitRueckEnde = sortedEntries(k).Item1
                                Exit For
                            End If
                        Next
                        ' Falls nichts gefunden, fallback auf nächsten Arbeitsbeginn (wie bisher)
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


                    ' --- Status ---
                    Dim status As String = "OK"
                    If Not arbeitsbeginn.HasValue Or Not blockEnd.HasValue Then
                        status = "FEHLERHAFT"
                    End If

                    ' --- Zeile hinzufügen ---
                    Dim tagName = datum.ToString("dddd", New CultureInfo("de-DE"))
                    Dim datumStr = datum.ToString("dd.MM.yyyy")
                    tagesdaten.Rows.Add(tagName, datumStr, baustelle, fahrzeitBereich, arbeitszeitBereich, arbeitszeit.ToString("0.00"), fahrzeitGesamt.ToString("0.00"), status)

                    ' Weiter hinter Block springen
                    i = blockEndIndex + 1
                Else
                    i += 1
                End If
            End While


            dgvTagesdaten.DataSource = tagesdaten
            dgvTagesdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvTagesdaten.ReadOnly = False

            AktualisiereSummen()
            geladeneDatei = openFileDialog1.FileName
            StarteFileWatcher(geladeneDatei)
        End If
    End Sub

    Private Sub ExportierenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportierenToolStripMenuItem.Click
        Dim auswahl = MessageBox.Show("Wähle das Exportformat:" & vbCrLf & vbCrLf &
                           "Ja = CSV" & vbCrLf & "Nein = PDF",
                           "Exportoptionen", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

        If auswahl = DialogResult.Yes Then
            ExportiereCSV()
        ElseIf auswahl = DialogResult.No Then
            ExportierePDF()
        End If
    End Sub

    Private Sub InfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InfoToolStripMenuItem.Click
        Dim about As New About()
        about.ShowDialog()
    End Sub

    Private Sub RechtlichesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RechtlichesToolStripMenuItem.Click
        Dim about As New Rechtliches()
        about.ShowDialog()
    End Sub

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class