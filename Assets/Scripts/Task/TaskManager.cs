using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    private List<string> taskList;
    private Dictionary<string,Task> currTaskList;         //当前进行中的任务
    private Dictionary<string,OnGoingTask> taskSchedule;         //有进度读取的任务
    private List<string> notStartTaskList;
    private List<string> finishedTaskList;
    private List<string> justFinishedTaskList;


    void Awake(){
        //为数据结构申请内存
        taskList = new List<string>();
        currTaskList = new Dictionary<string,Task>();
        taskSchedule = new Dictionary<string,OnGoingTask>();
        notStartTaskList = new List<string>();
        finishedTaskList = new List<string>();
        justFinishedTaskList = new List<string>();

        //读取任务文件的进度
        TaskSchedule ts = new TaskSchedule();
        ts.FilePath = Application.dataPath + "/File/taskSchedule.txt";
        ts.ReadTaskSchedule(notStartTaskList, taskSchedule, finishedTaskList, taskList);

    }

    public void StartTask(string name, int index)
    {
        StartCoroutine(canStart(name, index));
    }

    public void StartTask(string name, string sceneName)
    {

    }

    private IEnumerator canStart(string name, int index)
    {
        while(index != SceneManager.GetActiveScene().buildIndex)
        {
            yield return 0;
        }

        StartTask(name);
    }

    private IEnumerator canStart(string name, string sceneName)
    {
        while(sceneName != SceneManager.GetActiveScene().name)
        {
            yield return 0;
        }

        StartTask(name);
    }

    //开始任务
    private bool StartTask(string name)
    {
        //如果任务不存在或任务已开始,则直接返回
        if(!isTaskExist(name)){
            Debug.LogError("not fount task");    
            return false;
        }
        if(isTaskActive(name))return true;

        //实例化任务
        Task task = initializeTask(name);

        //添加开始的任务
        addOnGoingTask(task);

        //调用任务开始函数
        task.StartTask();

        return true;
    }
    

    //将处于进行中的任务继续
    public void ContinueTask(string name)
    {
        foreach(Task task in currTaskList.Values)
        {
            initializeTask(task.taskName);

            task.StartTask();
        }
    }

    // public void StopTask()
    // {

    // }

    //实例化任务
    private Task initializeTask(string name)
    {
        Debug.Log("shilihua");
        //从prefab中load任务
        Object prefab = Resources.Load("Prefabs/" + name) as Object;
        if (prefab == null) {
            Debug.LogError("not found task prefab");
            return null;
        }
        GameObject taskObject = Instantiate(prefab) as GameObject;

        //读取任务进度
        readHistory(taskObject.GetComponent<Task>());
        
        return taskObject.GetComponent<Task>();
    }

    //  读取任务进度
    private void readHistory(Task task)
    {
        if(!taskSchedule.ContainsKey(task.TaskName))return;
        OnGoingTask t = taskSchedule[task.TaskName] as OnGoingTask;
        task.CurrTaskNodeIndex = t.taskNodeIndex;
        task.CurrTaskNodeNum = t.taskNodeNum;
    }

    //  添加任务列表
    public void addOnGoingTask(Task task){
        task.TManager = this;
        currTaskList.Add(task.TaskName,task);

        if(isTaskNotStart(name))notStartTaskList.Remove(name);
        else if(!isTaskOnGoingSchedule(name))finishedTaskList.Remove(name);
    }

    //获取任务进度列表
    public Dictionary<string,OnGoingTask> getTaskSchedule()
    {
        return taskSchedule;
    }

    //判断任务是否存在
    private bool isTaskExist(string name)
    {
        foreach(string na in taskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    //判断任务是否为未开始
    private bool isTaskNotStart(string name)
    {
        foreach(string na in notStartTaskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    //判断任务是否有进行中的进度
    private bool isTaskOnGoingSchedule(string name)
    {
        return taskSchedule.ContainsKey(name);
    }

    //判断任务是否为进行中
    private bool isTaskActive(string name)
    {
        return currTaskList.ContainsKey(name);
    }

    //判断任务是否为已完成
    private bool isTaskFinished(string name)
    {
        foreach(string na in finishedTaskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    //移除任务
    public void removeTask(string name){
        taskList.Remove(name);
        currTaskList.Remove(name);
        notStartTaskList.Remove(name);
        finishedTaskList.Remove(name);
        justFinishedTaskList.Remove(name);
        taskSchedule.Remove(name);
    }


    public Task getOnGoingTaskByName(string name){
        bool index = currTaskList.ContainsKey(name);
        if(index)
            return currTaskList[name];
        else return null;
    }

    public List<string> getJustFinishedTask(){
        return justFinishedTaskList;
    }

   
    //相应任务完成的函数
    public void RespondTask(string name)
    {
        Task task = getOnGoingTaskByName(name);
        if(task == null)return;

        currTaskList.Remove(name);
        finishedTaskList.Add(name);
        justFinishedTaskList.Add(name);

    }

    //任务完成的事件函数
    public void FinishedTaskEvent()
    {
        foreach(string task in justFinishedTaskList){
            //任务完成后的操作

            justFinishedTaskList.Remove(task);
        }
    }

    
}


