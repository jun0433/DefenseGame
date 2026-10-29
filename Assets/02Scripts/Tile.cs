using UnityEngine;

public class Tile : MonoBehaviour
{

    // 현재 타일의 타워 건설 여부 확인
    public bool isBuildTower { set; get; }

    private void Awake()
    {
        isBuildTower = false;
    }
}
