using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

    public LayerMask passengerMask;
    //public Vector3 move;

    // bunch of postions that is relative to waypoint
    public Vector3[] localWaypoints;
    public Vector3[] globalWaypoints;

    // for the speed of platform 
    public float speed;

    public bool cyclic;

    public float waitTime;

    // index of gobal way point we are moving from
    int fromWaypointIndex;
    // percentage between 0 and 1
    float percentBetweenWaypoints;
    float nextMoveTime;
    List<PassengerMovement> passengerMovement;

    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>(); 
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        // global waypoints 
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();

      //  Vector3 velocity = move * Time.deltaTime;
        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }
    // using on the place to move method 
    Vector3 CalculatePlatformMovement()
    {


        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        //it makes it reset to zero each time it reaches global waypoint throught length
        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            // making cyclic movement 
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            // current time and the amount of time we must wait 
            nextMoveTime = Time.time + waitTime;
        }
        return newPos - transform.position;
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

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                // need to convert local postion into global postion in order to draw Gizmo
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] :  localWaypoints[i] + transform.position;
                // draw gizmo
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }

    }
}