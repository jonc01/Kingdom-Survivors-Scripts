using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP_Drop : MonoBehaviour
{
    [SerializeField] public int xpAmount = 1;
    // [SerializeField] float deleteTimer = 20;
    public bool isMoving = false;
    public Transform commanderTransform;
    public Commander_Combat commanderCombat;

    void Start()
    {
        // Invoke("DeleteObj", deleteTimer);
    }

    void OnEnable()
    {
        isMoving = false;
    }

    void Update()
    {
        if(!isMoving || commanderTransform == null) return;
        var step = 1 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, commanderTransform.position, step);

        if(Vector3.Distance(transform.position, commanderTransform.position) < 0.1f)
        {
            HitPlayer();
        }
    }

    void HitPlayer()
    {
        if(!isMoving) return;
        isMoving = false;
        commanderCombat.AddXP(xpAmount);
        GameManager.Instance.AddScore(xpAmount);
        DeleteObj();
    }

    private void DeleteObj()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
