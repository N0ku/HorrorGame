using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent monster;

    public Camera camera;
    public GameObject target;
    private float playerStress = 0;


    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        var targetRender = target.GetComponent<Renderer>();

        if (isLooking(camera, target))
        {
            targetRender.material.color = Color.red;
            monster.destination = player.position.normalized * -1;
            monster.speed = 0.5f;
        }
        else
        {
            targetRender.material.color = Color.white;
            monster.destination = player.position;
            monster.speed = calculateSpeed(playerStress);
        }
    }
    private bool isLooking(Camera c, GameObject target)
    {
        Vector3 targetViewportPosition = c.WorldToViewportPoint(target.transform.position);

        // return true if the camera is looking at the target and there is no obstacle in between

        // verify if the target is inside the camera viewport
        if (targetViewportPosition.x >= 0 && targetViewportPosition.x <= 1 &&
            targetViewportPosition.y >= 0 && targetViewportPosition.y <= 1 &&
            targetViewportPosition.z > 0)
        {
            // verify if there is an obstacle in between
            RaycastHit hit;
            if (Physics.Linecast(c.transform.position, target.transform.position, out hit))
            {
                if (hit.transform.gameObject == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private float calculateSpeed(float playerStress)
    {
        return 0.5f + (playerStress / 100);
    }

}
