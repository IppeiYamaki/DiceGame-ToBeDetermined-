using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp = 100;

    public TextMeshProUGUI hpText;

    void Start()
    {
        currentHp = maxHp;
        UpdateHpUI();
    }

    public void TakeDamage(int damage)
    {
        // プレイヤーのHPをダメージ分だけ減らす
        currentHp -= damage;

        // HPが0未満にならないようにする
        if (currentHp < 0)
        {
            currentHp = 0;
        }

        UpdateHpUI();
    }

    void UpdateHpUI()
    {
        // HP表示用のテキストを更新する
        if (hpText != null)
        {
            hpText.text = $"HP：{currentHp} / {maxHp}";
        }
    }
}