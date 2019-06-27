using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour 
{
    private enum statusType {NotStart, OnGoing, Finished};

    private int status;
    public int Status 
    {
        set { status = value; }
        get { return status; }
    }

    public string taskName;
    public string TaskName{
        set { taskName = value; }
        get { return taskName; }
    }

    private int currTaskNodeNum;
    public int CurrTaskNodeNum
    {
        set { currTaskNodeNum = value; }
        get { return currTaskNodeNum; }
    }

    private ArrayList taskNodeList;
    public ArrayList TaskNodeList
    {
        set { taskNodeList = value; }
        get { return taskNodeList; }
    }

    private int currTaskNodeIndex;
    public int CurrTaskNodeIndex
    {
        set { currTaskNodeIndex = value; }
        get { return currTaskNodeIndex; }
    }

    private int taskNodeNum;
    public int TaskNodeNum
    {
        get { return taskNodeList.Count; }
    }

    private int currLayer;
    public int CurrLayer 
    {
        set { currLayer = value; }
        get { return currLayer; }
    }

    private TaskManager taskManager;
    public TaskManager TManager
    {
        set { taskManager = value; }
        get { return taskManager; }
    }

    void Awake()
    {
        status = (int)statusType.NotStart;
        currTaskNodeNum = 0;
        currTaskNodeIndex = 0;
        currLayer = 0;

        taskNodeList = new ArrayList();
    }

    void Update()
    {
        
    }

    public bool StartTask(){
        Debug.Log(TaskNodeNum);
        if(TaskNodeNum == 0 )return false;

        status = (int)statusType.OnGoing;

        NextTaskNode();

        return true;
    }

    public bool addTaskNode(TaskNode tn)
    {
        //如果数组中没有，直接插入
        if(TaskNodeNum == 0) {
            tn.Parent = this;
            taskNodeList.Add(tn);
            return true;
        }

        //如果存在相同名字，不给add
        if(FindTaskNodeByName(tn.name) != null)return false;

        int index = -1;
        TaskNode m_tn;
        for(int i=0; i<TaskNodeNum; i++){
            m_tn = taskNodeList[i] as TaskNode;
            if(m_tn.layer > tn.layer){
                index = i;
                break;
            }
        }

        tn.Parent = this;
        if(index == -1) taskNodeList.Add(tn);
        else taskNodeList.Insert(index,tn);
        return true;
    }

    //响应TaskNode发来的触发信息
    public bool RespondTaskNode(int layer,string name){
        Debug.Log("respond");

        if(layer != currLayer) return false;
        
        FindTaskNodeByName(name).IsFinished = true;
        if(IsFinishThisLayer())NextTaskNode();
        
        return true;
    }

    //判断该层的任务是否已经完成
    private bool IsFinishThisLayer()
    {
        TaskNode tn;
        for(int i=currTaskNodeIndex; i<currTaskNodeIndex + currTaskNodeNum; i++){
            tn = taskNodeList[i] as TaskNode;
            if(!tn.IsFinished) return false;
        }

        return true;
    }

    //根据名字寻找TN
    private TaskNode FindTaskNodeByName(string name)
    {
        TaskNode tn;
        for(int i=0; i<TaskNodeNum; i++){
            tn = taskNodeList[i] as TaskNode;
            if(tn.TaskNodeName ==  name) return tn;
        }

        return null;
    }

    public TaskNode[] getCurrentTaskNode()
    {
        TaskNode[] re = new TaskNode[currTaskNodeNum];
        for(int i=0; i<currTaskNodeNum; i++){
            re[i] = taskNodeList[currTaskNodeIndex + i] as TaskNode;
        }
        return re;
    }

    //切换到下一层的任务
    private void NextTaskNode()
    {
        Debug.Log("next node");

        currTaskNodeIndex += currTaskNodeNum;
        if(currTaskNodeIndex >= TaskNodeNum) {
            status = (int)statusType.Finished;
            FinishedTask();
            return;
        }

        //find the number of enable taskNode 
        currTaskNodeNum = 1;

        TaskNode currNode, nextNode;
        int index;
        while(true){
            index = currTaskNodeIndex + currTaskNodeNum - 1;
            if(index >= TaskNodeNum - 1) break;

            currNode = taskNodeList[index] as TaskNode;
            nextNode = taskNodeList[index+1] as TaskNode;

            if(currNode.layer != nextNode.layer) break;
            
            currTaskNodeNum++;
        }
        currLayer++;

        //激活任务节点
        TaskNode tn;
        for(int i=currTaskNodeIndex; i<currTaskNodeIndex + currTaskNodeNum; i++){
            tn = taskNodeList[i] as TaskNode;
            tn.IsEnable = true;
        }
    }

    private void FinishedTask()
    {
        Debug.Log("finishTask");

        this.status = (int)statusType.Finished;
        taskManager.RespondTask(this.TaskName);

        Destroy(gameObject);
    }

}
