using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField]
    public AudioSource audioSource = null;

    [SerializeField]
    public Game game = null;

    public void PlaySoundOnce()
    {
        if (game.isSFXOn)
            audioSource.Play();
    }
}
