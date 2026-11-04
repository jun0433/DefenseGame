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
        // UpdateTowerData();
        // 타워 정보 Popup On
        gameObject.SetActive(true);
    }

    public void OffPopup()
    {
        // 타워 정보 Popup Off
        gameObject.SetActive(false);
    }
}
