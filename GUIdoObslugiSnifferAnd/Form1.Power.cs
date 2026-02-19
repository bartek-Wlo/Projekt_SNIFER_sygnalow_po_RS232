using System.Runtime.InteropServices;

namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    // Import funkcji z biblioteki systemowej Windows (kernel32.dll)
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern uint SetThreadExecutionState(uint esFlags);

    // Flagi sterujące zasilaniem
    private const uint ES_CONTINUOUS = 0x80000000;       // Stan ciągły (obowiązuje do odwołania)
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;  // Zapobiega uśpieniu systemu
    private const uint ES_DISPLAY_REQUIRED = 0x00000002; // Zapobiega wygaszeniu ekranu

    /// <summary>
    /// Metoda blokuje wygaszanie ekranu i usypianie komputera tak długo, jak aplikacja działa.
    /// </summary>
    private void ZablokujWygaszanie()
    {
        // Ustawiamy flagi: Ciągłość + Wymagany System + Wymagany Ekran
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    }

    /// <summary>
    /// (Opcjonalnie) Przywraca normalne zarządzanie energią. 
    /// Windows i tak robi to automatycznie po zamknięciu aplikacji.
    /// </summary>
    private void PrzywrocWygaszanie()
    {
        SetThreadExecutionState(ES_CONTINUOUS);
    }
}