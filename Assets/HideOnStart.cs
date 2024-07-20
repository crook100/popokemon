using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    // Awake is called even before start
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
