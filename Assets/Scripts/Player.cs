using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
        float jumpHeight = 4.0f;
        float timeToJumpApex= 0.4f;
        private bool m_FacingRight = true;
        float moveSpeed = 6.0f;
        float gravity;
    public   float jumpVelocity;
        Vector3 velocity;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        velocity.x = input.x * moveSpeed;
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
