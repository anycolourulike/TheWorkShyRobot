using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BiffoMover : MonoBehaviour
{
    Rigidbody rb;
    bool moveForward = false;
    bool moveBackward = false;
    bool moveRight = false;
    bool moveLeft = false;
    float horizontalMove;
    float verticalMove;
    public float speed = 300;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PointerDownForward()
    {       
        moveForward = true;        
    }

    public void PointerUpForward()
    {
        moveForward = false;
    }

    public void PointerDownBackward()
    {        
        moveBackward = true;        
    }

    public void PointerUpBackward()
    {
        moveBackward = false;
    }

    public void PointerDownRight()
    {
        moveRight = true;
    }

    public void PointerUpRight()
    {
        moveRight = false;
    }

    public void PointerDownLeft()
    {
        moveLeft = true;
    }

    public void PointerUpLeft()
    {
        moveLeft = false;
    }


    private void Movement()
    {
        if (moveLeft)
        {
            horizontalMove = -speed;
        }
        else if (moveRight)
        {
            horizontalMove = speed;
        }
        else
        {
            horizontalMove = 0;
        }

        if (moveForward)
        {
            verticalMove = speed;
        }
        else if (moveBackward)
        {
            verticalMove = -speed;
        }
        else
        {
            verticalMove = 0;
        }
    }

    private void Update()
    {
        Movement();
    }

    private void FixedUpdate()
    {        
        rb.velocity = new Vector3(horizontalMove * Time.deltaTime, rb.velocity.y,
                                  verticalMove * Time.deltaTime);
    }

}