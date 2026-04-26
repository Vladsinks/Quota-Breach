using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject AreYouSure, Error;

    void Start()
    {
        AreYouSure.SetActive(false);
        Error.SetActive(false);
    }

    void Update()
    {
        
    }

    public void MainSave()
    {
        
    }

    public void MainExitButton()
    {
        MainSave();
        Application.Quit();
    }

    public void MainNewGameButton()
    {
        if (PlayerPrefs.HasKey("Money")) AreYouSure.SetActive(true);
        else NewGame();
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("Money");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
    }

    public void MainContinueButton()
    {
        if(!PlayerPrefs.HasKey("Money")) Error.SetActive(true);
        else Continue();
    }

    public void Continue()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
    }
}
