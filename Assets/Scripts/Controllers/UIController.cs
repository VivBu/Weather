using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public event EventHandler OptionsMenuToggled;    

    public KeyCode OptionsMenuKey
    {
        get => _optionsMenuKey;
        set => _optionsMenuKey = value;
    }

    public bool IsOptionsMenuOpened
    {
        get;
        private set;
    } = false;

    public ReadOnlyDictionary<TextMeshProUGUI, (Func<string>, Func<bool>)> TextUpdateMap { get; }

    [SerializeField]
    private KeyCode _optionsMenuKey = KeyCode.E;

    [SerializeField]
    private Canvas _optionsMenu;

    private readonly Dictionary<TextMeshProUGUI, (Func<string>, Func<bool>)> _textUpdateMap = new();

    public UIController()
    {
        TextUpdateMap = new(_textUpdateMap);
    }

    private void Awake() {

        // initially, the options menu is not showed & cursor is hidden
       _optionsMenu.gameObject.SetActive(false);
       Cursor.visible = false;
       Cursor.lockState = CursorLockMode.Locked;
       OptionsMenuToggled?.Invoke(this, EventArgs.Empty);
    }

    void Update()
    {
        ProcessTextUpdates();
        ProcessGameOptions();
    }

    private void ProcessGameOptions()
    {
        if (Input.GetKeyDown(_optionsMenuKey))
        {
            IsOptionsMenuOpened = !_optionsMenu.gameObject.activeSelf;

            _optionsMenu.gameObject.SetActive(IsOptionsMenuOpened);
            Cursor.visible = IsOptionsMenuOpened;
            Cursor.lockState = IsOptionsMenuOpened ? CursorLockMode.None : CursorLockMode.Locked;
            OptionsMenuToggled?.Invoke(this, EventArgs.Empty);
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

    private void ProcessTextUpdates()
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
}
