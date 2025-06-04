using UnityEngine;

public static class FaceLandmark
{
    public static Vector2 EyePoint;

    public static readonly Vector3[] HandPoints = new Vector3[21];
    public static int HandFolds = 0;

    public static bool HasEyePointLeft => EyePoint != -Vector2.one;
    public static bool HasHandPoint => HandPoints[0] != -Vector3.one;

    private static Vector2 StringToVector2(string s)
    {
        var sData = s.Split(',');
        return new Vector2(float.Parse(sData[0]), float.Parse(sData[1]));
    }

    private static Vector2 StringToVector3(string s)
    {
        var sData = s.Split(',');
        return new Vector3(float.Parse(sData[0]), float.Parse(sData[1]), float.Parse(sData[2]));
    }

    public static void DataProcessing(string data)
    {
        var rawData = data.Split("##"); //Raw Data [0];

        EyePoint = StringToVector2(rawData[0]);

        var i = 0;
        foreach (var handRawData in rawData[1].Split('/'))
        {
            HandPoints[i] = StringToVector3(handRawData);
            i++;
        }

        if (HasHandPoint)
        {
            HandFolds = 0; // Reset HandFolds before calculating // 0 : paper , rock = 15, sissor = 3
            var binaryarr = new int[] { 1, 2, 4, 8 };
            for (i = 0; i < 4; i++)
                HandFolds += binaryarr[i] * IsFingerFold(i);
        }
    }

    private static readonly int[,] FingerIndexes = { { 8, 6 }, { 12, 10 }, { 16, 14 }, { 20, 17 } };
    // 0 = 검지, 1 = 중지, 2 = 약지, 3 = 소지 

    private static float GetDistanceXY(Vector3 p1, Vector3 p2)
    {
        var distanceXY = (new Vector2(p1.x, p1.y) - new Vector2(p2.x, p2.y)).magnitude;
        return distanceXY;
    }

    private static int IsFingerFold(int fingerIndex)
    {
        var startTip = FingerIndexes[fingerIndex, 1];
        var startDis = GetDistanceXY(HandPoints[startTip], HandPoints[0]);

        var endTip = FingerIndexes[fingerIndex, 0];
        var endDis = GetDistanceXY(HandPoints[endTip], HandPoints[0]);

        return (startDis > endDis) ? 1 : 0;
    }
    
    public static string ToStr()
    {
        var str = $"Eye Point: {EyePoint}";
        for (var i = 0; i < 21; i++)
        {
            str += $"\nHand Point: {HandPoints[i]}";
        }

        return str;
    }
}