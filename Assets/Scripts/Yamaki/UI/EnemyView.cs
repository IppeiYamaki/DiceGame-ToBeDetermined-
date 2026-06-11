using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敵表示用のView
/// 敵名、敵画像、HP、行動予測アイコン群を更新します
/// </summary>
public class EnemyView : MonoBehaviour
{
    [SerializeField]
    [Header("敵名テキスト")]
    private TMP_Text m_enemyNameText;

    [SerializeField]
    [Header("敵画像")]
    private Image m_enemyImage;

    [SerializeField]
    [Header("敵HP表示")]
    private HpView m_hpView;

    [SerializeField]
    [Header("敵Block表示")]
    [Tooltip("敵のBlock値を表示するView（任意）")]
    private BlockView m_blockView;

    [SerializeField]
    [Header("敵行動予測アイコン")]
    private IconListView m_intentIconListView;

    [SerializeField]
    [Header("敵ステータス効果アイコン")]
    [Tooltip("敵に付与されているバフ/デバフを表示するIconListView（任意）")]
    private IconListView m_statusIconListView;

    [SerializeField]
    [Header("アイコン設定")]
    private BattleIconSettings m_iconSettings;

    private Sprite m_runtimeEnemySprite;

    // アイドルアニメ用のフレームSprite（Textureから生成しキャッシュ）
    private readonly List<Sprite> m_idleFrameSprites = new List<Sprite>();

    // 行動時表示用のSprite（Textureから生成しキャッシュ）
    private Sprite m_actionSprite;

    // アイドルアニメの1コマあたりの表示時間
    private float m_idleFrameInterval = 0.4f;

    // アイドルアニメのコルーチン参照
    private Coroutine m_idleAnimationCoroutine;

    // 行動時表示中フラグ（trueの間はアイドルアニメを停止）
    private bool m_isActionVisualActive;

    // 現在表示中の敵定義（同一定義での再初期化を防ぐためのキャッシュ）
    private EnemyDefinition m_currentEnemyDefinition;

    /// <summary>
    /// 敵の基本表示を更新します
    /// </summary>
    /// <param name="enemyDefinition">敵定義</param>
    /// <param name="currentHp">現在HP</param>
    public void UpdateEnemy(EnemyDefinition enemyDefinition, int currentHp)
    {
        string enemyName = enemyDefinition != null ? enemyDefinition.EnemyName : "Enemy";
        int maxHp = enemyDefinition != null ? enemyDefinition.MaxHp : 1;

        UpdateEnemyName(enemyName);
        UpdateHp(currentHp, maxHp);
        UpdateEnemyVisual(enemyDefinition);
    }

    /// <summary>
    /// 敵名表示を更新します
    /// </summary>
    /// <param name="enemyName">敵名</param>
    public void UpdateEnemyName(string enemyName)
    {
        if (m_enemyNameText != null)
        {
            m_enemyNameText.text = string.IsNullOrEmpty(enemyName) ? "Enemy" : enemyName;
        }
    }

    /// <summary>
    /// 敵HP表示を更新します
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    /// <param name="maxHp">最大HP</param>
    public void UpdateHp(int currentHp, int maxHp)
    {
        if (m_hpView != null)
        {
            m_hpView.UpdateHp(currentHp, maxHp);
        }
    }

    /// <summary>
    /// 敵Block表示を更新します
    /// </summary>
    /// <param name="blockValue">現在Block値</param>
    public void UpdateBlock(int blockValue)
    {
        if (m_blockView != null)
        {
            m_blockView.UpdateBlock(blockValue);
        }
    }

