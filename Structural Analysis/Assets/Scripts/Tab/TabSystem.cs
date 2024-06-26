using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(ToggleGroup))]
public class TabSystem : MonoBehaviour
{
    private List<TabButton> tabButtons = new();
    [SerializeField] private Button addButton;
    [SerializeField] private TabButton tabButtonPrefab;
    [SerializeField] private Tab defaultTab;
    private Transform content;
    void Start()
    {
        content = transform;
        addButton.onClick.AddListener(() => AddTab(defaultTab));
    }
    private void OnDestroy()
    {
        addButton.onClick.RemoveAllListeners();
    }
    public void AddTab(Tab tab)
    {
        TabButton tabButton = Instantiate(tabButtonPrefab, content);
        tabButton.tab = tab;
        tabButton.tabSystem = this;
        if (!tabButtons.Contains(tabButton))
            tabButtons.Add(tabButton);
        addButton.transform.SetAsLastSibling();
    }
    public void RemoveTab(TabButton tabButton)
    {
        tabButtons.Remove(tabButton);
        Destroy(tabButton.gameObject);
    }
}
