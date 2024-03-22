using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    NetworkContext context;
    public int token;
    public bool isOwner;

    public float MaxMoveSpeed = 3f;
    public float MaxTurnSpeed = 5f;
    public float rudderAngleLimitation = 135f;
    public float forwardMu = 0.8f;
    public float backwardMu = 0.9f;
    public float wind2forceRate = 10f;

    public Transform rudder;
    public Transform sail;

    private float currentSpeed = 0f;
    private bool isCollision = false;
    private SailCanvasScale sailCanvas;
    private Wind wind;

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);
        isOwner = true;

        sailCanvas = FindAnyObjectByType<SailCanvasScale>();
        wind = FindAnyObjectByType<Wind>();
    }
    private struct Message
    {
        public bool isCollision;
        public Vector3 position;
        public Quaternion rotation;
        public int token;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOwner)
        {
            if (!isCollision)
            {
                Vector3 shipForward = transform.forward;
                Vector3 sailForward = sail.forward;
                Vector3 windDirection = wind.transform.forward;
                float sailScale = sailCanvas.scale;
                float windSpeed = wind.speed;
                float angleWindSail = Vector3.Angle(windDirection, sailForward);
                float angleSailShip = Vector3.Angle(sailForward, shipForward);
                float windContribution = Mathf.Cos(angleWindSail * Mathf.Deg2Rad) *
                    Mathf.Cos(angleSailShip * Mathf.Deg2Rad);
                bool direction = windContribution > 0 ? true : false;
                float windForce = windSpeed * wind2forceRate * windContribution * (sailScale - 0.2f) / 0.6f;
                float resistance = currentSpeed > 0 ?
                    -forwardMu * currentSpeed * currentSpeed :
                    backwardMu * currentSpeed * currentSpeed;
                currentSpeed += windForce * Time.deltaTime;
                currentSpeed += resistance * Time.deltaTime;
                //Debug.Log("wind" + windForce);
                //Debug.Log("resistance" + resistance);
                //Debug.Log("speed" + currentSpeed);
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

                // Calculate the turning angle of the rudder (-180~180)
                // Adjust the actual turning speed depending on the angle and the moving speed
                float rudderAngle = rudder.eulerAngles.z;
                if (rudderAngle >= 360 - rudderAngleLimitation) rudderAngle = -(360 - rudderAngle);
                float turnAmount = rudderAngle / rudderAngleLimitation * (Mathf.Abs(currentSpeed) / MaxMoveSpeed);
                transform.Rotate(0, -turnAmount * MaxTurnSpeed * Time.deltaTime, 0);
            }
            // Network
            token++;
            Message m = new Message();
            m.isCollision = this.isCollision;
            m.position = this.transform.position;
            m.rotation = this.transform.rotation;
            m.token = token;
            context.SendJson(m);
        }
    }

    public void Collision()
    {
        this.isCollision = true;
        Debug.Log("Boat Collision Event !!!");
    }

    public void UndoCollision()
    {
        this.isCollision = false;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        if (message.token > token)
        {
            //isOwner = false;
            token = message.token;
            this.isCollision = message.isCollision;
            transform.position = message.position;
            transform.rotation = message.rotation;
        }
        //Debug.Log(gameObject.name + " Updated");
    }
}
