using UnityEngine;

public class SendUI : MonoBehaviour
{
    public void OnButtonClick()
    {
        UdpManager.Instance.Send("0");
    }
}
