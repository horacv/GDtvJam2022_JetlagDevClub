using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject managerObject;
    Rigidbody rb;
    public float speed = 5;//speed
    public Vector3 jump;//force applied at jump
    public bool isGrounded;
    private GameObject[] pbList;//list that will hold pushable blocks in scene
    Vector3 blockObjDis = new Vector3(1.5f, 0.5f, 0);//distance between pushable block and game object centers
    private bool pulling;
    public Light light;//light emanating from character
    private float lightRange;
    void Start()
    {
        managerObject = GameObject.FindWithTag("Manager");
        Physics.gravity = new Vector3(0, -30.0F, 0);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jump = new Vector3(0.0f, 15.0f, 1.5f);
        pbList = GameObject.FindGameObjectsWithTag("Pushable");
        pulling = false;
        lightRange = 10;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
    }

    void Update()
    {
        //simple movements
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.S) && !pulling)
        {
            transform.Translate(-1 * Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 1, 0);
        }
        //jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            if (transform.position.y <= 1.6f) { rb.AddForce(jump, ForceMode.Impulse); }
        }

        //controlling light just to test out, will have game logic incorporated later
        if (Input.GetKey(KeyCode.UpArrow) && lightRange<500)
        {
            lightRange = lightRange + 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) && lightRange>0)
        {
            lightRange = lightRange - 1;
        }

        light.range = lightRange;
    }

    //function called by arm when in contact with block you can pull
    public void PullBlock(GameObject block) {

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.H))
        {
            pulling = true;
            block.GetComponent<Rigidbody>().velocity = transform.forward * -1 * speed;
        }
        else {
            pulling = false;
        }
    }
}
