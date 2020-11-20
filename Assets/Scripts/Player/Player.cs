using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Start is called before the first frame update

	public float move_speed;
	public float jump_speed;
	Rigidbody2D rg_player;
	public GameObject[] cheack_point;//0:地板检测  1：墙面检测

	public PlayerSound sound;
	public bool isMoving = false;

	void Move(float walk)
	{
		if (Mathf.Abs(walk) > 0)
		{
			if (!Physics2D.OverlapCircle(cheack_point[1].transform.position, 0.01f, LayerMask.GetMask("Ground"))||walk*(transform.localEulerAngles.y-1)>0)
			{
				if (walk > 0)
					transform.localEulerAngles = new Vector3(0, 0, 0);
				else if (walk < 0)
					transform.localEulerAngles = new Vector3(0, 180, 0);
				if (Mathf.Abs(walk) < 0.5 * move_speed)
				{
					transform.position += new Vector3(walk / Mathf.Abs(walk) * move_speed * 0.5f, 0) * Time.deltaTime;
				}
				else
				{
					transform.position += new Vector3(walk, 0) * Time.deltaTime;
				}
			}
			if(!isMoving)
			{
				isMoving = true;
				sound.startWalkSound();
			}
			sound.wheelSoundSwitch();
		}
		else
		{
			if (isMoving)
			{
				isMoving = false;
				sound.stopWalkSound();
			}
		}
	}

	void Jump()
	{
		if (Physics2D.OverlapCircle(cheack_point[0].transform.position, 0.01f, LayerMask.GetMask("Ground")))
		{
			rg_player.velocity += new Vector2(0, jump_speed);
		}
	}
	void Start()
	{
		rg_player = GetComponent<Rigidbody2D>();
		sound = GetComponent<PlayerSound>();
	}

	// Update is called once per frame
	void Update()
	{
		GetComponent<Animator>().SetBool("is_walk", isMoving);
		Move(Input.GetAxis("Horizontal")*move_speed);
        /*if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}*/
    }
}
