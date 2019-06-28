using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("进场动画设置")]
    public Animator openDoorAnim;
    public Animator CameraMoveAnim;
    public GameObject door;
    public GameObject gameController;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartAnimationController());
        //StartAnimationController();
       StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void StartAnimationController()
    {
        AudioSource au = door.GetComponent<AudioSource>();

        //开门
        openDoorAnim.SetTrigger("doorToOpen");
        
        CameraMoveAnim.SetTrigger("CameraForward");
        au.Play();
    

    }

    IEnumerator StartAnimation()
    {
        AudioSource au = door.GetComponent<AudioSource>();

        //开门
        openDoorAnim.SetTrigger("doorToOpen");
        au.Play();
        CameraMoveAnim.SetTrigger("CameraForward");

        AnimatorStateInfo info = CameraMoveAnim.GetCurrentAnimatorStateInfo(0);
        while(!info.IsName("MoveEnd")){
            info = CameraMoveAnim.GetCurrentAnimatorStateInfo(0);

            yield return 0;
        }
        
        CameraMoveAnim.enabled = false;

        GameController gc = gameController.GetComponent<GameController>();
        gc.OpenAnimationEnd();

    }

}

