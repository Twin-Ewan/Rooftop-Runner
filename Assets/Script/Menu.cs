using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject loadingText, OptionsCanvas, MenuCanvas;
    [SerializeField] TextMeshProUGUI highscoreText;

    // Volume Variables
    [SerializeField] public Text VolumeText;
    [SerializeField] public Slider VolumeSlider;
    [SerializeField] AudioClip ding;
    AudioSource audioSource;
    bool volChange = false;

    bool goodGraphics = false;

    Resolution oldResolution;


    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("Highscore")) highscoreText.text = "Highscore: " + PlayerPrefs.GetFloat("Highscore").ToString();
        else highscoreText.gameObject.SetActive(false);
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

    public void BTN_Options()
    {
        OptionsCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
    }

    public void SliderUpdate()
    {
        if ((VolumeSlider.value) == 50) VolumeText.text = "Volume: Normal";
        else if ((VolumeSlider.value) == 100) VolumeText.text = "Volume: Maximum";
        else if ((VolumeSlider.value) == 0) VolumeText.text = "Volume: Minimum";
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

        print(PlayerPrefs.GetFloat("Volume"));
    }

    public void BTN_Exit()
    {
        Application.Quit();
    }
}
