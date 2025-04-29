using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter))]
[DisallowMultipleComponent]
public class UpdateLayout : MonoBehaviour
{
    #region Fields

    private ContentSizeFitter _sizeFitter;
    private GameObject _gameObj;
    private RectTransform[] _rectTransforms;
    private HorizontalOrVerticalLayoutGroup _layoutGroup;
    private ScrollRect _scroll;
    private float _verticalScroll, _horizontalScroll;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        _sizeFitter = GetComponent<ContentSizeFitter>();
        if (transform.parent.parent.parent.GetComponent<ScrollRect>())
            _scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
        if (transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>())
            _layoutGroup = transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
        _gameObj = gameObject;
        LayoutGroup[] layouts = _gameObj.GetComponentsInChildren<LayoutGroup>();
        _rectTransforms = new RectTransform[layouts.Length];
        for (int i = 0; i < layouts.Length; i++)
            _rectTransforms[i] = layouts[i].GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        RestartSizeFitter();
    }

    public void RestartSizeFitter()
    {
        _ = StartCoroutine(IRestartSizeFitter());
    }
    private IEnumerator IRestartSizeFitter()
    {
        if (_scroll)
        {
            if (_scroll.horizontalScrollbar) _horizontalScroll = _scroll.horizontalScrollbar.value;
            if (_scroll.verticalScrollbar) _verticalScroll = _scroll.verticalScrollbar.value;
        }
        _sizeFitter.enabled = false;

        if (_layoutGroup)
        {
            _layoutGroup.enabled = false;
            yield return new WaitForEndOfFrame();
            _layoutGroup.enabled = true;
        }

        yield return new WaitForEndOfFrame();
        _sizeFitter.enabled = true;
        for (int i = 0; i < _rectTransforms.Length; i++)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransforms[i]);
        if (!_scroll) yield break;
        if (_scroll.horizontalScrollbar) _scroll.horizontalScrollbar.value = _horizontalScroll;
        if (_scroll.verticalScrollbar) _scroll.verticalScrollbar.value = _verticalScroll;
    }

    #endregion Methods
}