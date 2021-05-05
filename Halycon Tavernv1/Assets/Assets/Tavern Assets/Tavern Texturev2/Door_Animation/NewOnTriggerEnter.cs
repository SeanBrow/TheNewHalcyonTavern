using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOnTriggerEnter : MonoBehaviour
{

    public Animator doorAnimation; 
    // Start is called before the first frame update
    void Start()
    {
        //door is idle- set by animator (no need to write any code in start) 
    }

    // Update is called once per frame. We don't use update here. OnTriggerStay is used a contextual subsitute for update. So only when the player stands in the right collider does the script run.
    void Update()
    {
        //this is inside update 
    }

    private void OnTriggerEnter(Collider other) //called on the frame that player ("other") enters the collider
    {
        print("entered"); 

       
    }
    private void OnTriggerStay(Collider other) //called when player stays inside the collider
    {
        //door stays open 
        print("stay");
                     
    //    if (Input.GetKey("space"))   
     //   {
            doorAnimation.SetBool("isOpen", true); //door will open 
            //if door open is true, then close door by making dooropen false. If door open is false, open the door. 
      //  }
    }
    private void OnTriggerExit(Collider other)  //called on the frame when player exits the collider 
    {
        doorAnimation.SetBool("isOpen", false); //the door will close 
        print("exited"); 
    }
}
