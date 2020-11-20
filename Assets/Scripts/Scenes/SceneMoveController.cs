using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMoveController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform background;
    public Transform middleground;
    public Transform Camera;
    public float bg_move_speed;//0-1
    public float mg_move_speed;

    Vector3 velocity,pre_position;
    void Start()
    {
        pre_position = Camera.position;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Camera.position - pre_position;
        background.position += new Vector3(velocity.x * bg_move_speed, 0, 0);
        middleground.position += new Vector3(velocity.x * mg_move_speed, 0, 0);
        pre_position = Camera.position;
    }
}
