using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //Static member
    public static bool buyState;

    //public member
    public bool available;
    public GameObject tower;
    public int vertices = 40;
    public float lineWidth = 0.05f;
           
    //private member
    private Renderer render;
    private static LineRenderer lineRenderer;
    private Color startColor;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        startColor = render.material.color;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        buyState = false;
        tower = null;
    }

    void OnMouseDown()
    {
        if (!available && !buyState)
        {
            Upgrade();
            SetupRange();
        }
        else if (available && buyState)
            Buy();
        else
            DisableRange();          
    }

    public void CheckAvailability()
    {
        if (available)
            render.material.color = Color.green;
        else
            render.material.color = Color.red;
    }

    public void Cancel()
    {
        render.material.color = startColor;
    }

    public void Buy()
    {
        available = false;
        tower = GameManager.gm.BuyTower(this.gameObject);
    }

    public void Upgrade()
    {
        GameManager.gm.UpgradeTower(this.gameObject);
    }

    public void SetupRange()
    {
        lineRenderer.enabled = true;
        lineRenderer.loop = true;

        float angle = (2f * Mathf.PI) / vertices;
        float theta = 0f;
        float radius = 0f;
        Vector3 pos;

        lineRenderer.positionCount = vertices;

        for(int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (tower.tag == "Melee")
                radius = tower.GetComponent<MeleeTower>().radius;
            else if (tower.tag == "Range")
                radius = tower.GetComponent<RangeTower>().radius;

            pos = new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));
            lineRenderer.SetPosition(i, transform.position + pos);
            theta += angle;
        }
    }

    public void DisableRange()
    {
        lineRenderer.enabled = false;
    }
}
