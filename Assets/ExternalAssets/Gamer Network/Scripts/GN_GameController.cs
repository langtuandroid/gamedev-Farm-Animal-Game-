using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GN_GameController : MonoBehaviour {

	[Header ("Scene Selection")]
	public Scenes PreviousScene;
	public Scenes NextScene;

	[Header ("Game Dialogues")]
	public Game_Dialogues Game_Elements;
	public GameObject carController;

	[Header ("SFX Objects")]
	public SFX_Objects SFX_Elements;

	[Header ("Level Information")]
	public int PlayableLevels = 10;
	public Level_Data[] Levels;
	[Header ("Gameover States")]
	public bool ReasonBased;
	[Tooltip ("Gameover information is optional. This will not appear if un-checked.")]
	public GameOver[] States;
	public Image starSlider;
	public Image completedSlider;
	public int[] LevelBonus;
	public Text LevelBonusText, TimeBonusText, totalCoins;

	[Header ("Level End Delay")]
	public float GameWinDelay;
	public float GameLooseDelay;
	
	[Header ("Player cars")]
	public GameObject[] playableCars;
    public GameObject carCamera;
	public Sprite[] playableSteerings;
	public GameObject playerSteering;
	private Vector3 carLastPosition = Vector3.zero;
	private Quaternion carLastRotation = new Quaternion(0,0,0,0);
	private bool toggle;
	private bool revive, doubleCoins = false;

	//Local Variables
	GameObject PlayerMain;
	GameObject AudioSource_Parent;
	GameObject FX_AudioSource;
	//Timer
	int minutes;
	int seconds;
	string time;
	int reward;
	float TimePerct;
	private int currentLevel;
	private int currentPlayer;
	private int FinishCount = 0;
	private bool isTimerEnabled;
	private int Rewardamount = 0;
	[HideInInspector]
	public bool TimerPaused = false;

	#region debug

	[Header ("Current Level")]
	[Range (1, 20)]
	public int StartLevel = 1;
	[Header ("Debug Values")]
	public int ObjectivesLeft = 0;
	public float LevelTime = 0.0f;
	public float OriginalTime = 0.0f;
	private int tempTimeBonus = 0;

	#endregion

	private void OnEnable()
	{
        /*if (AdsManager.instance )
        {
            AdsManager.instance.rewardedDelegate += OnRewardedVideoComplete;
        }*/
	}

	private void OnDisable()
	{
        /*if (AdsManager.instance)
        {
            AdsManager.instance.rewardedDelegate -= OnRewardedVideoComplete;
        }*/
    }

	private void Awake()
	{
	}

	void Start () {
		//GameManager Variables Reset
		GameManager.Instance.GameStatus = null;

		Time.timeScale = 1;

		//Initialize Framework
		setControls();
		Init ();
	}

	#region Initialization

	void Init () {
		if (!GameManager.Instance.Initialized) {
			InitializeGame ();
		}
		SpecifyCurrentLevel ();
		InitializeLevel ();
		ActivateLevel ();
		LevelSpecificDecisions ();
	}

	void InitializeAudio (GameObject obj, string name) {
		AudioSource_Parent = GameObject.Find ("SFXController");
		obj = new GameObject (name);
		obj.transform.position = AudioSource_Parent.transform.position;
		obj.transform.rotation = AudioSource_Parent.transform.rotation;
		obj.transform.parent = AudioSource_Parent.transform;
		obj.AddComponent<AudioSource> ();
		obj.GetComponent<AudioSource> ().priority = 128;
	}

	void InitializeGame () {
		SaveData.Instance = new SaveData ();
		GN_SaveLoad.LoadProgress ();
		GameManager.Instance.Initialized = true;
	}
	public void setControls(){
		if (GameManager.Instance.steeringControl)
		{
			RCC_SettingsData.InstanceR.mobileControllerObject = RCC_SettingsData.MobileController.SteeringWheel;
			toggle = false;
		}else if (GameManager.Instance.arrowControl)
		{
			RCC_SettingsData.InstanceR.mobileControllerObject = RCC_SettingsData.MobileController.TouchScreen;
			toggle = true;
		}
	}

	public void changeControls()
	{
		if (toggle)
		{
			RCC_SettingsData.InstanceR.mobileControllerObject = RCC_SettingsData.MobileController.SteeringWheel;
			GameManager.Instance.steeringControl = true;
			GameManager.Instance.arrowControl = false;
			toggle = false;
		}else
		{
			RCC_SettingsData.InstanceR.mobileControllerObject = RCC_SettingsData.MobileController.TouchScreen;
			GameManager.Instance.steeringControl = false;
			GameManager.Instance.arrowControl = true;
			toggle = true;
		}
	}

	public void initializeCar()
	{
		carController.SetActive(true);
		carCamera.SetActive(true);
		playableCars[GameManager.Instance.CurrentPlayer].GetComponent<RCC_CarMainControllerV3>().StartEngine();
		if (playableCars[GameManager.Instance.CurrentPlayer].name.Contains("Sedan"))
		{
			playerSteering.GetComponent<Image>().sprite = playableSteerings[0];
		}else if (playableCars[GameManager.Instance.CurrentPlayer].name.Contains("Sports"))
		{
			playerSteering.GetComponent<Image>().sprite = playableSteerings[1];
		}else if (playableCars[GameManager.Instance.CurrentPlayer].name.Contains("Muscle"))
		{
			playerSteering.GetComponent<Image>().sprite = playableSteerings[2];
		}

	}

	void InitializeLevel () {

		Time.timeScale = 1;

		Game_Elements.LevelComplete.SetActive (false);
		Game_Elements.LevelFailed.SetActive (false);
		Game_Elements.LoadingScreen.SetActive (false);
		Game_Elements.PauseMenu.SetActive (false);

		//Reset Finish Points
		if (Levels [currentLevel - 1].Objectives.Length > 0) {
			for (int i = 0; i < Levels [currentLevel - 1].Objectives.Length; i++) {
				if (Levels [currentLevel - 1].Objectives [i].FinishPoint != null)
					Levels [currentLevel - 1].Objectives [i].FinishPoint.SetActive (false);
				if (Levels [currentLevel - 1].Objectives [i].Instruction == "")
					Debug.LogWarning ("Please write insctruction for Level->" + GameManager.Instance.CurrentLevel + " Objective->" + (i + 1) + " in the inspector !");
			}
		} else if (Levels [currentLevel - 1].Objectives.Length == 0) {
			Debug.LogError ("No Objectives have been defined in the inspector !");
		}

		//SpawnItems
		if (Levels [currentLevel - 1].playerCar.Length > 0)
		{
//			Levels[currentLevel - 1].playerCar[0].Item = Instantiate(playableCars[GameManager.Instance.CurrentPlayer], Levels [currentLevel - 1].playerCar [0].SpawnPoint.position, Levels [currentLevel - 1].playerCar [0].SpawnPoint.rotation); 
			Levels[currentLevel - 1].playerCar[0].Item = playableCars[GameManager.Instance.CurrentPlayer];
			Levels[currentLevel - 1].playerCar[0].Item.SetActive(true);
			Levels[currentLevel - 1].playerCar[0].Item.transform.position = Levels [currentLevel - 1].playerCar [0].SpawnPoint.position;
			Levels[currentLevel - 1].playerCar[0].Item.transform.rotation = Levels [currentLevel - 1].playerCar [0].SpawnPoint.rotation;
            //			for (int i = 0; i < Levels [currentLevel - 1].playerCar.Length; i++) {
            //				SetItemPosition (Levels [currentLevel - 1].playerCar [i].Item, Levels [currentLevel - 1].playerCar [i].SpawnPoint);
            //			}
        }

		if (Levels [currentLevel - 1].GiveReward) {
			if (Levels [currentLevel - 1].RewardLevels.Length == 0)
				Debug.LogError ("No Rewards have been defined in the inspector !");
		}

		//Initialize Audio Sources
		InitializeAudio (FX_AudioSource, "FX_AudioSource");
		FX_AudioSource = GameObject.Find ("FX_AudioSource");

	}

	public void reviveMe()
	{
		GameManager.Instance.GameStatus = null;
		Time.timeScale = 1;
		Game_Elements.Timer_txt.color = Color.white;
		GameManager.Instance.ResumeTimer();
		LevelTime = (Levels [currentLevel - 1].Minutes * 60)/2 + (Levels [currentLevel - 1].Seconds)/2;
		if (isTimerEnabled)
			InvokeRepeating ("GameTimer", 0, 1);
		carCamera.SetActive(true);
		EnableAudio();
		Game_Elements.continueScreen.SetActive(false);
		carController.SetActive(true);
		playableCars[GameManager.Instance.CurrentPlayer].transform.position = Levels[currentLevel - 1].playerCar[0].SpawnPoint.position;
		playableCars[GameManager.Instance.CurrentPlayer].transform.rotation = Levels[currentLevel - 1].playerCar[0].SpawnPoint.rotation;
		playableCars[GameManager.Instance.CurrentPlayer].GetComponent<Rigidbody>().isKinematic = false;
		playableCars[GameManager.Instance.CurrentPlayer].GetComponent<Rigidbody>().drag = 0.05f;
	}

	public void getDoubleCoins()
	{
		SaveData.Instance.Coins += tempTimeBonus;
		GN_SaveLoad.SaveProgress();
		int tempDoubleCoins = tempTimeBonus * 2;
		totalCoins.text = tempDoubleCoins.ToString();
	}

	void LevelSpecificDecisions () {
		if (Levels [currentLevel - 1].Objectives.Length > 0) {
			ActivateFinishPoint ();
		}

		if (Levels [currentLevel - 1].isTimeBased) {
			isTimerEnabled = true;
			Game_Elements.Timer.SetActive (true);
		} else {
			isTimerEnabled = false;
			Game_Elements.Timer.SetActive (false);
		}

		//In-Game Timer
		if (isTimerEnabled)
			InvokeRepeating ("GameTimer", 0, 1);
	}

	void SpecifyCurrentLevel () {
		GameManager.Instance.isParked = false;
		#if UNITY_EDITOR
		if (GameManager.Instance.EditorSession) {
			currentLevel = StartLevel;
		} else {
			currentLevel = GameManager.Instance.CurrentLevel;
		}
		#else
		currentLevel = GameManager.Instance.CurrentLevel;
		#endif
	}

	void SetItemPosition (GameObject Item, Transform Position) {
		Item.transform.position = Position.position;
		Item.transform.rotation = Position.rotation;
	}

	void ActivateLevel () {
		for (int i = 0; i < Levels.Length; i++) {
			if (i == currentLevel - 1) {
				Levels [i].LevelObject.SetActive (true);
//				Levels[currentLevel - 1].playerCar[0].Item.transform.position = temp.position;
//				Levels[currentLevel - 1].playerCar[0].Item.transform.rotation = temp.rotation;
			} else {
				Destroy (Levels [i].LevelObject);
			}
		}

		GameManager.Instance.Objectives = Levels [currentLevel - 1].Objectives.Length;
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		OriginalTime = (Levels [currentLevel - 1].Minutes * 60) + Levels [currentLevel - 1].Seconds;
		LevelTime = (Levels [currentLevel - 1].Minutes * 60) + Levels [currentLevel - 1].Seconds;
	}

	void ActivateFinishPoint () {
		if (FinishCount == 0) {
			if (Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
		} else if (FinishCount == Levels [currentLevel - 1].Objectives.Length) {
			if (Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint.SetActive (false);
		} else {
			if (Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint.SetActive (false);
			if (Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
		}
	}

	public void ShowInstruction () {
		Game_Elements.InstructionText.text = Levels [currentLevel - 1].Objectives [FinishCount].Instruction;
		FinishCount++;
	}

	void GameTimer () {
//		Debug.Log("Timer: " + TimerPaused);
		if (!TimerPaused) {
//			Debug.Log("Came here");
			if (LevelTime >= 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0)
				LevelTime -= 1;
			minutes = ((int)LevelTime / 60);
			seconds = ((int)LevelTime % 60);
			time = minutes.ToString ("00") + ":" + seconds.ToString ("00");
			Game_Elements.Timer_txt.text = time;

			if (LevelTime <= 15.0f && LevelTime > 0.0f) {
				Game_Elements.Timer_txt.color = Color.yellow;
				SFX_Elements.CountDown.SetActive (true);
				if (GameManager.Instance.Objectives <= 0) {
					SFX_Elements.CountDown.SetActive (false);
				}
			} else if (LevelTime == 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0) {
				SFX_Elements.CountDown.SetActive (false);
				GameManager.Instance.GameLoose ();
			}
		}
	}

	#endregion



	#region Controller Logic

	public void OnLevelCheck (int reasonIndex) {
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		if (GameManager.Instance.Objectives > 0 && GameManager.Instance.GameStatus != "Loose") {
			if (Levels [currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");
		} else if (GameManager.Instance.Objectives == 0) {
			if (Levels [currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");

			//Calculate Reward
			if (Levels [currentLevel - 1].GiveReward) {
				GiveRewards ();
			}
			DisableAudio ();
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelCompleteSFX);
			StartCoroutine (OnLevelStatus ());
		} else if (GameManager.Instance.GameStatus == "Loose") {
			DisableAudio ();
			if (ReasonBased)
				SetGameOverReason (reasonIndex);
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelFailedSFX);
			StartCoroutine (OnLevelStatus ());
		}
	}

	private void Update()
	{
		updateStarTimer();
	}

	private void updateStarTimer()
	{
		//Original value at the end was 1.0 - changed to 0.5 for testing purpose
		float timeReduced = (OriginalTime - LevelTime)/OriginalTime*0.5f;
		starSlider.fillAmount = 1 - timeReduced;

	}

	IEnumerator OnLevelStatus () {
		CancelInvoke ();
		playableCars[GameManager.Instance.CurrentPlayer].GetComponent<Rigidbody>().drag = 5f;
		carController.SetActive(false);
		GameManager.Instance.PauseTimer ();
		SFX_Elements.CountDown.SetActive (false);
		if (GameManager.Instance.GameStatus == "Loose") {
			yield return new WaitForSeconds (GameLooseDelay);
            /*if (AdsManager.instance)
            {
                if (AdsManager.instance.CanShowRewardedAd())
                {
                    revive = true;
                    doubleCoins = false;
                    Game_Elements.continueScreen.SetActive(true);
                }
                else
                {
                    levelFail();
                }
            }
            else
            {*/
                levelFail();
            /*}*/
        } else {
			UpdateLevel ();
			yield return new WaitForSeconds (GameWinDelay);
            float tempFilledAmount = starSlider.fillAmount;
            Debug.Log("Slider fill amount: " + tempFilledAmount);
            completedSlider.DOFillAmount(tempFilledAmount, 2.5f).SetUpdate(true);
            LevelBonusText.text = LevelBonus[currentLevel - 1].ToString();
            TimeBonusText.text = Mathf.RoundToInt(tempFilledAmount * 100).ToString();
            tempTimeBonus = Mathf.RoundToInt(tempFilledAmount * 100) + LevelBonus[currentLevel - 1];
            tempTimeBonus += tempTimeBonus * 5;
            SaveData.Instance.Coins += tempTimeBonus;
            GN_SaveLoad.SaveProgress();
            totalCoins.text = tempTimeBonus.ToString();
            /*if (AdsManager.instance)
            {
                if (AdsManager.instance.CanShowRewardedAd())
                {
                    revive = false;
                    doubleCoins = true;
                    Game_Elements.doubleCoinsButton.SetActive(true);
                }
                else
                {
                    Game_Elements.doubleCoinsButton.SetActive(true);
                }
            }
            else
            {*/
                Game_Elements.doubleCoinsButton.SetActive(true);
            /*}*/
            Game_Elements.LevelComplete.SetActive(true);
		}
		yield return new WaitForSeconds (0.5f);
		Time.timeScale = 0;
	}

	void SetGameOverReason (int reasonIndex) {
		
	}

	void CalculateRewardAmount (int index) {
		TimePerct = (LevelTime / ((Levels [currentLevel - 1].Minutes * 60) + Levels [currentLevel - 1].Seconds)) * 100;
		if ((int)TimePerct >= Levels [currentLevel - 1].RewardLevels [index].MinTime && (int)TimePerct <= Levels [currentLevel - 1].RewardLevels [index].MaxTime) {
			for (int i = 0; i < Levels [currentLevel - 1].RewardLevels [index].RewardInfo.Length; i++) {
				reward = Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardAmount;

				//Give Your Rewards Here
				switch (Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType) {
				case RewardTypes.Coins:
					Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType);
					break;
				case RewardTypes.Other:
					Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType);
					break;
				}
			}
		}
	}

	void GiveRewards () {
		if (Levels [currentLevel - 1].RewardLevels.Length > 0) {
			for (int i = 0; i < Levels [currentLevel - 1].RewardLevels.Length; i++) {
				//Give reward here
				CalculateRewardAmount (i);
			}
		} else {
			Debug.LogError ("No rewards have been defined in the inspector !");
		}
	}

	void DisableAudio () {
		for (int i = 0; i < SFX_Elements.BGMusicLoops.Length; i++) {
			SFX_Elements.BGMusicLoops [i].SetActive (false);
		}
	}
	
	void EnableAudio () {
		for (int i = 0; i < SFX_Elements.BGMusicLoops.Length; i++) {
			SFX_Elements.BGMusicLoops [i].SetActive (true);
		}
	}

	void UpdateLevel () {
		if (currentLevel == SaveData.Instance.Level) {
			SaveData.Instance.Level++;
			GN_SaveLoad.SaveProgress ();
		}
	}

	public void levelFail()
	{
		Game_Elements.continueScreen.SetActive(false);
		Game_Elements.LevelFailed.SetActive (true);
	}

	#endregion

	private void OnRewardedVideoComplete()
	{
		if (revive)
		{
			reviveMe();
		}
		else if(doubleCoins)
		{
			getDoubleCoins();
		}
	}
    public void ShowRewardedAdNow()
    {
        /*if (AdsManager.instance)
        {
            AdsManager.instance.ShowRewardedAd();
        }*/
    }
	#region Interface-Logic

	public void PauseGame () {
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
	}

	public void ResumeGame () {
		Time.timeScale = 1.0f;
        if (SaveData.Instance.IsSoundOn)
        {
            AudioListener.pause = false;
        }
    }

	public void RetryLevel () {
		if (SaveData.Instance.IsSoundOn)
		{
			AudioListener.pause = false;
		}
		Game_Elements.LoadingScreen.SetActive (true);
        /*if (AdsManager.instance && !SaveData.Instance.RemoveAds)
        {
            AdsManager.instance.ShowInterstitialAd();
        }*/
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void NextLevel () {
		if (SaveData.Instance.IsSoundOn)
		{
			AudioListener.pause = false;
		}
		if (currentLevel != PlayableLevels) {
			#if UNITY_EDITOR
			GameManager.Instance.EditorSession = false;
			#endif
			GameManager.Instance.CurrentLevel += 1;
			Game_Elements.LoadingScreen.SetActive (true);
            /*if (AdsManager.instance && !SaveData.Instance.RemoveAds)
            {
                AdsManager.instance.ShowInterstitialAd();
            }*/
            SceneManager.LoadScene (NextScene.ToString());
		}
	}

	public void MainMenu () {
		if (SaveData.Instance.IsSoundOn)
		{
			AudioListener.pause = false;
		}
		Game_Elements.LoadingScreen.SetActive (true);
        /*if (AdsManager.instance && !SaveData.Instance.RemoveAds)
        {
            AdsManager.instance.ShowInterstitialAd();
        }*/
        SceneManager.LoadScene (PreviousScene.ToString ());
	}

	#endregion
}