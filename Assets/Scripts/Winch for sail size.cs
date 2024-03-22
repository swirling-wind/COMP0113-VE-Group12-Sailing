using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Sailsizewinch : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        interactable = GetComponent<XRGrabInteractable>();
        interactable.firstSelectEntered.AddListener(onGrabbed);
        interactable.lastSelectExited.AddListener(onReleased);
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);

        audioSource = GetComponent<AudioSource>();
    }

    void onGrabbed(SelectEnterEventArgs ev)
    {
        //Debug.Log("Winch(Direction) Grabbed");
        token++;
        isOwner = true;
        startRotationAngle = transform.eulerAngles.x;
        if (startRotationAngle > 180) startRotationAngle = -180 + startRotationAngle - 180;
        startGrabPoint = ev.interactorObject.transform.position - attachPoint.transform.position;
        rotationAxis = attachPoint.right;

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
            if (rotationAngle > 90) rotationAngle = 90;
            if (rotationAngle < 0) rotationAngle = 0;
            transform.Rotate(rotationAngle, 0, 0);

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
