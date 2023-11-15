using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MonsterScript : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent monster;

    public GameObject playerObj;
    public Camera camera;
    public GameObject target;
    private float playerStress = 0;

    public AudioSource footsteps;

    private int timePlayerLookMonster = 0;

    public float radius;

    [Range(0, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obtructionMask;

    private bool hasSeenPlayer;

    public GameObject playerRef;

    public bool canSeePlayer = false;

    private Vector3 lastPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        var targetRender = target.GetComponent<Renderer>();
        footsteps.volume = monster.speed / 10;

        // FEATURE FOR FLASHLIGHT DON'T DELETE COMMENTED CODE

        /*  if (isLooking(camera, target))
         {
             targetRender.material.color = Color.red;
             timePlayerLookMonster++;
             monster.destination = player.position.normalized * -timePlayerLookMonster;
             monster.speed = 0.5f;
         }
         else
         {
             timePlayerLookMonster = 0;
             targetRender.material.color = Color.white;
             monster.destination = player.position;
             monster.speed = calculateSpeed(playerStress);
         } */

        if (hasSeenPlayer && canSeePlayer)
        {
            monster.destination = player.position;
            footsteps.enabled = true;
            lastPlayerPosition = player.position;
        }
        // To go on the last position of the player
        else if (hasSeenPlayer && !canSeePlayer)
        {
            monster.destination = lastPlayerPosition;
            footsteps.enabled = true;
        }
        else
        {
            footsteps.enabled = false;
            monster.destination = transform.position;
        }
    }
    private bool isLooking(Camera c, GameObject target)
    {
        Vector3 targetViewportPosition = c.WorldToViewportPoint(target.transform.position);

        if (targetViewportPosition.x >= 0 && targetViewportPosition.x <= 1 &&
            targetViewportPosition.y >= 0 && targetViewportPosition.y <= 1 &&
            targetViewportPosition.z > 0)
        {
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

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.position, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obtructionMask))
                {
                    if (!hasSeenPlayer) hasSeenPlayer = true;
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

}
