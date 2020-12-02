using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIExtensions;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    List<TextMeshProUGUI> textList = null;

    [Space(20)]
    [SerializeField]
    Button circleButton = null;

    [SerializeField]
    Button silverStarButton = null;

    [SerializeField]
    Button goldStarButton = null;

    [SerializeField]
    ShinyEffectForUGUI silverShineEffectComponent = null;

    [SerializeField]
    ShinyEffectForUGUI goldShineEffectComponent = null;

    [SerializeField]
    public Game game = null;

    [SerializeField]
    public Level attachedLevel = null;

    [System.NonSerialized]
    public Button buttonComponent = null;


    public void Initialize()
    {
        circleButton.GetComponent<PlaySound>().game = game;
        silverStarButton.GetComponent<PlaySound>().game = game;
        goldStarButton.GetComponent<PlaySound>().game = game;
    }

    void Start()
    {
        InvokeRepeating("AnimateStars", 0.0f, 4.0f);
    }

    void AnimateStars()
    {
        if (skipFirst)
        {
            skipFirst = false;
            return;
        }

        if (silverStarButton.gameObject.activeSelf)
            silverShineEffectComponent.Play();

        if (goldStarButton.gameObject.activeSelf)
            goldShineEffectComponent.Play();
    }
    bool skipFirst = true;

    public void OnLevelButtonClick()
    {
        if (game == null)
            return;

        if (attachedLevel == null)
            return;

        if (game.gameCamera.isInTransition)
            return;

        game.briefUI.Brief(game, attachedLevel);
    }

    public void SetButtonText()
    {
        if (textList == null)
            return;

        if (attachedLevel == null)
            return;

        foreach (var t in textList)
        {
            t.text = attachedLevel.buttonString;
        }
    }

    public void SetVisuals(StarType starType)
    {
        switch (starType)
        {
            case StarType.None:
            {
                buttonComponent = circleButton;
                gameObject.SetActive(false);
                circleButton.gameObject.SetActive(true);
                silverStarButton.gameObject.SetActive(false);
                goldStarButton.gameObject.SetActive(false);
            } break;
            case StarType.Silver:
            {
                buttonComponent = silverStarButton;
                gameObject.SetActive(false);
                circleButton.gameObject.SetActive(false);
                silverStarButton.gameObject.SetActive(true);
                goldStarButton.gameObject.SetActive(false);
            } break;
            case StarType.Gold:
            {
                buttonComponent = goldStarButton;
                gameObject.SetActive(false);
                circleButton.gameObject.SetActive(false);
                silverStarButton.gameObject.SetActive(false);
                goldStarButton.gameObject.SetActive(true);
            } break;
        }
    }
}
