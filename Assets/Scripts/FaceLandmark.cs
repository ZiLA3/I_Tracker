using UnityEngine;
public static class FaceLandmark
{
    public static Vector2 EyePoint;
    public static readonly Vector3[] HandPoints = new Vector3[21];

    public static bool HasEyePoint => EyePoint != -Vector2.one;
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