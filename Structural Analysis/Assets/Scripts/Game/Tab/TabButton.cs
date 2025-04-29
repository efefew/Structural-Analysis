using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TabButton : MonoBehaviour
{
    public Tab tab;
    [HideInInspector] public TabSystem tabSystem;
    private Toggle toggle;
    [SerializeField] private Button closeButton;
    public Text label;
    private void Awake()
    {
        label = GetComponent<Text>();
    }
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.group = tabSystem.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener((bool on) => tab.gameObject.SetActive(on));
        closeButton.onClick.AddListener(() => tabSystem.RemoveTab(this));
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}
