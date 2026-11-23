using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private TowerTemplateSO[] towerTemplate;  // 타워 정보

    [SerializeField]
    private PlayerGold playerGold;  // 타워 건설 시 골드 감소 처리

    [SerializeField]
    private EnemySpawner enemySpanwer;  // 현재 맵에 존재하는 적 리스트 정보를 얻기 위함

    [SerializeField]
    private SystemTextViwer systemTextViwer;    // 시스템 메시지 출력

    [SerializeField]
    private ObjectDetector objectDetector;

    private bool isOnTrowerButton = false;  // 타워 건설 버튼 체크
    private GameObject followTowerClone = null; // 임시 타워 사용 완료 삭제 저장 변수
    private int towerType;  // 타워 종류

    private bool isOnTowerButton = false; // 타워 건설 눌렀는지 체크

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        // 버튼 중복 방지
        if (isOnTowerButton) return;
        
        // 타워 건설 가능 여부 확인
        // 1. 타워 건설 필요 골드 부족
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            // 골드 부족
            systemTextViwer.PrintText(MESSAGE.MONEY);
            return;
        }

        // 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followerTowerPrefab);
        followTowerClone.GetComponent<ObjectFollowMousePostion>().Setup(objectDetector);

        // 타워 건설 버튼 설정
        isOnTowerButton = true;

        // 타워 건설 취소
        StartCoroutine(nameof(OnTowerCancelProcess));

    }

    public void TrySpawnTower(Transform tileTransform)
    {

        // 타워 건설 눌렀을 떄 타워 건설
        if (!isOnTowerButton) return;

        Tile tile = tileTransform.GetComponent<Tile>();

        // 타워 생성 가능 여부 확인
        // 현재 타일에 위치에 이미 타워가 건설 되어 있음
        if (tile.isBuildTower)
        {

            systemTextViwer.PrintText(MESSAGE.BUILD);
            return;
        }

        // 다시 타워 건설 버튼 변수 설정
        isOnTowerButton = false;

        // 타워가 생성되어 있음으로 변경
        tile.isBuildTower = true;

        // 타워 건설에 필요한 골드 감소
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        // 선택한 타일의 위치에 타워 건설
        Vector3 position = tileTransform.position + Vector3.back;

        // 선택한 타일의 위치에 타워 생성
       var clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);

        // 타워 무기에 enemySpanwer 정보 전달
        clone.GetComponent<TowerWeapon>().Setup(enemySpanwer, playerGold, tile);

        // 임시 타워 삭제
        Destroy(followTowerClone);

        // 타워 건설 취소 메소드 중지
        StopCoroutine(nameof(OnTowerCancelProcess));
    }

    private IEnumerator OnTowerCancelProcess()
    {
        while (true)
        {
            // ESC 또는 마우스 오른쪽 버튼 눌렀을 떄 취소
            if(Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }
}
