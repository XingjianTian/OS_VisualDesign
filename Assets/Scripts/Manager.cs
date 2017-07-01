using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DisplayState
{
    Play,
    Pause,
}

public class Manager : MonoBehaviour
{

    public DisplayState state;//展示状态,暂停或运行
    GUIStyle guiRectStyle;
    //按钮
    public Texture Pause_btn;
    public Texture Go_btn;
    public Texture Replay_btn;
    public Texture Exit_btn;
    float screenX;
    float screenY;

    //Process

    // Use this for initialization
    void Start()
    {
        screenX = Screen.width;
        screenY = Screen.height;
        guiRectStyle = new GUIStyle();
        guiRectStyle.border = new RectOffset(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI()
    {
        if (state == DisplayState.Play)
        {

            if (GUI.Button(new Rect(20, 20, Pause_btn.width, Pause_btn.height),
                Pause_btn,
                guiRectStyle))
            {
                state = DisplayState.Pause;
                Time.timeScale = 0;
            }
        }
        if (state == DisplayState.Pause)
        {
            if (GUI.Button(new Rect(screenX * 0.5f - Go_btn.width * 0.5f,
                screenY * 0.5f + Replay_btn.height * 0.5f + 10f,
                Go_btn.width, Go_btn.height),
                Go_btn,
                guiRectStyle))
            {
                Time.timeScale = 1;
               state = DisplayState.Play;

            }
            if (GUI.Button(new Rect(screenX * 0.5f - Replay_btn.width * 0.5f,
                screenY * 0.5f - Replay_btn.height * 0.5f,
                Replay_btn.width, Replay_btn.height),
                Replay_btn,
                guiRectStyle))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("DisplayScene");
            }

            if (GUI.Button(new Rect(screenX * 0.5f - Exit_btn.width * 0.5f,
                screenY * 0.5f - Replay_btn.height * 0.5f - Exit_btn.height - 10f,
                Exit_btn.width, Exit_btn.height),
                Exit_btn,
                guiRectStyle))
                SceneManager.LoadScene("GuildingScene");
        }
    }
}
    
