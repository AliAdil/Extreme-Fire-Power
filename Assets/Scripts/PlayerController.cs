using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;
    
    // initial jump force
    public float jumpForce = 6f;
    
    // jack running speed
    public float runningSpeed = 1.5f;
    
    //for storing layer of platfrom  for the detection of raycasting
    public LayerMask groundLayer;
    
    // for gaining access of animator through by adding player stripes in animator variable
    public Animator animator;

    // for getiting rigid body component 
    private Rigidbody2D rigidBody;
    //created variable for player starting postion of type vector3
    private Vector3 startingPosition;
    
    void Awake()
    {
        // PlayerController class instance for singleton approch 
        instance = this;
        rigidBody = GetComponent<Rigidbody2D>();

        // stored player starting postion is startingPosition variable  
        startingPosition = this.transform.position;
    }
	// Use this for initialization
	public void StartGame() {
        animator.SetBool("isAlive", true);
        this.transform.position = startingPosition;
	}
	
	// Update is called once per frame
    void Update()
    {
        //  this is exaclty same as if(Input.GetMouseButtonDown(0)	==	true)
       if (GameManager.instance.currentGameState == GameState.inGame)
       {
          
            if (Input.GetMouseButtonDown(0))
            {
                Jump();
            }
            animator.SetBool("isGrounded", IsGrounded());
        }
    }

    void FixedUpdate()
    {
       if (GameManager.instance.currentGameState == GameState.inGame)
       {
           
            if (rigidBody.velocity.x < runningSpeed)
            {
                //Vector is a simple C# constructor, and we are	passing	the	x and y values in brackets. Our
                //character is moving from left	to right, so we are	applying force with x equal	to the running speed and leaving velocity y unchanged.
                rigidBody.velocity = new Vector2(runningSpeed, rigidBody.velocity.y);

            }
        }
    }


    void Jump()
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    

    bool IsGrounded()
    {
        if (Physics2D.Raycast(this.transform.position, Vector2.down, 0.2f, groundLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Kill()
    {
        GameManager.instance.GameOver();
        animator.SetBool("isAlive", false);

        //check if highscore save if it is 
        if(PlayerPrefs.GetFloat("highscore",0)<this.GetDistance()){
            //save new highscore
            PlayerPrefs.SetFloat("highsore", this.GetDistance());

        }
    }

    public float GetDistance()
    {
        float traveledDistance = Vector2.Distance(new Vector2(startingPosition.x,0),new Vector2(this.transform.position.x,0));
        return traveledDistance;
    }
}
