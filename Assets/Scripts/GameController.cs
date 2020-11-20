using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MintAnimation;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] Frags;
    public GameObject frag_system;
    bool is_fragSystem_on = false;
    public int event_num = -1;
    int title_hint_value = 0;//用于表示当前title hint的使用情况
    public float hint_title_show_time;//用于表示hint展示时间
    public GameObject hint_title;
    public GameObject Player;
    Queue<int> frag_join_queue=new Queue<int>();

    public GameObject pause_menu;
    bool is_pause_menu_on = false;
    public GameObject[] Buttons;
    int button_value=0;

    public AK.Wwise.State NotInUI;
    public AK.Wwise.State InUI;
    public AK.Wwise.Event UISelectionEvent;
    public AK.Wwise.Event UIConfirmEvnet;
    public void frag_join(int frag_num)
    {
        frag_join_queue.Enqueue(frag_num);
        StartCoroutine(hint_title_show(1,frag_num,""));
    }

    IEnumerator hint_title_show(int info_type,int frag_num,string info)
    {
        //播放提示弹出音效
        switch(info_type)
        {
            case 1://碎片加入信息显示
                info = "关键词 " + "<color=#52e7ff><size=60>" + Frags[frag_num].GetComponent<Frag>().frag_name + "</color></size> 已加入思维云图";
                break;
            default://其他信息显示
                break;
        }
        hint_title.SetActive(true);
        info += "\n";
        hint_title.GetComponent<TextMeshProUGUI>().text += info;
        yield return new WaitForSeconds(hint_title_show_time);
        hint_title.GetComponent<TextMeshProUGUI>().text= hint_title.GetComponent<TextMeshProUGUI>().text.Remove(0,info.Length);
    }

    public void title_hint_show(int info_type, int frag_num,string info)
    {
        StartCoroutine(hint_title_show(info_type, frag_num, info));
    }

    IEnumerator frag_join_wait()
    {
        yield return new WaitForSeconds(0.1f);
        for (;frag_join_queue.Count>0; )
        {
            frag_system.GetComponent<FragSystem>().frag_join(frag_join_queue.Dequeue());
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void fragSystem_control()
    {
        is_fragSystem_on = !is_fragSystem_on;
        if (is_fragSystem_on)
            InUI.SetValue();
        else
        {
            frag_system.GetComponent<FragSystem>().OnCenterEvent.Stop(Player);
            NotInUI.SetValue();
        }

        frag_system.SetActive(is_fragSystem_on);
        Player.GetComponent<Player>().enabled = !is_fragSystem_on;
        if (is_fragSystem_on)
        {
            Player.GetComponent<Player>().sound.stopWalkSound();
            StartCoroutine(frag_join_wait());
        }
    }
    public void pauseMenu_control()
    {
        is_pause_menu_on = !is_pause_menu_on;
        if (is_pause_menu_on)
            InUI.SetValue();
        else
            NotInUI.SetValue();
        pause_menu.SetActive(is_pause_menu_on);
        if(is_pause_menu_on)
        {
            Player.GetComponent<Player>().sound.stopWalkSound();
        }
        Player.GetComponent<Player>().enabled = !is_pause_menu_on;
    }
    IEnumerator mainMenu_return()
    {
        /*AsyncOperation async = SceneManager.LoadSceneAsync("MainMenu");
        async.allowSceneActivation = false;
        //加载过场动画（简单一点,暂定）
        yield return new WaitForSeconds(1.0f);
        while (!async.isDone)
        {
            if (async.progress > 0.9f)
            {
                break;
            }
        }
        async.allowSceneActivation = true;
        yield return null;*/
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }
    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Cursor.visible = false;
        frag_system.SetActive(is_fragSystem_on);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)&&!is_pause_menu_on)
        {
            fragSystem_control();
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !is_fragSystem_on)
        {
            pauseMenu_control();
        }
        if(is_pause_menu_on)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                //播放按钮切换的音效
                UISelectionEvent.Post(Player);
                Buttons[button_value].GetComponent<CanvasGroup>().alpha = 0.2f;
                Buttons[button_value].GetComponent<MintAnimation_Position>().Stop();
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    button_value++;
                }
                else
                {
                    button_value += Buttons.Length - 1;
                }
                button_value %= Buttons.Length;
                Buttons[button_value].GetComponent<CanvasGroup>().alpha = 1.0f;
                Buttons[button_value].GetComponent<MintAnimation_Position>().Play();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                UIConfirmEvnet.Post(Player);
                switch (button_value)
                {
                    case 0:
                        pauseMenu_control();
                        break;
                    case 1:
                        StartCoroutine(mainMenu_return());
                        NotInUI.SetValue();
                        break;
                }
            }
        }
    }
}
