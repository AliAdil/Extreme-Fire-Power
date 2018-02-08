using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour {

    // to store the target which camera will be following 
    public Controller2D target;
    public float verticalOffset;
    public float lookAheadDstX;
    public float LookSmoothTimeX;
    public float verticalSmoothTime;
    
    // size which camera will be following 
    public Vector2 focusAreaSize;
    
    FocusArea focusArea;

    float currentLookAhedX;
    float targetLookAheadX;
    float lookAheedDirX;
    float smoothmoveAmountX;
    float smoothmoveAmountY;

    bool lookAheedStopped;

    void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
        Debug.Log("Mathf value -10 "+Mathf.Sign(-10));
        Debug.Log("Mathf value -10 "+Mathf.Sign(10));
    }

    // usually used for camera follow because it means that all the player movement has already been finished for the frame in its
    // own update method
    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);

        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        transform.position = (Vector3)focusPosition + Vector3.forward * -10;

        if (focusArea.moveAmount.x != 0)
        {
            lookAheedDirX = Mathf.Sign(focusArea.moveAmount.x);
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.moveAmount.x) && target.playerInput.x != 0)
            {
                lookAheedStopped = false;
                targetLookAheadX = lookAheedDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheedStopped)
                {
                    lookAheedStopped = true;
                    targetLookAheadX = currentLookAhedX + (lookAheedDirX * lookAheadDstX - currentLookAhedX) / 4f;
                }
            }
        }
       // targetLookAheadX = lookAheedDirX * lookAheadDstX;
        currentLookAhedX = Mathf.SmoothDamp(currentLookAhedX, targetLookAheadX, ref smoothmoveAmountX, LookSmoothTimeX);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothmoveAmountY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAhedX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 moveAmount;
        float left, right;
        float top, bottom;

        // constructor
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            moveAmount = Vector2.zero;

            // getting mid point add and then dividing them 
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            // for x axis 
            float shiftX = 0; 
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }

            left += shiftX;
            right += shiftX;

            // for y axis 
            float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }

            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            
            // value of how far focus area is moved in last frame 
            moveAmount = new Vector2(shiftX, shiftY); 
        }
    }


}
