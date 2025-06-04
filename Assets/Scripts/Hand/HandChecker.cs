using System;
using UnityEngine;
using System.Linq;

public class HandChecker : MonoBehaviour
{
    [SerializeField] HandCheck handCheck;
    
    private static readonly bool[][] FoldStyle = {
        new[]{ true, true, true, true }, // rock, catch on
        new[]{ true, true, false, false }, // scissors
        new[]{ false, false, false, false } // paper, catch off
    };

    public enum style
    {
        Rock = 15, // 1111
        Scissors = 3, // 1100
        Paper = 0 // 0000 
    }

    private void Update()
    {
        if(handCheck.handTrackingOn)
            return;

        //Debug.Log($"{FaceLandmark.HandFolds[0]}, {FaceLandmark.HandFolds[1]}, {FaceLandmark.HandFolds[2]}, {FaceLandmark.HandFolds[3]}");
        //var i = 0;
        //for (i = 0; i < 3; i++)
        //{
        //    var handFolds = FaceLandmark.HandFolds;
        //    if (handFolds == style.Rock)
        //        break;
        //}

        //switch (handCheck.HandType)
        //{
        //    case HandActionType.None:
        //        break;
        //    case HandActionType.RSP:
        //        handCheck.SetInputRSPType(i);
        //        break;
        //    case HandActionType.PullDown:
        //        if(i == 0)
        //            handCheck.SetLeverPulldown();
        //        break;
        //    case HandActionType.Catch:
        //        if (i == 0)
        //            handCheck.SetCatch();
        //        break;
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}
        
    }
    
}
