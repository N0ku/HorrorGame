using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System;

public class ProceduralGenerator : MonoBehaviour
{
    #region Local Class

    public class WallReference : MonoBehaviour
    {
        public ProceduralGenerator proceduralGenerator;
    }

    public class Cell
    {
        public int x;
        public int y;
        public string tag;
        public Color color;
        public Vector3 position;
        public bool Occupied;

        public Cell(int x, int y, Vector3 roomPosition)
        {
            this.x = x;
            this.y = y;
            this.position = new Vector3(x + roomPosition.x, roomPosition.y, y + roomPosition.z);
            Occupied = false;
        }
    }

    public class MapSize
    {

        public GameObject MapObject { get; set; }
        public List<WallReference> MapWalls { get; set; }
        public Cell[,] MapGrid { get; set; }
        public List<Cell> MapObjectPlacementCells { get; set; }
        public GameObject MapObjectsParent { get; set; }

        public MapSize(GameObject roomObject)
        {
            MapObject = roomObject;
            MapWalls = new List<WallReference>();
            MapGrid = new Cell[staticmapSizeX, staticmapSizeY];
            MapObjectPlacementCells = new List<Cell>();
            MapObjectsParent = new GameObject("WallParent");
        }

        public void SetPosition(Vector3 position)
        {
            MapObject.transform.position = position;
        }
    }
    #endregion


    [SerializeField] private int mapSizeX;
    [SerializeField] private int mapSizeY;
    [SerializeField] private GameObject mapTemplate;
    [SerializeField] private Material mapMaterial;
    [SerializeField] private bool displayGridInGame = true;
    [SerializeField] private List<GameObject> RoomPrefabs = new List<GameObject>();
    [SerializeField] private int numberOfRoom = 5;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private int maxPlacementAttempts = 10;
    static public int staticmapSizeX { get; set; }
    static public int staticmapSizeY { get; set; }


    // Pour les debilos, les prefab qu'on met sont en 2x3. Donc obliger de faire un spacing de 2 sinon ils se superposent.
    public static float wallSpacing = 2f;
    public Color northColor = Color.red;
    public Color southColor = Color.blue;
    public Color eastColor = Color.green;
    public Color westColor = Color.yellow;
    public Color elevator = Color.magenta;
    private NavMeshSurface[] navMeshSurface;
    private List<WallReference> specificRoomWalls = new List<WallReference>();
    private int placementAttempts = 0;

    [SerializeField] public RoomType roomInteractable;


    void Start()
    {

        GenerateRoom(1);
      
        navMeshSurface = FindObjectsOfType<NavMeshSurface>();

        foreach (var surface in navMeshSurface)
        {
            surface.BuildNavMesh();
        }

    }

    #region Map Generation

    void GenerateRoom(int count)
    {
        staticmapSizeX = mapSizeX;
        staticmapSizeY = mapSizeY;
        GameObject hiearchiNameMap = new GameObject("Room" + count);
        specificRoomWalls.Clear();
        MapSize generatedMap = new MapSize(hiearchiNameMap);
        int randomOffsetX = Mathf.RoundToInt(UnityEngine.Random.Range(-50f, 50f));
        int randomOffsetY = Mathf.RoundToInt(UnityEngine.Random.Range(-50f, 50f));
        generatedMap.SetPosition(new Vector3(randomOffsetX * 2 + 1, 0, randomOffsetY * 2));

        // Génération de la MapSize.
        for (int x = 0; x < mapSizeX; x++)
        {
            InstantiateWall(x + randomOffsetX, 0 + randomOffsetY, 0, generatedMap.MapObjectsParent.transform);
            InstantiateWall(x + randomOffsetX, mapSizeY - 1 + randomOffsetY, 0, generatedMap.MapObjectsParent.transform);
        }

        for (int y = 1; y < mapSizeY; y++)
        {
            InstantiateWall(0 + randomOffsetX, y + randomOffsetY, 90, generatedMap.MapObjectsParent.transform);
            InstantiateWall(mapSizeX + randomOffsetX, y + randomOffsetY, 90, generatedMap.MapObjectsParent.transform);
        }

        GenerateGrid(generatedMap);
        AssignTagsAndColors(generatedMap);
        PlaceRandomObjects(generatedMap);

    }


    void InstantiateWall(int x, int y, int rotation, Transform parent)
    {
        GameObject wall = Instantiate(mapTemplate, new Vector3(x * wallSpacing, 0, y * wallSpacing), Quaternion.identity, parent);

        if (mapMaterial != null)
        {
            MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = mapMaterial;
            }
        }
        wall.transform.rotation = Quaternion.Euler(0, rotation, 0);


