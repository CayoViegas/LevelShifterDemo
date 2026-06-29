using UnityEngine;

public class LevelShifterManager : MonoBehaviour
{
    public bool isLevelShifterActive = false;

    [Header("Configurações do Grid")]
    public int columns = 3;
    public int rows = 2;

    [Header("Referências Globais")]
    public GameObject globalCamera;
    public GameObject player;

    private MapBlock[,] mapGrid;
    private Vector2Int cursorCoord = new Vector2Int(0, 0);
    private MapBlock firstSelectedMap;

    private Rigidbody2D playerRb;
    private PlayerController playerController;

    void Start()
    {
        mapGrid = new MapBlock[columns, rows];

        playerRb = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();

        MapBlock[] allMaps = FindObjectsByType<MapBlock>(FindObjectsSortMode.None);
        foreach (MapBlock map in allMaps)
        {
            int xCoord = Mathf.RoundToInt(map.transform.position.x / 20f);
            int yCoord = Mathf.RoundToInt(map.transform.position.y / 12f);

            map.gridCoordinate = new Vector2Int(xCoord, yCoord);
            mapGrid[xCoord, yCoord] = map;
        }

        RefreshHovers();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleShifter(!isLevelShifterActive);
        }

        if (!isLevelShifterActive) return;

        HandleNavigation();
        HandleSelection();
    }

    private void ToggleShifter(bool state)
    {
        isLevelShifterActive = state;
        globalCamera.SetActive(state);
        
        if (isLevelShifterActive)
        {
            playerController.enabled = false;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
            playerRb.linearVelocity = Vector2.zero;

            MapBlock currentMap = GetMapUnderPlayer();
            if (currentMap != null)
            {
                player.transform.SetParent(currentMap.transform);
                cursorCoord = currentMap.gridCoordinate;
            }

            RefreshHovers();
        }
        else
        {
            player.transform.SetParent(null);
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            playerController.enabled = true;

            if (firstSelectedMap != null)
            {
                firstSelectedMap.ToggleKanji(false);
                firstSelectedMap = null;
            }
            
            RefreshHovers();
        }
    }

    private MapBlock GetMapUnderPlayer()
    {
        foreach (MapBlock map in mapGrid)
        {
            if (map == null) continue;

            BoxCollider2D mapCollider = map.GetComponent<BoxCollider2D>();

            if (mapCollider != null)
            {
                if (mapCollider.bounds.Contains(player.transform.position))
                {
                    return map;
                }
            }
        }

        return null;
    }

    private void RefreshHovers()
    {
        foreach (MapBlock map in mapGrid)
        {
            if (map != null) map.ToggleHover(false);
        }

        if (isLevelShifterActive)
        {
            mapGrid[cursorCoord.x, cursorCoord.y].ToggleHover(true);
        }
    }

    private void HandleNavigation()
    {
        Vector2Int oldCoord = cursorCoord;

        if (Input.GetKeyDown(KeyCode.RightArrow)) cursorCoord.x = Mathf.Clamp(cursorCoord.x + 1, 0, columns - 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) cursorCoord.x = Mathf.Clamp(cursorCoord.x - 1, 0, columns - 1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) cursorCoord.y = Mathf.Clamp(cursorCoord.y + 1, 0, rows - 1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) cursorCoord.y = Mathf.Clamp(cursorCoord.y - 1, 0, rows - 1);

        if (oldCoord != cursorCoord)
        {
            RefreshHovers();
        }
    }

    private void HandleSelection()
    {
        MapBlock mapUnderCursor = mapGrid[cursorCoord.x, cursorCoord.y];

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (firstSelectedMap != null)
            {
                firstSelectedMap.ToggleKanji(false);
                firstSelectedMap = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (firstSelectedMap == null)
            {
                firstSelectedMap = mapUnderCursor;
                firstSelectedMap.ToggleKanji(true);
            }
            else
            {
                if (firstSelectedMap != mapUnderCursor)
                {
                    mapUnderCursor.ToggleKanji(true);
                    SwapMaps(firstSelectedMap, mapUnderCursor);
                }

                firstSelectedMap.ToggleKanji(false);
                mapUnderCursor.ToggleKanji(false);
                firstSelectedMap = null;
            }
        }
    }

    private void SwapMaps(MapBlock mapA, MapBlock mapB)
    {
        Vector2Int tempCoord = mapA.gridCoordinate;
        mapA.gridCoordinate = mapB.gridCoordinate;
        mapB.gridCoordinate = tempCoord;

        mapGrid[mapA.gridCoordinate.x, mapA.gridCoordinate.y] = mapA;
        mapGrid[mapB.gridCoordinate.x, mapB.gridCoordinate.y] = mapB;

        Vector3 tempPos = mapA.transform.position;
        mapA.transform.position = mapB.transform.position;
        mapB.transform.position = tempPos;

        RefreshHovers();
    }
}
