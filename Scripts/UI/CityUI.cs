using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Game's map UI
public class CityUI : MonoBehaviour
{
    [SerializeField]
    public Animator animatorComponent = null;

    [SerializeField]
    public LevelButton buttonTemplate = null;

    [SerializeField]
    public Game game = null;

    [SerializeField]
    public Transform buttonsParentTransform = null;

    [SerializeField]
    public SettingsToggle settingsToggle = null;

    [SerializeField]
    public GameObject certificateButton = null;

    [SerializeField]
    public List<LevelButton> levelButtons = null;

    [Space(20)]
    [SerializeField]
    public List<GameStage> gameStagesList = null;

    void Update()
    {
        if (game.gameData.IsSolvedCompletely())
            certificateButton.SetActive(true);
        else
            certificateButton.SetActive(false);
    }

    public void Initialize()
    {
        foreach (var lb in levelButtons)
        {
            lb.Initialize();
            lb.SetButtonText();
        }
    }

    public void UpdateButtonsPositions()
    {
        foreach (var lb in levelButtons)
        {
            var levelPos = lb.attachedLevel.transform.position;
            var screen = game.gameCamera.cameraComponent.WorldToScreenPoint(levelPos);
            screen.z = (transform.position - game.gameCamera.cameraComponent.transform.position).magnitude;
            var position = game.gameCamera.cameraComponent.ScreenToWorldPoint(screen);

            lb.transform.position = position;
            lb.transform.localPosition = new Vector3(lb.transform.localPosition.x, lb.transform.localPosition.y, 0);
            lb.transform.localRotation = Quaternion.identity;
        }
    }

    public void UpdateButtonsVisuals()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].SetVisuals(game.gameData.levelStars[i]);
        }

        gameStagesList[0].ActivateStage();

        for (int i = 0; i < gameStagesList.Count - 1; i++)
        {
            if (gameStagesList[i].IsStageCompleted(game))
                gameStagesList[i + 1].ActivateStage();
        }
    }

    public void CertificateButton()
    {
        settingsToggle.Close();
        StartCoroutine("CertificateCoroutine");
    }

    IEnumerator CertificateCoroutine()
    {
        animatorComponent.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f);

        if (System.String.IsNullOrEmpty(game.gameData.date))
        {
            game.gameData.date = System.DateTime.Now.ToString("dd.MM.yyyy");
            game.Save();
        }

        game.certificateUI.animatorComponent.SetTrigger("Show");
        game.certificateUI.dateText.text = game.gameData.date;
        game.certificateUI.isShowing = true;

        yield return new WaitWhile(() => game.certificateUI.isShowing == true);

        yield return new WaitForSeconds(1.0f);

        animatorComponent.SetTrigger("Show");
    }

    public void MenuButton()
    {
        settingsToggle.Close();
        StartCoroutine("MenuButtonCoroutine");
    }

    IEnumerator MenuButtonCoroutine()
    {
        animatorComponent.SetTrigger("Hide");

        game.screenFade.SetColor(game.menuUI.screenFadeColor);

        game.screenFade.FadeIn();
        game.gameCamera.StartMoving(game.gameCamera.cameraCityInbetweenTransform, null);

        yield return new WaitWhile(() => game.gameCamera.isInTransition == true);

        game.MenuClouds();

        game.gameCamera.MoveImmediate(game.gameCamera.cameraMainMenuInbetweenTransform);

        game.screenFade.FadeOut();

        game.gameCamera.StartMoving(game.gameCamera.cameraMainMenuTransform, null);

        yield return new WaitForSeconds(2.0f);

        game.menuUI.animatorComponent.SetTrigger("Show");
        game.menuUI.isShowing = true;
    }
}

[System.Serializable]
public class GameStage
{
    public List<LevelButton> levelButtons = null;

    public bool IsStageCompleted(Game game)
    {
        var completed = true;
        foreach (var lb in levelButtons)
        {
            var index = game.levels.IndexOf(lb.attachedLevel);

            if (index < 0)
                Debug.LogError("Cannot find level");

            if (game.gameData.levelStars[index] == StarType.None)
            {
                completed = false;
                break;
            }
        }

        return completed;
    }

    public void ActivateStage()
    {
        foreach (var lb in levelButtons)
        {
            lb.gameObject.SetActive(true);
        }
    }
}
