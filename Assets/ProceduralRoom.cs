using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private int numberOfRooms = 5;
    [SerializeField] private float roomSizeMin = 1f;
    [SerializeField] private float roomSizeMax = 5f;

    [SerializeField] private float gridSize = 13f;

    [SerializeField] public ProceduralObject roomObject;

    void Start()
    {
        GenerateRooms();
    }


    void GenerateRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            GameObject room = CreateRoom();
            CreateDoorInWall(room);
            roomObject.GenerateObjectsOnFloor(room.transform.Find("Floor"));
        }
    }

    GameObject CreateRoom()
    {

        Vector3 roomPosition = GetRandomGridPosition();
        GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
        room.tag = "Room";
        room.transform.localScale = new Vector3(roomSizeMin, 1f, roomSizeMin);          

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
            int randomWallIndex = Random.Range(1, walls.Length); // Pour pas destroy le prefab de mur mettre index à 1
            Transform selectedWall = walls[randomWallIndex];
            Quaternion parentRotation = selectedWall.rotation;
            selectedWall.rotation = parentRotation;
            GameObject door = Instantiate(doorPrefab, selectedWall.position , Quaternion.identity, wallParent);
            door.transform.rotation = parentRotation;
            Destroy(selectedWall.gameObject);
        }
    }
}
