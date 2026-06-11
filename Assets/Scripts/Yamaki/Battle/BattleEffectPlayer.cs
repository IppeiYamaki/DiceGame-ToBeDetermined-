using UnityEngine;

/// <summary>
/// 戦闘中のエフェクト再生を担当するコンポーネント
/// Player/Enemy/Center アンカーを持ち、指定位置にエフェクト Prefab を生成します
/// 
/// 使い方:
/// 1. Inspector で Player/Enemy/Center の Transform を登録します
/// 2. PlayEffect(prefab, anchor, duration) を呼ぶとアンカー配下にエフェクトが生成されます
/// 3. duration 秒後に自動的に Destroy されます（duration=0 で削除しない）
/// </summary>
public class BattleEffectPlayer : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーアンカー")]
    [Tooltip("プレイヤー位置を表す Transform（エフェクト生成の親）")]
    private Transform m_playerAnchor;

    [SerializeField]
    [Header("敵アンカー")]
    [Tooltip("敵位置を表す Transform（エフェクト生成の親）")]
    private Transform m_enemyAnchor;

    [SerializeField]
    [Header("中央アンカー")]
    [Tooltip("画面中央を表す Transform（エフェクト生成の親）")]
    private Transform m_centerAnchor;

    /// <summary>
    /// 指定されたエフェクトを指定アンカーに生成します
    /// </summary>
    /// <param name="effectPrefab">生成するエフェクト Prefab（null の場合は何もしない）</param>
    /// <param name="anchor">エフェクトの表示位置</param>
    /// <param name="duration">エフェクトを削除するまでの秒数（0 で削除しない）</param>
    /// <returns>生成されたエフェクトの GameObject（生成できなかった場合は null）</returns>
    public GameObject PlayEffect(GameObject effectPrefab, EffectAnchorType anchor, float duration = 0f)
    {
        if (effectPrefab == null)
        {
            // null Prefab は無視（エラーログなし）
            return null;
        }

        Transform anchorTransform = GetAnchorTransform(anchor);
        if (anchorTransform == null)
        {
            Debug.LogWarning($"[BattleEffectPlayer] アンカー {anchor} の Transform が設定されていません。");
            return null;
        }

        // エフェクトを生成
        GameObject effectInstance = Instantiate(effectPrefab, anchorTransform);
        effectInstance.transform.localPosition = Vector3.zero;

        // duration が 0 より大きい場合は自動削除
        if (duration > 0f)
        {
            Destroy(effectInstance, duration);
        }

        return effectInstance;
    }

    /// <summary>
    /// 敵行動視点で EffectAnchorType を解決します
    /// Self → Enemy, Opponent → Player
    /// </summary>
    /// <param name="effectPrefab">生成するエフェクト Prefab</param>
    /// <param name="anchor">エフェクトの表示位置（Self/Opponent の場合は変換）</param>
    /// <param name="duration">エフェクトを削除するまでの秒数</param>
    /// <returns>生成されたエフェクトの GameObject</returns>
    public GameObject PlayEnemyActionEffect(GameObject effectPrefab, EffectAnchorType anchor, float duration = 0f)
    {
        // Self → Enemy, Opponent → Player へ変換
        EffectAnchorType resolvedAnchor = anchor;
        if (anchor == EffectAnchorType.Self)
        {
            resolvedAnchor = EffectAnchorType.Self; // 敵視点の Self は Enemy アンカー
        }
        else if (anchor == EffectAnchorType.Opponent)
        {
            resolvedAnchor = EffectAnchorType.Opponent; // 敵視点の Opponent は Player アンカー
        }

        return PlayEffect(effectPrefab, resolvedAnchor, duration);
    }

    /// <summary>
    /// アンカータイプから対応する Transform を取得します
    /// </summary>
    /// <param name="anchor">アンカータイプ</param>
    /// <returns>対応する Transform（見つからない場合は null）</returns>
    private Transform GetAnchorTransform(EffectAnchorType anchor)
    {
        switch (anchor)
        {
            case EffectAnchorType.Self:
                // 敵視点の Self は Enemy アンカー
                return m_enemyAnchor;

            case EffectAnchorType.Opponent:
                // 敵視点の Opponent は Player アンカー
                return m_playerAnchor;

            case EffectAnchorType.Center:
                return m_centerAnchor;

            default:
                Debug.LogWarning($"[BattleEffectPlayer] 不明なアンカータイプ: {anchor}");
                return null;
        }
    }
}
