using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 pos;
    public float speed = 30f;
    public float borderThickness = 10f;
    public float scrollSpeed = 5f;
    public float minY = 7f;
    public float maxY = 14f;
    public float minZ = -13f;
    public float maxZ = 2f;
    public float minX = -17f;
    public float maxX = 11f;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            if (touchDeltaPosition.x < 0 && Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);             
            }
            else if (touchDeltaPosition.x > 0 && Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
            }
            else if (touchDeltaPosition.y < 0 && Mathf.Abs(touchDeltaPosition.y) > Mathf.Abs(touchDeltaPosition.x))
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, minZ, maxZ));

            }
            else if (touchDeltaPosition.y > 0 && Mathf.Abs(touchDeltaPosition.y) > Mathf.Abs(touchDeltaPosition.x))
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, minZ, maxZ));
            }
        }

        if(Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;
            Vector2 touch2Prev = touch2.position - touch2.deltaPosition;

            float prevMagnitude = (touch1Prev - touch2Prev).magnitude;
            float currentMagnitude = (touch1.position - touch2.position).magnitude;
            float diff = currentMagnitude - prevMagnitude;

            pos = transform.position;
            pos.y -= diff * scrollSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }
    }
}
