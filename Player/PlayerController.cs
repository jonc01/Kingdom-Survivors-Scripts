using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public float moveSpeed = 0.6f; //0.4f - enemies can too easily catch up to Player
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isRallying;
    [SerializeField] Commander_Combat combat;
    [SerializeField] HeroPartyManager heroPartyManager;
    [Header("- Rally -")]
    [SerializeField] public Transform rallyPointOffset;
    [SerializeField] float rallyCooldown = 5;
    [SerializeField] private float rallyCooldownTimer;
    [SerializeField] TextMeshProUGUI cooldownNumbers;
    [SerializeField] Slider rallySlider;

    [Space(10)]
    [SerializeField] private Transform heroParty;

    public Vector2 movement;
    private Vector3 defaultScale;

    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        defaultScale = transform.localScale;
        if(combat == null) combat = GetComponent<Commander_Combat>();
        isRallying = false;
        
        rallyCooldownTimer = 0;
        rallySlider.maxValue = rallyCooldown;
        rallySlider.value = 0;
        cooldownNumbers.gameObject.SetActive(false);
    }

    void Update()
    {
        if(GameManager.Instance.gamePaused) return;
        if(!combat.isAlive) return;
        
        rallyCooldownTimer -= Time.deltaTime;
        if(rallyCooldownTimer > 0)
        {
            rallySlider.value = rallyCooldownTimer;
            cooldownNumbers.text = rallySlider.value.ToString("N0");
        }
        else cooldownNumbers.gameObject.SetActive(false);

        GetInput();
        Flip();
        MoveRallyPoint();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RallyHeroes();
        }
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.gamePaused) return;
        if(!combat.isAlive) { heroPartyManager.WipeParty(); return; }
        
        Move();
        isMoving = (rb.velocity == Vector2.zero);
    }

    void RallyHeroes()
    {
        if(rallyCooldownTimer > 0) return;

        StartCoroutine(RallyCO());

        rallyCooldownTimer = rallyCooldown;
        cooldownNumbers.gameObject.SetActive(true);
        rallySlider.value = rallyCooldownTimer;

        int heroPartySize = heroPartyManager.partySize;
        if(heroPartySize == 0) return;

        for(int i=0; i<heroPartySize; i++)
        {
            heroPartyManager.MoveHero(GetRandPos());
        }
    }

    IEnumerator RallyCO()
    {
        isRallying = true;
        yield return new WaitForSeconds(.5f);
        isRallying = false;
    }

    Vector3 GetRandPos()
    {
        Vector3 randPos = transform.position;
        float xPos = Random.Range(-.2f, .2f);
        float yPos = Random.Range(-.2f, .2f);
        randPos = new Vector3(randPos.x+xPos, randPos.y+yPos, 0);
        return randPos;
    }

    private void GetInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void MoveRallyPoint()
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;

        rallyPointOffset.position = new Vector3(
            xPos + (movement.x*.1f),
            yPos + (movement.y*.1f),
            0);
    }

    void Move()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
        //movement is normalized to prevent diagonal being faster
    }

    void Flip()
    {
        if(movement.x > 0)
        {
            transform.localScale = new Vector3(defaultScale.x*1f, defaultScale.y, 1);
        }
        if(movement.x < 0)
        {
            transform.localScale = new Vector3(defaultScale.x*-1f, defaultScale.y, 1);
        }
    }
}
