using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject canvasController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenAnimationEnd()
    {
        StartMenu sm = canvasController.GetComponent<StartMenu>();
        sm.StartCanvas();
    }
}
