using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
        // for three different type of wall jumps 
        public Vector2 wallJumpCLimb;
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;

        public float maxJumpHeight = 4.0f;
        public float minJumpHeight = 1.0f;
 
        public float timeToJumpApex= 0.4f;
        float accelerationTimeAirborne= .2f;
        float accelerationTImeGrounded= .1f;
        private bool m_FacingRight = true;
        public float moveSpeed = 6.0f;

        public float wallSlideSpeedMax = 3;
        public float wallStickTime = 0.25f;
        public float timeToWallUnstick;
    
        float gravity;
        float maxJumpmoveAmount;
        float minJumpmoveAmount;
        float moveAmountXSmoothing;

        Vector2 moveAmount;
        Controller2D controller;
        Vector2 directionalInput;

        bool wallSliding;
        int wallDirX;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpmoveAmount = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpmoveAmount = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
       // print("Gravity " + gravity + " Jump moveAmount " + maxJumpmoveAmount);
    }

    void Update()
    {
        CalculatemoveAmount();
        HandleWallSliding();

        controller.Move(moveAmount * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (!controller.collisions.slidingDownMaxSlope)
            {
                //moveAmount.y = 0;
                moveAmount.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                moveAmount.y = 0;
            }
        }


        if (directionalInput.x > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (directionalInput.x < 0 && m_FacingRight)
        {
            Flip();
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        // wall sliding jump
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                //Moving character facing wall ( -wallDirX mean moving away form wall) 
                moveAmount.x = -wallDirX * wallJumpCLimb.x;
                moveAmount.y = wallJumpCLimb.y;
            }
            // for jump off the wall 
            else if (directionalInput.x == 0)
            {
                moveAmount.x = - -wallDirX * wallJumpOff.x;
                moveAmount.y = wallJumpOff.y;
            }
            // wall leap jump 
            else
            {
                moveAmount.x = -wallDirX * wallLeap.x;
                moveAmount.y = wallLeap.y;

            }

        }
        //regular jump
        if (controller.collisions.below)
        {
            moveAmount.y = maxJumpmoveAmount;
        }
    }

    public void OnJumpInputUp()
    {
        if (moveAmount.y > minJumpmoveAmount)
        {
            moveAmount.y = minJumpmoveAmount;
        }
    }



    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        // wall sliding  variable 
        wallSliding = false;
        //checking for the case if wallsliding is true
        //in order to be true it have to colllide with walls to the left or to the right of our character
        //a character needs to not be touching the ground and also needs to be moving downwords
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && moveAmount.y < 0)
        {
            wallSliding = true;
            if (moveAmount.y < -wallSlideSpeedMax)
            {
                //reseting moveAmount wallslide
                moveAmount.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                moveAmountXSmoothing = 0;
                moveAmount.x = 0;
                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }
    }

    void CalculatemoveAmount()
    {
        float targetmoveAmountX = directionalInput.x * moveSpeed;
        moveAmount.x = Mathf.SmoothDamp(moveAmount.x, targetmoveAmountX, ref moveAmountXSmoothing, (controller.collisions.below) ? accelerationTImeGrounded : accelerationTimeAirborne);
        moveAmount.y += gravity * Time.deltaTime;
    }
}
