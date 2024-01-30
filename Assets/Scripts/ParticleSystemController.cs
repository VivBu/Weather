using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSystemController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] _particleSystems;

    [SerializeField]
    private Button _button;

    private void OnAwake()
    {
        if (_particleSystems is null || _particleSystems.Length == 0)
            throw new System.Exception("Please assign the particle system to control.");
    }

    public void StopParticleSystem()
    {
        foreach (ParticleSystem particleSystem in _particleSystems)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }


}
