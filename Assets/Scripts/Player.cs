using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
        public float jumpHeight = 4.0f;
        public float timeToJumpApex= 0.4f;
        float accelerationTimeAirborne= .2f;
        float accelerationTImeGrounded= .1f;
        private bool m_FacingRight = true;
        public float moveSpeed = 6.0f;

        public float wallSlideSpeedMax = 3;
    
        float gravity;
        float jumpVelocity;
        float velocityXSmoothing;

        Vector3 velocity;
        Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity " + gravity + " Jump Velocity " + jumpVelocity);
    }

    void Update()
    {
        // wall sliding  variable 
        bool wallSliding = false;
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

        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTImeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        
        if (input.x > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (input.x < 0 && m_FacingRight)
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
