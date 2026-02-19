namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private bool isSnifferActive = false; //PRZYCISK JEST NIE UŻYWANY Z PRZYCZYN HARDWARE
    private bool toggleUHF = false;
    private bool toggleHF = false;
    private bool togglePCUHF = false;
    private bool togglePCHF = false;
    private bool isInitPCActive = false;
    private bool isInitMEGAActive = false;
    private bool isUHFRestartActive = false;
    private bool isMEGARestartActive = false;
    private bool isDistanceNeeded = false; 
    private bool isUHFInitPending = false; 

    private bool isCheckSensorsActive = false; // Dla koloru DarkGoldenrod (trwa sprawdzanie)
    private bool isSensorError = false;        // Dla koloru IndianRed (wykryto błąd)


    private void AktualizujStanKontrolek(bool stan)
    {
        Font fontAktywny = new Font("Arial", 9, FontStyle.Bold);
        Font fontNieaktywny = new Font("Arial", 9, FontStyle.Regular);

        // --- FUNKCJA STYLIZUJĄCA PRZYCISKI ---
        void StylizujB(Button b, bool aktywny, Color? tloSpecjalne = null, Color? tekstSpecjalny = null) 
        {
            // Oznaczamy stan w Tagu, aby ObslugaButton mogła ignorować kliknięcia
            b.Tag = aktywny ? "ON" : "OFF"; 
            
            b.BackColor = aktywny ? (tloSpecjalne ?? colActive) : colDisabled;
            b.ForeColor = aktywny ? (tekstSpecjalny ?? colTextActive) : colTextDisabled;
            b.Font      = aktywny ? fontAktywny : fontNieaktywny;
            
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = aktywny ? colBorderActive : colBorderDisabled;
        }

        // --- FUNKCJA STYLIZUJĄCA POLA TEKSTOWE I ICH RAMKI ---
        void StylizujT(TextBox t, bool aktywny) 
        {
            t.Enabled   = aktywny;
            t.BackColor = aktywny ? Color.White : colDisabled;
            t.ForeColor = aktywny ? Color.Black : colTextDisabled;
            
            // Rodzic TextBoxa to Panel pełniący rolę obramowania
            if (t.Parent != null)
            {
                t.Parent.BackColor = aktywny ? colBorderActive : colBorderDisabled;
            }
        }

        // Przycisk Połącz / Rozłącz (NAPRAWA PUNKTU 2)
        // Jeśli stan == true (Połączono), tekst to Rozłącz -> ma mieć styl aktywny (colActive)
        // Jeśli stan == false (Rozłączono), tekst to Połącz -> ma mieć kolor IndianRed
        StylizujB(btnConnect, true, stan ? colActive : Color.IndianRed, colTextActive);
        btnConnect.Text = stan ? "Rozłącz" : "Połącz";

        // Pozostałe przyciski sterowania
        /*            Tryb SNIFFER: Zawsze 'false', bo funcja jest nie obsługiwana przez MEGA (problem z hardware)
        StylizujB(btnModeSniffer, stan, (stan && isSnifferActive) ? Color.IndianRed : colActive); */
        StylizujB(btnModeSniffer, false);
        // StylizujB(btnModeAnd, stan, colDisabled);
        StylizujB(btnModeAnd, false);

        StylizujB(btnSaveFile, stan);

        StylizujB(btnToggleUHF,   stan);
        StylizujB(btnToggleHF,    stan);
        StylizujB(btnTogglePCUHF, stan);
        StylizujB(btnTogglePCHF,  stan);

        StylizujB(btnToggleUHF,   stan, (stan && toggleUHF)   ? Color.SteelBlue : colUnimportant);
        StylizujB(btnToggleHF,    stan, (stan && toggleHF)    ? Color.SteelBlue : colUnimportant);
        StylizujB(btnTogglePCUHF, stan, (stan && togglePCUHF) ? Color.SteelBlue : colUnimportant);
        StylizujB(btnTogglePCHF,  stan, (stan && togglePCHF)  ? Color.SteelBlue : colUnimportant);

        StylizujB(btnSend, stan, colUnimportant);
        StylizujB(btnSetDistance, stan, (stan && isDistanceNeeded) ? Color.IndianRed : colActive);

        StylizujB(btnInitBuffPCtoMEGA, stan, (stan && isInitPCActive) ? Color.IndianRed : colActive);
        // StylizujB(btnInitBuffMEGAtoPC, stan, (stan && isInitMEGAActive) ? Color.IndianRed : colActive);
        StylizujB(btnInitBuffMEGAtoPC, false); 
        Color tloUHF = (stan && isUHFRestartActive) ? Color.IndianRed : 
                       (stan && isUHFInitPending)   ? Color.DarkGoldenrod : colActive;
        StylizujB(btnUHFRestart, stan, tloUHF);
        StylizujB(btnMEGARestart, stan, (stan && isMEGARestartActive) ? Color.IndianRed : colActive);

        StylizujB(btnClearMatrix, stan);
        StylizujB(btnClearCounts, stan);
        StylizujB(btnStopProduction, stan, colStop, colStopText);

        // Przycisk odśwież (aktywny tylko gdy nie ma połączenia)
        StylizujB(btnRefresh, !stan);
        
        // Przycisk pełnego ekranu (zawsze aktywny)
        StylizujB(btnFullScreen, true, colActive, colTextActive);
        
        // Przyciski nawigacji (zawsze aktywne)
        StylizujB(btnGoToInfo1, true, colNav, Color.Black);
        StylizujB(btnGoToInfo2, true, colNav, Color.Black);
        StylizujB(btnGoToSettings, true, colNav, Color.Black);
        StylizujB(btnGoToMain, true, colNav, Color.Black);

        // Porty
        cmbPorts.Enabled = !stan;
        cmbPorts.BackColor = !stan ? Color.White : colDisabled;
        cmbBaud.Enabled = !stan;
        cmbBaud.BackColor = !stan ? Color.White : colDisabled;
        
        // Pola tekstowe (NAPRAWA PUNKTU 3 - RAMKI)
        StylizujT(txtDataInput, stan);
        StylizujT(txtDistanceInput, stan);
        StylizujT(txtWidthInput, stan);


        Color tloSensors = (stan && isSensorError) ? Color.IndianRed : 
                   (stan && isCheckSensorsActive) ? Color.DarkGoldenrod : colActive;
        StylizujB(btnCheckSensors, stan, tloSensors);
    }
}