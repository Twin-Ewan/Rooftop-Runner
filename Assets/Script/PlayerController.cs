using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody RB;
    bool isGrounded = true;

    [Header("Player Settings")]
    [Range(0f, 15f)]
    [SerializeField] float jumpHeight;

    [Tooltip("The amount gravity effects the player as a percentage")]
    [Range(0f, 100f)]
    [SerializeField] float jumpGravity;
    bool applyJumpGravity;

    [Range(0f, 15f)]
    [SerializeField] float intialSpeed;

    [Tooltip("Distance needed to reach next speedup")]
    [SerializeField] int disThreshold;
    [Range(1f, 100f)]

    [Tooltip("Speedup amount as percentage")]
    [SerializeField] float speedupAmount;
    bool spedup = false;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.velocity = new Vector2(intialSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // The jump part of the jump
        if (Input.GetButton("Jump") && isGrounded)
        {
            isGrounded = false;
            RB.velocity = new Vector2(RB.velocity.x, jumpHeight);
        }

        // When player is mid jump and is holding the button make them fall slower so when they let go they fall fast
        if (Input.GetButton("Jump") && !isGrounded) applyJumpGravity = true;
        else if (!Input.GetButton("Jump") && !isGrounded) applyJumpGravity = false;

        // Increased speed by speedupAmount every mutliple of disTreshold (increase by 1 every 100 metres, etc.)
        // and when it does the first time it speeds it up, it'll toggle speedup to true so it only happens once
        if ((int)this.transform.position.x % disThreshold == 0)
        {
            if (spedup) return;
            else spedup = true;

            // For some reason the force applied isn't the force applied: it's 50x smaller? i.e 1 = 0.02
            // but as we speedupAmount is 1/100th of it's actual value we can just half it to get the number we need
            RB.AddForce(new Vector2(speedupAmount / 2, 0));
        }
        else if ((int)this.transform.position.x % disThreshold == 1) spedup = false;

        // GameOvers the player whenever they move too slow or fall
        if (RB.velocity.x < 5 || transform.position.y < -20)
        {
            GameObject.Find("GameManger").GetComponent<GameManger>().GameOver();
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        // Applies jumpGravity to conteract gravity as a percentage (25 = 25% of gravity)
        if (applyJumpGravity) RB.AddForce(Physics.gravity * (jumpGravity / 100) - Physics.gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
