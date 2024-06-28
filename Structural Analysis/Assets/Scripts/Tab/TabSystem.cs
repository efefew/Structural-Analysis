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
    private Transform tabButtonsConteiner;
    [SerializeField] private Transform tabsConteiner;
    void Start()
    {
        tabButtonsConteiner = transform;
        addButton.onClick.AddListener(() => AddTab(defaultTab));
    }
    private void OnDestroy()
    {
        addButton.onClick.RemoveAllListeners();
    }
    public void AddTab(Tab tabPrefab)
    {
        TabButton tabButton = Instantiate(tabButtonPrefab, tabButtonsConteiner);
        Tab tab = Instantiate(tabPrefab, tabsConteiner);
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
