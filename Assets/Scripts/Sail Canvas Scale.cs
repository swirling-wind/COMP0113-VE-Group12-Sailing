using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using UnityEngine;

public class SailCanvasScale : MonoBehaviour
{
    public float scale = 1f;

    NetworkContext context;
    public int token;
    public bool isOwner;

    public float angleLimitation = 90f;

    public Transform winchSize;

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        token = Random.Range(1, 10000);
        isOwner = true;
    }
    private struct Message
    {
        public float y_scale;
        public int token;
    }


    // Update is called once per frame
    void Update()
    {
        float sizeWinchAngle = winchSize.eulerAngles.x;
        //Debug.Log("sizeWinchAngle" + sizeWinchAngle / 2.0f * Mathf.Deg2Rad);
        // sizeWinchAngle is from 0 to 360. Map it to 0 to 1 and avoid the gap between degreee 0 and 360
        float new_y_scale = Mathf.Cos(sizeWinchAngle * Mathf.Deg2Rad);

        if (new_y_scale < 0.2f) new_y_scale = 0.4f;
        if (new_y_scale > 1.0f) new_y_scale = 1.0f;
        this.transform.localScale = new Vector3(1, new_y_scale - 0.2f, 1);
        scale = new_y_scale - 0.2f;

        //Debug.Log(transform.eulerAngles);
        token++;
        Message m = new Message();
        m.y_scale = this.transform.localScale.y;
        m.token = token;
        context.SendJson(m);
    }


    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        if (message.token > token)
        {
            //isOwner = false;
            token = message.token;
            transform.localScale = new Vector3(1, message.y_scale, 1);
        }
        //Debug.Log(gameObject.name + " Updated");
    }
}
