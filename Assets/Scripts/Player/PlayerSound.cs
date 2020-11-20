using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
	Player player;
	public AK.Wwise.Event MoveEvent;
	RaycastHit2D hit;

	public void wheelSoundSwitch()
	{
		hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, LayerMask.GetMask("Ground"));
		if (hit)
		{
			SoundMaterial sm = hit.collider.GetComponent<SoundMaterial>();
			if (sm != null)
				sm.material.SetValue(gameObject);
		}
	}

	public void startWalkSound()
	{
		MoveEvent.Post(gameObject);
	}

	public void stopWalkSound()
	{
		MoveEvent.Stop(gameObject);
	}

	// Start is called before the first frame update
	void Start()
	{
		player = GetComponent<Player>();
	}

	// Update is called once per frame
	//void Update()
	//{

	//}
}
