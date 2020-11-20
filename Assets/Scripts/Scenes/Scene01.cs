using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scene01 : MonoBehaviour
{
    [TextArea]
    public string[] openning_text;
    public GameObject openning_Text;
    public GameObject openning_bg;
    public GameObject openning_bg2;
    public GameObject game_scene;
    public GameObject openning;
    public GameObject CP_start;
    public GameObject bridge_block;
    public GameObject bridge_well;
    public GameObject bridge_broken;
    public float text_speed = 2f;
    int bridge_recover_value = 0;//用于标识修复桥的所需关键词的获取进度
    // Start is called before the first frame update
    void bridge_recover()
    {
        GetComponent<GameController>().fragSystem_control();
        bridge_broken.SetActive(false);
        bridge_well.SetActive(true);
    }
    IEnumerator openning_start()
    {
        yield return new WaitForSeconds(1.0f);
        for (int i=0;i<openning_text.Length;i++)
        {
            openning_Text.GetComponent<TextMeshProUGUI>().text += openning_text[i]+"\n";
            if(i % 2f == 0f)
                yield return new WaitForSeconds(text_speed + 1f);
            else
                yield return new WaitForSeconds(text_speed);
        }
        openning_Text.SetActive(false);
        openning_bg.SetActive(false);
        openning_bg2.SetActive(true);
        openning_bg2.GetComponent<MintAnimation_Color>().Play();
        yield return new WaitForSeconds(1f);
        game_scene.SetActive(true);
        openning_bg2.GetComponent<MintAnimation_CanvasAlpha>().Play();
        yield return new WaitForSeconds(1f);
        openning.SetActive(false);
        CP_start.SetActive(true);
        //GetComponent<GameController>().title_hint_show(-1, 0, "按 <color=#00ff00><size=60>Q</size></color> 键继续对话");
    }
    IEnumerator event_0()
    {
        GetComponent<GameController>().frag_join(0);
        GetComponent<GameController>().frag_join(10);
        GetComponent<GameController>().frag_join(18);
        yield return new WaitForSeconds(2f);
        GetComponent<GameController>().title_hint_show(-1, 0, "按 <color=#00ff00><size=60>TAB</size></color> 打开思维云图");
    }
    void Start()
    {
        StartCoroutine(openning_start());
    }

    // Update is called once per frame
    void Update()
    {
        switch(GetComponent<GameController>().event_num)
        {
            case 0:
                GetComponent<GameController>().event_num = -1;
                StartCoroutine(event_0());
                break;
            case 1:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(1);
                bridge_recover_value++;
                break;
            case 2:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(2);
                break;
            case 3:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(3);
                bridge_recover_value++;
                break;
            case 4:
                GetComponent<GameController>().event_num = -1;
                bridge_block.SetActive(false);
                bridge_recover();
                break;
            case 5:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(16);
                break;
            default:
                break;
        }
        if(bridge_recover_value==2)//当玩家获得废墟和桥两个关键词后进行操作提示
        {
            GetComponent<GameController>().title_hint_show(-1, -1, "试着将获得的关键词加入思考，也许想到什么");
            bridge_recover_value = -1;
        }
    }
}
