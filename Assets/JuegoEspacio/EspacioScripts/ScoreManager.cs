using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    private float scoreTimer = 0f;
    

    public Text scoreText;
    private bool scoreActivo = false;// UI.Text en lugar de TextMeshPro

    void Awake()
{
    if (Instance == null)
        Instance = this;
    else
        Destroy(gameObject);

    if (scoreText != null)
        scoreText.gameObject.SetActive(false); // 👈 apagado al iniciar
}


    void Update()
{
    scoreTimer += Time.deltaTime;

    if (scoreTimer >= 1f)
    {
        if (!scoreActivo && scoreText != null)
        {
            scoreText.gameObject.SetActive(true); // 👈 se activa al ganar el primer punto
            scoreActivo = true;
        }

        score += 3;
        scoreTimer = 0f;
        ActualizarUI();
    }
}

    public void RestarPorGolpe()
    {
        score -= 5;
        if (score < 0) score = 0;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "PUNTAJE: " + score.ToString("0000");
        }
    }
}
