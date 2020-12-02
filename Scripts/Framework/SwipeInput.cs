using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Touch swipes gestures detection class
public class SwipeInput : MonoBehaviour
{
    public const float MIN_SWIPE_DISTANCE = 0.05f;

    public static bool swipedRight = false;
    public static bool swipedLeft = false;
    public static bool swipedUp = false;
    public static bool swipedDown = false;

    Vector2 startPos;
    float startTime;
    bool singleSwipe = false;

    public void Update()
    {
        swipedRight = false;
        swipedLeft = false;
        swipedUp = false;
        swipedDown = false;

        if (Input.touches.Length > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = new Vector2(t.position.x/(float)Screen.width, t.position.y/(float)Screen.width);
                startTime = Time.time;
                singleSwipe = true;
            }

            if (t.phase == TouchPhase.Moved && singleSwipe)
            {
                ProcessSwipe(t);
            }

            if (t.phase == TouchPhase.Ended && singleSwipe)
            {
                ProcessSwipe(t);
            }
        }
    }

    void ProcessSwipe(Touch t)
    {
        Vector2 endPos = new Vector2(t.position.x/(float)Screen.width, t.position.y/(float)Screen.width);

        Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

        if (swipe.magnitude < MIN_SWIPE_DISTANCE)
            return;

        if (Mathf.Abs (swipe.x) > Mathf.Abs (swipe.y)) {
            if (swipe.x > 0) {
                swipedRight = true;
                singleSwipe = false;
            }
            else {
                swipedLeft = true;
                singleSwipe = false;
            }
        }
        else {
            if (swipe.y > 0) {
                swipedUp = true;
                singleSwipe = false;
            }
            else {
                swipedDown = true;
                singleSwipe = false;
            }
        }
    }
}
