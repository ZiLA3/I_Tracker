using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public HandCheck Hand { get; private set; }
    public EyeCheck Eye { get; private set; }
    public MissionManager Mission { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Hand = GetComponent<HandCheck>();
            Eye = GetComponent<EyeCheck>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
