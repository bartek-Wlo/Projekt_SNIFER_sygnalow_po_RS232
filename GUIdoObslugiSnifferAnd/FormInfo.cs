using System.Drawing;
using System.Windows.Forms;

namespace GUIdoObslugiSnifferAnd;

public class FormInfo : Form
{
    public Action<string>? OnNavigate;
    private RichTextBox rtb;

    // Definicje kolorów z LaTeX
    private readonly Color appTeal = Color.FromArgb(0, 153, 153);
    private readonly Color appRed = Color.FromArgb(177, 39, 39);
    private readonly Color appGreen = Color.FromArgb(0, 230, 118);
    private readonly Color appBlue = Color.Blue;

    public FormInfo()
    {
        this.Text = "Dokumentacja Systemu";
        this.Size = new Size(1000, 850);
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.WindowState = FormWindowState.Maximized;
        this.StartPosition = FormStartPosition.CenterScreen;

        rtb = new RichTextBox 
        { 
            Dock = DockStyle.Fill, 
            ReadOnly = true, 
            BackColor = Color.White,
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.None,
            Padding = new Padding(20)
        };

        FillDocumentContent();

        TableLayoutPanel nav = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 50, ColumnCount = 2 };
        nav.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        nav.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        Button btnSet = new Button { Text = "PANEL: ustawienia", Dock = DockStyle.Fill };
        btnSet.Click += (s, e) => { OnNavigate?.Invoke("SETTINGS"); this.Close(); };
        
        Button btnMain = new Button { Text = "PANEL: główny", Dock = DockStyle.Fill };
        btnMain.Click += (s, e) => { OnNavigate?.Invoke("MAIN"); this.Close(); };

        nav.Controls.Add(btnSet, 0, 0);
        nav.Controls.Add(btnMain, 1, 0);

