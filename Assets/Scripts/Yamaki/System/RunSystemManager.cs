using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ラン全体で共有するSystem情報を管理する常駐Manager。
/// マスターデータ参照、共有表示設定、オプション値、ラン経過時間を保持します。
/// </summary>
public class RunSystemManager : MonoBehaviour
{
    [SerializeField]
    [Header("ダイスマスタ")]
    private DiceMasterRegistry m_diceMasterRegistry;

    [SerializeField]
    [Header("バフ/デバフマスタ")]
    private StatusEffectRegistry m_statusEffectRegistry;

    [SerializeField]
    [Header("戦闘アイコン設定")]
    private BattleIconSettings m_battleIconSettings;

    [SerializeField]
    [Header("敵エンカウントプール一覧")]
    [Tooltip("MapのBattleノードに割り当てる敵プール(Pool1, Pool2, ...)\n" +
             "ノード選択時にプール内のエンカウントグループから1つが抽選されます")]
    private List<EnemyEncounterPool> m_enemyEncounterPools = new List<EnemyEncounterPool>();

    [SerializeField]
    [Header("マスター音量")]
    [Range(0f, 1f)]
    private float m_masterVolume = 1f;

    [SerializeField]
    [Header("BGM音量")]
    [Range(0f, 1f)]
    private float m_bgmVolume = 1f;

    [SerializeField]
    [Header("SE音量")]
    [Range(0f, 1f)]
    private float m_seVolume = 1f;

    private static RunSystemManager s_instance;
    private float m_runElapsedTime;
    private readonly List<EnemyDefinition> m_nextBattleEnemies = new List<EnemyDefinition>();

    /// <summary>現在有効なSystem Manager。</summary>
    public static RunSystemManager Instance => s_instance;

    /// <summary>ダイス/役マスタ。</summary>
    public DiceMasterRegistry DiceMasterRegistry => m_diceMasterRegistry;

    /// <summary>バフ/デバフマスタ。</summary>
    public StatusEffectRegistry StatusEffectRegistry => m_statusEffectRegistry;

    /// <summary>戦闘アイコン設定。</summary>
    public BattleIconSettings BattleIconSettings => m_battleIconSettings;

    /// <summary>登録済みの敵エンカウントプール数。</summary>
    public int EnemyEncounterPoolCount => m_enemyEncounterPools != null ? m_enemyEncounterPools.Count : 0;

    /// <summary>次の戦闘で出現させる敵編成が抽選済みか。</summary>
    public bool HasNextBattleEnemies => m_nextBattleEnemies.Count > 0;

    /// <summary>次の戦闘で出現させる敵編成。</summary>
    public IReadOnlyList<EnemyDefinition> NextBattleEnemies => m_nextBattleEnemies;

    /// <summary>ラン開始からの経過時間。</summary>
    public float RunElapsedTime => m_runElapsedTime;

    /// <summary>マスター音量。</summary>
    public float MasterVolume => m_masterVolume;

    /// <summary>BGM音量。</summary>
    public float BgmVolume => m_bgmVolume;

    /// <summary>SE音量。</summary>
    public float SeVolume => m_seVolume;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyActiveRegistries();
    }

    private void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }

    private void Update()
    {
        m_runElapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// 登録済みマスタをグローバル参照へ反映します。
    /// </summary>
    public void ApplyActiveRegistries()
    {
        if (m_diceMasterRegistry != null)
        {
            DiceMasterRegistry.SetActive(m_diceMasterRegistry);
        }

        if (m_statusEffectRegistry != null)
        {
            StatusEffectRegistry.SetActive(m_statusEffectRegistry);
        }
    }

    /// <summary>
    /// ラン経過時間を0に戻します。
    /// </summary>
    public void ResetRunElapsedTime()
    {
        m_runElapsedTime = 0f;
    }

    /// <summary>
    /// 音量設定を更新します。
    /// </summary>
    public void SetVolumes(float masterVolume, float bgmVolume, float seVolume)
    {
        m_masterVolume = Mathf.Clamp01(masterVolume);
        m_bgmVolume = Mathf.Clamp01(bgmVolume);
        m_seVolume = Mathf.Clamp01(seVolume);
    }

    /// <summary>
    /// ダイスマスタ参照を差し替え、Activeへ反映します。
    /// </summary>
    public void SetDiceMasterRegistry(DiceMasterRegistry diceMasterRegistry)
    {
        m_diceMasterRegistry = diceMasterRegistry;
        ApplyActiveRegistries();
    }

    /// <summary>
    /// バフ/デバフマスタ参照を差し替え、Activeへ反映します。
    /// </summary>
    public void SetStatusEffectRegistry(StatusEffectRegistry statusEffectRegistry)
    {
        m_statusEffectRegistry = statusEffectRegistry;
        ApplyActiveRegistries();
    }

    /// <summary>
    /// 指定インデックスの敵エンカウントプールから編成を1グループ抽選し、次戦闘の敵編成として保持します。
    /// 抽選に成功した場合は true、プール未登録・インデックス範囲外・有効グループ無しの場合は編成を変更せず false を返します。
    /// </summary>
    public bool SelectNextBattleEnemiesFromPool(int poolIndex)
    {
        if (m_enemyEncounterPools == null || poolIndex < 0 || poolIndex >= m_enemyEncounterPools.Count)
        {
            Debug.LogWarning($"[RunSystemManager] 敵エンカウントプールのインデックスが不正です: {poolIndex}");
            return false;
        }

        EnemyEncounterPool pool = m_enemyEncounterPools[poolIndex];
        if (pool == null)
        {
            Debug.LogWarning($"[RunSystemManager] 敵エンカウントプール({poolIndex})が未設定です。");
            return false;
        }

        EnemyEncounterGroup group = pool.GetRandomGroup();
        if (group == null)
        {
            Debug.LogWarning($"[RunSystemManager] プール「{pool.name}」に有効なエンカウントグループがありません。");
            return false;
        }

        m_nextBattleEnemies.Clear();
        foreach (EnemyDefinition enemy in group.Enemies)
        {
            if (enemy != null)
            {
                m_nextBattleEnemies.Add(enemy);
            }
        }

        return m_nextBattleEnemies.Count > 0;
    }

    /// <summary>
    /// 次戦闘の敵編成をクリアします。
    /// </summary>
    public void ClearNextBattleEnemies()
    {
        m_nextBattleEnemies.Clear();
    }
}
