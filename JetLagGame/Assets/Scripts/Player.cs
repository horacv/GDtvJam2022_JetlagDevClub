using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5;
    [SerializeField] public float jumpForce;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float characterRadius;

    [Header("Grab")]
    [SerializeField] Transform playerArm;
    [SerializeField] GameObject eDisplay;
    [SerializeField] GameObject eDisplayChild;

    [Header("Other Variables")]
    [SerializeField] Light pLight;//light emanating from character
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    // Runtime Variables
    private float inputX;
    private float inputY;
    private bool jump;

    public float lightAngle;
    private float lightHeight;
    private bool isGrounded;
    private bool pulling;
    [SerializeField] private GameObject grabbedObject;
    private int grabDirection;

    [SerializeField] private int deathNum;
    public static bool canMove;
    private bool firstHit;

    // Components
    Rigidbody rb;
    PlayerRespawn playerRespawn;

    // Other Objects
    GameObject managerObject;
    AudioManager audioManager;
    
    private FMOD.Studio.EventInstance movingBlockInstance;
    private bool isMovingBlockSound = false;
    void Start()
    {
        // Get Components
        rb = GetComponent<Rigidbody>();
        playerRespawn = GetComponent<PlayerRespawn>();

        // Get Objects
        managerObject = GameObject.FindWithTag("Manager");
        audioManager = FindObjectOfType<AudioManager>();

        Physics.gravity = new Vector3(0, -30.0F, 0);
        pulling = false;
        lightAngle = 50;
        lightHeight = 8.5f;

        deathNum = 0;
        canMove = true;
        firstHit = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player hit a spike
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spike") && firstHit)
        {
            firstHit = false;
            KillPlayer();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal") && Manager.score>=Manager.winScore)
        {
            rb.useGravity = false;
            Manager.ascending = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Spike") && firstHit)
        {
            firstHit = false;
            KillPlayer();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Trigger"))
        {
            managerObject.GetComponent<Manager>().DialogueTriggered(other.gameObject.tag);
            other.gameObject.SetActive(false);
        }
    }

    public void KillPlayer()
    {
        if (deathNum < 21) {deathNum += 1;} 
        managerObject.GetComponent<Manager>().PlayerDeath(deathNum-1);
        PlayPlayerScream();
    }

    public void ResetPlayer() {
        // Reset the player position to spawn location
        Transform respawnTransform = playerRespawn.GetRespawnPoint();
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
        rb.velocity = Vector3.zero;
        firstHit = true;
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

        pLight.spotAngle = lightAngle;
        
        // Keep light height the same
        pLight.transform.position = new Vector3(pLight.transform.position.x, lightHeight, pLight.transform.position.z);

        // Check if the player can grab an object
        eDisplay.SetActive(false);
        Collider[] armCollisions = Physics.OverlapBox(playerArm.position, playerArm.localScale / 2f, playerArm.rotation);
        foreach (Collider collider in armCollisions)
        {
            if (collider.gameObject.CompareTag("Pushable"))
            {
                eDisplay.SetActive(true);
                eDisplayChild.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(90, 0, 0);
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
        if (canMove) {
            vel.x = inputX * speed;
            vel.z = inputY * speed; 
        }
        

        //jump
        if (jump && isGrounded && !pulling)
        {
            vel.y = jumpForce;
            PlayerJumpSound();
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

        jump = false;
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

    void PlayPlayerScream()
    {
        FMODUnity.RuntimeManager.PlayOneShot(audioManager.sfx.mainCharacter.scream, transform.position);
    }
    
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
