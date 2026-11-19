using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;


public class UIPopupTower : MonoBehaviour
{

    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TowerAttackRange towerAttackRange;
    [SerializeField]
    private Button buttonUpgrade;
    [SerializeField]
    private SystemTextViwer systemTextViewer;

    private TowerWeapon currentTower;

    private void Awake()
    {
        OffPopup();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OffPopup();
        }
    }

    public void OnPopup(Transform towerWeapon)
    {
        // 출력해야하는 타워 정보를 받아와서 저장
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        // 타워 정보 갱신
        UpdateTowerData();
        // 타워 정보 Popup On
        gameObject.SetActive(true);
        // 타워공격 범위 활성화
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }

    public void OffPopup()
    {
        // 타워 정보 Popup Off
        gameObject.SetActive(false);
        // 타워 공격 범위 비활성화
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        imageTower.sprite = currentTower.TowerSprite;
        textDamage.text = $"Damage: {currentTower.Damage}";
        textRate.text = $"Rate: {currentTower.Rate}";
        textRange.text = $"Range: {currentTower.Range}";
        textLevel.text = $"Level: {currentTower.Level}";

        // 업그레이드 버튼 활성화
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel;
    }

    public void OnClickEventTowerUpgrade()
    {
        // 타워 업그레이드 시도
        bool isSuccess = currentTower.Upgrade();

        if (isSuccess)
        {
            // 타워 업그레이드 갱신
            UpdateTowerData();
            // 공격 범위 갱신
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            // 비용 부족
            systemTextViewer.PrintText(MESSAGE.MONEY);
        }
    }

    public void OnClickEventTowerSell()
    {
        // 타워 판매
        currentTower.Sell();
        // UI 비활성화
        OffPopup();
    }
}
