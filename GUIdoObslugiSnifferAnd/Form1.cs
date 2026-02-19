namespace GUIdoObslugiSnifferAnd;
/**
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true --output ./naped_portable
**/

public partial class Form1 : Form
{
    private static int dotsPerRow = 23;
    private static int aktualnaOdleglosc = 6;

    private static int indexHF_AND = 0;
    private static int indexUHF = indexHF_AND + aktualnaOdleglosc;

    public Form1() {
        InitializeComponent();
        this.Text = "Terminal Sterowania MEGA2560 - monitorowanie pracy sniffer";
        UstawIkoneZasobow();
        UstawRozmiarIPozycje();
        InicjalizujDane();
        AktualizujStanKontrolek(false);
        OdswiezPorty();
        RysujPodgladBuforMatrix();
        ObslugaButton();
        ObslugaSzerokosci(); 

        PrzelaczPanel(panelSettings);
    }

}


public class BufferedPanel : Panel
{
    public BufferedPanel() { this.DoubleBuffered = true; }
}
