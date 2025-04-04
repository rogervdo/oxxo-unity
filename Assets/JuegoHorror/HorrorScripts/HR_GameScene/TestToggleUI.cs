using UnityEngine;

public class TestToggleUI : MonoBehaviour
{
    public GameObject panel; // Assign the UI element in the Inspector

    public void ToggleUI()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
