using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainTask : Task
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override protected void FinishedEvent()
    {
        Debug.Log("son");
        SceneManager.LoadScene("StartMenu");
    }
}
