using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    Game game = null;

    [SerializeField]
    AudioSource audioSource = null;

    [SerializeField]
    List<AudioClip> audioClips = null;

    void Start()
    {
        audioSource.clip = audioClips[0];
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        if (game.isSoundOn)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    public void SetTrack(int index, bool fade)
    {
        if (audioSource.clip == audioClips[index])
            return;

        if (fade)
        {
            StartCoroutine(SwitchTrachCoroutine(index));
        }
        else
        {
            audioSource.clip = audioClips[index];
        }
    }

    // Coroutine for smooth transition between tracks
    public IEnumerator SwitchTrachCoroutine(int index)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        var duration = 0.5f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0.0f, currentTime / duration);
            yield return null;
        }

        audioSource.clip = audioClips[index];

        currentTime = 0;
        start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0.5f, currentTime / duration);
            yield return null;
        }
    }
}
