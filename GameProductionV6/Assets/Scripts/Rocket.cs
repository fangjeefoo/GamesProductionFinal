using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public Vector3 look;


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(look);
        if(target != null)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
        else
        {
            for(int i = 0; i < GameManager.gm.rocketList.Count; i++ )
            {
                if (GameObject.ReferenceEquals(this.gameObject, GameManager.gm.rocketList[i]))
                {
                    GameManager.gm.rocketList.Remove(GameManager.gm.rocketList[i]); //need to remove manually because destroy gameobject will not
                    break;                                 //update the list     
                }
            }
            Destroy(this.gameObject);
        }
    }
}
