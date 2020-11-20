using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotate_speed;//每旋转一圈需要的s数
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -rotate_speed * 360 * Time.deltaTime /(3.14f * 2)));
    }
}
