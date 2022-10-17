using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator Anim;
    [SerializeField] private int WalkSpeed, RotateSpeed;
    [SerializeField] private SceneController SceneController;
    [SerializeField] private Joystick _joystick;
    private Rigidbody rb;
    private bool Running = false;
    
    void Start() {
        rb = GetComponent<Rigidbody>();
        TurnScript();
    }
    public void TurnScript()
    {
        Running = false;
        Anim.SetBool("Running", Running);
    }

    void Update()
    {
        rb.MovePosition(transform.position + (transform.forward*_joystick.Vertical) * WalkSpeed * Time.deltaTime);
        if (_joystick.Vertical!=0) {Running = true;}
        else {Running = false;}
        
        Vector3 Rotate = new Vector3(0, 1, 0);
        if (_joystick.Vertical<0) {Rotate *= -1;}
        Quaternion Rotation = Quaternion.Euler(Rotate * _joystick.Horizontal * RotateSpeed * Time.deltaTime);
        rb.MoveRotation(rb.rotation * Rotation);
        Anim.SetBool("Running", Running);
    }


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "FinishLine"){
            SceneController.FinishGame();
        }
    }
}