    /// <summary>
    /// 敵画像を更新します
    /// アイドルアニメ用Textureが2枚以上設定されていれば往復ループアニメを開始します
    /// （例: 3枚 → 1,2,3,2,1,2,3,2,... の順で表示）
    /// 未設定の場合は従来どおり単一のSprite/Texture表示にフォールバックします
    /// </summary>
    /// <param name="enemyDefinition">敵定義</param>
    public void UpdateEnemyVisual(EnemyDefinition enemyDefinition)
    {
        if (m_enemyImage == null) return;

        // 同一の敵定義で既に初期化済みなら再初期化しない
        // （UpdateStatusViews等から頻繁に呼ばれてもアニメ・行動表示を維持する）
        if (enemyDefinition != null && enemyDefinition == m_currentEnemyDefinition)
        {
            return;
        }

        m_currentEnemyDefinition = enemyDefinition;

        // 既存のアニメを停止し、キャッシュを再構築
        StopIdleAnimation();
        BuildVisualSprites(enemyDefinition);

        // アイドルアニメが2枚以上あればループ再生を開始
        if (m_idleFrameSprites.Count >= 2)
        {
            m_enemyImage.sprite = m_idleFrameSprites[0];
            m_enemyImage.enabled = true;
            m_idleAnimationCoroutine = StartCoroutine(PlayIdleAnimationLoop());
            return;
        }

        // 1枚だけ設定されている場合はその1枚を表示
        if (m_idleFrameSprites.Count == 1)
        {
            m_enemyImage.sprite = m_idleFrameSprites[0];
            m_enemyImage.enabled = true;
            return;
        }

        // フォールバック: 従来の単一Sprite/Texture表示
        Sprite sprite = null;
        if (enemyDefinition != null)
        {
            sprite = enemyDefinition.VisualSprite;
            if (sprite == null && enemyDefinition.VisualTexture != null)
            {
                Texture2D texture = enemyDefinition.VisualTexture;
                m_runtimeEnemySprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sprite = m_runtimeEnemySprite;
            }
        }

        m_enemyImage.sprite = sprite;
        m_enemyImage.enabled = sprite != null;
    }

    /// <summary>
    /// 行動時画像を指定時間表示します（攻撃・防御などの行動演出用）
    /// 行動時Textureが未設定の場合は何もしません
    /// 表示終了後は自動的にアイドル表示へ復帰します
    /// </summary>
    /// <param name="duration">行動画像の表示時間（秒）</param>
    public void ShowActionVisual(float duration)
    {
        if (m_enemyImage == null || m_actionSprite == null) return;

        StartCoroutine(ShowActionVisualCoroutine(duration));
    }

    /// <summary>
    /// EnemyDefinition からアイドル/行動用Spriteのキャッシュを構築します
    /// </summary>
    /// <param name="enemyDefinition">敵定義</param>
    private void BuildVisualSprites(EnemyDefinition enemyDefinition)
    {
        m_idleFrameSprites.Clear();
        m_actionSprite = null;

        if (enemyDefinition == null) return;

        // アイドルアニメ用Textureからフレームを生成
        IReadOnlyList<Texture2D> idleTextures = enemyDefinition.IdleTextures;
        if (idleTextures != null)
        {
            foreach (Texture2D texture in idleTextures)
            {
                if (texture == null) continue;
                m_idleFrameSprites.Add(CreateSpriteFromTexture(texture));
            }
        }

        // 行動時Textureから行動Spriteを生成
        if (enemyDefinition.ActionTexture != null)
        {
            m_actionSprite = CreateSpriteFromTexture(enemyDefinition.ActionTexture);
        }

        m_idleFrameInterval = Mathf.Max(0.05f, enemyDefinition.IdleFrameInterval);
    }

    /// <summary>
    /// Texture2DからUI表示用のSpriteを生成します
    /// </summary>
    /// <param name="texture">元になるTexture2D</param>
    /// <returns>生成されたSprite</returns>
    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// アイドルアニメを往復順（1,2,3,2,1,...）で無限ループ再生します
    /// 行動時表示中はコマ送りを一時停止します
    /// </summary>
    private IEnumerator PlayIdleAnimationLoop()
    {
        int frameIndex = 0;
        int direction = 1;

        while (true)
        {
            // 行動時表示中はコマ送りを止めて待機
            if (m_isActionVisualActive)
            {
                yield return null;
                continue;
            }

            m_enemyImage.sprite = m_idleFrameSprites[frameIndex];

            yield return new WaitForSeconds(m_idleFrameInterval);

            // 往復インデックス更新（端で折り返す）
            frameIndex += direction;
            if (frameIndex >= m_idleFrameSprites.Count - 1)
            {
                frameIndex = m_idleFrameSprites.Count - 1;
                direction = -1;
            }
            else if (frameIndex <= 0)
            {
                frameIndex = 0;
                direction = 1;
            }
        }
    }

