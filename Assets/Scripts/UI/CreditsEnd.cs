using UnityEngine;

public class CreditsEnd : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject mainMenuPanel;
    public GameObject gameName;
    public float delay = 10f;

    void Start()
    {
        Invoke("ShowMainMenu", delay);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            ShowMainMenu();
        }
    }

    void ShowMainMenu()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        gameName.SetActive(true);
    }
}
