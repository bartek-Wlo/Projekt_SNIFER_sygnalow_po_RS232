using System.Drawing.Drawing2D;

namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    private int headIndex = 0; // Wskazuje na najnowszy element w tablicy
    private const int TotalHistorySize = BufferLenght * 4; // 256

    private void RysujSekcje(Graphics g, int sectionIdx)
    {
        // 1. Podstawowe ustawienia graficzne
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Stałe wymiarowe (zgodne z Twoim oryginalnym projektem)
        const int circleSize = 20;
        const int overlapOffset = 4;
        const int spacingX = 35;
        const int spacingY = 25;
        const int offsetX = 50;
        const int offsetY = 5;

        // 2. Obliczanie zakresu danych dla tej sekcji (panelu)
        // logicznyStart to numer anteny w historii, od której zaczyna się ten panel
        int logicznyStart = sectionIdx * dotsPerRow;
        int TotalHistorySize = BufferLenght * 4; // Zakładamy 256 elementów

        string[] labels = { "UHF", "HF", "AND" };
        Font labelFont = new Font("Arial", 10, FontStyle.Bold);

        // 3. Rysowanie trzech wierszy (UHF, HF, AND)
        for (int r = 0; r < 3; ++r) 
        {
            // Rysowanie etykiet wierszy po lewej stronie
            int y = offsetY + (r * spacingY);
            g.DrawString(labels[r], labelFont, Brushes.Black, 5, y);

            for (int i = 0; i < dotsPerRow; i++)
            {
                // Obliczamy indeks logiczny (którą antenę z kolei rysujemy)
                int logicznyIndeks = logicznyStart + i;

                // Jeśli wyjdziemy poza całkowity rozmiar historii, przerywamy rysowanie
                if (logicznyIndeks >= TotalHistorySize) break;

                // Mapowanie indeksu logicznego na fizyczny w buforze cyklicznym
                // headIndex to najnowszy element (pozycja 0 w Current Matrix)
                int fizycznyIndeks = (headIndex + logicznyIndeks) % TotalHistorySize;

                // Obliczanie pozycji X na ekranie
                int x = offsetX + (i * spacingX);

                // Pobranie stanu z tablicy
                AntennaState stan = matrixData[r][fizycznyIndeks];

                // 4. Logika rysowania kółek (Twoje oryginalne style kolorystyczne)
                switch (stan)
                {
                    case AntennaState.Gray:
                        g.FillEllipse(Brushes.Gray, x, y, circleSize, circleSize);
                        break;
                    case AntennaState.Green:
                        g.FillEllipse(Brushes.Green, x, y, circleSize, circleSize);
                        break;
                    case AntennaState.Red:
                        g.FillEllipse(Brushes.Red, x, y, circleSize, circleSize);
                        break;
                    case AntennaState.RedGreen:
                        // Dwa zachodzące na siebie kolory
                        g.FillEllipse(Brushes.Red, x - overlapOffset, y, circleSize, circleSize);
                        g.FillEllipse(Brushes.Green, x + overlapOffset, y, circleSize, circleSize);
                        break;
                    case AntennaState.RedGray:
                        g.FillEllipse(Brushes.Gray, x + overlapOffset, y, circleSize, circleSize);
                        g.FillEllipse(Brushes.Red, x - overlapOffset, y, circleSize, circleSize);
                        break;
                    case AntennaState.DoubleRed:
                        g.FillEllipse(Brushes.DarkRed, x - overlapOffset, y, circleSize, circleSize);
                        g.FillEllipse(Brushes.Red, x + overlapOffset, y, circleSize, circleSize);
                        break;
                    case AntennaState.Error:
                    default:
                        g.FillEllipse(Brushes.Black, x, y, circleSize, circleSize);
                        break;
                }
            }
        }
    }    



    private void RysujBufor(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int rectWidth = 20;
        int rectHeight = 20;
        int spacingX = 25;
        int offsetX = 10;
        int offsetY = 10;

        for (int i = 0; i < aktualnaOdleglosc; i++) {
            Brush b = bufferData[i] switch {
                BufforState.Good => Brushes.Green,
                BufforState.Bad  => Brushes.Red, 
                _                => Brushes.Gray
            };
            g.FillRectangle(b, offsetX + (i * spacingX), offsetY, rectWidth, rectHeight);
            g.DrawRectangle(Pens.Black, offsetX + (i * spacingX), offsetY, rectWidth, rectHeight);
        }
    }
}