using UnityEngine;

public class ObjectFollowMousePostion : MonoBehaviour
{
    private ObjectDetector objectDetector;

    public void Setup(ObjectDetector objectDetector)
    {
        this.objectDetector = objectDetector;
    }

    private void Update()
    {
        if (objectDetector == null) return;

        transform.position = objectDetector.GetPointerWolrdPosition();
    }
}
