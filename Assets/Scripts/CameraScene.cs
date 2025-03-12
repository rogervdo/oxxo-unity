using UnityEngine;

public class CameraScene : MonoBehaviour
{
    public int sceneId;
    Vector3 cameraPosition;

    void Awake()
    {
        cameraPosition = transform.position;
    }
}
