using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIExtensions;

// Brief UI is a screen containing info about the level with buttons to proceed or cancel level starting
public class BriefUI : MonoBehaviour
{
    [SerializeField]
    public Animator briefAnimator = null;

    [SerializeField]
    ShinyEffectForUGUI starEffect = null;

    [SerializeField]
    public TextMeshProUGUI levelNameText = null;

    [SerializeField]
    public TextMeshProUGUI goldMedalThresholdText = null;

    [SerializeField]
    public TextMeshProUGUI sofasCountText = null;

    [SerializeField]
    public Image sofaImage = null;

    void Start()
    {
        InvokeRepeating("AnimateStars", 0.0f, 4.0f);
    }

    void AnimateStars()
    {
        starEffect.Play();
    }

    public void Brief(Game game, Level level)
    {
        if (!isShowingBrief)
        {
            StartCoroutine(BriefCoroutine(game, level));
            isShowingBrief = true;
        }
    }
    bool isShowingBrief = false;

    IEnumerator BriefCoroutine(Game game, Level level)
    {
        game.cityUI.settingsToggle.Close();
        game.cityUI.animatorComponent.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f);

        levelNameText.text = level.levelName;
        goldMedalThresholdText.text = level.goldMedalThreshold.ToString();
        sofasCountText.text = level.sofasCount.ToString();
        sofaImage.sprite = game.GetSofaImage(level.sofaImage);

        briefAnimator.SetTrigger("Show");

        yield return new WaitUntil(() => okPressed == true || closePressed == true);

        if (okPressed)
        {
            briefAnimator.SetTrigger("Hide");

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(game.GoToLevel(level));
        }
        else if (closePressed)
        {
            briefAnimator.SetTrigger("Hide");

            yield return new WaitForSeconds(1.0f);

            game.cityUI.animatorComponent.SetTrigger("Show");
        }

        isShowingBrief = false;

        okPressed = false;
        closePressed = false;
    }
    bool okPressed = false;
    bool closePressed = false;

    public void OkButton()
    {
        okPressed = true;
    }

    public void CloseButton()
    {
        closePressed = true;
    }

}
