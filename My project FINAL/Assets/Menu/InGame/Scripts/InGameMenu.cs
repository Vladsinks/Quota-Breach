using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject Player;

    [SerializeField] private TMP_Text Sensivity_text;
    [SerializeField] private Scrollbar Sensivity_bar;

    [SerializeField] private TMP_Text SaveText;

    public float Money;
    public int RoomCount;

    float Sensivity;


    void Start()
    {
        menu.SetActive(false);
        Settings.SetActive(false);

        //Загрузка сенсы
        if (PlayerPrefs.HasKey("Sensivity")) Sensivity = PlayerPrefs.GetFloat("Sensivity");
        else Sensivity = 50f;
        Player.GetComponent<PlayerController>().mouseSensitivity = Sensivity;

        //Загрузка денег
        if (PlayerPrefs.HasKey("Money")) Money = PlayerPrefs.GetFloat("Money");
        else Money = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf)
            {
                ResumeButton();
            }
            
            else
            {
                menu.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Player.GetComponent<PlayerController>().enabled = false;
                Player.GetComponent<LidarVision>().enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F5)) Save();

        Sensivity_text.text = "Sensivity: " + (Sensivity_bar.value * 100f).ToString("F0");
    }

    public void MainMenuButton()
    {
        Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ExitGameButton()
    {
        Save();
        Application.Quit();
    }

    public void ResumeButton()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Player.GetComponent<PlayerController>().enabled = true;
        Player.GetComponent<LidarVision>().enabled = true;
        Settings.SetActive(false);

        Save();
    }

    public void SettingsButton()
    {
        if (Settings.activeSelf)
        {
            Settings.SetActive(false);
        }
        else
        {
            Settings.SetActive(true);
            Sensivity_bar.value = Player.GetComponent<PlayerController>().mouseSensitivity / 100;
        }
    }

    public void Save()
    {
        SaveText.gameObject.SetActive(true);
        Invoke("HideMessage", 2f);

        //Сохранение и применение сенсы
        Player.GetComponent<PlayerController>().mouseSensitivity = Sensivity_bar.value * 100;
        Sensivity = Sensivity_bar.value * 100;
        PlayerPrefs.SetFloat("Sensivity", Sensivity);

        //Сохранение денег
        PlayerPrefs.SetFloat("Money", Money);
    }

    void HideMessage()
    {
        SaveText.gameObject.SetActive(false);   
    }
}
