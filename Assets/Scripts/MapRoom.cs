using UnityEngine;

public class MapRoom : MonoBehaviour
{
    [Header("A Câmera deste Mapa")]
    public GameObject mapCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mapCamera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mapCamera.SetActive(false);
        }
    }
}
