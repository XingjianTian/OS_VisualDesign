using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ProcessState
{
    Create,
    Ready,
    Running,
    Blocked,
    Ended,
}

public enum ProcessMoveTo
{
    ToEntrance,
    ToExit,
    ToTarget,
}

public enum ProcessMoveFrom_To
{
    JustStay,
    CreateToReady,
    ReadyToRunning,
    RunningToReady,
    RunningToEnded,
    RunningToBloked,
    BlokedToReady,
}


public class Process : MonoBehaviour
{
    public Texture2D img;
    public bool isShowUp = false;
    public double time = 0;//时间变量，实现每timeslice秒NeedTime-,CPUTime+
    public bool startloop = true;

    public GameObject ps;//ProcessSchedule

    //
    public int ProcessID;//进程标识符
    public int ProcessPos;
    public ProcessState PS;//进程所处的状态
    public int Priority;//优先级
    public double ArrivalTime;//到达时间
    public double NeedTime;//还需要的时间
    public double CPUTime;//已占用的时间
    public bool IORequest = false;
    public int randomnum;
    //移动
    public ProcessMoveFrom_To MoveFromTo = ProcessMoveFrom_To.JustStay;
    public ProcessMoveTo MoveTo;

    //public int FunctionsCount = 0;

    public bool IfMoveOver = true;
    // public bool IfNewFinal = true;

