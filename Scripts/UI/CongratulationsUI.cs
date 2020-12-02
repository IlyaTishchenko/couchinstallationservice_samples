using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Coffee.UIExtensions;

// Congratulations UI is a screen displayed after level completion
public class CongratulationsUI : MonoBehaviour
{
    [SerializeField]
    public Animator congratsAnimator = null;

    [SerializeField]
    GameObject silverStar = null;

    [SerializeField]
    GameObject goldStar = null;

    [SerializeField]
    ShinyEffectForUGUI silverStarEffect = null;

    [SerializeField]
    ShinyEffectForUGUI goldStarEffect = null;

    [SerializeField]
    public TextMeshProUGUI scoreText = null;

    [SerializeField]
    public TextMeshProUGUI timeText = null;

    void Start()
    {
        InvokeRepeating("AnimateStars", 0.0f, 4.0f);
    }

    void AnimateStars()
    {
        if (silverStar.activeSelf)
            silverStarEffect.Play();

        if (goldStar.activeSelf)
            goldStarEffect.Play();
    }

    public void Congrats(Game game)
    {
        if (!isShowingCongrats)
        {
            game.activeLevel.StopTimer();
            StartCoroutine(CongratsCoroutine(game));
            isShowingCongrats = true;
        }
    }
    bool isShowingCongrats = false;

    IEnumerator CongratsCoroutine(Game game)
    {
        var goldStarEarned = game.history.Count <= game.activeLevel.goldMedalThreshold;

        if (goldStarEarned)
        {
            silverStar.SetActive(false);
            goldStar.SetActive(true);
        }
        else
        {
            silverStar.SetActive(true);
            goldStar.SetActive(false);
        }
        scoreText.text = game.history.Count.ToString() + "/" + game.activeLevel.goldMedalThreshold.ToString();
        var seconds = game.activeLevel.GetTime();
        timeText.text = System.TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");

        game.activeLevel.PlaySofaShine();

        game.levelUI.settingsToggle.Close();
        game.levelUI.animatorComponent.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f);

        game.levelUI.speechCloudText.text = game.activeLevel.speechCloudString.Replace("\\n", "\n");
        game.levelUI.ShowSpeechCloud();

        game.activeLevel.room.player.ShouldThink = false;

        game.activeLevel.room.player.dustParticles.Play();

        var dustShakeOffSpeed = 3.0f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * dustShakeOffSpeed)
        {
            game.activeLevel.room.player.playerAnimator.SetFloat("DustShakeOff", t);
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);

        game.levelUI.HideSpeechCloud();

        game.activeLevel.room.player.dustParticles.Stop();

        for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime * dustShakeOffSpeed)
        {
            game.activeLevel.room.player.playerAnimator.SetFloat("DustShakeOff", t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        congratsAnimator.SetTrigger("Show");

        yield return new WaitUntil(() => okPressed == true || restartPressed == true);

        congratsAnimator.SetTrigger("Hide");

        yield return new WaitForSeconds(1.0f);

        game.activeLevel.room.player.ShouldThink = true;

        var index = game.levels.IndexOf(game.activeLevel);
        if (index < 0)
            Debug.LogError("Cannot find level");

        if (goldStarEarned)
            game.gameData.levelStars[index] = StarType.Gold;
        else
            game.gameData.levelStars[index] = StarType.Silver;

        game.Save();

        game.cityUI.UpdateButtonsVisuals();

        if (okPressed)
        {
            game.BackToCity(true);
        }
        else if (restartPressed)
        {
            game.screenFade.SetColor(game.activeLevel.cameraParams.backgroundColor);
            game.screenFade.FadeIn();

            yield return new WaitWhile(() => game.screenFade.fadeOut == true);

            game.activeLevel.room.ResetRoom();
            game.activeLevel.MovablesToStart();
            game.history.Clear();
            game.levelUI.UpdateText(game.activeLevel.goldMedalThreshold, game.history.Count);

            game.screenFade.FadeOut();

            yield return new WaitWhile(() => game.screenFade.fadeOut == false);

            game.levelUI.animatorComponent.SetTrigger("Show");

            game.activeLevel.StartTimer();
        }

        isShowingCongrats = false;

        okPressed = false;
        restartPressed = false;
    }
    bool okPressed = false;
    bool restartPressed = false;

    public void OkButton()
    {
        okPressed = true;
    }

    public void RestartButton()
    {
        restartPressed = true;
    }
}
