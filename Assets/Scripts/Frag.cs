using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Frag : MonoBehaviour
{
    public string frag_name;
    public int frag_num;
    [TextArea]
    public string[] frag_info;//碎片携带信息文本
    public int[] target_frag_num;//能够合成的碎片编号集
    public int[] frag_needed;//合成需要的碎片编号集
    public bool is_event_trigger = false;
    public int event_num = -1;
    public int replace_frag_num;//生成的新碎片将替代的碎片编号，-1为无
    public bool is_triggered = false;

    private void Start()
    {
        if(frag_num==0)
        {
            frag_name = PlayerPrefs.GetString("Code", "C-405");
            GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Code", "C-405");
        }
    }
}
