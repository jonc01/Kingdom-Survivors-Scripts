using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    Transform commanderTransform;
    Commander_Combat commanderCombat;
    [SerializeField] public float pickUpRangeMult = 1f;
    [SerializeField] Vector3 base_pickupScale;

    void Start()
    {
        pickUpRangeMult = 1;
        base_pickupScale = transform.localScale;
        UpdatePickupRange();
        commanderTransform = GameManager.Instance.PlayerTransform;
        commanderCombat = commanderTransform.GetComponent<Commander_Combat>();
    }

    public void UpgradePickupRange(float percentIncrease)
    {
        pickUpRangeMult += percentIncrease;
        UpdatePickupRange();
    }

    private void UpdatePickupRange()
    {
        //Reset scale to default
        transform.localScale = base_pickupScale;

        //Set scale to new pickup range scale
        transform.localScale = new Vector3(
            transform.localScale.x * pickUpRangeMult, 
            transform.localScale.y * pickUpRangeMult, 
            transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Pickups")) return;

        var orb = collision.GetComponent<XP_Drop>();
        if(orb == null) return;
        orb.isMoving = true;
        orb.commanderTransform = commanderTransform;
        orb.commanderCombat = commanderCombat;
    }
}
