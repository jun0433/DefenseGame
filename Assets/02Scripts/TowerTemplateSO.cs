using UnityEngine;

[CreateAssetMenu]
public class TowerTemplateSO : ScriptableObject
{
    public GameObject towerPrefab;  // 타워 생성 프리팹
    public Weapon[] weapon; // 레벨별 타워 정보

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;   // 타워 이미지
        public float damage;    // 공격력
        public float rate;      // 공격 속도
        public float range;     // 공격 범위
        public int cost;        // 건설 비용
        public int sell;        // 판매 비용
    }
}
