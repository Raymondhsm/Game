using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    public TaskManager tm;


    // Start is called before the first frame update
    void Start()
    {
        tm.addTask(GameObject.Find("Task").GetComponent<Task>());

        tm.StartTask("test");

        ArrayList task = tm.getJustFinishedTask();
        foreach(Task t in task){
            Debug.Log(t.TaskName);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
