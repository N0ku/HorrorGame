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
    private float playerStress = 0;

    public AudioSource footsteps;

    private int MonsterSeenPlayerTime = 0;

    public float radius;

    [Range(0, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obtructionMask;

    private bool hasSeenPlayer;

    public bool canSeePlayer = false;

    private Vector3 lastPlayerPosition;

    public GameObject monsterTarget;

    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        footsteps.enabled = true;

        if (isLooking(camera, monsterTarget) &&
         hasSeenPlayer &&
         canSeePlayer &&
         FlashlightManager.flashlightIsOn &&
          FlashlightManager.isUsable)
        {
            MonsterSeenPlayerTime++;
            monster.destination = player.position.normalized * -MonsterSeenPlayerTime;
            monster.speed = 0.2f;

        }
        else if (hasSeenPlayer && canSeePlayer)
        {
            MonsterSeenPlayerTime = 0;
            monster.destination = player.position;
            lastPlayerPosition = player.position;
            monster.speed = calculateSpeed(playerStress);
        }
        else if (hasSeenPlayer && !canSeePlayer)
        {
            monster.destination = lastPlayerPosition;
            MonsterSeenPlayerTime++;

            if (MonsterSeenPlayerTime > 500)
            {
                hasSeenPlayer = false;
                MonsterSeenPlayerTime = 0;
            }
        }
        /*  else
         {
             footsteps.enabled = false;
             monster.destination = transform.position;
         } */
        else
        {
            // create a routine that makes the monster walk around the room
        }
    }
    private bool isLooking(Camera c, GameObject target)
    {
        Vector3 targetViewportPosition = c.WorldToViewportPoint(target.transform.position);

        if (targetViewportPosition.x > 0 && targetViewportPosition.x < 1 &&
            targetViewportPosition.y > 0 && targetViewportPosition.y < 1 &&
            targetViewportPosition.z > 0)
        {
            RaycastHit hit;
            if (Physics.Linecast(c.transform.position, target.transform.position, out hit))
            {
                if (hit.transform.gameObject.tag == "Monster")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
        else
        {
            return false;
        }
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

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
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
