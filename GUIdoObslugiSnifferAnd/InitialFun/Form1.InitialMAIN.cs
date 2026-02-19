using System.Reflection;
using System.IO.Ports;

namespace GUIdoObslugiSnifferAnd;

public partial class Form1 {
    public enum AntennaState { Gray, Green, Red, RedGreen, RedGray, DoubleRed, Error }
    public enum BufforState {Unknown, Good, Bad};
    private AntennaState[/*UHF/HF/AND*/][/*nr. anteny*/] matrixData = null!;
    private const int BufferLenght = 64;
    private BufforState[] bufferData = new BufforState[BufferLenght+1];
    

    private void UstawIkoneZasobow() {
        try  {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "GUIdoObslugiSnifferAnd.ShifAnd.ico";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName)) {
                if (stream != null) { this.Icon = new Icon(stream); }
                else  { System.Diagnostics.Debug.WriteLine($"Nie znaleziono zasobu: {resourceName}"); }
            }
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine("Błąd podczas ładowania ikony: " + ex.Message);
        }
    }

    private void UstawRozmiarIPozycje()
    {
        this.WindowState = FormWindowState.Maximized; /* OTWIERANIE OKNA ZMAKSYMILIZOWANEGO*/
        // this.FormBorderStyle = FormBorderStyle.None; /* USUWA PASEK NA GÓRZE I PASEK ZADAŃ */
        // this.TopMost = true; /* ZAWSZE NA WIERZCHU */

        var screen = Screen.PrimaryScreen!; /* ZACHOWANIE OKNA W TRYBIE NIE PEŁNO EKRANOWYM */
        this.Width = screen.Bounds.Width;
        this.Height = screen.Bounds.Height / 2;
        this.StartPosition = FormStartPosition.Manual;
        this.Location = new Point(0, 0);
    }


    private void InicjalizujDane()
    {
        matrixData = new AntennaState[3][]; // UHF, HF, AND
        for (int r = 0; r < 3; r++)
        {
            matrixData[r] = new AntennaState[BufferLenght*4]; 
            for (int i = 0; i < (BufferLenght*4); i++) matrixData[r][i] = AntennaState.Gray;
        }
        for (int i = 0; i < 65; i++) bufferData[i] = BufforState.Unknown;
    }




    private void OdswiezPorty()
    {
        cmbPorts.Items.Clear();
        string[] ports = SerialPort.GetPortNames();
        if (ports.Length > 0) {
            cmbPorts.Items.AddRange(ports);
            cmbPorts.SelectedIndex = 0;
        } else {
            cmbPorts.Text = "Brak portów";
        }
    }

    private void RysujPodgladBuforMatrix() {
        pnlMatrixBuffor.Paint  += (s, e) => RysujBufor(e.Graphics);
        pnlMatrixCurrent.Paint += (s, e) => RysujSekcje(e.Graphics, 0);
        pnlMatrixPrev1.Paint   += (s, e) => RysujSekcje(e.Graphics, 1);
        pnlMatrixPrev2.Paint   += (s, e) => RysujSekcje(e.Graphics, 2);
        pnlMatrixPrev3.Paint   += (s, e) => RysujSekcje(e.Graphics, 3);
    }

    


    private void ObslugaSzerokosci() {
        txtWidthInput.TextChanged += (s, e) => ObslugaZmianySzerokosci();
        txtWidthInput.KeyPress += (s, e) => {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        };
        this.Resize += (s, e) => {
            matrixLayout.SuspendLayout();
            matrixLayout.PerformLayout();
            matrixLayout.ResumeLayout();
        };
    }
    private void ObslugaZmianySzerokosci()
    {
        string tekst = txtWidthInput.Text;
        if (string.IsNullOrEmpty(tekst)) return;

        if (int.TryParse(tekst, out int val)) {
            if (val > 64) {
                val = 64;
                txtWidthInput.Text = "64";
                txtWidthInput.SelectionStart = txtWidthInput.Text.Length; // Kursor na koniec
            }  else if (val < 1) {
                val = 1;
            }

            dotsPerRow = val;

            OdswiezSzerokoscPaneli();
            matrixLayout.Invalidate(true);
        }
    }

    private void OdswiezSzerokoscPaneli()
    {
        // Parametry graficzne (muszą się zgadzać z RysujSekcje i RysujBufor)
        const int spacingX_Matrix = 35;
        const int offsetX_Matrix = 60; // Marginesy Matrixa
        
        const int spacingX_Buffer = 25;
        const int offsetX_Buffer = 10; // Marginesy Bufora

        // 1. Oblicz ile miejsca potrzebuje Matrix (dotsPerRow)
        int szerokoscMatrix = offsetX_Matrix + (dotsPerRow * spacingX_Matrix) + 50;

        // 2. Oblicz ile miejsca potrzebuje Bufor (aktualnaOdleglosc)
        int szerokoscBufora = offsetX_Buffer + (aktualnaOdleglosc * spacingX_Buffer) + 50;

        // 3. Wybierz większą wartość
        int ostatecznaSzerokosc = Math.Max(szerokoscMatrix, szerokoscBufora);

        // 4. Zaaplikuj do paneli
        Control[] paneleDoZmiany = { pnlMatrixBuffor, pnlMatrixCurrent, pnlMatrixPrev1, pnlMatrixPrev2, pnlMatrixPrev3 };
        
        foreach (var pnl in paneleDoZmiany) {
            if (pnl?.Parent != null) {
                pnl.Parent.Width = ostatecznaSzerokosc;
            }
        }

        // Wymuszenie przerysowania paska przewijania
        matrixLayout.AutoScroll = true;
        matrixLayout.PerformLayout();
    }
}

