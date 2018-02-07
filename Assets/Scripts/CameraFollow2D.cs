using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour {

    // to store the target which camera will be following 
    public Controller2D target;
    
    // size which camera will be following 
    public Vector2 focusAreaSize;

    struct FocusArea
    {
        public Vector2 centre;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            // getting mid point add and then dividing them 
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void update(Bounds targetBounds)
        {
            float shiftX = 0; 
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
        }
    }


}
