using UnityEngine;
using System.Collections.Generic; // Necesario para Listas

// Asegura que este GameObject tenga un BoxCollider2D
[RequireComponent(typeof(BoxCollider2D))]
public class SpawnArea : MonoBehaviour
{
    // Asigna en el Inspector: Qué prefabs de anomalía pueden aparecer aquí.
    public List<GameObject> possibleAnomalies;

    // NUEVO: Margen interior para el spawn. Ajustar en Inspector.
    [Tooltip("Margen interior para evitar que el borde de la anomalía se salga. Ajustar según mitad del tamaño de anomalías grandes.")]
    public Vector2 spawnPadding = new Vector2(0.5f, 0.5f);

    // Referencia al collider del área.
    private BoxCollider2D areaCollider;
    // Profundidad Z para generar.
    private float spawnZ = 0f;

    void Awake()
    {
        // Obtiene el collider.
        areaCollider = GetComponent<BoxCollider2D>();
        // Importante: Lo marca como Trigger para no colisionar físicamente.
        areaCollider.isTrigger = true;

        // Avisos si falta configuración.
        if (possibleAnomalies == null || possibleAnomalies.Count == 0) {
            Debug.LogWarning($"SpawnArea '{gameObject.name}' no tiene anomalías asignadas.", this);
        }
        if (!areaCollider.enabled) {
             Debug.LogWarning($"BoxCollider2D en SpawnArea '{gameObject.name}' desactivado.", this);
        }
    }

    // Devuelve un punto aleatorio DENTRO de los límites ajustados por el padding.
    public Vector3 GetRandomPointInArea()
    {
        Bounds bounds = areaCollider.bounds;

        // Calcula límites internos restando el padding.
        float minX = bounds.min.x + spawnPadding.x;
        float maxX = bounds.max.x - spawnPadding.x;
        float minY = bounds.min.y + spawnPadding.y;
        float maxY = bounds.max.y - spawnPadding.y;

        // Si el padding es demasiado grande para el área, genera en el centro.
        if (minX >= maxX || minY >= maxY) {
             Debug.LogWarning($"Padding ({spawnPadding}) en '{gameObject.name}' muy grande para el área ({bounds.size}). Generando en centro.", this);
             return new Vector3(bounds.center.x, bounds.center.y, spawnZ);
        }

        // Punto aleatorio dentro de los límites ajustados.
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, spawnZ);
    }

    // Devuelve un prefab de anomalía aleatorio de la lista de esta área.
    public GameObject GetRandomAnomalyPrefab()
    {
        if (possibleAnomalies == null || possibleAnomalies.Count == 0) {
            return null; // No hay nada que generar.
        }
        int randomIndex = Random.Range(0, possibleAnomalies.Count);
        return possibleAnomalies[randomIndex];
    }

    // --- Gizmos para visualizar el área en el Editor ---
    void OnDrawGizmos() {
        if (areaCollider == null) areaCollider = GetComponent<BoxCollider2D>();
        if (areaCollider == null) return;
        Gizmos.color = Color.green; // Color del área normal
        Gizmos.DrawWireCube(areaCollider.bounds.center, areaCollider.bounds.size);

        // Dibuja el área interna con padding para visualizarla
        Vector3 paddedCenter = areaCollider.bounds.center;
        Vector3 paddedSize = new Vector3(
            Mathf.Max(0, areaCollider.bounds.size.x - spawnPadding.x * 2),
            Mathf.Max(0, areaCollider.bounds.size.y - spawnPadding.y * 2),
            0.1f // Un poco de profundidad para que se vea
        );
         Gizmos.color = new Color(0, 1, 0, 0.3f); // Verde semitransparente
         Gizmos.DrawCube(paddedCenter, paddedSize); // Dibuja el área útil real
    }
    void OnDrawGizmosSelected() {
         if (areaCollider == null) return;
         Gizmos.color = Color.yellow; // Color al seleccionar
         Gizmos.DrawWireCube(areaCollider.bounds.center, areaCollider.bounds.size);
         // También dibuja el padding interno al seleccionar
         Vector3 paddedCenter = areaCollider.bounds.center;
          Vector3 paddedSize = new Vector3(
              Mathf.Max(0, areaCollider.bounds.size.x - spawnPadding.x * 2),
              Mathf.Max(0, areaCollider.bounds.size.y - spawnPadding.y * 2),
              0.1f
          );
          Gizmos.color = new Color(1, 0.92f, 0.016f, 0.4f); // Amarillo semitransparente
          Gizmos.DrawCube(paddedCenter, paddedSize);
    }
    // --- Fin Gizmos ---
}