using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailPhysics : MonoBehaviour
{
    public float speed2force = 10f;
    public float turbulence = 0.2f;

    private Cloth clothComponent;
    private Wind wind;

    // Start is called before the first frame update
    void Start()
    {
        clothComponent = GetComponent<Cloth>();
        wind = FindObjectOfType<Wind>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = speed2force * wind.transform.forward.normalized * wind.speed;
        clothComponent.externalAcceleration = force;
        clothComponent.randomAcceleration = turbulence * force;
    }
}
