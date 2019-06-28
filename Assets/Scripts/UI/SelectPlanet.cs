using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPlanet : MonoBehaviour
{
    public int selectedPlanet = 1; // 保存当前选择的星球，默认第一个
    private GameObject m_Gaea;
    private GameObject m_Uranos;
    private GameObject m_Rhea;

    private void Start()
    {
        m_Gaea = GameObject.Find("Planet Gaea");
        m_Uranos = GameObject.Find("Planet Uranos");
        m_Rhea = GameObject.Find("Planet Rhea");
        SelectOnePlanet(m_Gaea);
    }

    public void SelectOnePlanet(GameObject planet)
    {
        if(planet == m_Gaea)
        {
            Debug.Log("选择了星球：Gaea");
            selectedPlanet = 1;
            m_Gaea.GetComponent<ClickPlanet>().SelectThis();
            m_Uranos.GetComponent<ClickPlanet>().CancelSelect();
            m_Rhea.GetComponent<ClickPlanet>().CancelSelect();
        }
        if (planet == m_Uranos)
        {
            Debug.Log("选择了星球：Uranos");
            selectedPlanet = 2;
            m_Gaea.GetComponent<ClickPlanet>().CancelSelect();
            m_Uranos.GetComponent<ClickPlanet>().SelectThis();
            m_Rhea.GetComponent<ClickPlanet>().CancelSelect();
        }
        if (planet == m_Rhea)
        {
            Debug.Log("选择了星球：Rhea");
            selectedPlanet = 3;
            m_Gaea.GetComponent<ClickPlanet>().CancelSelect();
            m_Uranos.GetComponent<ClickPlanet>().CancelSelect();
            m_Rhea.GetComponent<ClickPlanet>().SelectThis();
        }
    }
}
