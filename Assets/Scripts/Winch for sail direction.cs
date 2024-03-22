using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Winchforsaildirection : MonoBehaviour
{
    XRGrabInteractable interactable;
    NetworkContext context;
    Transform parent;

    public int token;
    public bool isOwner;

    public Transform attachPoint;

    private float startRotationAngle;
    private Vector3 startGrabPoint;
    private Vector3 rotationAxis;

    private AudioSource audioSource;

    // Notice: Direction winch rotates around the x-axis
    void Start()
    {
        parent = transform.parent;
        interactable = GetComponent<XRGrabInteractable>();
        interactable.firstSelectEntered.AddListener(onGrabbed);
        interactable.lastSelectExited.AddListener(onReleased);
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);
        //isOwner = true; // It's not certain that the player who grabs the winch is the owner in the beginning

        audioSource = GetComponent<AudioSource>();
    }

    void onGrabbed(SelectEnterEventArgs ev)
    {
        //Debug.Log("Winch(Direction) Grabbed");
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
        //Debug.Log("Winch(Direction) Released");
        isOwner = false;
        transform.parent = parent;

    }
    public void ResetState()
    {
        this.transform.rotation = this.attachPoint.rotation;
    }

    public struct Message
    {
        public Quaternion rotation;
        public int token;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOwner)
        {
            transform.position = attachPoint.transform.position;
            transform.rotation = attachPoint.transform.rotation;

            IXRSelectInteractor interactor = interactable.GetOldestInteractorSelecting();
            Vector3 currentGrabPoint = interactor.transform.position - attachPoint.transform.position;
            float rotationAngle = Vector3.SignedAngle(
                Vector3.ProjectOnPlane(startGrabPoint, rotationAxis),
                Vector3.ProjectOnPlane(currentGrabPoint, rotationAxis),
                rotationAxis);

            rotationAngle = rotationAngle + startRotationAngle;
            if (rotationAngle > 180) rotationAngle = -180 + (rotationAngle - 180);
            if (rotationAngle < -180) rotationAngle = 180 + (rotationAngle + 180);
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
