// #define DISABLESTEAMWORKS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    [SerializeField]
    public GameCamera gameCamera = null;

    [SerializeField]
    public MusicManager musicManager = null;

    [SerializeField]
    public LevelUI levelUI = null;

    [SerializeField]
    public CityUI cityUI = null;

    [SerializeField]
    public MenuUI menuUI = null;

    [SerializeField]
    public BriefUI briefUI = null;

    [SerializeField]
    public PhoneUI phoneUI = null;

    [SerializeField]
    public CertificateUI certificateUI = null;

    [SerializeField]
    public AreYouSureUI areYouSureUI = null;

    [SerializeField]
    public CongratulationsUI CongratsUI = null;

    [SerializeField]
    public ScreenFade screenFade = null;

    [SerializeField]
    public GameObject cityMapGameObject = null;

    [SerializeField]
    public MeshRenderer cityMapMeshRenderer = null;

    [SerializeField]
    public List<GameObject> ObjectsOnMenuUI = null;

    [SerializeField]
    public List<GameObject> ObjectsOnCityUI = null;

    [SerializeField]
    public List<Sprite> sofaImages = null;

    [SerializeField]
    public List<Level> levels = null;

    [System.NonSerialized]
    public List<HistoryState> history = null;

    [System.NonSerialized]
    public Level activeLevel = null;

    [System.NonSerialized]
    public GameData gameData = null;

    [System.NonSerialized]
    public bool isSoundOn = true;

    [System.NonSerialized]
    public bool isSFXOn = true;

    public void Save()
    {
        SaveSystem.SaveGame(gameData);
    }

    public void Delete()
    {
        SaveSystem.DeleteSave();
        Load();

        cityUI.UpdateButtonsVisuals();

        foreach (var l in levels)
        {
            l.cameraParams.UpdatePosition();
            l.gameObject.SetActive(false);
        }
    }

    public void Load()
    {
        var data = SaveSystem.LoadGame();
        if (data == null)
            gameData = new GameData();
        else
            gameData = data;
    }

    void Awake()
    {
#if UNITY_STANDALONE
        Screen.SetResolution(1280, 720, false);
        levelUI.ShowShortcuts();
#endif

#if !UNITY_STANDALONE
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        levelUI.HideShortcuts();
#endif
    }

    void Start()
    {
        Load();

        musicManager.SetTrack(0, false);

        history = new List<HistoryState>();
        gameCamera.Initialize();
        MenuClouds();
        menuUI.PlayIntro();

        foreach (var l in levels)
        {
            l.cameraParams.UpdatePosition();
            l.gameObject.SetActive(false);
        }

        cityUI.Initialize();
        cityUI.UpdateButtonsVisuals();

        menuUI.SetVersionText("v " + Application.version);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleFullscreen();

        if (activeLevel == null)
        {
            cityUI.UpdateButtonsPositions();
            return;
        }

        var room = activeLevel.room;
        if (room == null)
        {
            Debug.Log("Room is null");
            return;
        }

        levelUI.UpdateUI(room.player.SpeechCloudTransform, gameCamera.cameraComponent);


        if (!room.IsSolved())
        {
            if (!gameCamera.isInTransition)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || SwipeInput.swipedLeft)
                    room.Move(Direction.left, true);

                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || SwipeInput.swipedRight)
                    room.Move(Direction.right, true);

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || SwipeInput.swipedUp)
                    room.Move(Direction.top, true);

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || SwipeInput.swipedDown)
                    room.Move(Direction.bottom, true);

                if (Input.GetKeyDown(KeyCode.Z))
                    UndoHistory();

                if (Input.GetKeyDown(KeyCode.R))
                    ResetHistory();

