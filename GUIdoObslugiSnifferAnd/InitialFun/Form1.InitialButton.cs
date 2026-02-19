using System.Text.RegularExpressions;
using System.Text; 

namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private int countGoodUHF = 0;
    private int countBadUHF = 0;
    private int countGoodHF = 0;
    private int countBadHF = 0;

    private void ObslugaButton() {
        btnRefresh.Click       += (s, e) => OdswiezPorty();
        btnConnect.Click       += (s, e) => ObslugaPolaczenia();

        btnModeSniffer.Click   += (s, e) => { 
            return; // PRZYCISK JEST NIE UŻYWANY Z PRZYCZYN HARDWARE
            if (btnModeSniffer.Tag?.ToString() == "OFF") return; 
            isSnifferActive = true;
            SendData("550F0F0F0F2080CC"); 
            btnModeSniffer.BackColor = Color.IndianRed;
        };

        btnModeAnd.Click       += (s, e) => { 
            if (btnModeAnd.Tag?.ToString() == "OFF") return; 
            isSnifferActive = false;
            SendData("55F0F0F0F0B973CC"); 
            btnModeSniffer.BackColor = colActive;
        };


        // Wiadomości do UHF
        btnToggleUHF.Click += (s, e) => {
            if (btnToggleUHF.Tag?.ToString() == "OFF") return;
            toggleUHF = !toggleUHF;
            SendData(toggleUHF ? "55EE0FFFFF96A3CC" : "55EE0F00002E53CC");
            AktualizujStanKontrolek(true);
        };

        // Wiadomości do HF
        btnToggleHF.Click += (s, e) => {
            if (btnToggleHF.Tag?.ToString() == "OFF") return;
            toggleHF = !toggleHF;
            SendData(toggleHF ? "55EE00FFFF51E9CC" : "55EE000000E919CC");
            AktualizujStanKontrolek(true);
        };

        // Wiadomości do PC UHF
        btnTogglePCUHF.Click += (s, e) => {
            if (btnTogglePCUHF.Tag?.ToString() == "OFF") return;
            togglePCUHF = !togglePCUHF;
            SendData(togglePCUHF ? "55EEFFFFFFA22FCC" : "55EEFF00001ADFCC");
            AktualizujStanKontrolek(true);
        };

        // Wiadomości do PC HF
        btnTogglePCHF.Click += (s, e) => {
            if (btnTogglePCHF.Tag?.ToString() == "OFF") return;
            togglePCHF = !togglePCHF;
            SendData(togglePCHF ? "55EEF0FFFF6565CC" : "55EEF00000DD95CC");
            AktualizujStanKontrolek(true);
        };
        


        // PC -> Buff
        btnInitBuffPCtoMEGA.Click += (s, e) => {
            if (btnInitBuffPCtoMEGA.Tag?.ToString() == "OFF") return;
            isInitPCActive = true;
            SendData("55660F0F0FA7E0CC");
            WyślijAktualnąOdległośćIBufor(); // Dodatkowe wysyłanie danych
            WyślijRamkiBufora();
            AktualizujStanKontrolek(true);
        };

        // Buff -> PC
        btnInitBuffMEGAtoPC.Click += (s, e) => {
            if (btnInitBuffMEGAtoPC.Tag?.ToString() == "OFF") return;
            isInitMEGAActive = true;

            gotDist = false;
            gotPart1 = false;
            gotPart2 = false;
            Array.Clear(tempInitBuffer, 0, tempInitBuffer.Length);
            
            SendData("5566F0F0F0ECD6CC");
            AktualizujStanKontrolek(true);
        };

        // czujnik UHF init (Restart)
        btnUHFRestart.Click += (s, e) => {
            if (btnUHFRestart.Tag?.ToString() == "OFF") return;
            isUHFInitPending = false;
            isUHFRestartActive = true;
            SendData("5566AAAAAA2DF5CC");
            AktualizujStanKontrolek(true);
        };

        // MEGA 2560 sniffer restart
        btnMEGARestart.Click += (s, e) => {
            if (btnMEGARestart.Tag?.ToString() == "OFF") return;
            isMEGARestartActive = true;
            SendData("55663333334727CC");
            toggleUHF = false;
            toggleHF = false;
            togglePCUHF = false;
            togglePCHF = false;
            isDistanceNeeded = true;
            AktualizujStanKontrolek(true);
        };



        btnCheckSensors.Click += (s, e) => {
            if (btnCheckSensors.Tag?.ToString() == "OFF") return;

            isCheckSensorsActive = true;
            isSensorError = false; // Resetujemy błąd przy nowej próbie
            
            SendData("55777777776CB8CC");
            AktualizujStanKontrolek(true);
        };



        btnSend.Click          += btnManualSend_Click;
        btnSetDistance.Click   += btnSetDistance_Click;

        // Nawigacja
        btnGoToSettings.Click  += (s, e) => PrzelaczPanel(panelSettings);
        btnGoToMain.Click      += (s, e) => PrzelaczPanel(panelMain);
        btnGoToInfo1.Click     += (s, e) => PokazOknoInfo();
        btnGoToInfo2.Click     += (s, e) => PokazOknoInfo();

        // Pełny ekran (Punkt 4)
        btnFullScreen.Click    += (s, e) => ObslugaTrybuFullScreen();
        btnStopProduction.Click += (s, e) => SendData("55222222221411CC");

        btnSaveFile.Click += (s, e) => ObslugaZapisuDoPliku();
        btnClearMatrix.Click += (s, e) => ObslugaCzyszczeniaMatrix();
        btnClearCounts.Click += (s, e) => {
            countGoodUHF = 0;
            countBadUHF = 0;
            countGoodHF = 0;
            countBadHF = 0;

            lblGoodUHF.Text = "Dobre UHF: 0";
            lblBadUHF.Text = "Złe UHF: 0";
            lblGoodHF.Text = "Dobre HF: 0";
            lblBadHF.Text = "Złe HF: 0";
        };
    }









    private void btnManualSend_Click(object? sender, EventArgs e) {
        string textToSend = txtDataInput.Text.Trim().Replace(" ", ""); 

        if (string.IsNullOrEmpty(textToSend)) return;

        if (textToSend.Length % 2 != 0 || !Regex.IsMatch(textToSend, @"^[0-9a-fA-F]+$")) {
            MessageBox.Show("Wprowadź prawidłowy HEX (parzysta liczba znaków, 0-9, A-F).");
            return;
        }

        SendData(textToSend.ToUpper());
        txtDataInput.Clear();
    }





    private void btnSetDistance_Click(object? sender, EventArgs e)
    {
        if (int.TryParse(txtDistanceInput.Text, out int distance) && distance >= 2 && distance <= 64)
        {
            aktualnaOdleglosc = Math.Clamp(distance, 2, 64);
            lblDistanceInfo.Text = $"Odległość: {aktualnaOdleglosc}";
            OdswiezSzerokoscPaneli();
            pnlMatrixBuffor.Invalidate();
            isDistanceNeeded = false;

            WyślijAktualnąOdległośćIBufor();
            AktualizujStanKontrolek(true);
        }
        else
        {
            MessageBox.Show("Wprowadź odległość 2-64.");
        }
    }

    private void WyślijAktualnąOdległośćIBufor()  {
        string distHex = aktualnaOdleglosc.ToString("X2");

        byte distByte = (byte)aktualnaOdleglosc;
        byte[] crcDataDist = new byte[] { 0xAA, distByte, distByte, distByte};
        ushort crcValue = CalculateCRC16MCRF4XX(crcDataDist);

        string crcH = (crcValue >> 8).ToString("X2");
        string crcL = (crcValue & 0xFF).ToString("X2");

        string newFrame = $"55AA{distHex}{distHex}{distHex}{crcL}{crcH}CC";
        SendData(newFrame);
    }

    private void WyślijRamkiBufora() {
        byte[] bytes = new byte[8];

        // Pakowanie 64 bitów (z 65 dostępnych) do 8 bajtów i liczenie sumy 'Good'
        for (int i = 0; i < 65; i++) {
            if (bufferData[i] == BufforState.Good) {
                if (i < 64) bytes[i / 8] |= (byte)(1 << (i % 8));
            }
        }

        // Ramka 1: [55] [42] [bajt1] [bajt2] [bajt3] [crc] [cc]
        byte[] f1Data = { 0x42, bytes[0], bytes[1], bytes[2] };
        ushort crc1 = CalculateCRC16MCRF4XX(f1Data);
        SendData($"5542{bytes[0]:X2}{bytes[1]:X2}{bytes[2]:X2}{(byte)(crc1 & 0xFF):X2}{(byte)(crc1 >> 8):X2}CC");

        // Ramka 2: [55] [DB] [bajt4] [bajt5] [bajt6] [crc] [cc]
        byte[] f2Data = { 0xDB, bytes[3], bytes[4], bytes[5] };
        ushort crc2 = CalculateCRC16MCRF4XX(f2Data);
        SendData($"55DB{bytes[3]:X2}{bytes[4]:X2}{bytes[5]:X2}{(byte)(crc2 & 0xFF):X2}{(byte)(crc2 >> 8):X2}CC");

        // Ramka 3: [55] [33] [bajt7] [bajt8] [suma] [crc] [cc]
        byte[] f3Data = { 0x33, bytes[6], bytes[7], (byte)aktualnaOdleglosc };
        ushort crc3 = CalculateCRC16MCRF4XX(f3Data);
        SendData($"5533{bytes[6]:X2}{bytes[7]:X2}{aktualnaOdleglosc:X2}{(byte)(crc3 & 0xFF):X2}{(byte)(crc3 >> 8):X2}CC");
    }







    private void ObslugaZapisuDoPliku()
    {
        using (SaveFileDialog sfd = new SaveFileDialog())
        {
            sfd.Filter = "Plik tekstowy (*.txt)|*.txt";
            sfd.Title = "Zapisz dane Sniffera";
            sfd.FileName = $"Raport_Sniffer_{DateTime.Now:yyyyMMdd_HHmm}.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== RAPORT PRACY SYSTEMU SNIFFER ===");
                sb.AppendLine($"Data zapisu: {DateTime.Now}");
                sb.AppendLine($"Distance (Odległość): {aktualnaOdleglosc}");
                sb.AppendLine();

                // --- ZAPIS BUFORA ---
                sb.AppendLine("--- ZAWARTOŚĆ BUFORA (Droga od UHF do HF) ---");
                for (int i = 0; i < aktualnaOdleglosc; i++)
                {
                    sb.Append($"[{i:D2}]: {bufferData[i]} \t");
                    if ((i + 1) % 4 == 0) sb.AppendLine();
                }
                sb.AppendLine("\n");

                // --- ZAPIS MATRIXA ---
                sb.AppendLine("--- ZAWARTOŚĆ MATRIX (Historia od najnowszych) ---");
                string[] wiersze = { "UHF", "HF ", "AND" };
                int TotalHistorySize = BufferLenght * 4;

                for (int r = 0; r < 3; r++)
                {
                    sb.Append(wiersze[r] + ": ");
                    for (int i = 0; i < TotalHistorySize; i++)
                    {
                        // Używamy logiki bufora cyklicznego, aby zapisać dane tak, jak widać je na ekranie
                        int fizycznyIdx = (headIndex + i) % TotalHistorySize;
                        string symbol = matrixData[r][fizycznyIdx] switch
                        {
                            AntennaState.Green => "[G] ",
                            AntennaState.Red => "[R] ",
                            AntennaState.RedGreen => "[RG]",
                            AntennaState.RedGray => "[R?]",
                            AntennaState.DoubleRed => "[RR]",
                            AntennaState.Gray => "[_] ",
                            _ => "[. ]"
                        };
                        sb.Append(symbol);
                        
                        // Co 'dotsPerRow' robimy nową linię dla czytelności w TXT
                        if ((i + 1) % dotsPerRow == 0 && i < TotalHistorySize - 1)
                        {
                            sb.AppendLine();
                            sb.Append("     "); // Wcięcie dla nowej linii w tym samym typie anteny
                        }
                    }
                    sb.AppendLine();
                    sb.AppendLine(new string('-', dotsPerRow * 3));
                }

                try
                {
                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Dane zostały zapisane pomyślnie.", "Zapis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void ObslugaCzyszczeniaMatrix()
    {
        var wynik = MessageBox.Show("Czy na pewno chcesz wyczyścić wszystkie dane wizualne?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (wynik == DialogResult.Yes)
        {
            // 1. Reset danych Matrixa (na szare)
            for (int r = 0; r < 3; r++)
            {
                for (int i = 0; i < BufferLenght * 4; i++)
                {
                    matrixData[r][i] = AntennaState.Gray;
                }
            }

            // 2. Reset Bufora drogi
            for (int i = 0; i < BufferLenght + 1; i++)
            {
                bufferData[i] = BufforState.Unknown;
            }

            // 3. Reset wskaźników logicznych
            headIndex = 0;

            // 4. Odświeżenie UI
            pnlMatrixBuffor.Invalidate();
            matrixLayout.Invalidate(true);
        }
    }
}





