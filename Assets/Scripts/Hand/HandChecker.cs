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

    private void Update()
    {
        Debug.Log($"{FaceLandmark.HandFolds[0]}, {FaceLandmark.HandFolds[1]}, {FaceLandmark.HandFolds[2]}, {FaceLandmark.HandFolds[3]}");
        var i = 0;
        for (i = 0; i < 3; i++)
        {
            var handFolds = FaceLandmark.HandFolds;
            if (FoldStyle[i].SequenceEqual(handFolds))
                break;
        }

        switch (handCheck.HandType)
        {
            case HandActionType.None:
                break;
            case HandActionType.RSP:
                handCheck.SetInputRSPType(i);
                break;
            case HandActionType.PullDown:
                handCheck.leverPullDown = i == 0;
                break;
            case HandActionType.Catch:
                handCheck.catchAction = i == 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
    
}
