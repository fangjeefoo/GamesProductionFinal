using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    void OnMouseDown()
    {
        if (this.tag == "Coin")
        {
            GameManager.gm.CollectMoney();
            Destroy(this.gameObject);
        }
        else
        {
            int type = Random.Range(0, 2);
            if(type == 0)
            {
                GameManager.gm.Interest();
                GameManager.gm.RefreshSkill();
                Destroy(this.gameObject);
            }
            else if(type == 1)
            {
                GameManager.gm.SpeedUp();
                Destroy(this.gameObject);
                GameManager.gm.RefreshSkill();
            }
        }
    }
}
