using UnityEngine;
using System.Collections;

public class EnemySpawnerEspacial : MonoBehaviour
{
    public GameObject enemyGameObject;

    public float maxHeight;
    public float minHeight;
    public float timeToSpawnMin; 
    public float timeToSpawnMax; 
    private float posicionOriginalX;


     IEnumerator SpawnerTimer()
    {
        // Espera un tiempo aleatorio entre el tiempo mínimo y máximo
        yield return new WaitForSeconds(Random.Range(timeToSpawnMin, timeToSpawnMax));

        // Instancia un enemigo en una posición aleatoria dentro de los límites especificados
        Instantiate(enemyGameObject, new Vector3(transform.position.x, Random.Range(minHeight, maxHeight), 0), Quaternion.identity);

        // Vuelve a llamar la corutina para seguir generando enemigos
        StartCoroutine(SpawnerTimer());
    }

   void Start()
{
    posicionOriginalX = transform.position.x;
    StartCoroutine(SpawnerTimer());
}

public void CambiarLadoSpawner()
{
    posicionOriginalX *= -1;
    transform.position = new Vector3(posicionOriginalX, transform.position.y, transform.position.z);
}
public void FlipSpawnerPosition()
{
    foreach (var spawner in FindObjectsOfType<EnemySpawnerEspacial>())
    {
        spawner.CambiarLadoSpawner();
    }
}



}
