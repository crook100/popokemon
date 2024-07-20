using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float destroy_time;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyNow", destroy_time);
    }

    void DestroyNow()
    {
        Destroy(gameObject);
    }
}
