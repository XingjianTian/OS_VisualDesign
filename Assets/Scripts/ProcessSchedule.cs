using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AlgorithmState {
    FCFS,
    SJF,
    RR,
    PF,
}

public class ProcessSchedule : MonoBehaviour {

    //控制输入
    public bool ifReadyToCreate = false;
    public bool ifcreated = false;
    //两个输入区域
    public InputField Input_Field;
    public int ProcessNum = 0;
    public InputField Input_Field2;
    public double TimeSlice = 1; 

    public AlgorithmState PAS;//选择的算法
    public GameObject manager;//管理整个程序的暂停
    public GameObject Process_Prefabs;//进程预设体
    public GameObject newprocess;//新进程

    public List<GameObject> Process_List;//进程链表
    public GameObject ProcessInfo;//所有进程信息
    public string Process_Information;//进程信息的字符化

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //获取输入
        if (ifReadyToCreate && ifcreated == false)
        {
            ProcessNum = System.Convert.ToInt32(Input_Field.text);
            TimeSlice = System.Convert.ToDouble(Input_Field2.text);

            CreateProcessList();
            Input_Field.readOnly = true;
            Input_Field2.readOnly = true;


            ProcessInfo.GetComponent<Text>().text = CreateProcessInformation();
            Time.timeScale = 0;
        }

