using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    public Animator animatorComponent = null;

    [SerializeField]
    public Game game = null;

    [SerializeField]
    public Color screenFadeColor = Color.white;

    [SerializeField]
    public TextMeshProUGUI versionText = null;

    [SerializeField]
    public GameCamera gameCamera = null;

    [SerializeField]
    public GameObject toggleButton = null;

    [Space(20)]
    [SerializeField]
    GameObject soundOnGameObject = null;

    [SerializeField]
    GameObject soundOffGameObject = null;

    [Space(20)]
    [SerializeField]
    GameObject sfxOnGameObject = null;

    [SerializeField]
    GameObject sfxOffGameObject = null;

    [System.NonSerialized]
    public bool isShowing = true;

    public void PlayIntro()
    {
        StartCoroutine("IntroCoroutine");
    }

    IEnumerator IntroCoroutine()
    {
        game.gameCamera.MoveImmediate(game.gameCamera.cameraMainMenuInbetweenTransform);

        game.screenFade.SetColor(screenFadeColor);
        game.screenFade.FadeOut();

        game.gameCamera.StartMoving(game.gameCamera.cameraMainMenuTransform, null);

        yield return new WaitWhile(() => game.gameCamera.isInTransition == true);

        animatorComponent.SetTrigger("Show");
    }

    public void StartButton()
    {
        if (gameCamera.isInTransition || game.screenFade.isFading)
            return;

        StartCoroutine("StartButtonCoroutine");
    }

    IEnumerator StartButtonCoroutine()
    {
        animatorComponent.SetTrigger("Hide");

        game.screenFade.SetColor(screenFadeColor);

        game.screenFade.FadeIn();
        game.gameCamera.StartMoving(game.gameCamera.cameraMainMenuInbetweenTransform, null);

        yield return new WaitWhile(() => game.gameCamera.isInTransition == true);

        game.CityClouds();

        game.gameCamera.MoveImmediate(game.gameCamera.cameraCityInbetweenTransform);

        game.screenFade.FadeOut();

        game.gameCamera.StartMoving(game.gameCamera.cameraCityTransform, null);

        yield return new WaitForSeconds(2.0f);

        game.cityUI.animatorComponent.SetTrigger("Show");
        isShowing = false;
    }

    public void SetVersionText(string version)
    {
        if (versionText == null)
            return;

        versionText.text = version;
    }

    public void SettingsButton()
    {
        animatorComponent.SetTrigger("ShowSettings");
    }

    public void CloseSettingsButton()
    {
        animatorComponent.SetTrigger("HideSettings");
    }

    public void AboutButton()
    {
        animatorComponent.SetTrigger("ShowAbout");
    }

    public void CloseAboutButton()
    {
        animatorComponent.SetTrigger("HideAbout");
    }

    public void StuckButton()
    {
        animatorComponent.SetTrigger("ShowStuck");
    }

    public void CloseStuckButton()
    {
        animatorComponent.SetTrigger("HideStuck");
    }

    void Start()
    {
#if !UNITY_STANDALONE
        toggleButton.SetActive(false);
#endif
    }

    void Update()
    {
        if (game.isSoundOn)
        {
            if (!soundOnGameObject.activeSelf)
                soundOnGameObject.SetActive(true);
            if (soundOffGameObject.activeSelf)
                soundOffGameObject.SetActive(false);
        }
        else
        {
            if (soundOnGameObject.activeSelf)
                soundOnGameObject.SetActive(false);
            if (!soundOffGameObject.activeSelf)
                soundOffGameObject.SetActive(true);
        }

        if (game.isSFXOn)
        {
            if (!sfxOnGameObject.activeSelf)
                sfxOnGameObject.SetActive(true);
            if (sfxOffGameObject.activeSelf)
                sfxOffGameObject.SetActive(false);
        }
        else
        {
            if (sfxOnGameObject.activeSelf)
                sfxOnGameObject.SetActive(false);
            if (!sfxOffGameObject.activeSelf)
                sfxOffGameObject.SetActive(true);
        }
    }

    public void SoundTogglePressed()
    {
        game.isSoundOn = !game.isSoundOn;
    }

    public void SFXTogglePressed()
    {
        game.isSFXOn = !game.isSFXOn;
    }

    public void QuitBitton()
    {
        game.areYouSureUI.callback = () => Application.Quit();
        game.areYouSureUI.animatorComponent.SetTrigger("Show");
    }

    public void ResetProgressButton()
    {
        game.areYouSureUI.callback = () => game.Delete();
        game.areYouSureUI.animatorComponent.SetTrigger("Show");
    }
}
