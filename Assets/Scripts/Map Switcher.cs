using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MapSwitcher : MonoBehaviour
{
    NetworkContext context;

    public GameObject map1; 
    public GameObject map2; 
    public int map1CheckpointsGroupIndex = 0; // map1对应的检查点组索引
    public int map2CheckpointsGroupIndex = 1; // map2对应的检查点组索引

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }
    private struct Message
    {
        public bool change;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchMap()
    {
        // 切换两个Terrain的激活状态
        map1.SetActive(!map1.activeSelf);
        map2.SetActive(!map2.activeSelf);

        ReloadCheckPoint();

        Message m = new Message();
        m.change = true;
        context.SendJson(m);
    }

    public void ReloadCheckPoint()
    {
        if (map1.activeSelf)
        {
            ChkManager.instance.SwitchCheckpointsGroup(map1CheckpointsGroupIndex);
            ResetCheckpoints(ChkManager.instance.checkpointsGroups[map1CheckpointsGroupIndex]);
        }
        else
        {
            ChkManager.instance.SwitchCheckpointsGroup(map2CheckpointsGroupIndex);
            ResetCheckpoints(ChkManager.instance.checkpointsGroups[map2CheckpointsGroupIndex]);
        }
    }

    private void ResetCheckpoints(Transform checkpointsGroup)
    {
        foreach (Transform checkpoint in checkpointsGroup)
        {
            checkpoint.GetComponent<Checkpoint>().ResetCheckpoint(); 
        }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        map1.SetActive(!map1.activeSelf);
        map2.SetActive(!map2.activeSelf);

        ReloadCheckPoint();
    }
}
