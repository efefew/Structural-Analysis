using System.Windows.Forms;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(TMP_Text))]
[DisallowMultipleComponent]
public class CopyButton : MonoBehaviour
{
    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        GetComponent<Button>().onClick.AddListener(Copy);
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(Copy);
    }

    private void Copy()
    {
        Clipboard.SetText(_text.text);
    }
}
