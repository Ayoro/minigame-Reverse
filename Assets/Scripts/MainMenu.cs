using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject icon;
    public GameObject main_menu;
    public string scene_name;
    int button_value=0;//用于表示当前选中的button
    public GameObject[] Buttons;
    bool is_start = false;

    public AK.Wwise.Event UISelectionEvent;
    public AK.Wwise.Event UIConfirmEvent;
    public GameObject MainCameraObject;


    IEnumerator SceneLoad()//用于加载下一个场景
    {
        //AsyncOperation async = SceneManager.LoadSceneAsync(scene_name);
        SceneManager.LoadScene(scene_name);
        //async.allowSceneActivation = false;
        PlayerPrefs.SetInt(scene_name, 0);
        //加载过场动画（简单一点,暂定）
        /*yield return new WaitForSeconds(1.0f);
        while (!async.isDone)
        {
            if (async.progress > 0.9f)
            {
                break;
            }
        }
        async.allowSceneActivation = true;*/
        yield return null;
    }

    public void game_start()
    {
        StartCoroutine(SceneLoad());
    }
    public void game_exit()
    {
        Application.Quit();
    }
    IEnumerator openning_show()
    {
        icon.GetComponent<MintAnimation_CanvasAlpha>().Play();
        yield return new WaitForSeconds(5f);
        main_menu.SetActive(true);
        yield return new WaitForSeconds(1f);
        is_start = true;
    }
    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        StartCoroutine(openning_show());
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(is_start)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                //播放按键切换音效
                UISelectionEvent.Post(MainCameraObject);
                Buttons[button_value].GetComponent<CanvasGroup>().alpha=0.2f;
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
                UIConfirmEvent.Post(MainCameraObject);
                switch (button_value)
                {
                    case 0:
                        game_start();
                        break;
                    case 1:
                        game_exit();
                        break;
                }
            }
        }
    }
}
