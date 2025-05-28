using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrackerTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            MissionManager missionManager = MissionManager.Instance;

            missionManager.PopMissionKey();
        }
    }
}
