using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitialization : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] player_point;
    public string scene_name;
    public GameObject Player;
    void Start()
    {
        Player.transform.position = player_point[PlayerPrefs.GetInt(scene_name, 0)].position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
