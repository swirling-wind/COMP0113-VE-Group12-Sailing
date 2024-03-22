using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;

public class SailCanvasTransform : MonoBehaviour
{
    NetworkContext context;
    public int token;
    public bool isOwner;

    public Transform winchDirection;

    // Notice: Direction winch rotates around the y-axis
    void Start()
    {
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);
        isOwner = true;
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
            float directionWinchAngle = winchDirection.eulerAngles.z;
            
            this.transform.localRotation = Quaternion.Euler(0, directionWinchAngle, 0);


            //Debug.Log(transform.eulerAngles);
            token++;
            Message m = new Message();
            m.rotation = this.transform.rotation;
            m.token = token;
            context.SendJson(m);
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        if (message.token > token)
        {
            //isOwner = false;
            token = message.token;
            transform.rotation = message.rotation;
        }
        //Debug.Log(gameObject.name + " Updated");
    }
}
