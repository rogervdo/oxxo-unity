using UnityEngine;
using UnityEngine.UI;

public class CameraButton : MonoBehaviour
{
    public Button uiButton; // Assign in Inspector
    private CameraController cameraController;
    private CameraMovement cameraMovement;

    void Start()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        cameraMovement = FindFirstObjectByType<CameraMovement>();

        if (cameraController == null)
        {
            Debug.LogError("CameraController script not found in the scene!");
        }

        if (uiButton != null)
        {
            uiButton.onClick.AddListener(OnButtonClick); // Attach button event
        }
        else
        {
            Debug.LogError("UI Button not assigned in the Inspector!");
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("UI Button Clicked!");
        if (cameraController != null)
        {
            cameraController.moveCamera(); // Call function to move camera
            cameraMovement.ResetMovimiento();
        }
    }
}
