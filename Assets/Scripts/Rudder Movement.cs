using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Ubiq.Geometry.Query;

public class RudderMovement : MonoBehaviour
{
    XRGrabInteractable interactable;
    NetworkContext context;
    Transform parent;

    public int token;
    public bool isOwner;

    public Transform attachPoint;
    public float angleLimitation = 135f;

    private float startRotationAngle;
    private Vector3 startGrabPoint;
    private Vector3 rotationAxis;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        interactable = GetComponent<XRGrabInteractable>();
        interactable.firstSelectEntered.AddListener(onGrabbed);
        interactable.lastSelectExited.AddListener(onReleased);
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);
        //isOwner = true; // Start by both exchanging the random tokens to see who wins...

        audioSource = GetComponent<AudioSource>();
    }

    void onGrabbed(SelectEnterEventArgs ev)
    {
        Debug.Log("Rudder Grabbed");
        token++;
        isOwner = true;
        startRotationAngle = transform.eulerAngles.z;
        if (startRotationAngle > 180) startRotationAngle = -180 + startRotationAngle - 180;
        startGrabPoint = ev.interactorObject.transform.position - attachPoint.transform.position;
        rotationAxis = attachPoint.forward;

        audioSource.PlayDelayed(0.2f);
    }

    void onReleased(SelectExitEventArgs ev)
    {
        Debug.Log("Rudder Released");
        isOwner = false;
        transform.parent = parent;
    }

    public void ResetState()
    {
        this.transform.rotation = this.attachPoint.rotation;
    }
    private struct Message
    {
        public Quaternion rotation;
        public int token;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOwner)
        {
            // While grabbing, rudder will come out of the hierarchy, need to adjust the transform to stick to the boat
            // Need the global coords of attachment. And the rotation of the ship, and the 15 degrees from attachment
            transform.position = attachPoint.transform.position;
            transform.rotation = attachPoint.transform.rotation;

            // Track the last grabbing position, calculate the ratation angle, and rotate on z-axis
            IXRSelectInteractor interactor = interactable.GetOldestInteractorSelecting();
            Vector3 currentGrabPoint = interactor.transform.position - attachPoint.transform.position;
            float rotationAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(startGrabPoint, rotationAxis), 
                Vector3.ProjectOnPlane(currentGrabPoint, rotationAxis),
                rotationAxis);
            // Take last rotation state into consideration
            rotationAngle = rotationAngle + startRotationAngle;
            if (rotationAngle > angleLimitation) rotationAngle = angleLimitation;
            if (rotationAngle < -angleLimitation) rotationAngle = -angleLimitation;
            transform.Rotate(0, 0, rotationAngle);

            Message m = new Message();
            m.rotation = this.transform.rotation;
            m.token = token;
            context.SendJson(m);
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        transform.position = attachPoint.transform.position;
        transform.rotation = message.rotation;
        if (message.token > token)
        {
            isOwner = false;
            token = message.token;
        }
        //Debug.Log(gameObject.name + " Updated");
    }
}
