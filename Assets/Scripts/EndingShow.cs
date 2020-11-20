using MintAnimation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingShow : MonoBehaviour
{
    [TextArea]
    public string[] Ending_text;
    public GameObject Ending_tmpro;
    public float text_speed;

    public AK.Wwise.Event ThemeMusicEvent;
    public AK.Wwise.Event ThemeMusicReleaseEvent;
    public GameObject WwiseObject;
    IEnumerator ending()
    {
        yield return new WaitForSeconds(2f);
        for(int i=0;i<Ending_text.Length;i++)
        {
            Ending_tmpro.GetComponent<TextMeshProUGUI>().text = Ending_text[i];
            Ending_tmpro.GetComponent<MintAnimation_CanvasAlpha>().Play();
            if (i == Ending_text.Length - 1)
                ThemeMusicReleaseEvent.Post(WwiseObject);
            yield return new WaitForSeconds(text_speed);
            Ending_tmpro.GetComponent<MintAnimation_CanvasAlpha>().Stop();
        }
        ThemeMusicEvent.Stop(WwiseObject);
        SceneManager.LoadScene("MainMenu");
    }
    void Start()
    {
        ThemeMusicEvent.Post(WwiseObject);
        StartCoroutine(ending());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
