namespace GUIdoObslugiSnifferAnd;

partial class Form1
{
    // ==========================================================
    // SEKCJA KOLORYSTYKI
    // ==========================================================
    private Color colPanelBg        = ColorTranslator.FromHtml("#009999"); 
    private Color colDisabled       = ColorTranslator.FromHtml("#008080"); 
    private Color colActive         = ColorTranslator.FromHtml("#006666"); 
    private Color colUnimportant    = ColorTranslator.FromHtml("#008080"); 
    private Color colNav            = Color.LightSkyBlue;                  
    private Color colStop           = Color.Yellow;                        
    private Color colStopText       = Color.Red;                           
    private Color colLabelText      = Color.White;                         
    private Color colTextActive     = ColorTranslator.FromHtml("#FFFFFF"); 
    private Color colTextDisabled   = ColorTranslator.FromHtml("#006666"); 
    private Color colBorderActive   = ColorTranslator.FromHtml("#004D4D"); 
    private Color colBorderDisabled = ColorTranslator.FromHtml("#004D4D"); 
    private Color colPlaceholder    = Color.PaleTurquoise;                 
    // ==========================================================

    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel mainLayout, panelSettings, panelMain, matrixLayout;
    private ComboBox cmbPorts, cmbBaud;
    private Button btnRefresh, btnConnect, btnModeSniffer, btnModeAnd, btnSaveFile, btnSend, btnSetDistance, btnClearMatrix, btnClearCounts, btnCheckSensors;
    private Button btnFullScreen, btnToggleUHF, btnToggleHF, btnTogglePCUHF, btnTogglePCHF, btnInitBuffPCtoMEGA, btnInitBuffMEGAtoPC, btnUHFRestart, btnMEGARestart, btnStopProduction;
    private Button btnGoToSettings, btnGoToMain, btnGoToInfo1, btnGoToInfo2;
    private TextBox txtDataInput, txtDistanceInput, txtWidthInput;
    private Label lblDistanceInfo, lblGoodUHF, lblBadUHF, lblGoodHF, lblBadHF, lblLastMessage, lblErrorStatus;
    private BufferedPanel pnlMatrixBuffor, pnlMatrixCurrent, pnlMatrixPrev1, pnlMatrixPrev2, pnlMatrixPrev3;

    // PANELE-RAMKI DLA POL TEKSTOWYCH
    private Panel pnlBorderData, pnlBorderDistance, pnlBorderWidth;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    // Metoda pomocnicza do tworzenia ramki wokół TextBox
    private Panel StworzPoleZObramowaniem(TextBox t, int height)
    {
        Panel p = new Panel { 
            Dock = DockStyle.Fill, 
            Padding = new Padding(1), // Grubość ramki
            Margin = new Padding(2, (height / 4), 2, 2),
            BackColor = colBorderDisabled // Kolor ramki
        };
        t.BorderStyle = BorderStyle.None;
        t.Dock = DockStyle.Fill;
        p.Controls.Add(t);
        return p;
    }

