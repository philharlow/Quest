using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public float Lifespan = 3;
    public float KillHeight = 10;

    public float Acceleration = 0.1f;
    public float Wander = 0.02f;
    public float MaxVelocity = 3f;

    [HideInInspector]
    public Vector3 Velocity;

    private void Start()
    {
        Destroy(gameObject, Lifespan);
    }

    void FixedUpdate ()
    {
        Velocity.y = Mathf.Min(Velocity.y + Acceleration, MaxVelocity);
        Velocity.x += Random.Range(-1f, 1f) * Wander;
        Velocity.z += Random.Range(-1f, 1f) * Wander;
        transform.Translate(Velocity * Time.deltaTime);

        if (transform.position.y >= KillHeight)
            Destroy(gameObject);
	}
}
