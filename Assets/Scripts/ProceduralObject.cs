using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs = new List<GameObject>();
    [SerializeField] private int numberOfObjects = 5;

    public void GenerateObjectsOnFloor(Transform floorParent)
    {
        Transform[] floors = floorParent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Sélectionnez aléatoirement un des enfants du parent "Floor"
            Transform selectedFloor = floors[Random.Range(0, floors.Length)];

            GameObject selectedPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
            Vector3 objectPosition = GetRandomPositionOnFloor(selectedFloor);
            Instantiate(selectedPrefab, objectPosition, Quaternion.identity, selectedFloor);
        }
    }

    private Vector3 GetRandomPositionOnFloor(Transform floor)
    {
        float randomX = Random.Range(-floor.localScale.x / 2f, floor.localScale.x / 2f);
        float randomZ = Random.Range(-floor.localScale.z / 2f, floor.localScale.z / 2f);
        return new Vector3(randomX, 0f, randomZ) + floor.position;
    }
}