using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPlanet : MonoBehaviour
{
    [Range(0f, 100f)]
    public float rotateSpeed = 50f;

    [HideInInspector]
    public bool isSelected = false; // 用此布尔值控制同一时刻只有一个星球被选中

    private Outline m_outline; // 物体边缘发光那条线

    void Awake()
    {
        // 初始化边缘线（通过脚本添加组件，所以在编辑器里看不到这个组件）
        m_outline = gameObject.AddComponent<Outline>();
        m_outline.OutlineMode = Outline.Mode.OutlineAll;
        m_outline.OutlineColor = Color.yellow;
        m_outline.OutlineWidth = 5f;
        m_outline.enabled = false;
    }

    private void Update()
    {
        // 使星球自转
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 当鼠标指着星球时，稍微高亮这个星球
    /// </summary>
    private void OnMouseEnter()
    {
        if(!isSelected && !EventSystem.current.IsPointerOverGameObject())
        {
            m_outline.OutlineColor = Color.white;
            m_outline.OutlineWidth = 5f;
            m_outline.enabled = true;
        }
    }

    /// <summary>
    /// 当鼠标离开星球时，取消高亮这个星球
    /// </summary>
    private void OnMouseExit()
    {
        if(!isSelected)
        {
            m_outline.enabled = false;
        }
    }

    /// <summary>
    /// 当鼠标点击这个星球时，选中这个星球，取消选中其他星球
    /// </summary>
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // 当点击到UI时，不响应
        GameObject.Find("SelectPlanet").GetComponent<SelectPlanet>().SelectOnePlanet(this.gameObject); // 选中且仅选中这个星球，具体的操作是调用父物体的脚本的函数
    }

    /// <summary>
    /// 外部调用此函数，选择并高亮这个星球
    /// </summary>
    public void SelectThis()
    {
        isSelected = true;
        m_outline.OutlineColor = Color.yellow;
        m_outline.OutlineWidth = 5f;
        m_outline.enabled = true;
    }

    /// <summary>
    /// 外部调用此函数，取消这个星球的选择
    /// </summary>
    public void CancelSelect()
    {
        isSelected = false;
        m_outline.enabled = false;
    }
}
