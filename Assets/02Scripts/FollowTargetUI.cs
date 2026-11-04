using UnityEngine;

public class FollowTargetUI : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void SetTarget(Transform target) => this.target = target;

    private void LateUpdate()
    {
        // 적이 파괴되면 UI 삭제
        if (target == null)
        {
           Destroy(gameObject);
            return;
        }

        // 오브젝트의 위치가 갱신된 이후에 UI도 함께 위치를 설정하도록 하기 위해 LateUpdate()에서 호출

        // 오브젝트의 월드 좌표를 기준으로 화면의 좌표값을 구함

        rectTransform.position = mainCamera.WorldToScreenPoint(target.position);
    }
}
