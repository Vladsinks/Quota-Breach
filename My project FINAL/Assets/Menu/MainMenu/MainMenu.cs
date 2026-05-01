using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject AreYouSure, Error, NewGameSettings, Settings, Buttons;
    [SerializeField] private TMP_InputField SeedField;
    int seed;

    [SerializeField] private TMP_Text Sensivity_text;
    [SerializeField] private Scrollbar Sensivity_bar;
    float Sensivity;

    

    void Start()
    {
        AreYouSure.SetActive(false);
        Error.SetActive(false);
        NewGameSettings.SetActive(false);
        Settings.SetActive(false);

        //Загрузка сенсы
        if (PlayerPrefs.HasKey("Sensivity")) Sensivity = PlayerPrefs.GetFloat("Sensivity");
        else Sensivity = 50f;
    }

    void Update()
    {
        Sensivity_text.text = "Sensivity: " + (Sensivity_bar.value * 100f).ToString("F0");
    }

    public void MainSave()
    {
        Sensivity = Sensivity_bar.value * 100;
        PlayerPrefs.SetFloat("Sensivity", Sensivity);
    }

    public void MainExitButton()
    {
        MainSave();
        Application.Quit();
    }

    public void MainNewGameButton()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            AreYouSure.SetActive(true);
            Buttons.SetActive(false);
        }
        else
        {
            NewGameSettings.SetActive(true);
            Buttons.SetActive(false);
        }
    }

    public void NewGame()
    {
        if (int.TryParse(SeedField.text, out int result))
        {
            string RawSeed = SeedField.text;
            seed = int.Parse(RawSeed);
        }
        else
        {
            seed = Random.Range(0, 999999);
        }
        PlayerPrefs.DeleteKey("Money");
        PlayerPrefs.SetInt("seed", seed);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GenerationTest");
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

    public void SettingsButton()
    {
        Settings.SetActive(true);
        Sensivity_bar.value = Sensivity / 100;
    }
}
