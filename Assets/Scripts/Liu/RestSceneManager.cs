using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RestSceneManager : MonoBehaviour
{
    [Header("HP設定")]
    public int currentHP = 40;
    public int maxHP = 100;

    [Header("HP UI")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    [Header("選択ボタン")]
    public Button healButton;
    public Button maxHPButton;

    [Header("結果画面")]
    public GameObject healResultPanel;
    public GameObject maxHPResultPanel;

    [Header("出発ボタン")]
    public Button healDepartButton;
    public Button maxHPDepartButton;

    [Header("戻るシーン名")]
    public string nextSceneName = "MapScene";

    void Start()
    {
        UpdateHPUI();

        healResultPanel.SetActive(false);
        maxHPResultPanel.SetActive(false);

        healButton.onClick.AddListener(OnClickHeal);
        maxHPButton.onClick.AddListener(OnClickMaxHP);

        healDepartButton.onClick.AddListener(OnClickDepart);
        maxHPDepartButton.onClick.AddListener(OnClickDepart);
    }

    void UpdateHPUI()
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        hpText.text = "HP: " + currentHP + " / " + maxHP;
    }

    void OnClickHeal()
    {
        int healAmount = currentHP / 2;

        currentHP += healAmount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        UpdateHPUI();

        healResultPanel.SetActive(true);
        maxHPResultPanel.SetActive(false);

        healButton.interactable = false;
        maxHPButton.interactable = false;
    }

    void OnClickMaxHP()
    {
        maxHP += 20;
        currentHP += 20;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        UpdateHPUI();

        maxHPResultPanel.SetActive(true);
        healResultPanel.SetActive(false);

        healButton.interactable = false;
        maxHPButton.interactable = false;
    }

    void OnClickDepart()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}