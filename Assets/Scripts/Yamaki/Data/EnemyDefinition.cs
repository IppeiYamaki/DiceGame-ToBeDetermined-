using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 敵の戦闘用マスターデータ
/// 敵名、最大HP、行動パターンを定義します
/// </summary>
[CreateAssetMenu(fileName = "EnemyDefinition_", menuName = "DiceGame/EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    [SerializeField]
    [Header("敵名")]
    [Tooltip("戦闘中に表示・参照する敵名")]
    private string m_enemyName = "Enemy";

    [SerializeField]
    [Header("最大HP")]
    [Tooltip("戦闘開始時の敵の最大HP")]
    private int m_maxHp = 50;

    [SerializeField]
    [Header("基礎攻撃力")]
    [Tooltip("行動パターンが未設定の場合に使用する敵の攻撃力")]
    private int m_attackPower = 15;

    [SerializeField]
    [Header("表示Sprite")]
    [Tooltip("敵をUI Imageに表示するためのSprite。設定されている場合はこちらを優先します")]
    private Sprite m_visualSprite;

    [SerializeField]
    [Header("表示Texture")]
    [Tooltip("Spriteが未設定の場合に使用するTexture2D")]
    private Texture2D m_visualTexture;

    [SerializeField]
    [Header("アイドルアニメ用Texture（複数）")]
    [Tooltip("待機中にループ表示するTexture群\n" +
             "例: 3枚設定すると 1→2→3→2→1→2→3... のように往復再生されます\n" +
             "空の場合は従来どおり表示Sprite/表示Textureの単一表示になります")]
    private List<Texture2D> m_idleTextures = new List<Texture2D>();

    [SerializeField]
    [Header("行動時Texture")]
    [Tooltip("攻撃や防御など行動した瞬間に表示するTexture（4枚目の画像）\n" +
             "未設定の場合は行動時もアイドルアニメを継続します")]
    private Texture2D m_actionTexture;

    [SerializeField]
    [Header("アイドルアニメ間隔（秒）")]
    [Tooltip("アイドルアニメの1コマあたりの表示時間")]
    [Min(0.05f)]
    private float m_idleFrameInterval = 0.4f;

    [SerializeField]
    [Header("HP帯別行動パターン")]
    [Tooltip("敵のHP割合に応じた行動セットのリスト\n" +
             "各HP帯（例: 100～51%, 50～0%）ごとに行動グループを定義します\n" +
             "空の場合は OnValidate で自動的に 100～0% の帯が1つ生成されます")]
    private List<EnemyHpRangeActionSet> m_hpRangeActionSets = new List<EnemyHpRangeActionSet>();

    /// <summary>
    /// 敵名
    /// </summary>
    public string EnemyName => m_enemyName;

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp => m_maxHp;

    /// <summary>
    /// 基礎攻撃力（行動パターンが未設定の場合のフォールバック値）
    /// </summary>
    public int AttackPower => m_attackPower;

    /// <summary>
    /// UI表示用Sprite
    /// </summary>
    public Sprite VisualSprite => m_visualSprite;

    /// <summary>
    /// UI表示用Texture
    /// </summary>
    public Texture2D VisualTexture => m_visualTexture;

    /// <summary>
    /// アイドルアニメ用Textureリスト（読み取り専用）
    /// 空の場合は単一画像表示にフォールバックします
    /// </summary>
    public IReadOnlyList<Texture2D> IdleTextures => m_idleTextures;

    /// <summary>
    /// 行動時に表示するTexture（未設定可）
    /// </summary>
    public Texture2D ActionTexture => m_actionTexture;

    /// <summary>
    /// アイドルアニメの1コマあたりの表示時間（秒）
    /// </summary>
    public float IdleFrameInterval => m_idleFrameInterval;

    /// <summary>
    /// HP帯別の行動セットリスト（読み取り専用）
    /// </summary>
    public IReadOnlyList<EnemyHpRangeActionSet> HpRangeActionSets => m_hpRangeActionSets;

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に戦闘用数値を有効範囲へ補正し、HP帯の不変条件を維持します
    /// - 数値補正: MaxHp >= 1, AttackPower >= 0
    /// - HP帯補正: 空なら 100～0% の帯を1つ生成
    ///            先頭の Upper = 100, 末尾の Lower = 0
    ///            隣接帯の連続性（帯[i].Lower = 帯[i+1].Upper + 1）を強制
    /// </summary>
    private void OnValidate()
    {
        // 数値補正
        m_maxHp = Mathf.Max(1, m_maxHp);
        m_attackPower = Mathf.Max(0, m_attackPower);

        // HP帯補正
        if (m_hpRangeActionSets == null)
        {
            m_hpRangeActionSets = new List<EnemyHpRangeActionSet>();
        }

        // 空の場合は 100～0 の帯を1つ生成
        if (m_hpRangeActionSets.Count == 0)
        {
            EnemyHpRangeActionSet defaultSet = new EnemyHpRangeActionSet();
            defaultSet.SetUpperPercent(100);
            defaultSet.SetLowerPercent(0);
            m_hpRangeActionSets.Add(defaultSet);
        }

        // 帯の不変条件を強制
        // 1. 先頭の Upper = 100
        if (m_hpRangeActionSets.Count > 0)
        {
            m_hpRangeActionSets[0].SetUpperPercent(100);
        }

        // 2. 末尾の Lower = 0
        if (m_hpRangeActionSets.Count > 0)
        {
            m_hpRangeActionSets[m_hpRangeActionSets.Count - 1].SetLowerPercent(0);
        }

        // 3. 隣接帯の連続性を強制（帯[i].Lower = 帯[i+1].Upper + 1）
        for (int i = 0; i < m_hpRangeActionSets.Count - 1; i++)
        {
            EnemyHpRangeActionSet current = m_hpRangeActionSets[i];
            EnemyHpRangeActionSet next = m_hpRangeActionSets[i + 1];

            // 次の帯の Upper を現在の帯の Lower - 1 に設定
            int expectedNextUpper = current.LowerPercent - 1;
            if (expectedNextUpper < 0)
            {
                // 現在の帯の Lower が 0 以下の場合は調整
                current.SetLowerPercent(1);
                expectedNextUpper = 0;
            }
            next.SetUpperPercent(expectedNextUpper);
        }

        // 4. 各帯の Upper >= Lower を保証
        foreach (EnemyHpRangeActionSet set in m_hpRangeActionSets)
        {
            if (set.UpperPercent < set.LowerPercent)
            {
                set.SetLowerPercent(set.UpperPercent);
            }
        }
    }
#endif
}
