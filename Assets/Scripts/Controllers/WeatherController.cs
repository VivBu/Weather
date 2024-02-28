using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeatherController : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem[] _sunParticleSystems;
    [SerializeField]
    private ParticleSystem[] _rainParticleSystems;
    [SerializeField]
    private ParticleSystem[] _stormParticleSystems;
    [SerializeField]
    private ParticleSystem[] _thunderstormParticleSystems;


    [SerializeField]
    private Button _sunButton;
    [SerializeField]
    private Button _rainButton;
    [SerializeField]
    private Button _stormButton;
    [SerializeField]
    private Button _thunderstormButton;

    private Dictionary<ParticleSystem[], bool> _sequenceStates = new();

    private void Awake() 
    {
        foreach (var pair in new[] { 
            (_sunParticleSystems, _sunButton), 
            (_rainParticleSystems, _rainButton),
            (_stormParticleSystems, _stormButton),
            (_thunderstormParticleSystems, _thunderstormButton)})
            {
                var button = pair.Item2;
                var systems = pair.Item1;

                button.onClick.AddListener(() => {
                    var isRunning = IsSequenceRunning(systems);

                    if (isRunning)
                        StopSequence(systems);
                    else
                        StartSequence(systems);

                    StyleButton(button, !isRunning);
                });

                // initially all sequences are stopped
                StopSequence(systems);
                StyleButton(button, false);
            }
    }

    private void StartSequence(ParticleSystem[] sequence)
    {
        foreach(var ps in sequence)
            ps.Play();
        _sequenceStates[sequence] = true;
    }
    
    private void StopSequence(ParticleSystem[] sequence)
    {
        foreach (var ps in sequence)
            ps.Stop();
        _sequenceStates[sequence] = false;
    }

    private bool IsSequenceRunning(ParticleSystem[] sequence)
    {
        if (!_sequenceStates.ContainsKey(sequence))
            throw new InvalidOperationException("Only predefined particle sytems are allowed [_thunderstormParticleSystems, _rainParticleSystems, _sunnyParticleSystems]");

        return _sequenceStates[sequence];
    }

    private void StyleButton(Button button, bool isRunning)
     {
        var color = isRunning ? Color.green : Color.red;
        button.GetComponent<Image>().color = color;
     }
}

public class WeatherComponent
{
    public string Name 
    {
        get => _name;
        set => _name = value;
    }
    public Button Button 
    {
        get => _button;
        set => _button = value;
    }
    public List<ParticleSystem> ParticleSystems => _particleSystems;


    [SerializeField]
    private string _name;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private List<ParticleSystem> _particleSystems;
}
