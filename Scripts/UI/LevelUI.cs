using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIExtensions;

public class LevelUI : MonoBehaviour
{
    [SerializeField]
    public Animator animatorComponent = null;

    [SerializeField]
    public Animator OhNoAnimator = null;

    [SerializeField]
    public TextMeshProUGUI text = null;

    [SerializeField]
    public TextMeshProUGUI speechCloudText = null;

    [SerializeField]
    public TextMeshProUGUI levelNameText = null;

    [SerializeField]
    public TextMeshProUGUI levelNumberText = null;

    [SerializeField]
    ShinyEffectForUGUI shineEffectComponent = null;

    [SerializeField]
    Game game = null;

    [SerializeField]
    List<GameObject> Shortcuts = null;

    [SerializeField]
    public SettingsToggle settingsToggle = null;

    [System.NonSerialized]
    public bool isFading = false;

    void Start()
    {
        InvokeRepeating("AnimateStars", 0.0f, 4.0f);
    }

    void AnimateStars()
    {
        shineEffectComponent.Play();
    }

    public void ShowShortcuts()
    {
        if (Shortcuts == null)
            return;

        foreach (var s in Shortcuts)
            s.SetActive(true);
    }

    public void HideShortcuts()
    {
        if (Shortcuts == null)
            return;

        foreach (var s in Shortcuts)
            s.SetActive(false);
    }

    public void ShowSpeechCloud()
    {
        if (!isShowing)
        {
            OhNoAnimator.SetTrigger("Show");
            isShowing = true;
        }
    }
    bool isShowing = false;

    public void UpdateUI(Transform speechCloudTransform, Camera cameraComponent)
    {
        var screen = cameraComponent.WorldToScreenPoint(speechCloudTransform.position);

        OhNoAnimator.transform.position = screen;
    }

    public void HideSpeechCloud()
    {
        if (isShowing)
            OhNoAnimator.SetTrigger("Hide");

        isShowing = false;
    }

    public void UpdateText(int currentGoldMedalThreshold, int moveCount)
    {
        text.text = moveCount.ToString() + "/" + currentGoldMedalThreshold.ToString();
    }

    public void UpdateLevelInfo(string levelName, string levelNumber)
    {
        levelNameText.text = levelName;
        levelNumberText.text = levelNumber;
    }

    public void FadeScreenBlinkWithCallback(Color backgroundColor, System.Action callback)
    {
        blinkCallback = callback;
        fadeScreenColor = backgroundColor;
        StartCoroutine("FadeScreenBlinkCoroutine");
    }
    System.Action blinkCallback = null;
    Color fadeScreenColor = Color.white;

    IEnumerator FadeScreenBlinkCoroutine()
    {
        isFading = true;

        game.screenFade.SetColor(fadeScreenColor);

        game.screenFade.FadeIn(blinkSpeed);
        yield return new WaitForSeconds(1.0f / blinkSpeed);

        if (blinkCallback != null)
        {
            blinkCallback();
            blinkCallback = null;
        }

        game.screenFade.FadeOut(blinkSpeed);
        yield return new WaitForSeconds(1.0f / blinkSpeed);

        isFading = false;
    }
    float blinkSpeed = 2.0f;
}
