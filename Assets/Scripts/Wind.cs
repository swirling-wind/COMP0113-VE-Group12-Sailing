using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float speed = 1f;

    NetworkContext context;
    private int speedToken;
    private int directionToken;

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }
    private struct Message
    {
        public string tag;
        public Quaternion rotation;
        public float speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Synchronize()
    {
        Message m = new Message();
        m.rotation = transform.rotation;
        m.speed = speed;
        context.SendJson(m);
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        transform.rotation = message.rotation;
        speed = message.speed;
        //Debug.Log(gameObject.name + " Updated");
    }
}
