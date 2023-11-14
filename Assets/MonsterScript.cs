using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject Monster;

    private float speed = 0.5f; // increase based on playerStress ?
    private float playerStress = 0;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Monster.transform.position = Vector3.MoveTowards(Monster.transform.position, Player.transform.position, speed * Time.deltaTime);   
    }

   
}
