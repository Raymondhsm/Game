using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour 
{
    public enum statusType {NotStart, OnGoing, Finished};

    private statusType status;
    public statusType Status 
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

    private List<TaskNode> taskNodeList;
    public List<TaskNode> TaskNodeList
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

        taskNodeList = new List<TaskNode>();
    }

    void Update()
    {
        
    }

    ~Task()
    {
        //如果任务不是进行中，不用保存进度
        if(this.Status != statusType.OnGoing) return;

        //保存进度
        Dictionary<string,OnGoingTask> dogt = taskManager.getTaskSchedule();

        //如果任务进度存在，改写
        if(dogt.ContainsKey(taskName))
        {
            dogt[taskName].taskNodeIndex = currTaskNodeIndex;
            dogt[taskName].taskNodeNum = currTaskNodeNum;
            dogt[taskName].taskNodeLayer = currLayer;
        }
        else        //如果任务进度不存在，添加
        {
            OnGoingTask o = new OnGoingTask();
            o.taskName = taskName;
            o.taskNodeIndex = currTaskNodeIndex;
            o.taskNodeNum = CurrTaskNodeNum;
            o.taskNodeLayer = CurrLayer;

            dogt.Add(taskName,o);
        }
    }

    public bool StartTask(){
        //如果任务节点为零，则返回
        if(TaskNodeNum == 0 )return false;

        //设置任务状态
        status = statusType.OnGoing;

        //如果任务没有进度读取，开始寻找下一个节点
        if(currLayer == 0) NextTaskNode();

        //激活任务节点
        ActiveTaskNode();

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
            m_tn = taskNodeList[i];
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
            tn = taskNodeList[i] ;
            if(!tn.IsFinished) return false;
        }

        return true;
    }

    //根据名字寻找TN
    private TaskNode FindTaskNodeByName(string name)
    {
        TaskNode tn;
        for(int i=0; i<TaskNodeNum; i++){
            tn = taskNodeList[i] ;
            if(tn.TaskNodeName ==  name) return tn;
        }

        return null;
    }

    //返回当前被激活的任务节点
    public TaskNode[] getCurrentTaskNode()
    {
        TaskNode[] re = new TaskNode[currTaskNodeNum];
        for(int i=0; i<currTaskNodeNum; i++){
            re[i] = taskNodeList[currTaskNodeIndex + i] ;
        }
        return re;
    }

    //切换到下一层的任务
    private void NextTaskNode()
    {
        Debug.Log("next node");

        //如果当前为最后一层节点,则任务完成
        currTaskNodeIndex += currTaskNodeNum;
        if(currTaskNodeIndex >= TaskNodeNum) {
            status = statusType.Finished;
            FinishedTask();
            return;
        }

        //find the number of enable taskNode 
        currTaskNodeNum = 1;

        //寻找下一层的任务节点
        TaskNode currNode, nextNode;
        int index;
        while(true){
            index = currTaskNodeIndex + currTaskNodeNum - 1;
            if(index >= TaskNodeNum - 1) break;

            currNode = taskNodeList[index] ;
            nextNode = taskNodeList[index+1] ;

            if(currNode.layer != nextNode.layer) break;
            
            currTaskNodeNum++;
        }
        currLayer++;
       
    }

    public void ActiveTaskNode()
    { 
        //激活任务节点
        TaskNode tn;
        for(int i=currTaskNodeIndex; i<currTaskNodeIndex + currTaskNodeNum; i++){
            tn = taskNodeList[i] ;
            tn.IsEnable = true;
        }
    }

    //任务完成事件函数
     private void FinishedTask()
    {
        Debug.Log("finishTask");

        this.status = statusType.Finished;
        taskManager.RespondTask(this.TaskName);

        FinishedEvent();
    }

    virtual protected void FinishedEvent()
    {
        Debug.Log("father");
        // Destroy(gameObject);
    }

}
