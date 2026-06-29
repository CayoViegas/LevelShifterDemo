using UnityEngine;

public class MapBlock : MonoBehaviour
{
    [Header("Elementos Visuais")]
    public GameObject hoverOverlay;
    public GameObject kanjiIcon;

    public Vector2Int gridCoordinate;

    public void ToggleHover(bool state)
    {
        hoverOverlay.SetActive(state);
    }

    public void ToggleKanji(bool state)
    {
        kanjiIcon.SetActive(state);
    }
}