        this.Controls.Add(rtb);
        this.Controls.Add(nav);
    }

    private void AppendText(string text, Font font, Color color, bool newLine = true)
    {
        rtb.SelectionStart = rtb.TextLength;
        rtb.SelectionLength = 0;
        rtb.SelectionFont = font;
        rtb.SelectionColor = color;
        rtb.AppendText(text + (newLine ? "\n" : ""));
    }

    private void FillDocumentContent()
    {
        var fTitle = new Font("Segoe UI", 18, FontStyle.Bold);
        var fTitleSub = new Font("Segoe UI", 14, FontStyle.Bold);
        var fSec = new Font("Segoe UI", 13, FontStyle.Bold);
        var fSubSec = new Font("Segoe UI", 11, FontStyle.Bold);
        var fBold = new Font("Segoe UI", 11, FontStyle.Bold);
        var fNormal = new Font("Segoe UI", 11, FontStyle.Regular);
        var fCode = new Font("Consolas", 11, FontStyle.Regular);

        // --- TYTUŁ ---
        AppendText("Dokumentacja obsługi SNIFERA realizującego operacje AND na sygnałach z anteny UHF i HF.", fTitle, appTeal);
        AppendText("System monitorowania pracy Sniffer MEGA2560", fTitleSub, appTeal);
        AppendText($"Data: {DateTime.Now.ToShortDateString()}", fNormal, Color.Gray);
        AppendText("\n" + new string('_', 60) + "\n", fNormal, Color.LightGray);

        // --- SEKCJA 1 ---
        AppendText("1 Pierwsze Uruchamianie", fSec, Color.Black);
        AppendText("  1. Wgranie programu na MEGA2560 (kabel USB B - USB a)", fNormal, Color.Black);
        AppendText("  2. Popranie Aplikacji sterującej Terminal_Sterowania_MEGA2560_do_czujnikow_HF_UHF.EXE", fNormal, Color.Black);
        AppendText("  3. zainstalowanie sterowników PL2303_driver", fNormal, Color.Black);
        AppendText("  4. Podłączenie Płytki roz. 2.", fNormal, Color.Black);

        // --- SEKCJA 2 ---
        AppendText("\n2 Podłączenie płytki MEGA2560 do maszyny", fSec, Color.Black);
        AppendText("  1. Sprawdzić czy wszystkie kabelki połączeniowe wewnątrz są podłączone", fNormal, Color.Black);
        AppendText("  2. Podłączyć kabel zasilający do płytki. ", fNormal, Color.Black, false);
        AppendText("NIE WOLNO PODŁĄCZAĆ JEDNOCZEŚNIE kabla zasilającego oraz kabla USB A - USB B do płytki", fBold, appRed);
        AppendText("  3. Należy zidentyfikować kable:", fNormal, Color.Black);
        AppendText("     • KABEL PC UHF (04)", fNormal, Color.Black);
        AppendText("     • KABEL PC HF (02)", fNormal, Color.Black);
        AppendText("     • KABEL READER UHF (03)", fNormal, Color.Black);
        AppendText("     • KABEL READER HF (01)", fNormal, Color.Black);
        AppendText("     Oraz odpowiadające im wejścia w MEGA2560 o takiej samej numeracji. Należy podłączyć kabel od READERów (01) i (03) oraz PC (02). Kabel od PC UHF pozostawić nie podłączony", fNormal, Color.Black);
        AppendText("  4. należy podłączyć kabel RS232 - USB A do urządzenia nadzorczego (tablet z programem) oraz portu COM (05)", fNormal, Color.Black);

        // --- SEKCJA 2.1 ---
        AppendText("\n2.1 Inicjalizacja UHF", fSubSec, Color.Black);
        AppendText("Stosować w przypadku: podłączenia białego kabla RS232 (żęńsko-żęński) do kabla PC UHF. Operacja powinna polegać na odłączeniu MODEM 0 od MEGA2560 i połączenia go z białym kablem. Wykonać połączenie w kolejności:", fNormal, Color.Black);
        AppendText("  • Kabel PC UHF", fNormal, Color.Black);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("Biały kabel RS232 (żęńsko-żęński)", fNormal, appBlue);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("MODEM 0. Krótki kolorowy kabelek (kilka przewodów połączonych szarymi wtyczkami) (żeńsko-męski)", fNormal, appBlue);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("Czarny kabel (męsko-męski)", fNormal, appBlue);
        AppendText("  • Kabel READER UHF", fNormal, Color.Black);
        AppendText("Gdy nie ma białego kabla RS232 stosować bez zaznaczonych, na niebiesko w tekście, przewodów", fNormal, appBlue);

        // --- SEKCJA 3 ---
        AppendText("\n3 Uruchamianie rutynowe", fSec, Color.Black);
        AppendText("  1. Wybrać BOUND: 38400 8N1 (domyślny)", fNormal, Color.Black);
        AppendText("  2. Wybrać COM_, gdzie _ to numer podłączonego COM", fNormal, Color.Black);
        AppendText("  3. Jeżeli nie ma do wyboru żadnego COM -> wcisnąć przycisk: [Odśwież Proty]", fNormal, Color.Black);
        AppendText("  4. Przy wybranym COM i BOUND nawiązać połączenie, przycisk ", fNormal, Color.Black, false); AppendText("[Połącz]", fBold, appRed);
        AppendText("  5. Sprawdzić czy jest połączenie pomiędzy aplikacją a SNIFEREM MEGA2560. W tym celu wcisnąć ", fNormal, Color.Black, false);
        AppendText("zielony", fNormal, appGreen, false); AppendText(" przycisk po prawej [Sprawdzanie komunikacji z czujnikami]. Przycisk po wciśnięciu zmieni barwę na żółtą. Po otrzymaniu informacji zwrotnej od MEGA2560 zmieni kolor na ", fNormal, Color.Black, false);
        AppendText("zielony", fNormal, appGreen, false); AppendText(" (Komunikacja z czujnikami działa) na ", fNormal, Color.Black, false);
        AppendText("czerwony", fNormal, appRed, false); AppendText(" (Któryś z czujników nie odpowiedział). ZAWSZE WYKONYWAĆ DWIE PRÓBY POŁĄCZENIA Z PŁYTKĄ", fBold, Color.Black);
        AppendText("  6. Kontynuować jeżeli przycisk sprawdzania komunikacji jest zielony (Po podwójnym sprawdzeniu).", fNormal, Color.Black);
        AppendText("  7. Ustawić odległość pomiędzy czujnikami: LICZBA ANTEN POMIĘDZY CZUJNIKAMI ", fBold, Color.Black, false); AppendText("+ 2.", fBold, appRed);
        AppendText("  8. Zakres odległości 2 - 64. Przycisk powinien przybrać zielony kolor po wciśnięciu.", fNormal, Color.Black);
        AppendText("  9. Znacjonalizować czujnik:", fNormal, Color.Black);
        AppendText("     a) METODA APLIKACJI: wcisnąć przycisk [Czujnik UHF init], po wciśnięciu zmieni barwę na czerwoną do momentu zainicjalizowania czujnika UHF. Kontynułować gdy przycisk jest zielony.", fNormal, Color.Black);
        AppendText("     b) METODA KLASYCZNA: Podłączyć kable zgodnie z roz. 2.1. Zainicjalizować maszynę za pomocą oryginalnego terminala.", fNormal, Color.Black);
        AppendText("  10. Zmienić panel za pomocą ", fNormal, Color.Black, false); AppendText("niebieskiego", fNormal, appBlue, false); AppendText(" przycisku na PANEL: GŁÓWNY", fNormal, Color.Black);
        AppendText("  11. (Opcjonalne) Dostosować szerokość MATRIX do szerokości na oryginalnym terminalu.", fNormal, Color.Black);
        AppendText("  12. Zależnie od potrzeb korzystać z: STOP (zatrzymuje produkcje, jeżeli trwa produkcja), (Wyczyść matrix) czyści dane w białej części aplikacji. (Wyczyść liczniki) zeruje liczniki UHF i HF: złych i dobrych anten.", fNormal, Color.Black);

        // --- SEKCJA 4 ---
        AppendText("\n4.1 Wizualizacja danych (Matrix i Bufor)", fSubSec, Color.Black);
        AppendText("  • Bufor drogi: Pasek reprezentujący fizyczną odległość między anteną UHF a HF. Pokazuje stan anten znajdujących się „w drodze”.", fNormal, Color.Black);
        AppendText("  • Matrix: Trzy wiersze (UHF, HF, AND) pokazujące historię ostatnich testów. Szerokość macierzy można dostosować w polu Szerokość Matrix.", fNormal, Color.Black);

        AppendText("\nLegenda kolorów anten", fSubSec, Color.Black);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("Szary", fBold, Color.Gray, false); AppendText(" – Brak danych", fNormal, Color.Black);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("Zielony", fBold, appGreen, false); AppendText(" – Antena Dobra. (jeden test)", fNormal, Color.Black);
        AppendText("  • ", fNormal, Color.Black, false); AppendText("Czerwony", fBold, appRed, false); AppendText(" – Antena Zła (jeden test).", fNormal, Color.Black);
        AppendText("  • Czerwono-Zielony – Antena Dobra (po dwóch testach. Pierwszy zakończony wykryciem złej anteny)", fBold, Color.Black);
        AppendText("  • Podwójny Czerwony – Antena Zła. (Po dwóch negatywnych testach)", fBold, Color.Black);
        AppendText("TĄ LOGIKĘ MOŻNA UPROŚCIĆ DO: DOMINUJĄCY KOLOR OZNACZA STAN KOŃCOWY ANTENY Czerwone złe, zielone dobre.", fBold, Color.Black);

        // --- SEKCJA 5 ---
        AppendText("\n4.2 Przyciski specjalne i diagnostyka", fSubSec, Color.Black);
        AppendText("  • Wiadomości do UHF / HF / PC UHF / PC HF", fNormal, Color.Black);
        AppendText("  • Pole do wpisywania wiadomości: \"Wpisane tam wartości, np. 123456 wysyłane są jako wartości binarne HEX 0x12 0x34 0x56\"", fNormal, Color.Black);
        AppendText("  • Wyślij dane (Wysyła zawartość pola do wpisywania wiadomości) Wysyła wiadomość do MEGA2560.", fNormal, Color.Black);

        // --- SEKCJA 6 ---
        AppendText("\n5 Komunikacja techniczna (Protokół)", fSec, Color.Black);
        AppendText("Aplikacja wykorzystuje ramki o stałej długości 8 bajtów:", fNormal, Color.Black);
        AppendText("[0x55] [CMD] [DATA1] [DATA2] [DATA3] [CRC_L] [CRC_H] [0xCC]", fCode, Color.DarkBlue);
        AppendText("Suma kontrolna: CRC-16/MCRF4XX.", fNormal, Color.Black);

        // --- SEKCJA 7 ---
        AppendText("\n6 Rozwiązywanie problemów", fSec, Color.Black);
        AppendText("  • Błąd „Niespójność danych” / „Niekompletna sekwencja”: Powtórzyć przesłanie bufora.", fNormal, Color.Black);
        AppendText("  • Status: MEGA2560 odpowiada, czujniki: czekam na odpowiedz: Traktować jako błąd powyżej 15s.", fNormal, Color.Black);
        AppendText("  • Brak portów COM: Podłącz USB, sprawdź sterowniki PL2303.", fNormal, Color.Black);
        AppendText("  • Błąd UHF i Błąd HF: Sprawdzić podłączenia. Inicjalizacja wg roz. 2.1.", fNormal, Color.Black);
        AppendText("  • Poważny błąd UHF: Restart maszyny.", fNormal, Color.Black);
        AppendText("\nProblemy można rozwiązać przez fizyczny przycisk (czerwony) na płytce MEGA2560.", fBold, appRed);
    }
}