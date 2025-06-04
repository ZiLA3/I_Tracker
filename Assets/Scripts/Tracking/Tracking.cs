using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : MonoBehaviour
{
    //public UDPManager udpReceive;
    //public GameObject[] handPoints;
    //[SerializeField] GameObject leftEye;
    //[SerializeField] GameObject rightEye;

    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    string data;

    //    data = data.Remove(0, 1);
    //    data = data.Remove(data.Length - 1, 1);
    //    print(data);
    //    string[] points = data.Split(',');
    //    print(points[0]);

    //    //0        1*3      2*3
    //    //x1,y1,z1,x2,y2,z2,x3,y3,z3

    //    // 21개는 포인트 개수
    //    for (int i = 0; i < 21; i++)
    //    {
    //        float handX = 7 - float.Parse(points[i * 3]) / 100;
    //        float handY = float.Parse(points[i * 3 + 1]) / 100;
    //        float handZ = float.Parse(points[i * 3 + 2]) / 100;

    //        handPoints[i].transform.localPosition = new Vector3(handX, handY, handZ);
    //    }

    //    int eyeStartIdx = 21 * 3;  // 63

    //    float leftEyeX = 7 - float.Parse(points[eyeStartIdx++]) / 100;
    //    float leftEyeY = float.Parse(points[eyeStartIdx++]) / 100;

    //    leftEye.transform.localPosition = new Vector3(leftEyeX, leftEyeY, 0);

    //    float rightEyeX = 7 - float.Parse(points[eyeStartIdx++]) / 100;
    //    float rightEyeY = float.Parse(points[eyeStartIdx++]) / 100;

    //    rightEye.transform.localPosition = new Vector3(rightEyeX, rightEyeY, 0);
    //}
}
