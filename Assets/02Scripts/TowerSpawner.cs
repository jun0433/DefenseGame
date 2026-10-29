using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private EnemySpawner enemySpanwer;  // 현재 맵에 존재하는 적 리스트 정보를 얻기 위함

    public void TrySpawnTower(Transform tileTransform)
    {

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

        // 선택한 타일의 위치에 타워 생성
       var clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);

        // 타워 무기에 enemySpanwer 정보 전달
        clone.GetComponent<TowerWeapon>().Setup(enemySpanwer);
    }
}
