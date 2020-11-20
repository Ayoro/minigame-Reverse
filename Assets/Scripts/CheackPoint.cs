using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheackPoint : MonoBehaviour
{
    [System.Serializable]
    
    public struct dialog_content
    {
        [TextArea]
        public string line;
        public int num;//文本显示目标位置编号,-1为结束标志
    }

    public enum point_type//检查点类型
    {
        cheackPoint,//存档点
        dialogPoint,//对话触发点
        eventPoint,//事件触发点,可能触发某个函数
        movePoint//移动点（移动至下一个场景）
    }

    public point_type pt;

    public GameObject game_controller;

    bool is_triggered = false;

    [Tooltip("用于标识是否需要提示")]
    public bool is_hint_needed = true;

    [Tooltip("操作提示")]
    public GameObject hint;//操作提示
    public Transform hint_point;
    [TextArea]
    public string hint_text;


    [Tooltip("用于标识是否直接触发")]
    public bool is_trigger_direct = true;

    [Tooltip("用于标识该检查点触发完毕后是否可重复使用")]
    public bool is_trigger_reuseful = false;


    [Header("DialogPoint")]

    [Tooltip("文本内容以及显示位置编号")]
    public List<dialog_content> dialog;
    [Tooltip("对话显示点,0号位默认设置为对话第一次出现的位置")]
    public Transform[] dialog_point;
    [Tooltip("对话框Text")]
    public GameObject dialog_text;
    int dialog_index=0;//用于指示当前文本的进度
    public GameObject Player;
    bool is_dialog_on = false;//判断dialog是否打开
    public bool is_event_trigger=false;
    public bool is_auto_play = false;
    public float auto_play_speed=3.0f;
    

    [Header("MovePoint")]

    public string scene_name;//即将进入场景的名称
    public int born_point_num;//出生点编号
    public GameObject Ending;

    [Header("EventPoint")]
    bool is_event_on = false;
    public int event_num = -1;

    IEnumerator trigger_reset()//重置检查点参数
    {
        yield return new WaitForSeconds(2f);
        is_event_on = false;
        is_dialog_on = false;
        dialog_index = 0;
        is_triggered = false;
    }
    IEnumerator auto_play()
    {
        is_dialog_on = false;
        yield return new WaitForSeconds(auto_play_speed);
        for(;dialog_index<dialog.Count;)
        {
            dialog_text.transform.localPosition = Camera.main.WorldToScreenPoint(dialog_point[dialog[dialog_index].num].position) - new Vector3(Screen.width, Screen.height) / 2;
            dialog_text.GetComponent<TextMeshProUGUI>().text = dialog[dialog_index++].line;
            //播放对话音效
            yield return new WaitForSeconds(auto_play_speed);
        }
        if (is_event_trigger)
        {
            event_on();
        }
        Player.GetComponent<Player>().enabled = true;
        dialog_text.SetActive(false);
        if (is_trigger_reuseful)
        {
            trigger_reset();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void event_on()
    {
        game_controller.GetComponent<GameController>().event_num = event_num;
    }

    IEnumerator SceneLoad()//用于加载下一个场景
    {
        /*AsyncOperation async= SceneManager.LoadSceneAsync(scene_name);
        async.allowSceneActivation = false;
        PlayerPrefs.SetInt(scene_name, born_point_num);
        //加载过场动画（简单一点,暂定）
        //yield return new WaitForSeconds(1.0f);
        while(!async.isDone)
        {
            if(async.progress>0.9f)
            {
                break;
            }
        }
        async.allowSceneActivation = true;
        yield return null;*/
        Ending.GetComponent<MintAnimation_CanvasAlpha>().Play();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene_name);
        yield return null;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name == "Player"&&!is_triggered)
        {
            if (is_hint_needed)
            {
                hint.SetActive(true);
                hint.GetComponent<TextMeshProUGUI>().text = hint_text;
                hint.transform.localPosition= Camera.main.WorldToScreenPoint(hint_point.position) - new Vector3(Screen.width, Screen.height) / 2;
            }

            if (pt==point_type.dialogPoint)
            {
                if(is_trigger_direct||Input.GetKeyDown(KeyCode.Q))
                {
                    Player.GetComponent<Player>().sound.stopWalkSound();
                    Player.GetComponent<Player>().enabled = false;
                    is_dialog_on = true;
                    dialog_text.SetActive(true);
                    dialog_text.transform.localPosition = Camera.main.WorldToScreenPoint(dialog_point[dialog[dialog_index].num].position) - new Vector3(Screen.width, Screen.height) / 2;
                    dialog_text.GetComponent<TextMeshProUGUI>().text = dialog[dialog_index++].line;
                    if (is_hint_needed)
                    {
                        hint.SetActive(false);
                    }
                    is_triggered = true;
                }
            }
            else if (pt==point_type.movePoint)
            {
                if (is_trigger_direct || Input.GetKeyDown(KeyCode.Q))
                {
                    if (is_hint_needed)
                    {
                        hint.SetActive(false);
                    }
                    Player.GetComponent<Player>().enabled = false;
                    is_triggered = true;
                    StartCoroutine(SceneLoad());
                }
            }
            else if(pt==point_type.eventPoint)
            {
                if (is_trigger_direct || Input.GetKeyDown(KeyCode.Q))
                {
                    if (is_hint_needed)
                    {
                        hint.SetActive(false);
                    }
                    is_event_on = true;
                    is_triggered = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (is_hint_needed)
        {
            hint.SetActive(false);
        }
    }

    void Start()
    {
        
        
    }

    void Update()
    {
        if(is_dialog_on)
        {
            if (is_auto_play)
            {
                StartCoroutine(auto_play());
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                if(dialog_index == dialog.Count)
                {
                    if(is_event_trigger)
                    {
                        event_on();
                    }
                    Player.GetComponent<Player>().enabled = true;
                    dialog_text.SetActive(false);
                    if(is_trigger_reuseful)
                    {
                        StartCoroutine(trigger_reset());
                        is_dialog_on = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else if(dialog_index < dialog.Count)
                {
                    //播放对话音效
                    dialog_text.transform.localPosition = Camera.main.WorldToScreenPoint(dialog_point[dialog[dialog_index].num].position) - new Vector3(Screen.width, Screen.height) / 2;
                    dialog_text.GetComponent<TextMeshProUGUI>().text = dialog[dialog_index++].line;
                }
            } 
        }
    }
}
