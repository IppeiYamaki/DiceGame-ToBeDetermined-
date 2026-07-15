using System.Collections;
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

    [Header("サウンド")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip healSound;
    public AudioClip levelUpSound;

    [Header("戻るシーン名")]
    public string nextSceneName = "NewScene";

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
        PlaySound(clickSound);

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

        StartCoroutine(PlayHealSound());
    }

    void OnClickMaxHP()
    {
        PlaySound(clickSound);

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

        StartCoroutine(PlayLevelUpSound());
    }

    void OnClickDepart()
    {
        PlaySound(clickSound);
        StartCoroutine(LoadNextScene());
    }

    IEnumerator PlayHealSound()
    {
        yield return new WaitForSeconds(0.1f);
        PlaySound(healSound);
    }

    IEnumerator PlayLevelUpSound()
    {
        yield return new WaitForSeconds(0.1f);
        PlaySound(levelUpSound);
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.15f);
        SceneManager.LoadScene(nextSceneName);
    }

    void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}