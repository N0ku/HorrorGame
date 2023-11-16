using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System;
using static RoomGenerator;

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
    [Header("DevTool Configuration")]

    [SerializeField] private bool displayGridInGame = true;
    [SerializeField] public RoomType roomInteractable;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private int maxPlacementAttempts = 10;
    public Color northColor = Color.red;
    public Color southColor = Color.blue;
    public Color eastColor = Color.green;
    public Color westColor = Color.yellow;
    [Space(15)]



    [Header("Creator Configuration")]
    [SerializeField] private int mapSizeX;
    [SerializeField] private int mapSizeY;
    [SerializeField] private GameObject mapTemplate;
    [SerializeField] private Material mapMaterial;
    [SerializeField] private List<GameObject> RoomPrefabs = new List<GameObject>();
    [SerializeField] private int numberOfRoom = 5;
    [Space(15)]

    [Header("Configuration de l'Étage (Selectionner l'étage)")]
    [SerializeField] private EtageType etageType = EtageType.Creator;
    [Space(15)]


    [Header("Configuration Étage 1")]
    [SerializeField] private int staticmapSizeX_Etage1;
    [SerializeField] private int staticmapSizeY_Etage1;
    [SerializeField] private Material mapMaterial_Etage1;
    [SerializeField] private List<GameObject> RoomPrefabs_Etage1 = new List<GameObject>();
    [SerializeField] private GameObject HistoryRoom_Etage1;
    [SerializeField] private GameObject ThomasRoom_Etage1 ;
    [SerializeField] private GameObject ElevatorExit_Etage1; 
    [Space(15)]



    [Header("Configuration Étage 2")]
    [SerializeField] private int staticmapSizeX_Etage2;
    [SerializeField] private int staticmapSizeY_Etage2;
    [SerializeField] private Material mapMaterial_Etage2;
    [SerializeField] private List<GameObject> RoomPrefabs_Etage2 = new List<GameObject>();
    [SerializeField] private GameObject HistoryRoom_Etage2 ;
    [SerializeField] private GameObject ThomasRoom_Etage2  ;
    [SerializeField] private GameObject ElevatorExit_Etage2;

    [Space(15)]

    [Header("Configuration Étage 3")]
    [SerializeField] private int staticmapSizeX_Etage3;
    [SerializeField] private int staticmapSizeY_Etage3;
    [SerializeField] private Material mapMaterial_Etage3;
    [SerializeField] private List<GameObject> RoomPrefabs_Etage3 = new List<GameObject>();
    [SerializeField] private GameObject HistoryRoom_Etage3  ;
    [SerializeField] private GameObject ThomasRoom_Etage3   ;
    [SerializeField] private GameObject ElevatorExit_Etage3 ;

    static public int staticmapSizeX { get; set; }
    static public int staticmapSizeY { get; set; }

    public GameObject historyRoom;
    public GameObject thomasRoom;
    public GameObject elevatorExitRoom;




    // Pour les debilos, les prefab qu'on met sont en 2x3. Donc obliger de faire un spacing de 2 sinon ils se superposent.
    public static float wallSpacing = 2f;

    //public Color elevator = Color.magenta;
    private NavMeshSurface[] navMeshSurface;
    private List<WallReference> specificRoomWalls = new List<WallReference>();
    private int placementAttempts = 0;



    void Start()
    {

        switch (etageType)
        {
            case EtageType.Etage1:
                staticmapSizeX = staticmapSizeX_Etage1;
                staticmapSizeY = staticmapSizeY_Etage1;
                mapMaterial = mapMaterial_Etage1;
                RoomPrefabs = RoomPrefabs_Etage1;
                historyRoom = HistoryRoom_Etage1;
                thomasRoom = ThomasRoom_Etage1;
                elevatorExitRoom = ElevatorExit_Etage1;
                numberOfRoom = 9;
                break;
            case EtageType.Etage2:
                staticmapSizeX = staticmapSizeX_Etage2;
                staticmapSizeY = staticmapSizeY_Etage2;
                mapMaterial = mapMaterial_Etage2;
                RoomPrefabs = RoomPrefabs_Etage2;
                historyRoom = HistoryRoom_Etage2;
                thomasRoom = ThomasRoom_Etage2;
                elevatorExitRoom = ElevatorExit_Etage2;
                numberOfRoom = 12;
                break;
            case EtageType.Etage3:
                staticmapSizeX = staticmapSizeX_Etage3;
                staticmapSizeY = staticmapSizeY_Etage3;
                mapMaterial = mapMaterial_Etage3;
                RoomPrefabs = RoomPrefabs_Etage3;
                historyRoom = HistoryRoom_Etage3;
                thomasRoom = ThomasRoom_Etage3;
                elevatorExitRoom = ElevatorExit_Etage3;
                numberOfRoom = 15;
                break;
            case EtageType.Creator:
                staticmapSizeX = mapSizeX;
                staticmapSizeY = mapSizeY;
                break;
            default:
                Debug.LogError("Type d'étage non géré : " + etageType);
                break;
        }
        GenerateRoom();

        navMeshSurface = FindObjectsOfType<NavMeshSurface>();

        foreach (var surface in navMeshSurface)
        {
            surface.BuildNavMesh();
        }

    }

    #region Map Generation

    void GenerateRoom()
    {

        GameObject hiearchiNameMap = new GameObject("Room");
        specificRoomWalls.Clear();
        MapSize generatedMap = new MapSize(hiearchiNameMap);
        generatedMap.SetPosition(new Vector3(0, 0, 0));

        // Génération de la MapSize.
        for (int x = 0; x < staticmapSizeX; x++)
        {
            InstantiateWall(x, 0, 0,0,0, generatedMap.MapObjectsParent.transform);
            InstantiateWall(x, staticmapSizeY - 1, 0, 0, 0, generatedMap.MapObjectsParent.transform);
        }

        for (int y = 1; y < staticmapSizeY; y++)
        {
            InstantiateWall(0, y,0, 90, 0, generatedMap.MapObjectsParent.transform);
            InstantiateWall(staticmapSizeX, y, 0, 90,0, generatedMap.MapObjectsParent.transform);
        }
        

        GenerateGrid(generatedMap);
        AssignTagsAndColors(generatedMap);
        PlaceRandomObjects(generatedMap);
        GenerateCeil(generatedMap);

    }

    void GenerateCeil(MapSize map)
    {
        map.SetPosition(new Vector3(0, 0, 0));
        for (int x = 0; x < staticmapSizeX; x++)
        {
            InstantiateWall(x, x, 1.5f, 0, 90, map.MapObjectsParent.transform);
            for (int y = 0; y < staticmapSizeY; y++)
            {
                InstantiateWall(x, y - 1,1.5f, 0, 90, map.MapObjectsParent.transform);
            }
        }
        //for (int y = 1; y < staticmapSizeY; y++)
        //{
        //    InstantiateWall(0, y, 90, 90, map.MapObjectsParent.transform);
        //    InstantiateWall(staticmapSizeX, y, 90, 90, map.MapObjectsParent.transform);
        //}
    }


    void InstantiateWall(int x, int y,float z, int rotation,int rotationX, Transform parent)
    {
        GameObject wall = Instantiate(mapTemplate, new Vector3(x * wallSpacing, z * wallSpacing, y * wallSpacing), Quaternion.identity, parent);

        if (mapMaterial != null)
        {
            MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = mapMaterial;
            }
        }
        wall.transform.rotation = Quaternion.Euler(rotationX, rotation, 0);


        WallReference wallReference = wall.AddComponent<WallReference>();
        wallReference.proceduralGenerator = this;
        specificRoomWalls.Add(wallReference);
    }



    void GenerateGrid(MapSize room)
    {
        room.MapGrid = new Cell[staticmapSizeX, staticmapSizeY];

        for (int x = 0; x < staticmapSizeX; x++)
        {
            for (int y = 0; y < staticmapSizeY; y++)
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
            else if (cell.y == staticmapSizeY - 1)
            {
                cell.tag = "South";
                cell.color = southColor;
            }

            if (cell.x == 0)
            {
                cell.tag = "West";
                cell.color = westColor;
            }
            else if (cell.x == staticmapSizeX - 1)
            {
                cell.tag = "East";
                cell.color = eastColor;
            }
            //TODO Faire des case + tag pour Elevator

            if (cell.x != 0 && cell.x != staticmapSizeX && cell.y != 0 && cell.y != staticmapSizeY)
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
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, room.MapObjectPlacementCells.Count);
            Cell randomCell = room.MapObjectPlacementCells[randomIndex];
            switch (i)
            {
                case 1:
                    GameObject randomPrefab = historyRoom;
                    int randomRotations = UnityEngine.Random.Range(0, 4);
                    randomPrefab.transform.rotation = Quaternion.Euler(0, randomRotations * 90, 0);
                    Vector3 prefabSize = GetPrefabSize(randomPrefab);

                    if (CanPlacePrefab(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, randomPrefab.transform.rotation))
                    {
                        Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.MapObject.transform.position.x, 0, randomCell.y * wallSpacing + room.MapObject.transform.position.z);

                        // Applique la rotation avant de placer l'objet
                        GameObject instantiatedPrefab = Instantiate(randomPrefab, objectPosition, Quaternion.identity, room.MapObjectsParent.transform);
                        instantiatedPrefab.transform.rotation = randomPrefab.transform.rotation;

                        RemoveOccupiedCells(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, instantiatedPrefab.transform.rotation);
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
                    break;
                case 2:
                    randomPrefab = thomasRoom;
                    randomRotations = UnityEngine.Random.Range(0, 4);
                    randomPrefab.transform.rotation = Quaternion.Euler(0, randomRotations * 90, 0);
                    prefabSize = GetPrefabSize(randomPrefab);

                    if (CanPlacePrefab(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, randomPrefab.transform.rotation))
                    {
                        Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.MapObject.transform.position.x, 0, randomCell.y * wallSpacing + room.MapObject.transform.position.z);

                        // Applique la rotation avant de placer l'objet
                        GameObject instantiatedPrefab = Instantiate(randomPrefab, objectPosition, Quaternion.identity, room.MapObjectsParent.transform);
                        instantiatedPrefab.transform.rotation = randomPrefab.transform.rotation;

                        RemoveOccupiedCells(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, instantiatedPrefab.transform.rotation);
                        placementAttempts = 0;
                    }
                    else
                    {
                        Debug.Log("PAS LA 2 !");

                        i--;
                        placementAttempts++;
                        if (placementAttempts >= maxPlacementAttempts)
                        {
                            Debug.Log("Arrêt de la génération en raison du nombre maximum d'essais atteint.");
                            break;
                        }
                    }

                    break;
                case 3:
                    randomPrefab = elevatorExitRoom;
                    randomRotations = UnityEngine.Random.Range(0, 4);
                    randomPrefab.transform.rotation = Quaternion.Euler(0, randomRotations * 90, 0);
                    prefabSize = GetPrefabSize(randomPrefab);

                    if (CanPlacePrefab(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, randomPrefab.transform.rotation))
                    {
                        Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.MapObject.transform.position.x, 0, randomCell.y * wallSpacing + room.MapObject.transform.position.z);

                        // Applique la rotation avant de placer l'objet
                        GameObject instantiatedPrefab = Instantiate(randomPrefab, objectPosition, Quaternion.identity, room.MapObjectsParent.transform);
                        instantiatedPrefab.transform.rotation = randomPrefab.transform.rotation;

                        RemoveOccupiedCells(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, instantiatedPrefab.transform.rotation);
                        placementAttempts = 0;
                    }
                    else
                    {
                        i--;
                        placementAttempts++;
                        if (placementAttempts >= maxPlacementAttempts)
                        {
                            Debug.Log("PAS LA 3 !");

                            Debug.Log("Arrêt de la génération en raison du nombre maximum d'essais atteint.");
                            break;
                        }
                    }
                    Debug.Log("PAS LA !!!");

                    break;
                default:
                    Debug.LogError("Soucis dans le placement des salles necessaires");
                    break;
            }

        }

        for (int i = 3; i < numberOfRoom; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, room.MapObjectPlacementCells.Count);
            Cell randomCell = room.MapObjectPlacementCells[randomIndex];


            // Récupère la taille du prefab et les cellules occupées par le prefab
            GameObject randomPrefab = RoomPrefabs[UnityEngine.Random.Range(0, RoomPrefabs.Count)];
            int randomRotations = UnityEngine.Random.Range(0, 4);
            randomPrefab.transform.rotation = Quaternion.Euler(0, randomRotations * 90, 0);

            Vector3 prefabSize = GetPrefabSize(randomPrefab);

            if (CanPlacePrefab(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, randomPrefab.transform.rotation))
            {
                Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.MapObject.transform.position.x, 0, randomCell.y * wallSpacing + room.MapObject.transform.position.z);

                // Applique la rotation avant de placer l'objet
                GameObject instantiatedPrefab = Instantiate(randomPrefab, objectPosition, Quaternion.identity, room.MapObjectsParent.transform);
                instantiatedPrefab.transform.rotation = randomPrefab.transform.rotation;

                RemoveOccupiedCells(randomCell, prefabSize, room.MapGrid, room.MapObjectPlacementCells, instantiatedPrefab.transform.rotation);
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

    bool CanPlacePrefab(Cell startCell, Vector3 prefabSize, Cell[,] grid, List<Cell> availableCells, Quaternion rotation)
    {
        // Ajoutez la logique pour vérifier si le placement du prefab est possible dans la cellule de départ
        for (int x = startCell.x; x < startCell.x + prefabSize.x / wallSpacing; x++)
        {
            for (int y = startCell.y; y < startCell.y + prefabSize.z / wallSpacing; y++)
            {
                Vector3 rotatedPosition = rotation * new Vector3(x - startCell.x, 0, y - startCell.y);
                int rotatedX = Mathf.RoundToInt(rotatedPosition.x) + startCell.x;
                int rotatedY = Mathf.RoundToInt(rotatedPosition.z) + startCell.y;

                if (rotatedX < 0 || rotatedX >= grid.GetLength(0) || rotatedY < 0 || rotatedY >= grid.GetLength(1) || grid[rotatedX, rotatedY].Occupied)
                {
                    Debug.Log("En Dehors de la grille ou déjà occupé");
                    return false;
                }
            }
        }
        return true;
    }


    void RemoveOccupiedCells(Cell startCell, Vector3 prefabSize, Cell[,] grid, List<Cell> availableCells, Quaternion rotation)
    {
        // Retire les cellules occupées par le prefab de la liste des cellules disponibles
        for (int x = startCell.x; x < startCell.x + prefabSize.x / wallSpacing; x++)
        {
            for (int y = startCell.y; y < startCell.y + prefabSize.z / wallSpacing; y++)
            {
                Vector3 rotatedPosition = rotation * new Vector3(x - startCell.x, 0, y - startCell.y);
                int rotatedX = Mathf.RoundToInt(rotatedPosition.x) + startCell.x;
                int rotatedY = Mathf.RoundToInt(rotatedPosition.z) + startCell.y;

                if (rotatedX >= 0 && rotatedX < grid.GetLength(0) && rotatedY >= 0 && rotatedY < grid.GetLength(1) && grid[rotatedX, rotatedY] != null)
                {
                    availableCells.Remove(grid[rotatedX, rotatedY]);
                    grid[rotatedX, rotatedY].Occupied = true;
                    Debug.Log(grid[rotatedX, rotatedY] + "C'est occupé");
                }
            }
        }
    }
    #endregion



}