#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.G))
                    levelUI.ShowShortcuts();

                if (Input.GetKeyDown(KeyCode.H))
                    levelUI.HideShortcuts();

                if (Input.GetKeyDown(KeyCode.Keypad1))
                    room.SolveRoom(0);
                if (Input.GetKeyDown(KeyCode.Keypad2))
                    room.SolveRoom(1);
                if (Input.GetKeyDown(KeyCode.Keypad3))
                    room.SolveRoom(2);
                if (Input.GetKeyDown(KeyCode.Keypad4))
                    room.SolveRoom(3);
                if (Input.GetKeyDown(KeyCode.Keypad5))
                    room.SolveRoom(4);
                if (Input.GetKeyDown(KeyCode.Keypad6))
                    room.SolveRoom(5);

                if (Input.GetKeyDown(KeyCode.Keypad9))
                    CongratsUI.Congrats(this);
#endif
            }
        }
        else
        {
            CongratsUI.Congrats(this);
        }
    }

    public void ToggleFullscreen()
    {
#if UNITY_STANDALONE
        fullscreen++;

        if (fullscreen == 2)
            fullscreen = 0;

        switch (fullscreen)
        {
            case 0:
            {
                Screen.SetResolution(1280, 720, false);
            } break;
            case 1:
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            } break;
        }
#endif
    }
#if UNITY_STANDALONE
    int fullscreen = 0;
