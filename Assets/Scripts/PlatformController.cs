using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

    public LayerMask passengerMask;
    public Vector3 move;

    List<PassengerMovement> passengerMovement;

    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>(); 
    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = move * Time.deltaTime;

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            // check if the passenger isnt already contained in our dictonary
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                // to only get one getcomponent called  per passenger 
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }


            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //passenger.transform.GetComponent<Controller2D>().Move(passenger.velocity , passenger.standingOnPlatform);
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }
    // Any controller 2D that is affected by platform WRO standing on it or below ir being side of it of pushed horizontally or vertically
    // Anything that is going to move by platform We are going to call them passenger
    void CalculatePassengerMovement(Vector3 velocity)
    { 
        // all of the passengers that moved this frame
        HashSet<Transform> movedPasssengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                // if we found the passenger how far we want to move passenger
                if (hit)
                {
                    // if the hit passengers doesnt contain hit transform only then we will move transform
                    if (!movedPasssengers.Contains(hit.transform)) { 
                        
                        //moved passenger is added in Hash Tranform list each passanger will be moved one time per frame
                        movedPasssengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        //Add new passanger movement to passengers list 
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                       // hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
        // Horizontall moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                if (hit)
                {
                    // if the hit passengers doesnt contain hit transform only then we will move transform
                    if (!movedPasssengers.Contains(hit.transform))
                    {

                        //moved passenger is added in Hash Tranform list each passanger will be moved one time per frame
                        movedPasssengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        //pushed by side 
                        float pushY = -skinWidth;

                        //Add new passanger movement to passengers list 
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        //hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
        // Passanger on top of a horizontally or downword moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin =  raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up , rayLength, passengerMask);

                // if we found the passenger how far we want to move passenger
                if (hit)
                {
                    // if the hit passengers doesnt contain hit transform only then we will move transform
                    if (!movedPasssengers.Contains(hit.transform))
                    {

                        //moved passenger is added in Hash Tranform list each passanger will be moved one time per frame
                        movedPasssengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        //Add new passanger movement to passengers list 
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                      //  hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
    }
    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _tranform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatforms)
        {
            transform = _tranform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatforms;
            
            
        }

    }
}