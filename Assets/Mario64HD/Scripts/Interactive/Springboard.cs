using UnityEngine;
using System.Collections;

public class Springboard : TriggerableObject {

    public Transform AnimatedMesh;

    public float velocity = 15.0f;
    public float lift = 50.0f;

    private float lastSpringTime;

    public override bool StandingOn(Vector3 position)
    {
        if (SuperMath.Timer(lastSpringTime, 1.0f))
        {
            AnimatedMesh.GetComponent<Animation>().Play();

            GameObject.FindGameObjectWithTag("Player").GetComponent<MarioMachine>().MegaSpring(transform.forward, velocity, lift);

            GetComponent<AudioSource>().Play();

            lastSpringTime = Time.time;
        }

        return false;
    }
}