    public Vector3 FinalPosition;
    public Vector3 Begin;
    // Use this for initialization
    void Start()
    {
        ps = GameObject.FindWithTag("PS");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //运动函数
    public void action()
    {
        int WaitToRunProcessNum = ps.GetComponent<ProcessSchedule>().ProcessNum;
        for (int i = 0; i < ps.GetComponent<ProcessSchedule>().ProcessNum; i++)
        {
            if (ps.GetComponent<ProcessSchedule>().Process_List[i].
                GetComponent<Process>().PS == ProcessState.Ended)
                WaitToRunProcessNum--;
        }

        if (CanAct())
        {
            if (PS != ProcessState.Ended)
            {
                MoveFromReadyToRunning();

                if (PS == ProcessState.Running && MoveFromTo == ProcessMoveFrom_To.JustStay)
                {
                    //StartCoroutine(ParameterChangePerSecond());
                    if (NeedTime > ps.GetComponent<ProcessSchedule>().TimeSlice)
                    {
                        if (time >= ps.GetComponent<ProcessSchedule>().TimeSlice &&
                            NeedTime > 0)
                        //用时间变量实现重复调用
                        {
                            Change(ps.GetComponent<ProcessSchedule>().TimeSlice);
                            if (NeedTime > 0 && WaitToRunProcessNum > 1)
                            {
                                randomnum = Random.Range(1, 5);
                                //改变movefromto值
                                if (randomnum == 4)
                                {
                                    MoveFromRunningToBloked();
                                    IORequest = true;
                                }
                                else if (ps.GetComponent<ProcessSchedule>().PAS
                                    == AlgorithmState.RR)
                                    MoveFromRunningToReady();
                            }
                            time = 0;
                        }
                        time += Time.deltaTime;
                    }
                    else if (NeedTime >= 1)
                    {
                        if (time >= 1 && NeedTime > 0)
                        //用时间变量实现重复调用
                        {
                            Change(1);
                            time = 0;
                        }
                        time += Time.deltaTime;
                        if (NeedTime <= 0)
                        {
                            GetComponent<TextMesh>().color = Color.green;
                            //StopCoroutine(ParameterChangePerSecond());
                            MoveFromRunningToEnded();
                        }
                    }
                }
                else if (MoveFromTo != ProcessMoveFrom_To.RunningToEnded)
                {
                    if (IORequest)
                    {
                        MoveFromRunningToBloked();
                        MoveFromBlokedToReady();
                    }
                    else //if (ps.GetComponent<ProcessSchedule>().PAS== AlgorithmState.RR)
                        MoveFromRunningToReady();

                    if (PS == ProcessState.Ready
                        //&& MoveFromTo == ProcessMoveFrom_To.JustStay
                        && time != 0)
                    {
                        startloop = false;
                        IORequest = false;
                    }
                }
                else
                {
                    MoveFromRunningToEnded();
                }
            }
        }
    }

    //每秒参数变化
    public void Change(double time)
    {
        if (NeedTime > 0)
        {
            NeedTime -= time;
            CPUTime += time;
            GetComponent<TextMesh>().color =
                GetComponent<TextMesh>().color == Color.white ? Color.green : Color.white;
        }
    }

    /*
    public IEnumerator ParameterChangePerSecond()
    {
        yield return new WaitForSeconds(5);
        if (NeedTime > 0)
        {
            NeedTime--;
            CPUTime++;
        }
         
    }*/

    //移动函数公用
    public void settingRandomFinalPos()
    {
        Vector2 p = Random.insideUnitCircle * 1f;

        switch (MoveFromTo)
        {
            case ProcessMoveFrom_To.CreateToReady:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(-3.3F, 2.5F, 0); break;
            case ProcessMoveFrom_To.ReadyToRunning:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(3.4f, 2.5f, 0); break;
            case ProcessMoveFrom_To.RunningToEnded:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(6.2f, -2.6f, 0); break;
            case ProcessMoveFrom_To.RunningToBloked:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(-0.1f, -2.6f, 0); break;
            case ProcessMoveFrom_To.RunningToReady:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(-3.3F, 2.5F, 0); break;
            case ProcessMoveFrom_To.BlokedToReady:
                FinalPosition = new Vector3(p.x, p.y, 0) + new Vector3(-3.3F, 2.5F, 0); break;

        }
        Vector3 Begin = transform.position;
        IfMoveOver = false;
        //IfNewFinal = false;
    }
    public void MoveOver()
    {
        if (transform.position.x >= FinalPosition.x - 0.05 && transform.position.x <= FinalPosition.x + 0.05
            && transform.position.y >= FinalPosition.y - 0.05 && transform.position.y <= FinalPosition.y + 0.05)
        {
            IfMoveOver = true;
            switch (MoveFromTo)
            {
                case ProcessMoveFrom_To.CreateToReady: PS = ProcessState.Ready; break;
                case ProcessMoveFrom_To.ReadyToRunning: PS = ProcessState.Running; break;
                case ProcessMoveFrom_To.RunningToEnded: PS = ProcessState.Ended; break;
                case ProcessMoveFrom_To.RunningToBloked: PS = ProcessState.Blocked; break;
                case ProcessMoveFrom_To.RunningToReady: PS = ProcessState.Ready; break;
                case ProcessMoveFrom_To.BlokedToReady: PS = ProcessState.Ready; break;
            }
            MoveFromTo = ProcessMoveFrom_To.JustStay;
            MoveTo = ProcessMoveTo.ToEntrance;
            //FunctionsCount++;
        }
    }
    public void MoveNavigation()
    {
        double Precision = 0.025;
        Vector3 Entrance = Vector3.zero;
        Vector3 Exit = Vector3.zero;
        switch (MoveFromTo)
        {
            case ProcessMoveFrom_To.CreateToReady:
                {
                    Entrance = new Vector3(-5, -1, 0);
                    Exit = new Vector3(-4, 1, 0);
                }
                break;
            case ProcessMoveFrom_To.ReadyToRunning:
                {
                    Entrance = new Vector3(-1, 3, 0);
                    Exit = new Vector3(1, 3, 0);
                }
                break;
            case ProcessMoveFrom_To.RunningToEnded:
                {
                    Entrance = new Vector3(4, 1, 0);
                    Exit = new Vector3(5, -1, 0);
                }
                break;
            case ProcessMoveFrom_To.RunningToReady:
                {
                    Entrance = new Vector3(1, 2.2f, 0);
                    Exit = new Vector3(-1, 2.2f, 0);
                }
                break;
            case ProcessMoveFrom_To.RunningToBloked:
                {
                    Entrance = new Vector3(2, 1, 0);
                    Exit = new Vector3(1, -1, 0);
                }
                break;
            case ProcessMoveFrom_To.BlokedToReady:
                {
                    Entrance = new Vector3(-1, -1, 0);
                    Exit = new Vector3(-2, 1, 0);
                }
                break;
        }
        if (transform.position.x >= Begin.x - Precision && transform.position.x <= Begin.x + Precision &&
            transform.position.y >= Begin.y - Precision && transform.position.y <= Begin.y + Precision)
            MoveTo = ProcessMoveTo.ToEntrance;
        if (transform.position.x >= Entrance.x - Precision && transform.position.x <= Entrance.x + Precision &&
            transform.position.y >= Entrance.y - Precision && transform.position.y <= Entrance.y + Precision)
            MoveTo = ProcessMoveTo.ToExit;
        if (transform.position.x >= Exit.x - Precision && transform.position.x <= Exit.x + Precision &&
            transform.position.y >= Exit.y - Precision && transform.position.y <= Exit.y + Precision)
            MoveTo = ProcessMoveTo.ToTarget;

        switch (MoveTo)
        {
            case ProcessMoveTo.ToEntrance:
                transform.position = Vector3.MoveTowards(transform.position, Entrance, Time.deltaTime * 1.0f);
                break;
            case ProcessMoveTo.ToExit:
                transform.position = Vector3.MoveTowards(transform.position, Exit, Time.deltaTime * 1.0f);
                break;
            case ProcessMoveTo.ToTarget:
                transform.position = Vector3.MoveTowards(transform.position, FinalPosition, Time.deltaTime * 1.0f);
                break;

        }

    }

    public bool CanAct()
    {
        if (PS == ProcessState.Ready)//&&!startloop)
        {
            foreach(var progress in ps.GetComponent<ProcessSchedule>().Process_List)
            {
                if (
                   //已经有进程在运行
                   (progress.GetComponent<Process>().MoveFromTo == ProcessMoveFrom_To.JustStay
                   && progress.GetComponent<Process>().PS == ProcessState.Running
                   && progress.GetComponent<Process>().ProcessID != ProcessID)
                   ||
                   //决定无进程在运行时，谁先运行
                   (progress.GetComponent<Process>().MoveFromTo == ProcessMoveFrom_To.ReadyToRunning
                   && progress.GetComponent<Process>().PS == ProcessState.Ready
                   && progress.GetComponent<Process>().ProcessPos < ProcessPos
                   && !IfMoveOver
                   //.ProcessID != ProcessID
                  )
                   )
                        return false;
            }
        }
        return true;
    }

    public void MoveFromCreateToReady()
    {
        if (PS == ProcessState.Create && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
          MoveFromTo == ProcessMoveFrom_To.CreateToReady))
        {
            MoveFromTo = ProcessMoveFrom_To.CreateToReady;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }


    }
    //从就绪状态移动到运行状态
    public void MoveFromReadyToRunning()
    {


        if (PS == ProcessState.Ready && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
        MoveFromTo == ProcessMoveFrom_To.ReadyToRunning))
        {
            MoveFromTo = ProcessMoveFrom_To.ReadyToRunning;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }

    }
    //从运行状态移动到就绪状态
    public void MoveFromRunningToReady()
    {

        if (PS == ProcessState.Running && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
        MoveFromTo == ProcessMoveFrom_To.RunningToReady))
        {
            MoveFromTo = ProcessMoveFrom_To.RunningToReady;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }

    }
    //从运行状态移动到到阻塞状态
    public void MoveFromRunningToBloked()
    {

        if (PS == ProcessState.Running && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
        MoveFromTo == ProcessMoveFrom_To.RunningToBloked))
        {
            MoveFromTo = ProcessMoveFrom_To.RunningToBloked;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }

    }
    //从运行状态到结束区域
    public void MoveFromRunningToEnded()
    {

        if (PS == ProcessState.Running && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
        MoveFromTo == ProcessMoveFrom_To.RunningToEnded))
        {
            MoveFromTo = ProcessMoveFrom_To.RunningToEnded;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }

    }
    //从阻塞状态到就绪状态
    public void MoveFromBlokedToReady()
    {
        if (PS == ProcessState.Blocked && (MoveFromTo == ProcessMoveFrom_To.JustStay ||
        MoveFromTo == ProcessMoveFrom_To.BlokedToReady))
        {
            MoveFromTo = ProcessMoveFrom_To.BlokedToReady;
            if (IfMoveOver == true)// && IfNewFinal == true)
            {
                //开始移动,设置移动目标以及最终坐标点（只设置一次）

                settingRandomFinalPos();
            }
            if (IfMoveOver == false)
            {
                MoveNavigation();
                MoveOver();
            }
        }

    }


    //实时显示
    void OnMouseEnter(){isShowUp = true;}
    void OnMouseExit(){isShowUp = false;}
    void OnGUI()
    {
        if (isShowUp)
        {
            string Info = "";
            Info += string.Format("进程编号：{0}\n到达时间：{1}\n还需时间：{2}\n已用时间：{3}\n优先级：{4}\n进程状态：\n{5}",
                ProcessID,
                ArrivalTime,
                NeedTime,
                CPUTime,
                Priority,
                PS
                );
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = img;    //设置背景填充  
            fontStyle.normal.textColor = new Color(1, 1, 1);   //设置字体颜色  
            fontStyle.fontSize = 35;       //字体大小  
            fontStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 230, 290), Info,fontStyle);
        }
    }
}
