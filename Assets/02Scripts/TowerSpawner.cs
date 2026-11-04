using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private int towerBuildGold = 50;    // 타워 건설 골드

    [SerializeField]
    private PlayerGold playerGold;  // 타워 건설 시 골드 감소 처리

    [SerializeField]
    private EnemySpawner enemySpanwer;  // 현재 맵에 존재하는 적 리스트 정보를 얻기 위함

    public void TrySpawnTower(Transform tileTransform)
    {
        // 타워 건설 가능 여부 확인
        // 1. 타워 건설 필요 골드 부족
        if (towerBuildGold > playerGold.CurrentGold) return;


        Tile tile = tileTransform.GetComponent<Tile>();

        // 타워 생성 가능 여부 확인
        // 1. 현재 타일에 위치에 이미 타워가 건설 되어 있음
        if (tile.isBuildTower)
        {
            Debug.Log("이미 건설되어 있음");
            return;
        }

        // 타워가 생성되어 있음으로 변경
        tile.isBuildTower = true;

        // 타워 건설에 필요한 골드 감소
        playerGold.CurrentGold -= towerBuildGold;

        // 선택한 타일의 위치에 타워 건설
        Vector3 position = tileTransform.position + Vector3.back;

        // 선택한 타일의 위치에 타워 생성
       var clone = Instantiate(towerPrefab, position, Quaternion.identity);

        // 타워 무기에 enemySpanwer 정보 전달
        clone.GetComponent<TowerWeapon>().Setup(enemySpanwer);
    }
}
