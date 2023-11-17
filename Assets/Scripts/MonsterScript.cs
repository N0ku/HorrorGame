using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using static ProceduralGenerator;

public class MonsterScript : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent monster;

    private GameObject playerObj;
    private Camera camera;
    private float playerStress = 0;

    public AudioSource footsteps;

    public AudioSource killSound;

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

    public static bool isPlayerKilled = false;

    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
        playerObj = GameObject.Find("Player");
        player = playerObj.transform;
        camera = playerObj.GetComponentInChildren<Camera>();
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
            monster.speed = 0.3f;

        }
        else if (hasSeenPlayer && canSeePlayer)
        {
            MonsterSeenPlayerTime = 0;
            monster.destination = player.position;
            lastPlayerPosition = player.position;
            monster.speed = calculateSpeed(playerStress);

            if (Vector3.Distance(transform.position, player.position) < 1.5f)
            {
                KillPlayer();
            }
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
        else
        {
            if (monster.remainingDistance < 0.5f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
                Vector3 finalPosition = hit.position;
                monster.destination = finalPosition;
            }

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
        return 1.2f + (playerStress / 100);
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

    private void KillPlayer()
    {
        isPlayerKilled = true;
        killSound.enabled = true;
        GameObject playerEverything = GameObject.Find("Player");
        playerEverything.GetComponent<PlayerMovement>().enabled = false;

        killSound.volume = 5f;
        killSound.Play();

        if (actualEtage != EtageType.Etage1) TeleportPlayer();
        else playerEverything.GetComponent<PlayerMovement>().enabled = true;
    }

    private void TeleportPlayer()
    {
        GameObject playerEverything = GameObject.Find("Player");
        GameObject elevator = GameObject.FindWithTag("manageThomasElevator");

        if (elevator != null)
        {
            playerEverything.transform.position = elevator.transform.position;
            playerEverything.GetComponent<PlayerMovement>().enabled = true;
            elevator.transform.parent.GetComponent<Animator>().Play("OpenDoors");
            int nbOfDeaths = int.Parse(playerEverything.GetComponent<Inventory>().GetInventory()[2]);
            player.GetComponent<Inventory>().AddItem((nbOfDeaths + 1).ToString());
        }

        // When he left the elevator, he can't go back in
        if (Vector3.Distance(playerEverything.transform.position, elevator.transform.position) > 2f)
        {
            elevator.transform.parent.GetComponent<Animator>().Play("CloseDoors");
        }
    }
}
