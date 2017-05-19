using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Left;
    public GameObject Right;
    public Vector2 DistanceRange = new Vector2(5,25);

    float spreadLerp = 0;
    float velocity = 0;

	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();
        // decay velocity
        velocity = Mathf.Max(-10, velocity - (20f * Time.deltaTime));
        spreadLerp = Mathf.Clamp01(spreadLerp + (velocity * Time.deltaTime));

        float offset = DistanceRange.x + (spreadLerp * (DistanceRange.y - DistanceRange.x));

        // Update images
        setY(Top, offset);
        setY(Bottom, -offset);
        setX(Left, -offset);
        setX(Right, offset);
    }

    void Fire()
    {
        velocity = 5f;
    }

    void setX(GameObject obj, float val)
    {
        RectTransform trans = obj.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(val, trans.anchoredPosition.y);
    }
    void setY(GameObject obj, float val)
    {
        RectTransform trans = obj.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, val);
    }
}