#endif

    public void MenuClouds()
    {
        foreach (var o in ObjectsOnMenuUI)
            o.SetActive(true);

        foreach (var o in ObjectsOnCityUI)
            o.SetActive(false);
    }

    public void CityClouds()
    {
        foreach (var o in ObjectsOnMenuUI)
            o.SetActive(false);

        foreach (var o in ObjectsOnCityUI)
            o.SetActive(true);
    }

    public Sprite GetSofaImage(SofaImage sofaImage)
    {
        return sofaImages[(int)sofaImage];
    }

    public void BackToCityButton()
    {
        if (gameCamera.isInTransition)
            return;

        if (levelUI.isFading)
            return;

        levelUI.animatorComponent.SetTrigger("Hide");
        BackToCity(false);
    }

    public void BackToCity(bool fromCongrats)
    {
        if (activeLevel == null)
            return;

        levelUI.settingsToggle.Close();
        StartCoroutine(GoToLevel(null, fromCongrats));
    }

    public IEnumerator GoToLevel(Level level, bool fromCongrats = false)
    {
        if (gameCamera.isInTransition)
            yield break;

        var prevLevel = activeLevel;

        activeLevel = level;

        history.Clear();

        if (activeLevel == null)
        {
            screenFade.SetColor(prevLevel.cameraParams.backgroundColor);
            screenFade.FadeIn();

            yield return new WaitWhile(() => screenFade.fadeOut == true);

            yield return new WaitForSeconds(0.5f);

            cityMapGameObject.SetActive(true);
            prevLevel.gameObject.SetActive(false);

            gameCamera.MoveImmediate(gameCamera.cameraCityInbetweenTransform);
            gameCamera.StartMoving(gameCamera.cameraCityTransform, null);

            screenFade.FadeOut();

            musicManager.SetTrack(0, true);

            yield return new WaitWhile(() => screenFade.fadeOut == false);

            if (prevLevel.buttonString == "35" && !gameData.IsSolvedCompletely())
            {
                yield return new WaitWhile(() => gameCamera.isInTransition == true);

                phoneUI.animatorComponent.SetTrigger("Show");
                phoneUI.isShowing = true;

                yield return new WaitWhile(() => phoneUI.isShowing == true);

                yield return new WaitForSeconds(1.0f);

#if !DISABLESTEAMWORKS
                if (SteamManager.Initialized)
                {
                    SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_04");
                }
#endif
            }
            else if (gameData.IsSolvedCompletely() && fromCongrats)
            {
                yield return new WaitWhile(() => gameCamera.isInTransition == true);

                Debug.Log("Game Completed!!!");

                if (System.String.IsNullOrEmpty(gameData.date))
                {
                    gameData.date = System.DateTime.Now.ToString("dd.MM.yyyy");
                    Save();
                }

                certificateUI.animatorComponent.SetTrigger("Show");
                certificateUI.dateText.text = gameData.date;
                certificateUI.isShowing = true;

                yield return new WaitWhile(() => certificateUI.isShowing == true);

                yield return new WaitForSeconds(1.0f);

#if !DISABLESTEAMWORKS
                if (SteamManager.Initialized)
                {
                    SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_05");
                }
#endif
            }
            else
            {
                yield return new WaitForSeconds(0.5f);

#if !DISABLESTEAMWORKS
                if (SteamManager.Initialized)
                {
                    if (gameData.IsSolvedLevelsForGettingSmarterAchievement())
                    {
                        SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_01");
                    }
                    if (gameData.IsSolvedLevelsForHalfwayToSuccessAchievement())
                    {
                        SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_02");
                    }
                    if (gameData.IsSolvedLevelsForUrbanistAchievement())
                    {
                        SteamAchievements.UnlockSteamAchievement("ACHIEVEMENT_03");
                    }
                }
#endif
            }

            cityUI.animatorComponent.SetTrigger("Show");
        }
        else
        {
            levelUI.UpdateText(activeLevel.goldMedalThreshold, history.Count);
            levelUI.UpdateLevelInfo(activeLevel.levelName, activeLevel.buttonString);

            var room = activeLevel.room;
            if (room == null)
            {
                Debug.Log("Room is null");
                yield break;
            }
            room.ResetRoom();
            activeLevel.MovablesToStart();

            screenFade.SetColor(activeLevel.cameraParams.backgroundColor);
            screenFade.FadeIn();

            gameCamera.StartMoving(activeLevel.cameraParams, null, true);

            yield return new WaitWhile(() => screenFade.fadeOut == true);

            cityMapGameObject.SetActive(false);
            activeLevel.gameObject.SetActive(true);

            yield return new WaitWhile(() => gameCamera.isInTransition == true);

            screenFade.FadeOut();

            musicManager.SetTrack(activeLevel.trackIndex, true);

            yield return new WaitWhile(() => screenFade.fadeOut == false);

            levelUI.animatorComponent.SetTrigger("Show");

            activeLevel.StartTimer();
        }
    }

    //
    // Undo system implementation
    //

    public void RecordHistory(string state, Direction playerDir)
    {
        if (activeLevel == null)
            return;

        if (gameCamera.isInTransition)
            return;

        var room = activeLevel.room;
        if (room == null)
        {
            Debug.Log("Room is null");
            return;
        }

        var hs = new HistoryState(state, playerDir);
        history.Add(hs);
        levelUI.UpdateText(activeLevel.goldMedalThreshold, history.Count);
    }

    public void UndoHistory()
    {
        if (activeLevel == null)
            return;

        if (gameCamera.isInTransition)
            return;

        if (levelUI.isFading)
            return;

        var room = activeLevel.room;
        if (room == null)
        {
            Debug.Log("Room is null");
            return;
        }

        if (history.Count < 1)
            return;

        var lastHistory = history[history.Count - 1];

        room.MakeMove(lastHistory.state, Direction.right, false, false);

        room.player.targetDir = lastHistory.playerDir;

        history.RemoveAt(history.Count - 1);
        levelUI.UpdateText(activeLevel.goldMedalThreshold, history.Count);
    }

    public void ResetHistory()
    {
        if (activeLevel == null)
            return;

        if (gameCamera.isInTransition)
            return;

        if (levelUI.isFading)
            return;

        var room = activeLevel.room;
        if (room == null)
        {
            Debug.Log("Room is null");
            return;
        }

        levelUI.FadeScreenBlinkWithCallback(activeLevel.cameraParams.backgroundColor, () => {
            room.ResetRoom();
            activeLevel.MovablesToStart();
            history.Clear();
            levelUI.UpdateText(activeLevel.goldMedalThreshold, history.Count);
        });
    }

    public struct HistoryState
    {
        public string state;
        public Direction playerDir;

        public HistoryState(string _state, Direction _playerDir)
        {
            this.state = _state;
            this.playerDir = _playerDir;
        }
    }
}
