using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject StartMenuCanvas;       // 开始界面画布
    public GameObject OptionCanvas;          // 设置界面画布
    public GameObject InformationCanvas;     // 关于界面画布
    public GameObject ExitCanvas;            // 退出界面画布
    public GameObject TrainTipCanvas;       //教程关提示画布
    public Slider VolumeSlider;              // 设置界面调整音量的滑块
    public Slider MouseSensitivitySlider;    // 设置界面调整鼠标灵敏度的滑块
    public Text VolumeValueText;             // 设置界面调整音量的文本
    public Text MouseSensitivityValueText;   // 设置界面调整鼠标灵敏度的文本

    void Start()
    {
        // 场景载入时，初始化界面
        StartMenuCanvas.SetActive(false);
        TrainTipCanvas.SetActive(false);
        OptionCanvas.SetActive(false);
        InformationCanvas.SetActive(false);
        ExitCanvas.SetActive(false);

        // 当没有游戏存档时，初始化游戏存档
        if (!PlayerPrefs.HasKey("_VOLUME"))
        {
            PlayerPrefs.SetFloat("_VOLUME", 100f);
        }
        if (!PlayerPrefs.HasKey("_MOUSE_SENSITIVITY"))
        {
            PlayerPrefs.SetFloat("_MOUSE_SENSITIVITY", 1f);
        }
    }

    /// <summary>
    /// 点击“开始游戏”按钮加载游戏场景
    /// </summary>
    public void OnClickPlay()
    {
        Debug.Log("点击了开始游戏按钮");
        int selectedPlanet = GameObject.Find("SelectPlanet").GetComponent<SelectPlanet>().selectedPlanet; // 选择的是第几个星球

        // 在这里写进入场景的代码
        switch (selectedPlanet)
        {
            case 1:
                SceneManager.LoadScene("SampleScene");
                break;
            case 2:
                SceneManager.LoadScene("SampleScene");
                break;
            case 3:
                SceneManager.LoadScene("SampleScene");
                break;
            default:
                break;
        }
    }

    public void OnClickTrainSession()
    {
        SceneManager.LoadScene("TrainingSession");
    }

    public void OnClickJumpTrain()
    {
        TrainTipCanvas.SetActive(false);
        StartMenuCanvas.SetActive(true);
    }

    public void StartCanvas()
    {
        bool isPassTrain = PlayerPrefs.HasKey("isPassTrain");
        if(isPassTrain)
            StartMenuCanvas.SetActive(true);
        else TrainTipCanvas.SetActive(true);
    }

    /// <summary>
    /// 点击“设置”按钮，进行音量、鼠标灵敏度设置
    /// </summary>
    public void OnClickOption()
    {
        Debug.Log("点击了设置按钮");
        StartMenuCanvas.SetActive(false);
        OptionCanvas.SetActive(true);
        InformationCanvas.SetActive(false);
        ExitCanvas.SetActive(false);

        // 输出各个界面的active情况
        Debug.Log("当前GameObject:" + gameObject.name);
        Debug.Log("StartMenuCanvas:" + StartMenuCanvas.activeSelf);
        Debug.Log("OptionCanvas:" + OptionCanvas.activeSelf);
        Debug.Log("InformationCanvas:" + InformationCanvas.activeSelf);

        // 在打开设置界面时，按照游戏存档调整滑块的值
        VolumeSlider.value = PlayerPrefs.GetFloat("_VOLUME");
        MouseSensitivitySlider.value = PlayerPrefs.GetFloat("_MOUSE_SENSITIVITY");
        VolumeValueText.text = VolumeSlider.value.ToString();
        MouseSensitivityValueText.text = MouseSensitivitySlider.value.ToString();
    }

    /// <summary>
    /// 在设置界面，拉动滑块调整音量
    /// </summary>
    public void OnVolumeSliderValueChange()
    {
        Debug.Log("调整了音量大小");
        PlayerPrefs.SetFloat("_VOLUME", VolumeSlider.value); // 保存音量到游戏存档
        VolumeValueText.text = VolumeSlider.value.ToString();
    }

    /// <summary>
    /// 在设置界面，拉动滑块调整鼠标灵敏度
    /// </summary>
    public void OnMouseSensitivitySliderValueChange()
    {
        Debug.Log("调整了鼠标灵敏度");
        PlayerPrefs.SetFloat("_MOUSE_SENSITIVITY", MouseSensitivitySlider.value); // 保存鼠标灵敏度到游戏存档
        MouseSensitivityValueText.text = MouseSensitivitySlider.value.ToString();
    }

    /// <summary>
    /// 点击“设置界面”的“返回”按钮，返回到上一级
    /// </summary>
    public void OnClickOptionBack()
    {
        Debug.Log("点击了设置界面的返回按钮");
        ReturnToStartMenu();
    }

    /// <summary>
    /// 点击“关于”按钮，查看开发人员信息
    /// </summary>
    public void OnClickInfo()
    {
        Debug.Log("点击了关于按钮");
        StartMenuCanvas.SetActive(false);
        OptionCanvas.SetActive(false);
        InformationCanvas.SetActive(true);
        ExitCanvas.SetActive(false);
    }

    /// <summary>
    /// 点击“关于界面”的“返回”按钮，返回到上一级
    /// </summary>
    public void OnClickInfoBack()
    {
        Debug.Log("点击了关于界面的返回按钮");
        ReturnToStartMenu();
    }

    /// <summary>
    /// 点击“退出游戏”按钮弹出退出确认界面
    /// </summary>
    public void OnClickQiut()
    {
        Debug.Log("点击了退出游戏按钮");
        StartMenuCanvas.SetActive(false);
        OptionCanvas.SetActive(false);
        InformationCanvas.SetActive(false);
        ExitCanvas.SetActive(true);
    }

    /// <summary>
    /// 在退出界面点击确认按钮退出游戏
    /// </summary>
    public void OnClickQuitYes()
    {
        Debug.Log("点击了退出界面的确定按钮");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//调试阶段的结束
#else
        Application.Quit();//编译过后生效
#endif
    }

    /// <summary>
    /// 在退出界面点击取消返回主界面
    /// </summary>
    public void OnClickQuitBack()
    {
        Debug.Log("点击了退出界面的返回按钮");
        ReturnToStartMenu();
    }

    /// <summary>
    /// 返回主界面
    /// </summary>
    private void ReturnToStartMenu()
    {
        StartMenuCanvas.SetActive(true);
        OptionCanvas.SetActive(false);
        InformationCanvas.SetActive(false);
        ExitCanvas.SetActive(false);
    }
}
