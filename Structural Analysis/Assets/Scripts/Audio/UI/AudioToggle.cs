using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggle : AudioUI
{
    private Toggle _toggle;

    protected override void Awake()
    {
        base.Awake();
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(Play);
    }

    private void Play(bool value)
    {
        AudioPlay();
    }
}
