using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDestroy : MonoBehaviour
{
    public float time;
    void Awake()
    {
        Invoke("DestroyObject", time);
    }

    void DestroyObject()
    {
        if(this.gameObject)
            Destroy(this.gameObject);
    }
}
