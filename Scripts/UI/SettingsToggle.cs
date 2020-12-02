using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    [SerializeField]
    Game game = null;

    [SerializeField]
    Animator animator = null;

    [SerializeField]
    GameObject cogGameObject = null;

    [SerializeField]
    GameObject crossGameObject = null;

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

    void Start()
    {
        cogGameObject.SetActive(true);
        crossGameObject.SetActive(false);
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

    public void TogglePressed()
    {
        cogGameObject.SetActive(!cogGameObject.activeSelf);
        crossGameObject.SetActive(!crossGameObject.activeSelf);

        if (!opened)
        {
            animator.SetTrigger("Show");
            opened = true;
        }
        else
        {
            animator.SetTrigger("Hide");
            opened = false;
        }
    }
    bool opened = false;

    public void SoundTogglePressed()
    {
        game.isSoundOn = !game.isSoundOn;
    }

    public void SFXTogglePressed()
    {
        game.isSFXOn = !game.isSFXOn;
    }

    public void Close()
    {
        if (!opened)
            return;

        TogglePressed();
    }
}
