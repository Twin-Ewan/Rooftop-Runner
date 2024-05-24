using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject loadingText, InstructionsCanvas, HighscoreCanvas, OptionsCanvas, MenuCanvas;
    [SerializeField] TextMeshProUGUI[] highscoreText;

    [SerializeField] Button instructionsBTN;

    // Volume Variables
    [SerializeField] public Text VolumeText;
    [SerializeField] public Slider VolumeSlider;
    [SerializeField] AudioClip ding;
    AudioSource audioSource;
    bool volChange = false;

    bool goodGraphics = false;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        
        // Checks to see if there's already highscores (aka have you played the game before)
        // and if not sets them automatically
        if (!PlayerPrefs.HasKey("Highscore0"))
        {
            PlayerPrefs.SetInt("Highscore0", 1000);
            PlayerPrefs.SetInt("Highscore1", 2500);
            PlayerPrefs.SetInt("Highscore2", 5000);
            PlayerPrefs.SetInt("Highscore3", 10000);
            PlayerPrefs.SetInt("Highscore4", 25000);

            // Changes the button's colour to highlight it
            instructionsBTN.image.color = Color.yellow;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && volChange)
        {
            volChange = false;
            audioSource.PlayOneShot(ding, VolumeSlider.value/50);
        }
    }

    public void BTN_Start()
    {
        SceneManager.LoadScene("Gameplay");
        loadingText.SetActive(true);
    }

    public void BTN_Highscores()
    {
        HighscoreCanvas.SetActive(true);
        MenuCanvas.SetActive(false);

        int i = 0;
        foreach(TextMeshProUGUI t in highscoreText) 
        {
            t.text = (-i + highscoreText.Length) + ": " + PlayerPrefs.GetInt("Highscore" + i).ToString();
            i++;
        }
        
    }

    public void BTN_ResetHighscores()
    {
        PlayerPrefs.SetInt("Highscore0", 1000);
        PlayerPrefs.SetInt("Highscore1", 2500);
        PlayerPrefs.SetInt("Highscore2", 5000);
        PlayerPrefs.SetInt("Highscore3", 10000);
        PlayerPrefs.SetInt("Highscore4", 25000);

        // Updates the list
        BTN_Highscores();
    }

    public void BTN_HighscoreExit()
    {
        MenuCanvas.SetActive(true);
        HighscoreCanvas.SetActive(false);
    }

    public void BTN_Instruct()
    {
        instructionsBTN.image.color = new Color(.9176f, .3607f, .5137f, 0.7843f); // #EA5C83, The usual colour

        MenuCanvas.SetActive(false);
        InstructionsCanvas.SetActive(true);
    }

    public void BTN_InstructExit()
    {
        MenuCanvas.SetActive(true);
        InstructionsCanvas.SetActive(false);
    }

    public void BTN_Options()
    {
        OptionsCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
    }

    public void SliderUpdate()
    {
        if ((VolumeSlider.value) == 50) VolumeText.text = "Volume: Normal";
        else if ((VolumeSlider.value) == 100) VolumeText.text = "Volume: Maximum";
        else if ((VolumeSlider.value) == 0) VolumeText.text = "Volume: Muted";
        else VolumeText.text = "Volume: " + (VolumeSlider.value).ToString() + "%";

        volChange = true;
    }

    public void BTN_OptionsRes()
    {
        if (goodGraphics)
        {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, 
                Screen.resolutions[Screen.resolutions.Length - 1].height,
                Screen.fullScreenMode);
        }
        else
        {
            Screen.SetResolution(640, 360, Screen.fullScreenMode);
        }
        goodGraphics = !goodGraphics;
    }

    public void BTN_OptionsExit()
    {
        PlayerPrefs.SetFloat("Volume", VolumeSlider.value/100);
        OptionsCanvas.SetActive(false);
        MenuCanvas.SetActive(true);
    }

    public void BTN_Exit()
    {
        Application.Quit();
    }
}
