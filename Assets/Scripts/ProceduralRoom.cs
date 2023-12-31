using UnityEngine;



public class ProceduralRoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private int numberOfRooms = 5;
    [SerializeField] private float roomSizeMin = 1f;
    [SerializeField] private float roomSizeMax = 5f;
    [SerializeField] private int numberOfTrappedRooms = 2;
    [SerializeField] private int numberOfCardRooms = 1;
    [SerializeField] private float gridSize = 13f;
    [SerializeField] public ProceduralObject roomObject;
    [SerializeField] public RoomType roomInteractable;

   

    void Start()
    {
        GenerateRooms();
    }


    void GenerateRooms()
    {
        GenerateRoomsOfType(RoomType.Standard, numberOfRooms - numberOfTrappedRooms - numberOfCardRooms);
        GenerateRoomsOfType(RoomType.Trapped, numberOfTrappedRooms);
        //GenerateRoomsOfType(RoomType.CardRoom, numberOfCardRooms);
    }

    void GenerateRoomsOfType(RoomType roomType,int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject room = CreateRoom(roomType);
            CreateDoorInWall(room);

            roomObject.GenerateObjectsOnFloor(room.transform.Find("Floor"));
        }
    }

    GameObject CreateRoom(RoomType roomType)
    {
        Vector3 roomPosition = GetRandomGridPosition();
        GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
        room.tag = "Room";
        room.transform.localScale = new Vector3(roomSizeMin, 1f, roomSizeMin);

        switch (roomType)
        {
            case RoomType.Standard:
                break;
            case RoomType.Trapped:
                break;
            
        }

        return room;
    }

    Vector3 GetRandomGridPosition()
    {
        float randomX = Mathf.Round(UnityEngine.Random.Range(-50f, 50f) / gridSize) * gridSize;
        float randomZ = Mathf.Round(UnityEngine.Random.Range(-50f, 50f) / gridSize) * gridSize;
        return new Vector3(randomX, 0f, randomZ);
    }

    void CreateDoorInWall(GameObject room)
    {
        Transform wallParent = room.transform.Find("Wall");
        if (wallParent != null)
        {
            Transform[] walls = wallParent.GetComponentsInChildren<Transform>();
            int randomWallIndex = Random.Range(1, walls.Length); // Pour pas destroy le prefab de mur mettre index � 1
            Transform selectedWall = walls[randomWallIndex];
            Quaternion parentRotation = selectedWall.rotation;
            selectedWall.rotation = parentRotation;
            GameObject door = Instantiate(doorPrefab, selectedWall.position , Quaternion.identity, wallParent);
            door.transform.rotation = parentRotation;
            Destroy(selectedWall.gameObject);
        }
    }

   
}
