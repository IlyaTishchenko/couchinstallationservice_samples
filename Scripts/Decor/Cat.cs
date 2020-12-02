using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Movable
{
    [SerializeField]
    public ParticleSystem ZParticleSystem = null;

    void Update()
    {
        if (ZParticleSystem != null && !ZParticleSystem.isPlaying)
            ZParticleSystem.Play();
    }
}
