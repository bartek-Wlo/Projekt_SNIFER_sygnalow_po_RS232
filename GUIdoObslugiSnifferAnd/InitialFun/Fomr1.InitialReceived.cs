using System.Text; 

namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private List<byte> serialBuffer = new List<byte>();

    private byte[] tempInitBuffer = new byte[8]; // Tu sklejamy bajty z ramek 0x42, 0xDB, 0x33
    private int tempInitDistance = 6;            // Tu przechowujemy odległość z ramki 0xAA
    private bool gotDist = false;                // Czy odebrano 0xAA?
    private bool gotPart1 = false;               // Czy odebrano 0x42?
    private bool gotPart2 = false;               // Czy odebrano 0xDB?

    private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
    {
        if (_serialPort == null || !_serialPort.IsOpen) return;

        try
        {
            int bytesToRead = _serialPort.BytesToRead;
            byte[] tempBuffer = new byte[bytesToRead];
            _serialPort.Read(tempBuffer, 0, bytesToRead);
            serialBuffer.AddRange(tempBuffer);

            StringBuilder textData = new StringBuilder();

            // Szukamy ramki: 0x55 [4 bajty danych] [CRC_L] [CRC_H] 0xCC (razem 8 bajtów)
            while (serialBuffer.Count >= 8) {
                if (serialBuffer[0] == 0x55) {
                    if (serialBuffer[7] == 0xCC) {
                        // Mamy potencjalną ramkę
                        byte[] frame = serialBuffer.GetRange(0, 8).ToArray();
                        serialBuffer.RemoveRange(0, 8);
                        
                        if (VerifyFrameCRC(frame))                         {
                            this.Invoke(new Action(() => ChooseCommand(frame)));
                        } else {
                            System.Diagnostics.Debug.WriteLine("Zła ramka: " + frame);
                            lblErrorStatus.Text = $"Zła ramka: {frame}";
                        }
                    } else {
                        // Bajt startowy poprawny, ale brak bajtu stopu na 8. pozycji - usuwamy śmieci
                        textData.Append((char)serialBuffer[0]);
                        serialBuffer.RemoveAt(0);
                    }
                } else {
                    textData.Append((char)serialBuffer[0]);
                    serialBuffer.RemoveAt(0);
                }
            }
            if (textData.Length > 0) {
                string incomingString = textData.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(incomingString)) {
                    this.Invoke(new Action(() => {
                        lblLastMessage.Text = incomingString;
                    }));
                }
            }
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine("Błąd odbioru: " + ex.Message);
        }
    }

    private bool VerifyFrameCRC(byte[] frame)
    {
        // Dane do CRC: bajty od 1 do 4 (UHF_Del, HF, AND, UHF_New)
        byte[] dataForCRC = new byte[] { frame[1], frame[2], frame[3], frame[4] };
        ushort calculated = CalculateCRC16MCRF4XX(dataForCRC);
        
        ushort received = (ushort)(frame[5] | (frame[6] << 8)); // Little Endian
        return calculated == received;
    }


    private void ChooseCommand(byte[] frame) {
        switch (frame[1]) {
            case 0xFF: 
            case 0x00: 
            case 0xF0: 
                ProcessProductionFrame(frame);
                break;
            case 0x0F: 
                ok_responsce(frame);
                break;
            // case 0xEE: 
            //     break;
            case 0xAA: 
                distance_responsce(frame);
                break;
            case 0xBB: 
                ProcessInitDistanceFrame(frame); 
                break;
            case 0x42: 
                ProcessBufferPart1(frame);
                break;
            case 0xDB: 
                ProcessBufferPart2(frame);
                break;
            case 0x33: 
                ProcessBufferPart3_Finalize(frame);
                break;
            case 0x66: 
                break;
            case 0x55: 
                break;
            case 0xCC: 
                break;
            case 0x22: 
                break;
        }
    }


    private void ProcessProductionFrame(byte[] frame) {
        byte delayedUHF_Raw = frame[1]; // UHF anteny przy HF
        byte currentHF_Raw  = frame[2]; // Wynik HF
        byte andResult_Raw  = frame[3]; // Faza / Wynik AND
        byte newUHF_Raw     = frame[4]; // Nowa antena przy UHF
        // 0x0F - P1 OK, 0x00 - P1 Fail, 0xFF - P2 OK, 0xF0 - P2 Fail

        // --- 3. OBSŁUGA BUFORA (Pasek nad Matrixami) ---
        if (andResult_Raw == 0x0F || andResult_Raw == 0x00)
        {
            for (int i = aktualnaOdleglosc - 1; i > 0; i--)
            {
                bufferData[i] = bufferData[i - 1];
            }
            bufferData[0] = newUHF_Raw switch {
                0xFF => BufforState.Good,
                0x00 => BufforState.Bad,
                _    => BufforState.Unknown
            };

            headIndex--; // Przesunięcie wskaźnija na matrix gdy to jest pierwsze zapytanie
            if (headIndex < 0) headIndex = (BufferLenght * 4) - 1;
        }



        AntennaState stateUHF = delayedUHF_Raw == 0xFF ? AntennaState.Green : AntennaState.Red;
        AntennaState stateHF = AntennaState.Gray;
        AntennaState stateAND = AntennaState.Gray;

        switch (andResult_Raw) {
            case 0x0F: // 2.1. AND 1 DOBRY (Finał)
                stateHF = AntennaState.Green;
                stateAND = AntennaState.Green;
                countGoodUHF++;
                countGoodHF++;
                break;

            case 0x00: // 2.2. AND 1 ZŁY (Tymczasowy)
                stateHF = currentHF_Raw == 0xFF ? AntennaState.Green : AntennaState.RedGray;
                stateAND = AntennaState.RedGray;
                break;

            case 0xFF: // 2.3. AND 2 DOBRY (Finał - Nadpisanie)
                stateHF = AntennaState.RedGreen;
                stateAND = AntennaState.RedGreen;
                countGoodUHF++;
                countGoodHF++;
                break;

            case 0xF0: // 2.4. AND 2 ZŁY (Finał - Nadpisanie)
                stateAND = AntennaState.DoubleRed; // RedRed
                
                if (stateUHF == AntennaState.Green) { ++countGoodUHF; } 
                else { ++countBadUHF; }

                if (currentHF_Raw == 0xFF) { stateHF = AntennaState.Green; ++countGoodHF;}// Poniważ nie mogło zmienić stanu, bo gdy UHF jest złe to nie powtaża się badań, więc HF musiało być oryginalnie dobre.
                else if (currentHF_Raw == 0x00) { stateHF = AntennaState.DoubleRed; ++countBadHF; }
                else if (currentHF_Raw == 0xF0) { stateHF = AntennaState.RedGray;   ++countBadHF; }
                break;
        }

        // 6. Zapis do matrixa (W fazie AND 2 nadpisuje ten sam headIndex co AND 1 Fail)
        matrixData[0][headIndex] = stateUHF;
        matrixData[1][headIndex] = stateHF;
        matrixData[2][headIndex] = stateAND;

        // 7. ODŚWIEŻENIE INTERFEJSU
        this.Invoke(new Action(() => {
            lblGoodUHF.Text = $"Dobre UHF: {countGoodUHF}";
            lblBadUHF.Text = $"Złe UHF: {countBadUHF}";
            lblGoodHF.Text = $"Dobre HF: {countGoodHF}";
            lblBadHF.Text = $"Złe HF: {countBadHF}";
            
            pnlMatrixBuffor.Invalidate();
            matrixLayout.Invalidate(true);
        }));
    }




    private void ok_responsce(byte[] frame) {
        switch (frame[2]) {
            case 0x33:
                this.Invoke(new Action(() => {
                    isCheckSensorsActive = false; 
                    byte uhafStat = frame[3];  byte hfStat = frame[4];

                    if (uhafStat == 0x00 && hfStat == 0x00) {
                        isSensorError = false;
                        lblErrorStatus.Text = "Status: Kom - OK";
                    } else if (uhafStat == 0xAA && hfStat == 0xAA) {
                        isSensorError = true;
                        lblErrorStatus.Text = "Status: MEGA2560 odpowiada,\nczujniki: czekam na odpowiedz";
                    } else {
                        isSensorError = true;
                        string msg = "ERROR: ";
                        if (uhafStat == 0xFF) msg += "Błąd UHF; ";
                        if (hfStat == 0xFF) msg += "Błąd HF;";
                        lblErrorStatus.Text = msg;
                    }
                    AktualizujStanKontrolek(true);
                }));
                break;
            case 0x42:
                if (frame[3] == 0xFF && frame[4] == 0xFF) {
                    this.Invoke(new Action(() => { lblErrorStatus.Text = "ERROR: inicjalizacji bufora w MEGA2560"; }));
                }
                else if (frame[3] == 0x00 && frame[4] == 0x00) {
                    isInitPCActive = false; 
                    this.Invoke(new Action(() => { lblErrorStatus.Text = "Status: bufor w MAGA2560 - wczytano"; AktualizujStanKontrolek(true); }));
                }
                break;
            case 0xAA: 
                isMEGARestartActive = false;
                AktualizujStanKontrolek(true);
                break;
            case 0xEE: 
                isUHFRestartActive = false;
                AktualizujStanKontrolek(true);
                break;
            case 0xDD:
                if (frame[3] == 0xDD && frame[4] == 0xDD) {
                    this.Invoke(new Action(() => { lblErrorStatus.Text = "ERROR: Poważny błąd UHF,\nWymagany restart całej maszyny."; }));
                }
                break;
        };
    }

        private void distance_responsce(byte[] frame) {
        if(frame[2] == frame[3] && frame[3] == frame[4]) {
            aktualnaOdleglosc = Math.Clamp((int)frame[2], 2, 64);
            lblDistanceInfo.Text = $"Odległość: {aktualnaOdleglosc}";
            pnlMatrixBuffor.Invalidate();
        }
    }



    private void ProcessInitDistanceFrame(byte[] frame) {
        if (frame[2] == frame[3] && frame[3] == frame[4]) {
            tempInitDistance = Math.Clamp((int)frame[2], 2, 64);
            gotDist = true;
        }
    }

    private void ProcessBufferPart1(byte[] frame) {
        tempInitBuffer[0] = frame[2];
        tempInitBuffer[1] = frame[3];
        tempInitBuffer[2] = frame[4];
        gotPart1 = true;
    }

    private void ProcessBufferPart2(byte[] frame) {
        tempInitBuffer[3] = frame[2];
        tempInitBuffer[4] = frame[3];
        tempInitBuffer[5] = frame[4];
        gotPart2 = true;
    }

    private void ProcessBufferPart3_Finalize(byte[] frame) {
        tempInitBuffer[6] = frame[2];
        tempInitBuffer[7] = frame[3];
        int distCheck = frame[4]; 

        if (gotDist && gotPart1 && gotPart2) {
            if (distCheck == tempInitDistance) {
                aktualnaOdleglosc = tempInitDistance;

                for (int i = 0; i < 64; i++) {
                    byte currentByte = tempInitBuffer[i / 8];
                    bool isSet = (currentByte & (1 << (i % 8))) != 0;

                    bufferData[i] = isSet ? BufforState.Good : BufforState.Bad;
                }
                bufferData[64] = BufforState.Unknown;

                this.Invoke(new Action(() => {
                    lblDistanceInfo.Text = $"Odległość: {aktualnaOdleglosc}";
                    
                    isInitMEGAActive = false;
                    lblErrorStatus.Text = "Status: Bufor wczytano"; 
                    AktualizujStanKontrolek(true);
                    pnlMatrixBuffor.Invalidate();
                    matrixLayout.Invalidate(true);
                }));
            } 
            else {
                // BŁĄD WALIDACJI: Dystanse się nie zgadzają
                this.Invoke(new Action(() => {
                    lblErrorStatus.Text = $"ERROR: Niespójność danych ({tempInitDistance} vs {distCheck})";
                }));
            }
        } 
        else {
            // BŁĄD SEKWENCJI: Brak poprzednich ramek
            this.Invoke(new Action(() => {
                lblErrorStatus.Text = "ERROR: Niekompletna sekwencja ramek";
            }));
        }

        // Reset flag po próbie złożenia całości
        gotDist = false; gotPart1 = false; gotPart2 = false;
    }
}