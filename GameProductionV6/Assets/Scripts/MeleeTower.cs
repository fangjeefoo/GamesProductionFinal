using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    //Static member
    public static int price;
    public static int upgradePrice;
    public static int skillPrice;

    //Public member
    public float radius = 2.0f;
    public float fireRate = 5f;
    [HideInInspector]
    public float currentFireRate;
    public GameObject rocket;
    public int damage = 50;
    public int upgradeDamage = 100;
    public float upgradeRange = 0.5f;
    [Tooltip("Percent of base damage")]
    public int fireDamage = 10;
    [Tooltip("Percent of base speed")]
    public int iceSlow = 50;
    public bool ice = false;
    public bool fire = false;
    public bool damageUp = false;
    public bool rangeUp = false;

    //Private member
    private Vector3 look;
    private float fireTime;
    private GameObject target;
    private AudioSource soundEffect;

    void Awake()
    {
        price = 100;
        fireTime = 0.0f;
        currentFireRate = fireRate;
        upgradePrice = 200;
        skillPrice = 200;
        soundEffect = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTarget();
        if (target)
        {
            look = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);    
            transform.GetChild(1).LookAt(look);

            if (Time.time >= fireTime)
                Fire();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Fire()
    {     
        fireTime = Time.time + currentFireRate;
        GameManager.gm.rocketList.Add(Instantiate(rocket, transform));
        GameManager.gm.rocketList[GameManager.gm.rocketList.Count - 1].GetComponent<Rocket>().target = target;
        GameManager.gm.rocketList[GameManager.gm.rocketList.Count - 1].GetComponent<Rocket>().look = look;
        soundEffect.Play();
    }

    void CheckTarget()
    {
        float shortestDistance = Mathf.Infinity;
        float distance;
        GameObject temp = null;

        for (int i = 0; i < GameManager.gm.enemyList.Count; i++)
        {
            if (GameManager.gm.enemyList[i])
            {
                distance = Vector3.Distance(GameManager.gm.enemyList[i].transform.position, transform.position);

                if (distance < shortestDistance)
                {
                    temp = GameManager.gm.enemyList[i];
                    shortestDistance = distance;
                }
            }
        }

        if (temp != null && shortestDistance <= radius)
            target = temp;
        else
            target = null;
    }
}
