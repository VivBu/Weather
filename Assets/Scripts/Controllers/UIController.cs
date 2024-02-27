using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public ReadOnlyDictionary<TextMeshProUGUI, (Func<string>, Func<bool>)> TextUpdateMap { get; }

    private readonly Dictionary<TextMeshProUGUI, (Func<string>, Func<bool>)> _textUpdateMap = new();

    public UIController()
    {
        TextUpdateMap = new(_textUpdateMap);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pair in TextUpdateMap)   
        {
            var needsUpdateFunc = pair.Value.Item2;

            if (needsUpdateFunc.Invoke())
            {
                var textMeshPro = pair.Key;
                var updateFunc = pair.Value.Item1;

                var newValue = updateFunc.Invoke();
                if (textMeshPro.text == newValue)
                    continue;
                textMeshPro.text = newValue;
            }
        }
    }

    public void RegisterTextUpdate(TextMeshProUGUI textMeshPro, Func<string> updateDelegate, Func<bool> needsUpdate)
    {
        _textUpdateMap.Add(textMeshPro, (updateDelegate, needsUpdate));
    }

    public void UnregisterTextUpdate(TextMeshProUGUI textMeshPro)
    {
        _textUpdateMap.Remove(textMeshPro);
    }
}
