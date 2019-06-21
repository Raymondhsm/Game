using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject Information;//关于按钮点击之后出现的画布
    public GameObject Canvas;     //开始界面画布
    public GameObject Option;     //设置按钮点击之后出现的画布

    // Start is called before the first frame update
    void Start()
    {
        Information.SetActive(false);
        Option.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 点击“开始游戏”按钮加载游戏场景
    /// </summary>
    public void OnClickPlay()
    {
        Debug.Log("点击了开始游戏按钮");
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// 点击“退出游戏”按钮退出游戏界面
    /// </summary>
    public void OnClickQiut()
    {
        Debug.Log("点击了退出游戏按钮");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//调试阶段的结束
#else
        Application.Quit();//编译过后生效
#endif
    }
    /// <summary>
    /// 点击“关于”按钮，查看开发人员信息
    /// </summary>
    public void OnClickInfo()
    {
        Debug.Log("点击了关于按钮");
        Information.SetActive(true);
    }

    /// <summary>
    /// 点击“关于界面”的“返回”按钮，返回到上一级
    /// </summary>
    public void OnClickInfoBack()
    {
        Debug.Log("点击了关于界面的返回按钮");
        Information.SetActive(false);
    }

    /// <summary>
    /// 点击“设置”按钮，进行音量、鼠标灵敏度设置
    /// </summary>
    public void OnClickOption()
    {
        Debug.Log("点击了设置按钮");
        Option.SetActive(true);
    }

    /// <summary>
    /// 点击“设置界面”的“返回”按钮，返回到上一级
    /// </summary>
    public void OnClickOptionBack()
    {
        Debug.Log("点击了设置界面的返回按钮");
        Option.SetActive(false);
    }
}
