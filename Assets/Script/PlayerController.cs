using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody RB;
    bool isGrounded, isJumping;
    float JumpForce = 0;
    float JumpTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.velocity = new Vector2(20, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump")) isJumping = true;
        else if (isGrounded) isJumping = false;

        if (isGrounded && (Input.GetButtonUp("Jump") || JumpForce >= 10))
        {
            isGrounded = false;
            if (JumpForce >= 10) JumpForce = 10; 
            RB.velocity = new Vector2(RB.velocity.x, JumpForce);
            print("Jump " + JumpForce);
        }

        if (isJumping) JumpForce += Time.deltaTime * 60;
        else JumpForce = 0;


        //print(isJumping + ", " + isGrounded + ", " + JumpForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
