namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private int guiElementHeight = 45; 
    private bool isFullScreen = false;


    private void PrzelaczPanel(TableLayoutPanel panel)
    {
        panelMain.Visible = false;
        panelSettings.Visible = false;
        
        mainLayout.Controls.Remove(panelMain);
        mainLayout.Controls.Remove(panelSettings);

        int rowCount = (panel == panelSettings) ? 7 : 3;
        int nowaWysokosc = (rowCount * guiElementHeight) + 15;
        
        mainLayout.RowStyles[0].Height = nowaWysokosc;
        
        panel.Visible = true;
        mainLayout.Controls.Add(panel, 0, 0);
    }

    private void ObslugaTrybuFullScreen()
    {
        if (!isFullScreen)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            isFullScreen = true;
            btnFullScreen.Text = "Tryb okienkowy";
        }
        else
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            isFullScreen = false;
            btnFullScreen.Text = "Tryb pełno-ekranowy";
        }
    }

    private void PokazOknoInfo()
    {
        FormInfo info = new FormInfo();
        info.OnNavigate += (target) => {
            if (target == "MAIN") PrzelaczPanel(panelMain);
            else if (target == "SETTINGS") PrzelaczPanel(panelSettings);
        };
        info.Show();
    }
}