using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour {

    // to store the target which camera will be following 
    public Controller2D target;
    
    // size which camera will be following 
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
    }

    // usually used for camera follow because it means that all the player movement has already been finished for the frame in its
    // own update method
    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        // constructor
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            velocity = Vector2.zero;

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
            velocity = new Vector2(shiftX, shiftY); 
        }
    }


}
