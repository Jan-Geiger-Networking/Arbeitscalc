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
                If sortedEntries(i).Item2 = "Anfahrt" Then
                    Dim anfahrt = sortedEntries(i)
                    Dim baustelle = anfahrt.Item3
                    Dim datum = anfahrt.Item1.Date

                    ' Suche Arbeitsbeginn
                    Dim arbeitsbeginn As Tuple(Of DateTime, String, String) = Nothing
                    If i + 1 < sortedEntries.Count AndAlso sortedEntries(i + 1).Item2 = "Arbeitsbeginn" AndAlso sortedEntries(i + 1).Item3 = baustelle Then
                        arbeitsbeginn = sortedEntries(i + 1)
                        ' Block starten
                        Dim blockStart = arbeitsbeginn.Item1
                        Dim blockEnd As DateTime? = Nothing
                        Dim status As String = "OK"
                        Dim j = i + 2
                        Dim foundAbfahrt = False

                        While j < sortedEntries.Count
                            ' 1. Abfahrt auf dieser Baustelle
                            If sortedEntries(j).Item2 = "Abfahrt" AndAlso sortedEntries(j).Item3 = baustelle Then
                                blockEnd = sortedEntries(j).Item1
                                foundAbfahrt = True
                                Exit While
                            End If
                            ' 2. Anfahrt/Arbeitsbeginn auf ANDERER Baustelle = Blockende
                            If (sortedEntries(j).Item2 = "Anfahrt" Or sortedEntries(j).Item2 = "Arbeitsbeginn") AndAlso sortedEntries(j).Item3 <> baustelle Then
                                blockEnd = sortedEntries(j).Item1
                                status = "FEHLERHAFT"
                                Exit While
                            End If
                            ' 3. Arbeitsende auf dieser Baustelle als Notnagel (Tagesende)
                            If sortedEntries(j).Item2 = "Arbeitsende" AndAlso sortedEntries(j).Item3 = baustelle Then
                                blockEnd = sortedEntries(j).Item1
                                status = "FEHLERHAFT"
                                Exit While
                            End If
                            j += 1
                        End While

                        If blockEnd.HasValue Then
                            ' Zeiten berechnen
                            Dim fahrzeit = (arbeitsbeginn.Item1 - anfahrt.Item1).TotalMinutes / 60 ' Anfahrt
                            If foundAbfahrt AndAlso j + 1 < sortedEntries.Count AndAlso sortedEntries(j + 1).Item2 = "Arbeitsende" AndAlso sortedEntries(j + 1).Item3 = baustelle Then
                                fahrzeit += (sortedEntries(j + 1).Item1 - blockEnd.Value).TotalMinutes / 60 ' Rückfahrt
                            End If
                            Dim arbeitszeit = (blockEnd.Value - arbeitsbeginn.Item1).TotalMinutes / 60 - 0.75
                            If arbeitszeit < 0 Then arbeitszeit = 0
                            Dim tagName = datum.ToString("dddd", New CultureInfo("de-DE"))
                            Dim datumStr = datum.ToString("dd.MM.yyyy")
                            Dim fahrzeitBereich = $"{anfahrt.Item1:HH\:mm}–{arbeitsbeginn.Item1:HH\:mm}"
                            If foundAbfahrt AndAlso j + 1 < sortedEntries.Count AndAlso sortedEntries(j + 1).Item2 = "Arbeitsende" AndAlso sortedEntries(j + 1).Item3 = baustelle Then
                                fahrzeitBereich &= $", {blockEnd.Value:HH\:mm}–{sortedEntries(j + 1).Item1:HH\:mm}"
                            End If
                            Dim arbeitszeitBereich = $"{arbeitsbeginn.Item1:HH\:mm}–{blockEnd.Value:HH\:mm}"

                            tagesdaten.Rows.Add(tagName, datumStr, baustelle, fahrzeitBereich, arbeitszeitBereich, arbeitszeit.ToString("0.00"), fahrzeit.ToString("0.00"), status)

                            ' i vorspulen bis zum nächsten „Anfahrt“ (bzw. zum Ende dieses Blocks)
                            i = j
                            If foundAbfahrt AndAlso j + 1 < sortedEntries.Count AndAlso sortedEntries(j + 1).Item2 = "Arbeitsende" AndAlso sortedEntries(j + 1).Item3 = baustelle Then
                                i += 1
                            End If
                        Else
                            ' Kein Blockende gefunden, Fehler
                            i += 1
                        End If
                    Else
                        i += 1
                    End If
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
End Class