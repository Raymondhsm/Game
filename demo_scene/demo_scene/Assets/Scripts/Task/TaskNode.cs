
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNode : MonoBehaviour
{
    private bool isEnable;
    public bool IsEnable
    {
        set { isEnable = value; }
        get { return isEnable; }
    }

    private bool isFinished;
    public bool IsFinished
    {
        set { isFinished = value; }
        get { return isFinished; }
    }

    public string taskNodeName;
    public string TaskNodeName
    {
        set { taskNodeName = value; }
        get { return taskNodeName; }
    }

    private Task parent;
    public Task Parent
    {
        set { parent = value; }
        get { return parent; }
    }

    public int layer;

    void Awake()
    {
        Debug.Log("tn Start");
        parent = gameObject.GetComponentInParent<Task>();
        isEnable = false;
        isFinished = false;
        parent.addTaskNode(this);
    }

    void OnTriggerEnter(Collider collider){
        Debug.Log(isEnable);
        if(!isEnable) return;
        if(collider.CompareTag("Player")){
            NotifyTask();
        }
    }

    private void NotifyTask(){
        Debug.Log("notifyTask");

        bool result = parent.RespondTaskNode(layer, taskNodeName);

        if(result){
            isEnable = false;

            //其他完成该任务节点的操作
            //。。。
        }
    }

}