    private void InitializeComponent()
    {
        this.mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, (guiElementHeight * 7) + 25)); 
        this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        InicjalizujPanelSettings();
        InicjalizujPanelMain();
        InicjalizujPanelMatrix();

        this.mainLayout.Controls.Add(matrixLayout, 0, 1);
        this.Controls.Add(mainLayout);
        this.BackColor = Color.White;
    }

    private void InicjalizujPanelSettings()
    {
        panelSettings = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 7, BackColor = colPanelBg, Padding = new Padding(5) };
        for (int i = 0; i < 4; i++) panelSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        for (int i = 0; i < 7; i++) panelSettings.RowStyles.Add(new RowStyle(SizeType.Absolute, guiElementHeight));

        Font labelFont = new Font("Arial", 12, FontStyle.Bold);

        panelSettings.Controls.Add(new Label { Text = "PANEL: USTAWIENIA", Font = labelFont, ForeColor = colLabelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        
        TableLayoutPanel portBaudContainer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        portBaudContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        portBaudContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        cmbBaud = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(2, (guiElementHeight / 4), 2, 0) };
        cmbBaud.Items.AddRange(new string[] { "38400 8N1", "38400 8E1", "115200 8N1" });
        cmbBaud.SelectedIndex = 0; // Domyślnie 38400 8N1

        cmbPorts = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(2, (guiElementHeight / 4), 2, 0) };
        portBaudContainer.Controls.Add(cmbBaud, 0, 0);
        portBaudContainer.Controls.Add(cmbPorts, 1, 0);
        panelSettings.Controls.Add(portBaudContainer, 1, 0);
        btnRefresh = new Button { Text = "Odśwież porty", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnRefresh, 2, 0);
        btnConnect = new Button { Text = "Połącz", Dock = DockStyle.Fill, BackColor = Color.IndianRed, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnConnect, 3, 0);

        btnModeSniffer = new Button { Text = "Tryb pracy SNIFFER", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnModeSniffer, 0, 1);
        btnModeAnd = new Button { Text = "Tryb pracy AND gate", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnModeAnd, 1, 1);
        btnSaveFile = new Button { Text = "Zapisz dane do pliku .txt", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnSaveFile, 2, 1);
        btnFullScreen = new Button { Text = "Tryb pełno ekranowy", Dock = DockStyle.Fill, Margin = new Padding(2), BackColor = colActive, Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = colTextActive, FlatStyle = FlatStyle.Flat };
        btnFullScreen.FlatAppearance.BorderColor = colBorderActive;
        panelSettings.Controls.Add(btnFullScreen, 3, 1);

        btnToggleUHF = new Button { Text = "Wiadomości do UHF", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnToggleUHF, 0, 2);
        btnToggleHF = new Button { Text = "Wiadomości do HF", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnToggleHF, 1, 2);
        btnTogglePCUHF = new Button { Text = "Wiadomości do PC UHF", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnTogglePCUHF, 2, 2);
        btnTogglePCHF = new Button { Text = "Wiadomości do PC HF", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnTogglePCHF, 3, 2);

        // TEXTBOX DATA + RAMKA
        txtDataInput = new TextBox { PlaceholderText = "Wpisane: np. 123456 WYSYŁA jako HEX 0x12 0x34 0x56" };
        pnlBorderData = StworzPoleZObramowaniem(txtDataInput, guiElementHeight);
        panelSettings.Controls.Add(pnlBorderData, 0, 3);

        btnSend = new Button { Text = "Wyślij dane", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnSend, 1, 3);
        lblLastMessage = new Label { Text = "Ostatnia Wiadomość: ---", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont, ForeColor = colLabelText };
        panelSettings.Controls.Add(lblLastMessage, 2, 3); panelSettings.SetColumnSpan(lblLastMessage, 2);

        // TEXTBOX DISTANCE + RAMKA
        txtDistanceInput = new TextBox { PlaceholderText = "Wprowadź odległość (2-64)" };
        pnlBorderDistance = StworzPoleZObramowaniem(txtDistanceInput, guiElementHeight);
        panelSettings.Controls.Add(pnlBorderDistance, 0, 4);

        btnSetDistance = new Button { Text = "Ustaw odległość", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnSetDistance, 1, 4);
        lblDistanceInfo = new Label { Text = "Odległość: 6", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont, ForeColor = colLabelText };
        panelSettings.Controls.Add(lblDistanceInfo, 2, 4);
        btnCheckSensors = new Button { Text = "Sprawdzenie komunikacji z czujnikami", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnCheckSensors, 3, 4);

        btnInitBuffPCtoMEGA = new Button { Text = "Init Buff (PC->MEGA)", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnInitBuffPCtoMEGA, 0, 5);
        btnInitBuffMEGAtoPC = new Button { Text = "Init Buff (MEGA->PC)", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnInitBuffMEGAtoPC, 1, 5);
        btnUHFRestart = new Button { Text = "Czujnik UHF init", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnUHFRestart, 2, 5);
        btnMEGARestart = new Button { Text = "MEGA2560 SNIFER Restart", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelSettings.Controls.Add(btnMEGARestart, 3, 5);
        lblErrorStatus = new Label { Text = "Errors: brak", ForeColor = ColorTranslator.FromHtml("#b12727"), Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, Font = new Font("Arial", 10, FontStyle.Bold) };
        panelSettings.Controls.Add(lblErrorStatus, 1, 6); 

        btnGoToInfo1 = new Button { Text = "Informacje o programie", Dock = DockStyle.Fill, BackColor = colNav, Margin = new Padding(2), Font = labelFont, FlatStyle = FlatStyle.Flat, ForeColor = Color.Black };
        btnGoToInfo1.FlatAppearance.BorderColor = colBorderActive;
        panelSettings.Controls.Add(btnGoToInfo1, 0, 6);
        btnGoToMain = new Button { Text = "PANEL: GŁÓWNY", Dock = DockStyle.Fill, BackColor = colNav, Margin = new Padding(2), Font = labelFont, FlatStyle = FlatStyle.Flat, ForeColor = Color.Black };
        btnGoToMain.FlatAppearance.BorderColor = colBorderActive;
        panelSettings.Controls.Add(btnGoToMain, 3, 6);
    }

    private void InicjalizujPanelMain()
    {
        panelMain = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3, BackColor = colPanelBg, Padding = new Padding(5) };
        for (int i = 0; i < 4; i++) panelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        for (int i = 0; i < 3; i++) panelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, guiElementHeight));

        Font labelFont = new Font("Arial", 12, FontStyle.Bold);

        panelMain.Controls.Add(new Label { Text = "PANEL: GŁÓWNY", Font = labelFont, ForeColor = colLabelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        
        // TEXTBOX WIDTH + RAMKA
        txtWidthInput = new TextBox { PlaceholderText = "Ustaw szerokość Matrix: 23" };
        pnlBorderWidth = StworzPoleZObramowaniem(txtWidthInput, guiElementHeight);
        panelMain.Controls.Add(pnlBorderWidth, 1, 0);

        btnClearMatrix = new Button { Text = "Wyczyść matrix", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelMain.Controls.Add(btnClearMatrix, 2, 0);
        btnClearCounts = new Button { Text = "Wyczyść liczniki", Dock = DockStyle.Fill, Margin = new Padding(2), FlatStyle = FlatStyle.Flat };
        panelMain.Controls.Add(btnClearCounts, 3, 0);

        lblGoodUHF = new Label { Text = "Dobre UHF: 0", ForeColor = ColorTranslator.FromHtml("#00E676"), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont };
        panelMain.Controls.Add(lblGoodUHF, 0, 1);
        lblBadUHF = new Label { Text = "Złe UHF: 0", ForeColor = ColorTranslator.FromHtml("#b12727"), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont };
        panelMain.Controls.Add(lblBadUHF, 1, 1);
        lblGoodHF = new Label { Text = "Dobre HF: 0", ForeColor = ColorTranslator.FromHtml("#00E676"), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont };
        panelMain.Controls.Add(lblGoodHF, 2, 1);
        lblBadHF = new Label { Text = "Złe HF: 0", ForeColor = ColorTranslator.FromHtml("#b12727"), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = labelFont };
        panelMain.Controls.Add(lblBadHF, 3, 1);

        btnGoToInfo2 = new Button { Text = "Informacje o programie", Dock = DockStyle.Fill, BackColor = colNav, Margin = new Padding(2), Font = labelFont, FlatStyle = FlatStyle.Flat, ForeColor = Color.Black };
        btnGoToInfo2.FlatAppearance.BorderColor = colBorderActive;
        panelMain.Controls.Add(btnGoToInfo2, 0, 2);
        btnStopProduction = new Button { Text = "STOP", Dock = DockStyle.Fill, BackColor = colStop, ForeColor = colStopText, Margin = new Padding(2), Font = new Font("Arial", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
        btnStopProduction.FlatAppearance.BorderColor = colBorderActive;
        panelMain.Controls.Add(btnStopProduction, 1, 2);
        btnGoToSettings = new Button { Text = "PANEL: USTAWIENIA", Dock = DockStyle.Fill, BackColor = colNav, Margin = new Padding(2), Font = labelFont, FlatStyle = FlatStyle.Flat, ForeColor = Color.Black };
        btnGoToSettings.FlatAppearance.BorderColor = colBorderActive;
        panelMain.Controls.Add(btnGoToSettings, 3, 2);
    }

    private void InicjalizujPanelMatrix()
    {
        matrixLayout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, ColumnCount = 1, BackColor = Color.White };
        pnlMatrixBuffor = StworzSekcje(matrixLayout, "Bufor", 50);
        pnlMatrixCurrent = StworzSekcje(matrixLayout, "Current Test Matrix", 90);
        pnlMatrixPrev1 = StworzSekcje(matrixLayout, "Previous Matrix", 90);
        pnlMatrixPrev2 = StworzSekcje(matrixLayout, "Second Previous Matrix", 90);
        pnlMatrixPrev3 = StworzSekcje(matrixLayout, "Third Previous Matrix", 90);
    }

    private BufferedPanel StworzSekcje(TableLayoutPanel parent, string tytul, int h) 
    {
        parent.RowCount++;
        parent.RowStyles.Add(new RowStyle(SizeType.Absolute, h + 25)); 
        
        // Panel kontener – wyłączamy mu Dock.Fill, żeby mógł być szerszy niż ekran
        Panel container = new Panel { 
            Height = h + 25, 
            Margin = new Padding(0), 
            Padding = new Padding(5),
            Width = 1000 // Szerokość tymczasowa, zostanie nadpisana
        };
        
        Label l = new Label { Text = tytul, Font = new Font("Arial", 11, FontStyle.Bold), Dock = DockStyle.Top, Height = 20 };
        
        // Nasz panel do rysowania – on musi mieć konkretną szerokość
        BufferedPanel bp = new BufferedPanel { 
            Dock = DockStyle.Fill // Wypełnia kontener
        };
        
        container.Controls.Add(bp); 
        container.Controls.Add(l);
        parent.Controls.Add(container);
        return bp;
    }
}