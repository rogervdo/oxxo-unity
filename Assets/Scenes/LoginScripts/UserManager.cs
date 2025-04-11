using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public int? CurrentUserId { get; private set; }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentUserId = null;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetCurrentUser(int userId) {
        CurrentUserId = userId;
    }

    public void ClearUser() {
        CurrentUserId = null;
    }
}