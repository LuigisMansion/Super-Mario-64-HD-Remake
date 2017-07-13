using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMaster : MonoBehaviour {
    public bool DebugGUI;

		private int redCoinStarCount;
		private int raceSoundCount = 0;

    public float FadeInTime = 0.3f;
	public float StarAniTime = 4f;
    public float ExitFadeOutTime = 0.5f;

		public bool raceStarted;
		public bool raceEnded = false;
		public bool marioWon;

	public GameObject Appear;

    public AudioSource MusicSource;
    public AudioSource SoundSource;

    public MarioInput MarioIn;
    public KeybindingInputHandler Handler;

    public GameObject DefaultInputs;

    public GameObject PauseMenu;
    public GameObject PauseCamera;
	public AudioClip raceFanfare;
    public AudioClip raceMusic;
    public AudioClip CoinSound;
	public AudioClip RedSound;
	public AudioClip StarSound;
	public AudioClip StarAppear;
    public AudioClip PauseSound;
    public AudioClip BowserLaugh;
    public MatteFade WhiteMatte;
    public MatteFade BlackMatte;
    public BowserMask DeathMask;
    public Text FramerateText;
    public int GoldMarioCoinAmount;

    private int currentCoins = 0;
	public int currentRed = 0;
    private CoinTextHandler coinTextHandler;

    private bool paused;
	private bool addable;

    private InputManager inputManager;

    private Camera mainCamera;

    void Awake()
    {
        Application.targetFrameRate = 150;

				redCoinStarCount = 0;

				raceStarted = false;

        // There can be only one!
        if (GameObject.FindObjectsOfType<GameMaster>().Length > 1)
        {
            Debug.LogError("Multiple GameMaster components detected in the scene. Limit 1 GameMaster per scene");
        }

        coinTextHandler = GetComponent<CoinTextHandler>();

        coinTextHandler.UpdateValue(currentCoins);

        inputManager = GameObject.FindObjectOfType<InputManager>();

        if (inputManager == null)
        {
            inputManager = ((GameObject)Instantiate(DefaultInputs, Vector3.zero, Quaternion.identity)).GetComponent<InputManager>();
        }
        
        MarioIn.input = inputManager;
        Handler.input = inputManager;

        PauseMenu.SetActive(false);
        PauseCamera.SetActive(false);

        mainCamera = Camera.main;

        WhiteMatte.FadeIn(FadeInTime);

        if (!DebugGUI)
            FramerateText.gameObject.SetActive(false);

        MusicSource.timeSamples = (int)(1 * MusicSource.clip.frequency);
    }

    private float lastFrameRateUpdate;

    void Update()
    {
				print (currentRed);
		//Instantiate(Appear,new Vector3(59,3,67), Quaternion.identity);
				if (currentCoins >= GoldMarioCoinAmount && redCoinStarCount == 0)
		{
						Instantiate(Appear,new Vector3(59,3,67), Quaternion.Euler(new Vector3(270, 90, 90)));
			  SoundSource.PlayOneShot(StarAppear);
						redCoinStarCount+=1;
					
		}
				if (currentRed == 8)
				{
						Instantiate(Appear,new Vector3(57,10.26f,21.15f), Quaternion.Euler(new Vector3(270, 90, 90)));
						SoundSource.PlayOneShot(StarAppear);
						currentRed++;

				}
        if (inputManager.PauseDown())
        {
            if (paused)
                Unpause();
            else
                Pause();
        }

        if ((float)MusicSource.timeSamples / (float)MusicSource.clip.frequency > 140.7f)
        {
            MusicSource.timeSamples = (int)(72.775f * MusicSource.clip.frequency);
        }

        if (DebugGUI && SuperMath.Timer(lastFrameRateUpdate, 0.25f))
        {
            lastFrameRateUpdate = Time.time;
            FramerateText.text = (1.0f / Time.deltaTime).ToString("F0");
        }
    }

    bool[] wasPlaying;

    private void Pause()
    {
        AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

        wasPlaying = new bool[allSources.Length];

        for (int i = 0; i < allSources.Length; i++)
        {
            var source = allSources[i];

            if (source.isPlaying)
            {
                wasPlaying[i] = true;
                source.Pause();
            }
        }

        MarioIn.enabled = false;

        Time.timeScale = 0;
        paused = true;
        PauseMenu.SetActive(true);
        PauseCamera.SetActive(true);
        mainCamera.enabled = false;
        inputManager.UpdateKeyBindings();

        SoundSource.PlayOneShot(PauseSound);
    }

    private void Unpause()
    {
        AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

        for (int i = 0; i < allSources.Length; i++)
        {
            var source = allSources[i];

            if (wasPlaying[i])
            {
                source.Play();
            }
        }

        MarioIn.enabled = true;

        Time.timeScale = 1;
        paused = false;
        PauseMenu.SetActive(false);
        PauseCamera.SetActive(false);
        mainCamera.enabled = true;

        SoundSource.PlayOneShot(PauseSound);
    }

		private void pauseAudio(){
				AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

				wasPlaying = new bool[allSources.Length];

				for (int i = 0; i < allSources.Length; i++)
				{
						var source = allSources[i];

						if (source.isPlaying)
						{
								wasPlaying[i] = true;
								source.Pause();
						}
				}
		}

		private void resumeAudio(){
				AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

				for (int i = 0; i < allSources.Length; i++)
				{
						var source = allSources[i];

						if (wasPlaying[i])
						{
								source.Play();
						}
				}

		}

    public void ClosePauseMenu()
    {
        Unpause();
    }

    public void ExitToMainMenu()
    {
        StartCoroutine(ExitToMenu());
    }

    public void GameOver()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        MarioIn.enabled = false;

        yield return new WaitForSeconds(1.0f);

        SoundSource.PlayOneShot(BowserLaugh);

        DeathMask.PlayMask(1.5f);

        StartCoroutine(FadeOutMusic(1.7f));

        yield return new WaitForSeconds(1.5f);

        BlackMatte.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        Application.LoadLevel(0);
    }

    IEnumerator FadeOutMusic(float time)
    {
        float i = 0;

        float initialVolume = MusicSource.volume;

        while (i < 1)
        {
            MusicSource.volume = Mathf.Lerp(initialVolume, 0, i);

            i += Time.deltaTime / time;

            yield return 0;
        }
    }

		public void raceStart(){
				raceStarted = true;
				pauseAudio ();
				SoundSource.PlayOneShot(raceFanfare);
				SoundSource.clip=raceMusic;
				SoundSource.loop=true;
				SoundSource.Play ();
		}
		public void raceEnd(){
				if (raceSoundCount == 0 && raceStarted==true) {
						SoundSource.PlayOneShot (raceFanfare);
						raceSoundCount += 1;
				}
		}
	
    public void AddCoin()
    {
        currentCoins = Mathf.Clamp(currentCoins + 1, 0, 999);

        SoundSource.PlayOneShot(CoinSound);

        coinTextHandler.UpdateValue(currentCoins);

        if (currentCoins >= GoldMarioCoinAmount)
        {
			SoundSource.PlayOneShot(StarAppear);
			GameObject.FindObjectOfType<MarioMachine>().GoldMarioUpgrade();
			GameObject machine = ((GameObject)Instantiate(Appear,new Vector3(59,3,67), Quaternion.identity));
        }
    }
	public void AddStar()
	{
		SoundSource.PlayOneShot(StarSound);
		StartCoroutine(StarExit());
		
	}


    public void AddCoin(int coins)
    {
        StartCoroutine(AddMultipleCoins(coins));
    }

		public void addRed(int coins){
		   StartCoroutine(AddRedCoins(coins));
		}

    public void FadeWhiteMatteOut(float time)
    {
        WhiteMatte.FadeOut(time);
    }

    public void FadeWhiteMatteIn(float time)
    {
        WhiteMatte.FadeIn(time);
    }

    IEnumerator ExitToMenu()
    {
				currentRed = 0;

        Time.timeScale = 1;

        SoundSource.PlayOneShot(PauseSound);

        BlackMatte.FadeOut(ExitFadeOutTime);

        yield return new WaitForSeconds(ExitFadeOutTime);

        Application.LoadLevel("StarMenu");
    }

	IEnumerator StarExit()
	{
				currentRed = 0;

				AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

				wasPlaying = new bool[allSources.Length];

				for (int i = 0; i < allSources.Length; i++)
				{
						var source = allSources[i];

						if (source.isPlaying)
						{
								wasPlaying[i] = true;
								source.Pause();
						}
				}

				SoundSource.clip=raceMusic;
				SoundSource.loop=false;
				SoundSource.Stop ();

				MarioIn.enabled = false;

		Time.timeScale = 1;

		SoundSource.PlayOneShot(StarSound);

		yield return new WaitForSeconds(StarAniTime);

		BlackMatte.FadeOut(ExitFadeOutTime);

		yield return new WaitForSeconds(ExitFadeOutTime);

		Application.LoadLevel("StarMenu");
	}

    IEnumerator AddMultipleCoins(int coins)
    {
        int remainingCoins = coins;


        float delay = 0.02f;

        float i = 1.1f;

        while (remainingCoins > 0)
        {
            while (i < 1.0f)
            {
                i += Time.deltaTime / delay;

                yield return 0;
            }

            SoundSource.PlayOneShot(CoinSound);

            remainingCoins--;
            currentCoins = Mathf.Clamp(currentCoins + 1, 0, 999);

            coinTextHandler.UpdateValue(currentCoins);

            if (currentCoins == GoldMarioCoinAmount)
            {
                GameObject.FindObjectOfType<MarioMachine>().GoldMarioUpgrade();
            }

            i = 0;
        }
    }

		IEnumerator AddRedCoins(int coins)
		{
				int remainingCoins = coins;


				float delay = 0.02f;

				float i = 1.1f;

				while (remainingCoins > 0)
				{
						while (i < 1.0f)
						{
								i += Time.deltaTime / delay;

								yield return 0;
						}

						SoundSource.PlayOneShot(RedSound);

						remainingCoins--;
						currentCoins = Mathf.Clamp(currentCoins + 1, 0, 999);

						coinTextHandler.UpdateValue(currentCoins);

						if (currentCoins == GoldMarioCoinAmount)
						{
								GameObject.FindObjectOfType<MarioMachine>().GoldMarioUpgrade();
						}

						i = 0;
				}
		}
}
