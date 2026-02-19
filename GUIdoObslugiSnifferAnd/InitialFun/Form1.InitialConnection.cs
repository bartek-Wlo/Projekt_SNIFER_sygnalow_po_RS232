using System.IO.Ports;
using System;

namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private SerialPort? _serialPort;
    private bool czyPolaczono = false;

    private void ObslugaPolaczenia() {
        if (czyPolaczono) { 
            ProbaRozlaczenia(); 
            if (!czyPolaczono) { PrzywrocWygaszanie(); }
        } else {
            if (cmbPorts.SelectedItem == null || cmbPorts.SelectedItem.ToString() == "Brak portów") {
                MessageBox.Show("Wybierz prawidłowy port COM.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string wybranyPort = cmbPorts.SelectedItem.ToString()!;
            ProbaPolaczenia(wybranyPort);
            if (czyPolaczono) { ZablokujWygaszanie(); }
        }
    }
    private void ProbaPolaczenia(string portName) {
        try {
            string selectedConfig = cmbBaud.SelectedItem?.ToString() ?? "115200 8N1";
            int baudRate = 115200;
            Parity parity = Parity.None;

            // Rozpoznawanie konfiguracji
            if (selectedConfig.StartsWith("38400")) {
                baudRate = 38400;
                parity = selectedConfig.Contains("8E1") ? Parity.Even : Parity.None;
            } else if (selectedConfig.StartsWith("115200")) {
                baudRate = 115200;
                parity = Parity.None;
            }

            _serialPort = new SerialPort(portName) {
                BaudRate = baudRate,
                Parity = parity,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.Open();

            czyPolaczono = true;
            isDistanceNeeded = true; isUHFInitPending = true;
            btnConnect.Text = "Rozłącz";
            btnConnect.BackColor = colActive;
            AktualizujStanKontrolek(true);

        } catch (Exception ex) {
            MessageBox.Show($"Nie udało się otworzyć portu: {ex.Message}", "Błąd Połączenia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            czyPolaczono = false;
            AktualizujStanKontrolek(false);
            _serialPort = null;
        }
    }
    private void ProbaRozlaczenia() {
        try {
            if (_serialPort != null)  {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                if (_serialPort.IsOpen) _serialPort.Close();
                _serialPort.Dispose();
            }
        } catch (Exception ex)  {
            MessageBox.Show($"Utracono połączenie z portem. Error: {ex.Message}\nRozłączam!", "Błąd Połączenia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        } finally {
            czyPolaczono = false;
            _serialPort = null;
            
            btnConnect.Text = "Połącz";
            btnConnect.BackColor = Color.IndianRed;
            AktualizujStanKontrolek(false);
        }
    }


    private void SendData(string hexData) {
        if (_serialPort != null && _serialPort.IsOpen) {
            try {
            byte[] buffer = HexStringToByteArray(hexData);
            _serialPort.Write(buffer, 0, buffer.Length); 

            } catch (Exception ex) {
                MessageBox.Show($"Błąd podczas wysyłania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private byte[] HexStringToByteArray(string hex) {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }
}