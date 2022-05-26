using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5;
    [SerializeField] float jumpForce;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float characterRadius;

    [Header("Grab")]
    [SerializeField] Transform playerArm;

    [Header("Other Variables")]
    Vector3 blockObjDis = new Vector3(1.5f, 0.5f, 0);//distance between pushable block and game object centers
    [SerializeField] Light pLight;//light emanating from character
    public Transform respawnTransform;
    [SerializeField] float deathY;

    // Runtime Variables
    private float inputX;
    private float inputY;
    private bool jump;

    private float lightAngle;
    private float lightHeight;
    private bool isGrounded;
    private bool pulling;
    [SerializeField] private GameObject grabbedObject;
    private int grabDirection;

    private GameObject[] pbList;//list that will hold pushable blocks in scene

    // Components
    Rigidbody rb;

    // Other Objects
    GameObject managerObject;
    AudioManager audioManager;
    
    private FMOD.Studio.EventInstance movingBlockInstance;
    private bool isMovingBlockSound = false;
    void Start()
    {
        // Get Components
        rb = GetComponent<Rigidbody>();

        // Get Objects
        managerObject = GameObject.FindWithTag("Manager");
        audioManager = FindObjectOfType<AudioManager>();

        Physics.gravity = new Vector3(0, -30.0F, 0);
        pbList = GameObject.FindGameObjectsWithTag("Pushable");
        pulling = false;
        lightAngle = 30;
        lightHeight = 8.5f;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player hit a spike
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spike"))
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        // Reset the player position to spawn location
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
        rb.velocity = Vector3.zero;
    }

    void Update()
    {
        // Ground check
        Ray ray = new Ray(groundCheckTransform.position, Vector3.down);
        Debug.DrawRay(groundCheckTransform.position, -transform.up * groundCheckDistance, Color.yellow);
        Collider[] collisions = Physics.OverlapCapsule(groundCheckTransform.position, groundCheckTransform.position - new Vector3(0, groundCheckDistance, 0), characterRadius, groundMask);
        isGrounded = collisions.Length > 0;

        // Get inputs
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
            jump = true;

        // Controlling light just to test out, will have game logic incorporated later
        if (Input.GetKey(KeyCode.Keypad0) && lightAngle < 90)
        {
            lightAngle = lightAngle + 1;
        }
        if (Input.GetKey(KeyCode.Keypad1) && lightAngle > 0)
        {
            lightAngle = lightAngle - 1;
        }
        pLight.spotAngle = lightAngle;
        
        // Keep light height the same
        pLight.transform.position = new Vector3(pLight.transform.position.x, lightHeight, pLight.transform.position.z);

        // Kill the player if they fall too far down
        if (transform.position.y <= deathY)
            KillPlayer();

        // Check if the player can grab an object
        Collider[] armCollisions = Physics.OverlapBox(playerArm.position, playerArm.localScale / 2f, playerArm.rotation);
        foreach (Collider collider in armCollisions)
        {
            if (collider.gameObject.CompareTag("Pushable"))
            {
                PullBlock(collider.gameObject);
                break;
            }
        }

        // Check if player let go of object
        if (Input.GetKeyUp(KeyCode.E) && pulling)
        {
            pulling = false;
            grabbedObject.transform.parent = null;
            grabbedObject = null;
            StopMovingBlockSound();
        }

        MovingBlockSound();
        
        // Rotate Player
        if (!pulling)
        {
            if (inputX > 0)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                grabDirection = 0;
            }
            else if (inputX < 0)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                grabDirection = 2;
            }
            else if (inputY > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                grabDirection = 1;
            }
            else if (inputY < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                grabDirection = 3;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.velocity;

        //simple movements
        vel.x = inputX * speed;
        vel.z = inputY * speed;

        //jump
        if (jump && isGrounded && !pulling)
        {
            vel.y = jumpForce;
            PlayerJumpSound();
            jump = false;
        }

        if (pulling)
        {
            if (grabDirection == 0 || grabDirection == 2)
                vel.z = 0;
            else if (grabDirection == 1 || grabDirection == 3)
                vel.x = 0;

            Rigidbody grabbedRb = grabbedObject.GetComponent<Rigidbody>();
            grabbedRb.velocity = new Vector3(vel.x, 0, vel.z);

        }

        rb.velocity = vel;
    }

    //function called by arm when in contact with block you can pull
    public void PullBlock(GameObject block)
    {
        if (Input.GetKeyDown(KeyCode.E) && !pulling)
        {
            pulling = true;
            grabbedObject = block;
            block.transform.parent = transform;
        }
    }

    
    #region AUDIO METHODS

    void MovingBlockSound()
    {
        if (pulling && !isMovingBlockSound && rb.velocity.normalized.magnitude == 1)
        {
            PlayMovingBlockSound();
        }
        
        if (isMovingBlockSound && rb.velocity.normalized.magnitude != 1)
        {
            StopMovingBlockSound();
        }
        
        //Debug.Log(rb.velocity.normalized.magnitude);
    }
    void PlayerJumpSound() => FMODUnity.RuntimeManager.PlayOneShot(audioManager.sfx.mainCharacter.jump, transform.position);
    
    void PlayMovingBlockSound()
    {
        movingBlockInstance = FMODUnity.RuntimeManager.CreateInstance(audioManager.sfx.interactables.movingBlocks);
        movingBlockInstance.setParameterByNameWithLabel(audioManager.parameters.boxParameter, audioManager.parameters.boxLabels[0]);
        movingBlockInstance.start();
        movingBlockInstance.release(); 
        isMovingBlockSound = true;
    }

    void StopMovingBlockSound()
    {
        movingBlockInstance.setParameterByNameWithLabel(audioManager.parameters.boxParameter, audioManager.parameters.boxLabels[1]);
        isMovingBlockSound = false;
    }
    #endregion
}
