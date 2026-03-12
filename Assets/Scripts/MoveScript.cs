using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveScript : MonoBehaviour
{
    public float distance = 5f;
    public float speed = 5f;
    public float turn = 1f;
    public float angle = 30f;
    public Transform point;

    void Start()
    {
        
    }
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        ReactionToObstacles();

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Garbage")
        {
            collision.gameObject.SetActive(false);
        }
    }
    
    private void Turn(float turn)
    {
        transform.Rotate(Vector3.up * turn * Time.deltaTime);
    }
    private void ReactionToObstacles()
    {
        Ray rayR = new Ray(point.position, Quaternion.Euler(0, angle, 0) * point.right);
        Ray rayL = new Ray(point.position, Quaternion.Euler(0, -angle, 0) * point.right);

        RaycastHit hit;

        if (Physics.Raycast(rayR, out hit, distance))
        {
            string objectTag = hit.collider.tag;
            if (objectTag != "Garbage" && hit.distance < 2f)
            {
                transform.Translate(Vector3.right * -1 * speed * 4 * Time.deltaTime);
            }
        }

        if (Physics.Raycast(rayR, out hit, distance) && Physics.Raycast(rayL, out hit, distance))
        {
            string objectTag = hit.collider.tag;
            if (objectTag != "Garbage")
            {
                //микро поворот
                Turn(turn);
            }
        }
        else if (Physics.Raycast(rayR, out hit, distance))
        {
            string objectTag = hit.collider.tag;
            if (objectTag != "Garbage")
            {
                //микро поворот
                Turn(-turn);
            }
        }
        else if (Physics.Raycast(rayL, out hit, distance))
        {
            string objectTag = hit.collider.tag;
            if (objectTag != "Garbage")
            {
                //микро поворот
                Turn(turn);
            }
        }

        Debug.DrawRay(point.position, Quaternion.Euler(0, angle, 0) * point.right * distance, Color.red);
        Debug.DrawRay(point.position, Quaternion.Euler(0, -angle, 0) * point.right * distance, Color.red);
    }
}
