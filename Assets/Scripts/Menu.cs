using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public GameObject ProcessInfo;
    public GameObject CheckText;
    public ProcessSchedule ps;
    public Canvas quitCanvas;
    public GameObject Panel1;
    public GameObject Panel2;
    public GameObject Panel3;
    public GameObject AllVisualObjects;
    public GameObject CheckBtn;
	// Use this for initialization
	void Start () {
        Panel1.SetActive(true);
        Panel2.SetActive(false);
        Panel3.SetActive(false);
        AllVisualObjects.SetActive(false);
        CheckText.SetActive(false);
        CheckBtn.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    //场景转换函数
    public void CreatePress(){SceneManager.LoadScene("DisplayScene");}
    public void HelpPress(){SceneManager.LoadScene("HelpScene");}
    public void ExitPress(){Application.Quit();}
    public void BackPress(){SceneManager.LoadScene("GuildingScene");}

    public void StartPress()
    {
        quitCanvas.enabled = false;
        //Panel1.SetActive(false);
        //Panel2.SetActive(false);
        CheckBtn.SetActive(true);
        AllVisualObjects.SetActive(true);
        
        for (int i = 0; i < ps.Process_List.Count; i++)
            ps.Process_List[i].SetActive(true);
        Time.timeScale = 1;
    }
    public void FCFSPress()
    {
        ps.ifReadyToCreate = true;
        Panel1.SetActive(false);
        Panel2.SetActive(true);
        ps.PAS = AlgorithmState.FCFS;
        
    }
    public void SJFPress()
    {
        ps.ifReadyToCreate = true;
        Panel1.SetActive(false);
        Panel2.SetActive(true);
        ps.PAS = AlgorithmState.SJF;
    }
    public void RRPress()
    {
        ps.ifReadyToCreate = true;
        Panel1.SetActive(false);
        Panel2.SetActive(true);
        ps.PAS = AlgorithmState.RR;
    }
    public void PFPress()
    {
        ps.ifReadyToCreate = true;
        Panel1.SetActive(false);
        Panel2.SetActive(true);
        ps.PAS = AlgorithmState.PF;
    }

    //查看canvas俩部件回调函数
    public void ExitCheckPress()
    {
        AllVisualObjects.SetActive(true);
        for (int i = 0; i < ps.Process_List.Count; i++)
            ps.Process_List[i].SetActive(true);
        Panel3.SetActive(false);
    }

    public void CheckPress()
    {
        AllVisualObjects.SetActive(false);
        for (int i = 0; i < ps.Process_List.Count; i++)
            ps.Process_List[i].SetActive(false);
        Panel3.SetActive(true);
        ProcessInfo.GetComponent<Text>().text = ps.CreateProcessInformation();
    }
}
