using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private List<string> taskList;
    private Hashtable currTaskList;
    private Hashtable TaskSchedule;
    private List<string> notStartTaskList;
    private List<string> finishedTaskList;
    private List<string> justFinishedTaskList;


    void Awake(){
        taskList = new List<string>();
        currTaskList = new Hashtable();
        TaskSchedule = new Hashtable();
        notStartTaskList = new List<string>();
        finishedTaskList = new List<string>();
        justFinishedTaskList = new List<string>();

        TaskSchedule ts = new TaskSchedule();
        ts.FilePath = Application.dataPath + "/File/taskSchedule.txt";
        ts.ReadTaskSchedule(notStartTaskList, TaskSchedule, finishedTaskList, taskList);

    }

     public bool StartTask(string name)
    {
        if(!isTaskExist(name))return false;
        if(isTaskActive(name))return true;

        Task task = initializeTask(name);

        addOnGoingTask(task);

        task.StartTask();

        return true;
    }

    private Task initializeTask(string name)
    {
        Object prefab = Resources.Load("Prefabs/" + name) as Object;
        if (prefab == null) {
            Debug.LogError("not found task");
            return null;
        }
        GameObject taskObject = Instantiate(prefab) as GameObject;

        readHistory(taskObject.GetComponent<Task>());
        
        return taskObject.GetComponent<Task>();
    }

    private void readHistory(Task task)
    {
        if(!TaskSchedule.ContainsKey(task.TaskName))return;
        OnGoingTask t = TaskSchedule[task.TaskName] as OnGoingTask;
        task.CurrTaskNodeIndex = t.taskNodeIndex;
        task.CurrTaskNodeNum = t.taskNodeNum;
    }

    public void addOnGoingTask(Task task){
        task.TManager = this;
        currTaskList.Add(task.TaskName,task);

        if(isTaskNotStart(name))notStartTaskList.Remove(name);
        else if(!isTaskOnGoing(name))finishedTaskList.Remove(name);
    }


    private bool isTaskExist(string name)
    {
        foreach(string na in taskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    private bool isTaskNotStart(string name)
    {
        foreach(string na in notStartTaskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    private bool isTaskOnGoing(string name)
    {
        return TaskSchedule.ContainsKey(name);
    }

    private bool isTaskActive(string name)
    {
        return currTaskList.ContainsKey(name);
    }

    private bool isTaskFinished(string name)
    {
        foreach(string na in finishedTaskList)
        {
            if(na == name)return true;
        }
        return false;
    }

    public void removeTask(string name){
        taskList.Remove(name);
        currTaskList.Remove(name);
        notStartTaskList.Remove(name);
        finishedTaskList.Remove(name);
        justFinishedTaskList.Remove(name);
    }


    public Task getOnGoingTaskByName(string name){
        bool index = currTaskList.ContainsKey(name);
        if(index)
            return currTaskList[name] as Task;
        else return null;
    }

    public List<string> getJustFinishedTask(){
        return justFinishedTaskList;
    }

   

    public void RespondTask(string name)
    {
        Task task = getOnGoingTaskByName(name);
        if(task == null)return;

        currTaskList.Remove(name);
        finishedTaskList.Add(name);
        justFinishedTaskList.Add(name);

    }

    public void FinishedTaskEvent()
    {
        foreach(string task in justFinishedTaskList){
            //任务完成后的操作

            justFinishedTaskList.Remove(task);
        }
    }

    
}


