using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] bool poolObject = false;
    [SerializeField] public bool piercing = false;
    [SerializeField] public float flightSpeed;
    [SerializeField] float flightDuration = 1f;
    [SerializeField] private float flightTimer;
    [Header("Audio")]
    [SerializeField] PlayAudioClips playAudioClips;

    [Header("Set at Instantiate")]
    [SerializeField] public Vector3 spawnPos;
    [SerializeField] public Vector3 targetPos;
    [SerializeField] public float projectileDamage = 1;
    private bool targetReached;
    [SerializeField] float destroyDelay = 1f;
    Vector3 moveDir;

    void Start() { Setup(); }
    void OnEnable() { Setup(); }

    private void Setup()
    {
        flightTimer = 0;
        targetReached = false;
        AimAtTarget(targetPos);
        moveDir = (targetPos - transform.position).normalized;
    }

    private void AimAtTarget(Vector3 targetPosition)
    {
        Vector3 aimDirection = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }


    void Update()
    {
        flightTimer += Time.deltaTime;

        if(targetReached) return;
        // Vector3 moveDir = (targetPos - transform.position).normalized;
        transform.position += moveDir * flightSpeed * Time.deltaTime;

        if(flightTimer >= flightDuration)
        {
            targetReached = true;
            Invoke("DestroySelf", destroyDelay);
        }
    }

    private void DestroySelf()
    {
        if(poolObject) gameObject.SetActive(false);
        else Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(targetReached) return;
        //Stop dealing damage if target has been reached
        //Piercing arrows won't deal damage while no longer flying
        var targetObj = collider.GetComponent<Base_Combat>();
        if (targetObj == null) return;
        targetObj.TakeDamage(projectileDamage);

        if(!piercing)
        {
            Invoke("DestroySelf", .3f);
            targetReached = true;
            if(playAudioClips != null) playAudioClips.PlayRandomClip();
        }
    }
}