        WallReference wallReference = wall.AddComponent<WallReference>();
        wallReference.proceduralGenerator = this;
        specificRoomWalls.Add(wallReference);
    }

    

    void GenerateGrid(MapSize room)
    {
        room.MapGrid = new Cell[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                room.MapGrid[x, y] = new Cell(x, y, room.MapObject.transform.position);
            }
        }
    }
    void AssignTagsAndColors(MapSize map)
    {
        foreach (var cell in map.MapGrid)
        {
            if (cell.y == 0)
            {
                cell.tag = "North";
                cell.color = northColor;
            }
            else if (cell.y == mapSizeY - 1)
            {
                cell.tag = "South";
                cell.color = southColor;
            }

            if (cell.x == 0)
            {
                cell.tag = "West";
                cell.color = westColor;
            }
            else if (cell.x == mapSizeX - 1)
            {
                cell.tag = "East";
                cell.color = eastColor;
            }
            //TODO Faire des case + tag pour Elevator

            if (cell.x != 0 && cell.x != mapSizeX - 1 && cell.y != 0 && cell.y != mapSizeY - 1)
            {
                map.MapObjectPlacementCells.Add(cell);
            }

            DrawCell(cell);
        }
    }

    void DrawCell(Cell cell)
    {
        if (displayGridInGame)
        {
            GameObject cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellObject.transform.parent = gridParent.transform;
            cellObject.transform.position = cell.position;
            cellObject.GetComponent<Renderer>().material.color = cell.color;
        }
    }
    #endregion

    #region Placement and collision testing Room

    void PlaceRandomObjects(MapSize room)
    {
        room.MapObjectsParent = new GameObject("ObjectsParent");

        for (int i = 0; i < numberOfRoom ; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, room.MapObjectPlacementCells.Count);
            Cell randomCell = room.MapObjectPlacementCells[randomIndex];

            // Récupère la taille du prefab et les cellules occupées par le prefab
            GameObject randomPrefab = RoomPrefabs[UnityEngine.Random.Range(0, RoomPrefabs.Count)];
            int randomRotations = UnityEngine.Random.Range(0, 4);
            randomPrefab.transform.rotation = Quaternion.Euler(0, randomRotations * 90, 0);
            Vector3 prefabSize = GetPrefabSize(randomPrefab);

            if (CanPlacePrefab(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells))
            {
                Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.MapObject.transform.position.x, 0, randomCell.y * wallSpacing + room.MapObject.transform.position.z);
                Instantiate(randomPrefab, objectPosition, Quaternion.identity, room.MapObjectsParent.transform);
                RemoveOccupiedCells(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells);
                placementAttempts = 0;
            }
            else
            {
              
                i--;
                placementAttempts++;
                if (placementAttempts >= maxPlacementAttempts)
                {
                    Debug.Log("Arrêt de la génération en raison du nombre maximum d'essais atteint.");
                    break; 
                }
            }
        }
    }

    Vector3 GetPrefabSize(GameObject prefab)
    {
        BoxCollider collider = prefab.GetComponent<BoxCollider>();
        if (collider != null)
        {
            Debug.Log(collider.size);
            return collider.size;
        }
        else
        {
            Debug.Log("Probleme de collider");
            return Vector3.zero;
        }
    }

    bool CanPlacePrefab(Cell startCell, Vector3 prefabSize, Cell[,] grid, List<Cell> availableCells)
    {
        Debug.Log("Je suis rentrer");

        // Ajoutez la logique pour vérifier si le placement du prefab est possible dans la cellule de départ
        for (int x = startCell.x; x < startCell.x + prefabSize.x / wallSpacing; x++)
        {
            for (int y = startCell.y; y < startCell.y + prefabSize.z / wallSpacing ; y++)
            {
                Debug.Log($"Checking grid[{x},{y}]");

                if (x < 0 || x >= grid.GetLength(0)|| y<0 || y>=grid.GetLength(1)|| grid[x, y].Occupied)
                {
                    Debug.Log("En Dehors de la grille ou déjà occuper");
                    return false;
                }
            }
        }
        return true;
    }

  
    void RemoveOccupiedCells(Cell startCell, Vector3 prefabSize, Cell[,] grid, List<Cell> availableCells)
    {
        // Retire les cellules occupées par le prefab de la liste des cellules disponibles
        for (int x = startCell.x; x < startCell.x + prefabSize.x ; x++)
        {
            for (int y = startCell.y; y < startCell.y + prefabSize.z ; y++)
            {
                if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x, y] != null)
                {
                    availableCells.Remove(grid[x, y]);
                    grid[x, y].Occupied = true;
                    Debug.Log(grid[x, y] + "C'est occupé");
                }
            }
        }
    }
    #endregion

    

}

//bool IsCellEmpty(Cell cell, Vector3 prefabSize, Cell[,] grid)
//{
//    for (int x = cell.x; x < cell.x + prefabSize.x / wallSpacing; x++)
//    {
//        for (int y = cell.y; y < cell.y + prefabSize.z / wallSpacing; y++)
//        {
//            if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1) || grid[x, y].Occupied)
//            {
//                return false; 
//            }
//        }
//    }
//    return true;
//}