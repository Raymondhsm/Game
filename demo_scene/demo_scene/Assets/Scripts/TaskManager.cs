using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private Hashtable taskList;
    private Hashtable currTaskList;
    private Hashtable notStartTaskList;
    private Hashtable finishedTaskList;
    private ArrayList justFinishedTaskList;


    void Awake(){
        taskList = new Hashtable();
        currTaskList = new Hashtable();
        notStartTaskList = new Hashtable();
        finishedTaskList = new Hashtable();
        justFinishedTaskList = new ArrayList();

        //initializeTask();
    }

    private void initializeTask()
    {

    }

    private void readHistory(Task task)
    {

    }


    public void addTask(Task task){
        task.TManager = this;
        taskList.Add(task.TaskName,task);
        notStartTaskList.Add(task.TaskName,task);
    }

    public void removeTask(string name){
        taskList.Remove(name);
        currTaskList.Remove(name);
        notStartTaskList.Remove(name);
        finishedTaskList.Remove(name);
        justFinishedTaskList.Remove(getTaskByName(name));
    }

    public Task getTaskByName(string name){
        bool index = taskList.ContainsKey(name);
        if(index)
            return taskList[name] as Task;
        else return null;
    }

    public ArrayList getJustFinishedTask(){
        return justFinishedTaskList;
    }

    public bool StartTask(string name)
    {
        Task task = getTaskByName(name);
        if(task == null)return false;
        else{
            notStartTaskList.Remove(name);
            currTaskList.Add(name,getTaskByName(name));
            return task.StartTask();
        }
    }

    public void RespondTask(string name)
    {
        Task task = getTaskByName(name);
        if(task == null)return;

        currTaskList.Remove(name);
        finishedTaskList.Add(name,task);
        justFinishedTaskList.Add(task);

    }

    public void FinishedTaskEvent()
    {
        foreach(Task task in justFinishedTaskList){
            //任务完成后的操作

            justFinishedTaskList.Remove(task);
        }
    }

    
}
