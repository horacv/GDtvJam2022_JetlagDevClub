using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = gameObject.transform.parent.gameObject;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Pushable")) {
            player.GetComponent<Player>().PullBlock(other.gameObject);
        }
    }
}