    /// <summary>
    /// 行動時画像を一定時間表示し、終了後にアイドル表示へ戻します
    /// </summary>
    /// <param name="duration">表示時間（秒）</param>
    private IEnumerator ShowActionVisualCoroutine(float duration)
    {
        m_isActionVisualActive = true;
        m_enemyImage.sprite = m_actionSprite;
        m_enemyImage.enabled = true;

        yield return new WaitForSeconds(Mathf.Max(0.05f, duration));

        m_isActionVisualActive = false;

        // アイドルアニメがない場合は先頭フレームまたは従来表示へ戻す
        if (m_idleFrameSprites.Count > 0)
        {
            m_enemyImage.sprite = m_idleFrameSprites[0];
        }
        else if (m_runtimeEnemySprite != null)
        {
            m_enemyImage.sprite = m_runtimeEnemySprite;
        }
        else if (m_currentEnemyDefinition != null && m_currentEnemyDefinition.VisualSprite != null)
        {
            m_enemyImage.sprite = m_currentEnemyDefinition.VisualSprite;
        }
    }

    /// <summary>
    /// アイドルアニメのコルーチンを停止します
    /// </summary>
    private void StopIdleAnimation()
    {
        if (m_idleAnimationCoroutine != null)
        {
            StopCoroutine(m_idleAnimationCoroutine);
            m_idleAnimationCoroutine = null;
        }

        m_isActionVisualActive = false;
    }

    /// <summary>
    /// 敵行動予測アイコンをBattleActionTypeから更新します
    /// </summary>
    /// <param name="actionTypes">表示したい行動種別リスト</param>
    public void UpdateIntentIcons(IReadOnlyList<BattleActionType> actionTypes)
    {
        if (m_intentIconListView == null) return;

        List<Sprite> sprites = new List<Sprite>();
        if (actionTypes != null && m_iconSettings != null)
        {
            foreach (BattleActionType actionType in actionTypes)
            {
                Sprite sprite = m_iconSettings.GetIconForActionType(actionType);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }
        }

        m_intentIconListView.UpdateIcons(sprites);
    }

    /// <summary>
    /// 敵行動予測アイコンをSpriteリストから直接更新します
    /// </summary>
    /// <param name="sprites">表示したいSpriteリスト</param>
    public void UpdateIntentIcons(IReadOnlyList<Sprite> sprites)
    {
        if (m_intentIconListView != null)
        {
            m_intentIconListView.UpdateIcons(sprites);
        }
    }

    /// <summary>
    /// 敵行動予測アイコンをSpriteと数値ラベルの組み合わせから更新します
    /// </summary>
    /// <param name="displays">表示したいアイコンデータリスト</param>
    public void UpdateIntentIconDisplays(IReadOnlyList<IconDisplayData> displays)
    {
        if (m_intentIconListView != null)
        {
            m_intentIconListView.UpdateIconDisplays(displays);
        }
    }

    /// <summary>
    /// 敵行動予測アイコンをクリアします
    /// </summary>
    public void ClearIntentIcons()
    {
        if (m_intentIconListView != null)
        {
            m_intentIconListView.ClearIcons();
        }
    }

    /// <summary>
    /// 敵ステータス効果アイコンをSpriteリストから更新します
    /// </summary>
    /// <param name="sprites">表示したいSpriteリスト（バフ/デバフ）</param>
    public void UpdateStatusIcons(IReadOnlyList<Sprite> sprites)
    {
        if (m_statusIconListView != null)
        {
            m_statusIconListView.UpdateIcons(sprites);
        }
    }

    /// <summary>
    /// 敵ステータス効果アイコンをクリアします
    /// </summary>
    public void ClearStatusIcons()
    {
        if (m_statusIconListView != null)
        {
            m_statusIconListView.ClearIcons();
        }
    }
}
