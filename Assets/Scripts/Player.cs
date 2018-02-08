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
        float maxJumpVelocity;
        float minJumpVelocity;
        float velocityXSmoothing;

        Vector3 velocity;
        Controller2D controller;
        Vector2 directionalInput;

        bool wallSliding;
        int wallDirX;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
       // print("Gravity " + gravity + " Jump Velocity " + maxJumpVelocity);
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
                velocity.x = -wallDirX * wallJumpCLimb.x;
                velocity.y = wallJumpCLimb.y;
            }
            // for jump off the wall 
            else if (directionalInput.x == 0)
            {
                velocity.x = - -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            // wall leap jump 
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;

            }

        }
        //regular jump
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void Update()
    {
       
        
        
        wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTImeGrounded : accelerationTimeAirborne);
       

        // wall sliding  variable 
        wallSliding = false;
        //checking for the case if wallsliding is true
        //in order to be true it have to colllide with walls to the left or to the right of our character
        //a character needs to not be touching the ground and also needs to be moving downwords
        if( (controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 ){
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                //reseting velocity wallslide
                velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0; 
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

 
        
      

 
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
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
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
