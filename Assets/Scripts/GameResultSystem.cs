using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultSystem : MonoBehaviour
{
    [Header("Balans Punktów")]
    public float punktyZaCelnosc = 500f;
    public float punktyZaSekunde = 10f;

    [Header("Progi Wypłaty")]
    public int progZloto = 600;
    public int progSrebro = 300;

    public void ObliczWynikIKonczGre(float czasZostal, int liczbaBledow, int dlugoscTekstu, bool czyZdazylW_Czasie)
    {
        int zarobioneMonety = 0;

        // 1. Czy czas się skończył?
        if (!czyZdazylW_Czasie)
        {
            zarobioneMonety = 0; // Wkurzony
        }
        else
        {
            // --- MATEMATYKA ---
            if (dlugoscTekstu == 0) dlugoscTekstu = 1; 
            float celnosc = (float)(dlugoscTekstu - liczbaBledow) / dlugoscTekstu;
            if (celnosc < 0) celnosc = 0;

            float wynikZJakoszi = celnosc * punktyZaCelnosc;
            float wynikZCzasu = czasZostal * punktyZaSekunde;
            float wynikKoncowy = wynikZJakoszi + wynikZCzasu;

            // --- PROGI ---
            if (wynikKoncowy >= progZloto) zarobioneMonety = 100;      // Happy
            else if (wynikKoncowy >= progSrebro) zarobioneMonety = 50; // Neutral
            else zarobioneMonety = 25;                                 // Angry (bo słabo)
        }

        // --- ZAPIS I POWRÓT ---
        ClientManager.WygraneMonety = zarobioneMonety;
        ClientManager.WracamyZGry = true; // <--- NOWOŚĆ! Ważna flaga.

        SceneManager.LoadScene("MainMenu");
    }
}