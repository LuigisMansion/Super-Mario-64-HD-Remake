using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarMenu : MonoBehaviour {

    public GameObject DefaultInputs;

    public GameObject ControlsMenu;
    public MatteFade WhiteMatte;
    public MatteFade BlackMatte;
    public KeybindingInputHandler Handler;

    public float InitialWait = 1.0f;
    public float FadeInTime = 0.6f;
    public float FadeOutTime = 1.0f;

    public AudioClip EnterLevel;
    public AudioClip LetsAGo;
    public AudioClip ClickButton;

    public GameObject QuitButtonExe;
    public GameObject ControlsButtonExe;
    public GameObject ControlsButtonWebplayer;

    public string LevelName;
    public StarGraphic Star;

    private InputManager inputManager;

    public void Awake()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();

        if (!inputManager)
        {
            inputManager = ((GameObject)Instantiate(DefaultInputs, Vector3.zero, Quaternion.identity)).GetComponent<InputManager>();
        }
    }

    public void Start()
    {
        inputManager.UpdateKeyBindings();

        Handler.input = inputManager;

        ControlsMenu.SetActive(false);

        StartCoroutine(FadeIntoMenu());

        if (Application.isWebPlayer)
        {
            ControlsButtonWebplayer.SetActive(true);

            ControlsButtonExe.SetActive(false);
            QuitButtonExe.SetActive(false);
        }
    }

    IEnumerator FadeIntoMenu()
    {
        yield return new WaitForSeconds(InitialWait);

        BlackMatte.FadeIn(FadeInTime);

        yield return new WaitForSeconds(InitialWait * 2);

        WhiteMatte.FadeIn(FadeInTime);

        GetComponent<AudioSource>().Play();
    }

    void LoadLevel()
    {
        Application.LoadLevel(LevelName);
    }

    public void OpenControlsMenu()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickButton);

        ControlsMenu.SetActive(true);
    }

    public void CloseControlsMenu()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickButton);

        ControlsMenu.SetActive(false);
    }

    public void ExitGame()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickButton);

        Application.Quit();
    }

    public void StartLevel()
    {
        // Let's-a-Go!
        WhiteMatte.FadeOut(FadeOutTime);

        GetComponent<AudioSource>().PlayOneShot(EnterLevel);
        GetComponent<AudioSource>().PlayOneShot(LetsAGo);

        Star.StartLevelAnimation();

        Invoke("LoadLevel", FadeOutTime);
    }
}
