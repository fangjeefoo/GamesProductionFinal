using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int healthPoint;
    [HideInInspector]
    public int door;
    public float speed;
    public GameObject particle;
    public GameObject money;
    public GameObject[] wayPoint1;
    public GameObject[] wayPoint2;

    //Private variable
    private int wayPointIndex;
    private bool slow;
    private float normalSpeed;
    private Animator animator;
    private Renderer render;
    private Color startColor;


    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Renderer>() != null)
            render = GetComponent<Renderer>();
        else if (GetComponentInChildren<Renderer>() != null)
            render = GetComponentInChildren<Renderer>();
        startColor = render.material.color;
        normalSpeed = speed;
        wayPointIndex = 0;
        slow = false;
        animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("Run Forward", true);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] wayPoint;
        if(door == 1)
            wayPoint = wayPoint1;
        else
            wayPoint = wayPoint2;

        Vector3 direction = wayPoint[wayPointIndex].transform.position - transform.position;
        transform.position += speed * direction.normalized * Time.deltaTime;        

        if (direction.magnitude <= 0.05f)
        {
            wayPointIndex++;
            if(wayPointIndex < wayPoint.Length)
            {
                direction = wayPoint[wayPointIndex].transform.position - transform.position;
                Rotation(direction);
            }
            else
            {
                GameManager.gm.state = "lose";
                money = null;
                DestroyObject();
            }           
        }             
    }

    public void Rotation(Vector3 direction)
    {
        if (direction.x > 1)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);           
        }            
        else if(direction.x < -1)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);          
        }            
        else if (direction.z > 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }            
        else if (direction.z < -1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }            
    }

    public void DestroyObject()
    {
        if (particle)
            Instantiate(particle, transform.position, Quaternion.Euler(0, 0, 0));
        if(money)
            Instantiate(money, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Rocket" && obj.gameObject.GetComponent<Rocket>().target == this.gameObject)
        {
            if (obj.transform.parent.tag == "Melee" && obj.transform.parent.GetComponent<MeleeTower>().fire == false)
                healthPoint -= obj.transform.parent.GetComponent<MeleeTower>().damage;
            else if (obj.transform.parent.tag == "Range" && obj.transform.parent.GetComponent<RangeTower>().fire == false)
                healthPoint -= obj.transform.parent.GetComponent<RangeTower>().damage;
            else if (obj.transform.parent.tag == "Melee" && obj.transform.parent.GetComponent<MeleeTower>().fire == true)
            {                
                healthPoint = healthPoint - obj.transform.parent.GetComponent<MeleeTower>().damage;
                healthPoint = healthPoint - (obj.transform.parent.GetComponent<MeleeTower>().damage * obj.transform.parent.GetComponent<MeleeTower>().fireDamage / 100);
            }                
            else if (obj.transform.parent.tag == "Range" && obj.transform.parent.GetComponent<RangeTower>().fire == true)
            {              
                healthPoint = healthPoint - obj.transform.parent.GetComponent<RangeTower>().damage;
                healthPoint = healthPoint  - (obj.transform.parent.GetComponent<RangeTower>().damage * obj.transform.parent.GetComponent<RangeTower>().fireDamage / 100);
            }

            if (obj.transform.parent.tag == "Melee" && obj.transform.parent.GetComponent<MeleeTower>().ice == true && slow == false)
                StartCoroutine(MeleeSlowDown(obj.gameObject));
            else if (obj.transform.parent.tag == "Range" && obj.transform.parent.GetComponent<RangeTower>().ice == true && slow == false)
                StartCoroutine(RangeSlowDown(obj.gameObject));

            for (int i = 0; i < GameManager.gm.rocketList.Count; i++)
            {
                if (GameObject.ReferenceEquals(obj.gameObject, GameManager.gm.rocketList[i]))
                {
                    GameManager.gm.rocketList.Remove(GameManager.gm.rocketList[i]); //need to remove manually because destroy gameobject will not
                    break;                                 //update the list     
                }
            }
            Destroy(obj.gameObject);
        }

        if (healthPoint <= 0)
        {
            for(int i = 0; i < GameManager.gm.enemyList.Count; i++)
            {
                if (GameObject.ReferenceEquals(this.gameObject, GameManager.gm.enemyList[i]))
                {
                    GameManager.gm.enemyList.Remove(GameManager.gm.enemyList[i]);
                    break;
                }
            }
            DestroyObject();
        }
    }

    IEnumerator MeleeSlowDown(GameObject obj)
    {    
        speed = speed * obj.transform.parent.GetComponent<MeleeTower>().iceSlow / 100;
        slow = true;
        yield return new WaitForSeconds(5f);
        speed = normalSpeed;
        slow = false;
    }

    IEnumerator RangeSlowDown(GameObject obj)
    {
        speed = speed * obj.transform.parent.GetComponent<RangeTower>().iceSlow / 100;
        slow = true;
        render.material.color = Color.blue;
        yield return new WaitForSeconds(5f);
        render.material.color = startColor;
        speed = normalSpeed;
        slow = false;
    }
}
