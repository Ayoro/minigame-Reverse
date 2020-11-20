using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene02 : MonoBehaviour
{

    public GameObject exit;
    public GameObject right_boundray;
    public GameObject Openning;
    void Start()
    {
        Openning.GetComponent<MintAnimation_CanvasAlpha>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GetComponent<GameController>().event_num)
        {
            case 0:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(5);
                break;
            case 1:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(6);
                GetComponent<GameController>().frag_join(7);
                GetComponent<GameController>().frag_join(12);
                break;
            case 2:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(8);
                break;
            case 3:
                GetComponent<GameController>().event_num = -1;
                GetComponent<GameController>().frag_join(9);
                GetComponent<GameController>().frag_join(10);
                GetComponent<GameController>().frag_join(11);
                GetComponent<GameController>().frag_join(12);
                break;
            case 4:
                GetComponent<GameController>().event_num = -1;
                exit.SetActive(true);
                right_boundray.SetActive(false);
                break;
            case 5:
                GetComponent<GameController>().event_num = -1;
                exit.SetActive(true);
                break;
            default:
                break;
        }
    }
}
