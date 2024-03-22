using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChkManager : MonoBehaviour
{
    [SerializeField] public GameObject ChkCounter;
    

    [SerializeField] public Transform[] checkpointsGroups;
    public int currentGroupIndex = 0;

    public static ChkManager instance;
    //public Transform checkpoints;

    public int chkCount = 0;
    public int chkNum = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // chkNum = checkpoints.childCount;
        // ChkCounter.GetComponent<Text>().text = "Checkpoints: " +
        //     Convert.ToString(chkCount) +
        //     " / " +
        //     Convert.ToString(chkNum);
        currentGroupIndex = 0;
        SwitchCheckpointsGroup(currentGroupIndex);
    }

    public void Update()
    {
        
    }

    public void IncrementCounter()
    {
        
        chkCount++;
        Debug.Log("Check point Count: " + chkCount); 
        ChkCounter.GetComponent<Text>().text = "Checkpoints: " + 
            Convert.ToString(chkCount) + 
            " / " +
            Convert.ToString(chkNum);
    
    }
    
    public void SwitchCheckpointsGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= checkpointsGroups.Length)
        {
            Debug.LogError("Checkpoints group index out of range.");
            return;
        }

        currentGroupIndex = groupIndex;
        Transform checkpoints = checkpointsGroups[groupIndex];
        chkNum = checkpoints.childCount;
        chkCount = 0; // 可以根据需要重置或保留计数

        // 更新UI显示
        ChkCounter.GetComponent<Text>().text = "Checkpoints: " + chkCount + " / " + chkNum;

        // 可选：在这里添加任何需要的逻辑来激活/禁用相应的检查点GameObject
    }
    
}