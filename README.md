Arbeitszeit-Auswertung mit CSV-Import aus pds Mitarbeiter App
Dieses Tool wertet Arbeits- und Fahrzeiten aus den von der pds Mitarbeiter App exportierten CSV-Dateien aus. Du bekommst damit eine Übersicht über deine Arbeitszeit und Fahrzeit pro Woche und Monat.

CSV-Export aus der pds Mitarbeiter App
Folge diesen Schritten, um deine Buchungen als CSV zu exportieren:

1. Öffne die App und gehe auf „Zeit“
<img src="img/Screenshot_20250530-213812_pds Mitarbeiter.png" alt="Übersicht" width="300"/>
2. Wähle „Protokoll“ aus
<img src="img/Screenshot_20250530-213834_pds Mitarbeiter.png" alt="Zeit Menü" width="300"/>
3. Tippe oben rechts auf das Export-Symbol
<img src="img/Screenshot_20250530-213842_pds Mitarbeiter.png" alt="Protokoll Export" width="300"/>
4. Wähle den gewünschten Zeitraum (immer vom 1. bis zum letzten Tag des Monats) und exportiere die CSV-Datei
<img src="img/Screenshot_20250530-213851_pds Mitarbeiter.png" alt="CSV Export" width="300"/>
Hinweis: Immer vom 1. des Monats bis zum letzten auswählen!

Import in das Auswertungs-Tool
Starte das Tool (z.B. als .exe oder über die Konsole).

Importiere die exportierte CSV-Datei über die Schaltfläche „Importieren“ oder ziehe die Datei einfach in das Programmfenster.

<img src="img/Screenshot 2025-05-30 220626.png" alt="CSV Export" width="300"/>

Das Tool wertet nun automatisch Arbeitszeit (von Arbeitsbeginn bis Abfahrt) und Fahrzeit (Anfahrt bis Arbeitsbeginn + Abfahrt bis Arbeitsende) pro Woche und Monat aus. Die Berechnung erfolgt immer im 1.00 Stundenformat (z.B. 30min = 0.5, 1h = 1.0).

Hinweise zur CSV-Datei
Es wird ausschließlich die Original-CSV aus der pds Mitarbeiter App unterstützt.

Beispielausgabe
Woche	Arbeitszeit (h)	Fahrzeit (h)
KW 18/2025	36.50	6.25
KW 19/2025	39.00	7.00

Fragen oder Fehler?
Melde dich einfach per Issue auf GitHub!
