using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    public TaskManager tm;


    // Start is called before the first frame update
    void Start()
    {
        
        tm.StartTask("TrainTask",1);
        DontDestroyOnLoad(tm.gameObject);

        List<string> task = tm.getJustFinishedTask();
        foreach(string t in task){
            Debug.Log(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
