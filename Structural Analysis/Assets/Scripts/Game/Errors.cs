using TMPro;
using UnityEngine;

public class Errors : MonoBehaviour
{
    private string _output;
    private string _stack;
    [SerializeField]
    private GameObject _logMenu;
    [SerializeField]
    private TMP_Text _label;
    [SerializeField]
    private Transform _content;

    private void OnEnable() => Application.logMessageReceived += HandleLog;

    private void OnDisable() => Application.logMessageReceived -= HandleLog;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _logMenu.SetActive(!_logMenu.activeInHierarchy);
        }
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        _output = logString;
        _stack = stackTrace;
        TMP_Text text = Instantiate(_label, _content);
        text.text = _output;
        switch (type)
        {
            case LogType.Error:
                text.color = Color.red;
                break;
            case LogType.Warning:
                text.color = Color.yellow;
                break;
            case LogType.Assert:
                text.color = Color.gray;
                break;
            case LogType.Exception:
                text.color = Color.magenta;
                break;
            case LogType.Log:
                text.color = Color.white;
                break;
            default:
                break;
        }
    }
}
