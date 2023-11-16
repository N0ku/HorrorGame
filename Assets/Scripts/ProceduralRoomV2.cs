using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

using static RoomGenerator;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int roomSizeX;
    [SerializeField] private int roomSizeY;
    [SerializeField] private List<Room> generatedRooms = new List<Room>();
    [SerializeField] private GameObject wallTemplate;
    [SerializeField] private GameObject doorTemplate;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private bool displayGridInGame = true;
    [SerializeField] private List<GameObject> objectPrefabs = new List<GameObject>();
    [SerializeField] private int numberOfObjects = 5;
    [SerializeField] private int numberOfRooms = 5;
    private NavMeshSurface[] navMeshSurface;
    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject objectsParent;
    [SerializeField] private GameObject gridParent;

    // Pour les debilos, les prefab qu'on met sont en 2x3. Donc obliger de faire un spacing de 2 sinon ils se superposent.
    public static float wallSpacing = 2f;
    public Color northColor = Color.red;
    public Color southColor = Color.blue;
    public Color eastColor = Color.green;
    public Color westColor = Color.yellow;
    private bool doorGenerated = false;
    private List<WallReference> specificRoomWalls = new List<WallReference>();
    static public int staticRoomSizeX { get; set; }
    static public int staticRoomSizeY { get; set; }

    void Start()
    {

        for (int i = 0; i < numberOfRooms; i++)
        {
            doorGenerated = false;
            GenerateRoom(i);
        }

        navMeshSurface = FindObjectsOfType<NavMeshSurface>();

        foreach (var surface in navMeshSurface)
        {
            surface.BuildNavMesh();
        }

    }

    void GenerateRoom(int count)
    {
        staticRoomSizeX = roomSizeX;
        staticRoomSizeY = roomSizeY;
        GameObject namedRoom = new GameObject("Room" + count);
        specificRoomWalls.Clear();
        Room newRoom = new Room(namedRoom);
        int randomOffsetX = Mathf.RoundToInt(Random.Range(-50f, 50f));
        int randomOffsetY = Mathf.RoundToInt(Random.Range(-50f, 50f));
        newRoom.SetPosition(new Vector3(randomOffsetX * 2 + 1, 0, randomOffsetY * 2));


        for (int x = 0; x < roomSizeX; x++)
        {
            InstantiateWall(x + randomOffsetX, 0 + randomOffsetY, 0, newRoom.ObjectsParent.transform);
            InstantiateWall(x + randomOffsetX, roomSizeY - 1 + randomOffsetY, 0, newRoom.ObjectsParent.transform);
        }

        for (int y = 1; y < roomSizeY; y++)
        {
            InstantiateWall(0 + randomOffsetX, y + randomOffsetY, 90, newRoom.ObjectsParent.transform);
            InstantiateWall(roomSizeX + randomOffsetX, y + randomOffsetY, 90, newRoom.ObjectsParent.transform);
        }

        ReplaceRandomWallWithDoor();
        GenerateGrid(newRoom);
        AssignTagsAndColors(newRoom);
        PlaceRandomObjects(newRoom);
    }


    void InstantiateWall(int x, int y, int rotation, Transform parent)
    {
        GameObject wall = Instantiate(wallTemplate, new Vector3(x * wallSpacing, 0, y * wallSpacing), Quaternion.identity, parent);

        if (wallMaterial != null)
        {
            MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = wallMaterial;
            }
        }
        wall.transform.rotation = Quaternion.Euler(0, rotation, 0);


        WallReference wallReference = wall.AddComponent<WallReference>();
        wallReference.roomGenerator = this;
        specificRoomWalls.Add(wallReference);
    }

    void ReplaceRandomWallWithDoor()
    {
        if (!doorGenerated)
        {
            if (specificRoomWalls.Count > 0)
            {
                int randomIndex = Random.Range(0, specificRoomWalls.Count);
                WallReference randomWall = specificRoomWalls[randomIndex];

                Instantiate(doorTemplate, new Vector3(randomWall.transform.position.x, 0, randomWall.transform.position.z), randomWall.transform.rotation, wallsParent.transform);

                Destroy(randomWall.gameObject);
                doorGenerated = true;
            }
        }
    }

    void GenerateGrid(Room room)
    {
        room.Grid = new Cell[roomSizeX, roomSizeY];

        for (int x = 0; x < roomSizeX; x++)
        {
            for (int y = 0; y < roomSizeY; y++)
            {
                room.Grid[x, y] = new Cell(x, y, room.RoomObject.transform.position);
            }
        }
    }


    void AssignTagsAndColors(Room room)
    {
        foreach (var cell in room.Grid)
        {
            if (cell.y == 0)
            {
                cell.tag = "North";
                cell.color = northColor;
            }
            else if (cell.y == roomSizeY - 1)
            {
                cell.tag = "South";
                cell.color = southColor;
            }

            if (cell.x == 0)
            {
                cell.tag = "West";
                cell.color = westColor;
            }
            else if (cell.x == roomSizeX - 1)
            {
                cell.tag = "East";
                cell.color = eastColor;
            }

            if (cell.x != 0 && cell.x != roomSizeX - 1 && cell.y != 0 && cell.y != roomSizeY - 1)
            {
                room.ObjectPlacementCells.Add(cell);
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
    void PlaceRandomObjects(Room room)
    {
        for (int i = 0; i < numberOfObjects && room.ObjectPlacementCells.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, room.ObjectPlacementCells.Count);
            Cell randomCell = room.ObjectPlacementCells[randomIndex];

            Vector3 objectPosition = new Vector3(randomCell.x * wallSpacing + room.RoomObject.transform.position.x, 0, randomCell.y * wallSpacing + room.RoomObject.transform.position.z);
            Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Count)], objectPosition, Quaternion.identity, room.ObjectsParent.transform);

            room.ObjectPlacementCells.RemoveAt(randomIndex);
        }
    }

    public class WallReference : MonoBehaviour
    {
        public RoomGenerator roomGenerator;
    }

    public class Cell
    {
        public int x;
        public int y;
        public string tag;
        public Color color;
        public Vector3 position;

        public Cell(int x, int y, Vector3 roomPosition)
        {
            this.x = x;
            this.y = y;
            //Debug.Log(this.position = new Vector3(x * wallSpacing + roomPosition.x, roomPosition.y, y * wallSpacing + roomPosition.z));
            this.position = new Vector3(x * wallSpacing + roomPosition.x, roomPosition.y, y * wallSpacing + roomPosition.z);
        }
    }


    public class Room
    {

        public GameObject RoomObject { get; set; }
        public List<WallReference> RoomWalls { get; set; }
        public Cell[,] Grid { get; set; }
        public List<Cell> ObjectPlacementCells { get; set; }
        public GameObject ObjectsParent { get; set; }

        public Room(GameObject roomObject)
        {

            RoomObject = roomObject;
            RoomWalls = new List<WallReference>();
            Grid = new Cell[staticRoomSizeX, staticRoomSizeY];
            ObjectPlacementCells = new List<Cell>();
            ObjectsParent = new GameObject("ObjectsParent");
        }

        public void SetPosition(Vector3 position)
        {
            RoomObject.transform.position = position;
        }
    }
}
