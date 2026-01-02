using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Wrzuc tu obiekt menu_pauzy z hierarchii")]
    public GameObject panelOpcji; // Nowa zmienna!

    void Update()
    {
        // Nasłuchiwanie klawisza ESC w każdej klatce
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelOpcji != null)
            {
                // To jest sprytny trik:
                // Sprawdź czy jest włączony. Jak tak -> wyłącz. Jak nie -> włącz.
                // Wykrzyknik oznacza "zaprzeczenie".
                panelOpcji.SetActive(!panelOpcji.activeSelf);
            }
        }
    }

    public void UruchomGre()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void WylaczGre()
    {
        Application.Quit();
        Debug.Log("Wychodzenie z gry...");
    }
}