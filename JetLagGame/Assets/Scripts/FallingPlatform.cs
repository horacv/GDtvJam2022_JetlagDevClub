using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float fallSpeed;
    [SerializeField, Tooltip("The amount of time it waits before falling when a player lands on it")] float stayTime;
    [SerializeField] float shakeAmount;
    [SerializeField] bool respawn;
    [SerializeField] float respawnTime;
    [SerializeField] AudioManager audioManager;
    
    Vector3 originalPosition;

    float timeUntilFall = 0;
    float timeUntilRespawn = 0;
    bool falling = false;
    bool respawning = false;
    float timeFallen = 0;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (falling)
            return;

        // Check if the player lands on the platform
        if (collision.gameObject.GetComponent<Player>())
        {
            Debug.Log("Colliding with player");
            timeUntilFall = stayTime;
            falling = true;
            PlayFallingPlatformSound();
        }
    }

    void Update()
    {
        if (falling)
        {
            // Check if it is falling or shaking
            if (timeUntilFall <= 0)
            {
                // If it is falling, make the platform fall
                rb.isKinematic = false;

                timeUntilRespawn = respawnTime;
                falling = false;
                respawning = true;
            }
            else
            {
                // If it is still shaking, subtract from the shake time
                timeUntilFall -= Time.deltaTime;
                Vector3 targetPosition = originalPosition + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
                transform.position = Vector3.Lerp(originalPosition, targetPosition, shakeAmount * Time.deltaTime);
            }
        }
        else if (respawning)
        {
            // If it is respawning, count down until respawn
            timeUntilRespawn -= Time.deltaTime;

            if (timeUntilRespawn <= 0)
            {
                // Reset the platform position on respawn
                respawning = false;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                transform.position = originalPosition;
            }
        }
    }

    void PlayFallingPlatformSound()
    {
        FMOD.Studio.EventInstance eventInstance;
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(audioManager.sfx.interactables.fallingPlatforms);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, GetComponent<Transform>());
        eventInstance.start();
        eventInstance.release();
    }
}
