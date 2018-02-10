using UnityEngine;
using System.Collections;


public class Controller2D : RaycastController
{

    public float maxSlopeAngle = 80f;
   
    // Public Refrence 
    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    //overrides over start method in raycastcontroller
   public override void Start()
    {
       // calling start method of raycastcontroller and then we will continoue over this start method
        base.Start();
        collisions.faceDir = 1;

    }
    // move overload method 
   public void Move(Vector2 moveAmount, bool standingOnPlatform)
   {
       Move (moveAmount, Vector2.zero, standingOnPlatform);
   }



    // this method is called when platform is moving and taking input to so we overload it in upper method 
    public void Move(Vector2 moveAmount, Vector2 input , bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        // player input for pass thorugh platform
        playerInput = input;

     
        // descending slope
        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        // face direction
        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        // ascending slope
       // comented this becouse of jump sliding function 
        // if (moveAmount.x != 0)
        //{
            HorizontalCollisions(ref moveAmount);
        //}
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        // math abs is using becouse moveAmount can be in negative 
        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX /** rayLength*/, Color.red);

            if (hit)
            {
                //
                //         <-----|-----
                //         player in platform movement

                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    //print(slopeAngle);
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }
                // check if not climbing the slope then we want to check the rest of rays collisions
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    //sloped detection and remove player virabtion when collided to slops form  sides..
                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY /** rayLength*/, Color.red);

            if (hit)
            {
                // to pass through obsticals which contain through tag 
                if (hit.collider.tag == "Through")
                {
                    // one mean we are moving up 
                    if (directionY == 1 || hit.distance == 0 )
                    {
                        continue;
                    }
                    if (collisions.fallingThorughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThorughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;
                    }
                }

                // constraint moveAmount to dont move thing through it 
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
                //sloped detection and remove player virabtion when collided to slops form  top etc.

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        // to slope joint togather and making V shape to overcome that problem
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }

        }

    }
    // checking climbe slope
    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if(moveAmount.y <= climbmoveAmountY)
        {
          //  print("Jumping on slope");
        moveAmount.y = climbmoveAmountY;
        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
        collisions.below = true;
        collisions.climbingSlope = true;
        collisions.slopeAngle = slopeAngle;
        }

    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = 
           Physics2D.Raycast(raycastOrigins.bottomLeft,Vector2.down, Mathf.Abs(moveAmount.y)+ skinWidth, collisionMask);
        
        RaycastHit2D maxSlopeHitRight =
          Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
        SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);

        if (!collisions.slidingDownMaxSlope)
        {

            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomRight;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
            if (hit)
            {   //hit.normal is direction that is perpendicular to slope
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;

                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope( RaycastHit2D hit , ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            // if the slop angle exeeds the max slop angle 
            if (slopeAngle > maxSlopeAngle)
            { // some trignometry 
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
            }
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThorughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 moveAmountOld;
        // 1 would mean character facing right -1 mean it is facing left  
        public int faceDir;
        public bool fallingThorughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }


}
