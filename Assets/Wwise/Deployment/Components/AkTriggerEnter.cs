#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
using System.Diagnostics;

public class AkTriggerEnter : AkTriggerBase
{
	public UnityEngine.GameObject triggerObject = null;

	private void OnTriggerEnter(UnityEngine.Collider in_other)
	{
		if (triggerDelegate != null && (triggerObject == null || triggerObject == in_other.gameObject))
			triggerDelegate(in_other.gameObject);
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.