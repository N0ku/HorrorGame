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
            monster.speed = 1 + playerStress / 100;
        }
    }

    private bool isLooking(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }

}
