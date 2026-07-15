using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 敵をクリックしてターゲット選択できるようにするコンポーネント。
/// EnemyView の子Objectとして配置し、TargetMarkerを表示/非表示します。
/// </summary>
public class EnemySelectable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Header("ターゲットマーカー（任意）")]
    private GameObject m_targetMarker;

    [SerializeField]
    [Header("自動生成用マーカー画像（任意）")]
    [Tooltip("m_targetMarker 未設定時に上に重ねるための Sprite を指定できます。未指定なら単色の半透明オーバーレイになります。")]
    private Sprite m_targetMarkerSprite;

    [SerializeField]
    [Header("自動生成マーカーの色")]
    private Color m_targetMarkerColor = new Color(1f, 0f, 0f, 0.45f);

    public event Action<EnemySelectable> Clicked;

    private bool m_isTargeted;

    private void Awake()
    {
        if (m_targetMarker != null)
        {
            m_targetMarker.SetActive(false);
            return;
        }

        // m_targetMarker が未設定なら UI オーバーレイを自動生成する
        // このコンポーネントは EnemyView の UI 要素（RectTransform）上に置かれる想定
        var rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            // UI でない場合は自動生成を行わない
            return;
        }

        var markerGo = new GameObject("TargetMarker", typeof(RectTransform));
        markerGo.transform.SetParent(transform, false);
        var markerRt = markerGo.GetComponent<RectTransform>();
        markerRt.anchorMin = Vector2.zero;
        markerRt.anchorMax = Vector2.one;
        markerRt.offsetMin = Vector2.zero;
        markerRt.offsetMax = Vector2.zero;

        var img = markerGo.AddComponent<Image>();
        img.raycastTarget = false;
        if (m_targetMarkerSprite != null)
        {
            img.sprite = m_targetMarkerSprite;
            img.color = m_targetMarkerColor;
            img.type = Image.Type.Sliced;
            img.preserveAspect = true;
        }
        else
        {
            // 単色矩形の半透明オーバーレイ
            img.color = m_targetMarkerColor;
        }

        // 既存の表示より手前に表示する
        markerGo.transform.SetAsLastSibling();
        markerGo.SetActive(false);
        m_targetMarker = markerGo;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetTargeted(!m_isTargeted);
        Clicked?.Invoke(this);
    }

    public void SetTargeted(bool targeted)
    {
        m_isTargeted = targeted;
        if (m_targetMarker != null) m_targetMarker.SetActive(targeted);
    }

    public bool IsTargeted => m_isTargeted;
}
