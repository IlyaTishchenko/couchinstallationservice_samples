using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    public Room room = null;

    [SerializeField]
    public string buttonString = "";

    [SerializeField]
    public string levelName = "";

    [SerializeField]
    public int trackIndex = 0;

    [SerializeField]
    public int goldMedalThreshold = 0;

    [SerializeField]
    public int sofasCount = 0;

    [SerializeField]
    public SofaImage sofaImage = SofaImage.Blue;

    [SerializeField]
    public string speechCloudString = "";

    [System.NonSerialized]
    public CameraParams cameraParams = null;

    [System.NonSerialized]
    List<Sofa> sofas = null;

    void Start()
    {
        Initialize();
        timer = 0.0f;
        isRunning = false;

        sofas = GetComponentsInChildren<Sofa>().ToList();
    }
    float timer = 0.0f;
    bool isRunning = false;

    void Update()
    {
        if (isRunning)
            timer += Time.deltaTime;
    }

    public void StartTimer()
    {
        timer = 0.0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTime()
    {
        return timer;
    }

    public void PlaySofaShine()
    {
        foreach (var s in sofas)
        {
            if (s.shineParticles == null || s.shineParticles.isPlaying)
                continue;

            if (s.isShining)
                s.shineParticles.Play();
        }
    }

    public void Initialize()
    {
        cameraParams = GetComponentInChildren<CameraParams>();
        if (cameraParams == null)
            Debug.LogError("There is no CameraParams component");
    }

    public void MovablesToStart()
    {
        var movables = GetComponentsInChildren<Movable>();

        if (movables == null)
            return;

        foreach (var m in movables)
        {
            m.MoveImmediate();
        }
    }
}

public enum SofaImage
{
    Blue,
    Brown,
    Red,
    WhiteDefault,
    WhiteClinic,
    Black,
    BrownBar,
    BenchBusStation,
    CinemaRed,
    BenchChurch,
    BenchDefault,
}
