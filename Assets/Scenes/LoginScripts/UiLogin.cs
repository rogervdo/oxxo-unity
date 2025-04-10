using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public InputField inputNickname;
    public InputField inputPassword;

    public void OnLoginButtonClick()
    {
        string nickname = inputNickname.text;
        string password = inputPassword.text;

        APIManagerLogin.Instance.IniciarLogin(nickname, password);
    }
}