        //调度算法演示
        ChooseAndDoAlgorithm();
     
    }
   

    public void CreateProcessList()
    {
        
        for(int i = 0;i<ProcessNum;i++)
        {
            Vector2 p = Random.insideUnitCircle * 1.5f;
            Vector3 pos = new Vector3(p.x,p.y,0)+new Vector3(-6,(float)-2.595,0);
            newprocess = Instantiate(Process_Prefabs, pos, Quaternion.identity);
            newprocess.SetActive(false);
            newprocess.GetComponent<TextMesh>().text = System.Convert.ToString(i+1);

            newprocess.GetComponent<Process>().ProcessID = i + 1;
            newprocess.GetComponent<Process>().PS = ProcessState.Create;
            newprocess.GetComponent<Process>().Priority = Random.Range(0, 4);
            newprocess.GetComponent<Process>().ArrivalTime = Random.Range(0, 10);
            newprocess.GetComponent<Process>().NeedTime = Random.Range(1, 11);
            newprocess.GetComponent<Process>().CPUTime = 0;
            Process_List.Add(newprocess);
            /*Process newPCB = newprocess.GetComponent<Process>();
           
            newPCB.ProcessID = i+1;
            newPCB.PS = ProcessState.Ready;
            newPCB.Priority = Random.Range(0, 4);
            newPCB.ArrivalTime = Random.Range(0, 10);
            newPCB.NeedTime = Random.Range(1, 11);
            newPCB.CPUTime = 0;

            Process_List.Add(newPCB);*/

        }
        ifcreated = true;      
    }
    public string CreateProcessInformation()
    {
        string result = "";
        foreach(var process in Process_List)
        {
            result += string.Format("{0}          {1}             {2}           {3}           {4}       {5}\n",
                process.GetComponent<Process>().ProcessID,
                process.GetComponent<Process>().ArrivalTime,
                process.GetComponent<Process>().NeedTime,
                process.GetComponent<Process>().CPUTime,
                process.GetComponent<Process>().Priority,
                process.GetComponent<Process>().PS
                );
        }
        return result;
    }
    public void ChooseAndDoAlgorithm()
    {
        if (Process_List.Count != 0)
        {
            switch (PAS)
            {
                case AlgorithmState.FCFS:
                    Process_List.Sort((x, y) =>
                    {
                        if (x.GetComponent<Process>().ArrivalTime != y.GetComponent<Process>().ArrivalTime)
                            return x.GetComponent<Process>().ArrivalTime.CompareTo(y.GetComponent<Process>().ArrivalTime);
                        else if (x.GetComponent<Process>().NeedTime != y.GetComponent<Process>().NeedTime)
                            return x.GetComponent<Process>().NeedTime.CompareTo(y.GetComponent<Process>().NeedTime);
                        else
                        {
                            return x.GetComponent<Process>().ProcessID.CompareTo(y.GetComponent<Process>().ProcessID);
                        }
                    }); break;//按到达时间排序
                case AlgorithmState.PF:
                    Process_List.Sort((x, y) =>
                    {
                        if (x.GetComponent<Process>().Priority != y.GetComponent<Process>().Priority)
                            return x.GetComponent<Process>().Priority.CompareTo(y.GetComponent<Process>().Priority);
                        else if (x.GetComponent<Process>().NeedTime != y.GetComponent<Process>().NeedTime)
                            return x.GetComponent<Process>().NeedTime.CompareTo(y.GetComponent<Process>().NeedTime);
                        else
                            return x.GetComponent<Process>().ProcessID.CompareTo(y.GetComponent<Process>().ProcessID);
                    }); break;//按优先级排序
                case AlgorithmState.SJF:
                    Process_List.Sort((x, y) =>
                    {
                        if (x.GetComponent<Process>().NeedTime != y.GetComponent<Process>().NeedTime)
                            return x.GetComponent<Process>().NeedTime.CompareTo(y.GetComponent<Process>().NeedTime);
                        else if (x.GetComponent<Process>().ArrivalTime != y.GetComponent<Process>().ArrivalTime)
                            return x.GetComponent<Process>().ArrivalTime.CompareTo(y.GetComponent<Process>().ArrivalTime);
                        else
                        {
                            return x.GetComponent<Process>().ProcessID.CompareTo(y.GetComponent<Process>().ProcessID);
                        }
                    }); break;//按所需时间排序
                case AlgorithmState.RR:
                    Process_List.Sort((x, y) =>
                    {
                        if (x.GetComponent<Process>().ArrivalTime != y.GetComponent<Process>().ArrivalTime)
                            return x.GetComponent<Process>().ArrivalTime.CompareTo(y.GetComponent<Process>().ArrivalTime);
                        else if (x.GetComponent<Process>().NeedTime != y.GetComponent<Process>().NeedTime)
                            return x.GetComponent<Process>().NeedTime.CompareTo(y.GetComponent<Process>().NeedTime);
                        else
                        {
                            return x.GetComponent<Process>().ProcessID.CompareTo(y.GetComponent<Process>().ProcessID);
                        }
                    }); break;//到达排序
            }

            foreach (var process in Process_List)
                process.GetComponent<Process>().MoveFromCreateToReady();

            for (int i = 0; i < Process_List.Count; i++)
            {
                Process_List[i].GetComponent<Process>().ProcessPos = i;

                if (i == 0)
                    Process_List[i].GetComponent<Process>().action();
                else if (
                (Process_List[i - 1].GetComponent<Process>().PS != Process_List[i].GetComponent<Process>().PS &&
                Process_List[i - 1].GetComponent<Process>().MoveFromTo != ProcessMoveFrom_To.JustStay) ||
                    (Process_List[i - 1].GetComponent<Process>().PS == ProcessState.Ended &&
                    Process_List[i - 1].GetComponent<Process>().MoveFromTo == ProcessMoveFrom_To.JustStay) ||
                    (Process_List[i - 1].GetComponent<Process>().PS == ProcessState.Blocked &&
                    Process_List[i - 1].GetComponent<Process>().MoveFromTo == ProcessMoveFrom_To.JustStay) ||
                    (Process_List[i - 1].GetComponent<Process>().PS == ProcessState.Ready &&
                    !Process_List[i - 1].GetComponent<Process>().startloop &&
                    Process_List[i - 1].GetComponent<Process>().MoveFromTo == ProcessMoveFrom_To.JustStay)
                    )
                    Process_List[i].GetComponent<Process>().action();
            }
        }
    }

}



