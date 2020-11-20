using JetBrains.Annotations;
using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FragSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject game_controller;
    public GameObject Player;

    public GameObject[] frag_point;
    public Transform frag_parent;//Frag的父目录
    public int[] frag_initial;//初始的frag
    GameObject[] frag_index;
    GameObject[] Frags;
    int[] frag_selected;//被选中参与联想的碎片的编号集

    Frag frag_now;
    int frag_now_index=0;

    public GameObject control_hint;

    Collider2D hit;

    public GameObject info_area;//用于显示碎片携带的信息文本
    bool is_info_on = false;//用于判断是否正在阅读文本
    int info_index=0;//用于表示当前文本的进度

    public GameObject center;
    bool is_center=false;
    int target_frag_num=-1;

    float dilate=1;
    int dilate_control=-1;
    bool is_highlighting = false;

    public AK.Wwise.Event OnCenterEvent;
    public AK.Wwise.Event OnHoverEvent;
    public AK.Wwise.Event ConfirmEvent;
    public AK.Wwise.Event ResultEvent;

    public bool frag_join(int frag_num)//将新的碎片加入到思维云图中
    {

        for (int i=0;i<frag_index.Length;i++)
        {
            hit = Physics2D.OverlapCircle(frag_index[i].transform.position, 0.1f);
            if(hit && hit.tag=="Frag"&&hit.GetComponent<Frag>().frag_num==frag_num)
            {
                return false;
            }
            else if(!hit)
            {
                Instantiate<GameObject>(Frags[frag_num], frag_parent).transform.localPosition = frag_index[i].transform.localPosition;
                return true;
            }
        }
        return false;
    }
    void highlight()
    {
        if(is_center)
        {
            Debug.Log(center.GetComponentsInChildren<MintAnimation_Rotation>().Length);
            for(int i=0;i<center.GetComponentsInChildren<MintAnimation_Rotation>().Length;i++)
            {
                center.GetComponentsInChildren<MintAnimation_Rotation>()[i].Play();
                control_hint.transform.localPosition = center.transform.localPosition + new Vector3(0, -200);
                control_hint.GetComponent<TextMeshProUGUI>().text = "<color=#52e7ff><size=50>Q</size></color>:联想";
            }
        }
        else
        {
            is_highlighting = true;
            dilate = 0.9f;
            dilate_control = -1;
            if(hit)
            {
                hit.GetComponent<TextMeshProUGUI>().fontMaterial.SetFloat("_UnderlayDilate", dilate);
                control_hint.transform.localPosition = hit.transform.localPosition + new Vector3(0, -50);
                control_hint.GetComponent<TextMeshProUGUI>().text = "<color=#52e7ff><size=50>Q</size></color>:思考  <color=#52e7ff><size=50>E</size></color>:联想";
            }
        }
    }
    void highlight_cancel()
    {
        if (is_center)
        {
            for (int i = 0; i < center.GetComponentsInChildren<MintAnimation_Rotation>().Length; i++)
            {
                center.GetComponentsInChildren<MintAnimation_Rotation>()[i].Stop();
            }
        }
        else
        {
            is_highlighting = false;
            dilate = -0.9f;
            dilate_control = 1;
            if (hit)
            {
                hit.GetComponent<TextMeshProUGUI>().fontMaterial.SetFloat("_UnderlayDilate", dilate);
            }
        }
    }

    void frag_replace()
    {
        for(int i=0;i<frag_index.Length;i++)
        {
            hit = Physics2D.OverlapCircle(frag_index[i].transform.position, 0.1f);
            if (hit && hit.tag == "Frag"&&hit.GetComponent<Frag>().frag_num== Frags[target_frag_num].GetComponent<Frag>().replace_frag_num)
            {
                Destroy(hit.gameObject);
                Instantiate<GameObject>(Frags[target_frag_num], frag_parent).transform.localPosition = frag_index[i].transform.localPosition;
                break;
            }
        }
    }
    
    void frag_get(int sign)
    {
        highlight_cancel();
        for (int i = 0; i < frag_point.Length; i++)
        {
            frag_now_index += sign + frag_point.Length;
            frag_now_index %= frag_point.Length;
            hit = Physics2D.OverlapCircle(frag_point[frag_now_index].transform.position, 0.1f);
            if (hit && hit.tag == "Frag")
            {
                frag_now = hit.GetComponent<Frag>();
                break;
            }
        }
        highlight();
    }
    void thinking()//对单个碎片进行思考
    {
        if (info_index < frag_now.frag_info.Length)
        {
            //播放对话音效
            info_area.GetComponent<TextMeshProUGUI>().text = frag_now.frag_info[info_index++];
        }
        else
        {
            info_area.GetComponent<TextMeshProUGUI>().text = "";
            is_info_on = false;
            info_index = 0;
            if(Frags[frag_now.frag_num].GetComponent<Frag>().is_event_trigger&& !frag_now.is_triggered)
            {
                game_controller.GetComponent<GameController>().event_num = frag_now.event_num;
                frag_now.is_triggered = true;
            }
        }
    }
    int association()//将多个碎片合成进行联想
    {
        int frag_max=1;
        int frag_needed_num=1;
        Frag selected_frag,target_frag;
        for(int i=0;i<frag_point.Length;i++)
        {
            if(frag_selected[i]!=-1)
            {
                selected_frag = Frags[frag_selected[i]].GetComponent<Frag>();
                for (int j=0;j< selected_frag.target_frag_num.Length; j++)
                {
                    target_frag= Frags[selected_frag.target_frag_num[j]].GetComponent<Frag>();
                    for (int k=0;k< target_frag.frag_needed.Length;k++)
                    {
                        for (int l=i+1;l< frag_point.Length;l++)
                        {
                            if (frag_selected[l] >= 0)
                            {
                                if (target_frag.frag_needed[k] == frag_selected[l])
                                {
                                    frag_needed_num++;
                                }
                                else
                                {
                                    frag_needed_num = 1;
                                    break;
                                }
                            }
                        }
                        if (frag_needed_num == target_frag.frag_needed.Length)
                        {
                            for(int l=0;l< frag_index.Length;l++)
                            {
                                hit = Physics2D.OverlapCircle(frag_index[0].transform.position, 0.1f);
                                if (hit && hit.tag == "Frag")
                                {
                                    if(hit.GetComponent<Frag>().frag_num== target_frag.frag_num)
                                    {
                                        return 3;//已经联想过了
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            target_frag_num = target_frag.frag_num;
                            return 0;//成功进行联想
                        }
                        frag_max = frag_needed_num > frag_max ? frag_needed_num : frag_max;
                        frag_needed_num = 1;
                    }
                }
                if (frag_needed_num > 1)
                {
                    return 1;//还差一点进行联想
                }
                else
                {
                    return 2;//不构成联想
                }
            }
        }
        return 4;//不存在的情况
    }
    void frag_point_initializition()
    {
        float theta = 3.14f;
        float delta = theta / (frag_point.Length - 1);
        for(int i=0;i<frag_point.Length;i++)
        {
            frag_point[i].transform.localPosition = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta))*800-new Vector3(0,400);
            theta -= delta;
        }
    }
    void frag_index_initializition()
    {
        frag_index = new GameObject[frag_point.Length];
        int mid = (int)(frag_point.Length / 2);
        frag_index[0] = frag_point[mid];
        frag_now_index = mid;
        for (int i = 1, j = mid + 1, k = mid - 1; j < frag_point.Length || k >= 0;)
        {
            if (k >= 0)
            {
                frag_index[i++] = frag_point[k--];
            }
            if (j < frag_point.Length)
            {
                frag_index[i++] = frag_point[j++];
            }
        }
    }
    IEnumerator frag_initializition()
    {
        for (int i = 0; i < frag_initial.Length; i++)
        {
            Instantiate<GameObject>(Frags[frag_initial[i]], frag_parent).transform.localPosition = frag_index[i].transform.localPosition;
        }
        yield return new WaitForSeconds(0.2f);//Instantiate应该是另开线程执行，会有延迟
        hit = Physics2D.OverlapCircle(frag_index[0].transform.position, 0.1f);
        if (hit)
        {
            frag_get(1);
        }
    }
    void frag_selected_initializition()
    {
        Collider2D hit1;
        for (int i = 0; i < frag_selected.Length; i++)
        {
            if(frag_selected[i]!=-1)
            {
                hit1=Physics2D.OverlapCircle(frag_point[i].transform.position, 0.1f);
                if (hit1 && hit1.tag == "Frag")
                {
                    hit1.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }
            frag_selected[i]=-1;
        }
    }
    void initializition()
    {
        Frags = game_controller.GetComponent<GameController>().Frags;
        frag_selected = new int[frag_point.Length];
        frag_point_initializition();
        frag_index_initializition();
        StartCoroutine(frag_initializition());
        frag_selected_initializition();
    }
    
    
    void Start()
    {
        initializition();
    }
    
    void Update()
    {
        if(is_info_on)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                thinking();
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.UpArrow))
            {
                //播放碎片和中心的切换音效
                OnHoverEvent.Post(Player);
                highlight_cancel();
                is_center = !is_center;
                if (is_center)
                    OnCenterEvent.Post(Player);
                else
                    OnCenterEvent.Stop(Player);
                highlight();
            }
            if(is_center)//当中心被选中时
            {
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    //播放中心点击音效
                    ResultEvent.Post(Player);
                    switch (association())
                    {
                        case 0:
                            if (Frags[target_frag_num].GetComponent<Frag>().replace_frag_num!=-1)
                            {
                                frag_replace();
                                info_area.GetComponent<TextMeshProUGUI>().text = "似乎想到了什么";
                            }
                            else
                            {
                                frag_join(target_frag_num);
                                info_area.GetComponent<TextMeshProUGUI>().text = "似乎想到了什么";
                            }
                            target_frag_num = -1;
                            break;
                        case 1:
                            info_area.GetComponent<TextMeshProUGUI>().text = "感觉还差了些什么";
                            break;
                        case 2:
                            info_area.GetComponent<TextMeshProUGUI>().text = "感觉没有什么联系";
                            break;
                        case 3:
                            info_area.GetComponent<TextMeshProUGUI>().text = "这个之前想到过了";
                            break;
                        default:
                            break;
                    }
                    frag_selected_initializition();
                }
            }
            else//当选中为碎片时
            {
                if(Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //播放碎片切换音效
                    frag_get(1);
                    OnHoverEvent.Post(Player);
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    //播放碎片切换音效
                    frag_get(-1);
                    OnHoverEvent.Post(Player);
                }
                if(is_highlighting)
                {
                    if(dilate<-1||dilate>1)
                    {
                        dilate_control *= -1;
                    }
                    dilate += dilate_control * Time.deltaTime * 2;
                    if(hit)
                    {
                        hit.GetComponent<TextMeshProUGUI>().fontMaterial.SetFloat("_UnderlayDilate", dilate);
                    }
                }
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    //播放碎片选中音效
                    is_info_on=true;
                    info_area.GetComponent<TextMeshProUGUI>().text = frag_now.frag_info[info_index++];
                    ResultEvent.Post(Player);
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    //播放碎片选中音效
                    ConfirmEvent.Post(Player);
                    if (frag_selected[frag_now_index]!=-1)
                    {
                        frag_selected[frag_now_index] = -1;
                        frag_now.GetComponent<TextMeshProUGUI>().color = Color.white;
                    }
                    else
                    {
                        frag_selected[frag_now_index] = frag_now.frag_num;
                        frag_now.GetComponent<TextMeshProUGUI>().color = Color.red;
                    }
                }
            }
        }
    }
}
