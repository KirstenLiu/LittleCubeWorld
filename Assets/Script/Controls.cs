using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TIP: DRAW A STATE MACHINE INSTEAD OF THESE SHITS:

//TIP: Unit checked. 1 unit = 1 meter in unity3d engine by default.
public enum VerticalMoveState 
{ 
    fallState, 
    stopState,
    doubleFallState, 
};

public class Controls : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float doubleFallSpeed = 3f;
    public new Transform transform;
    public new Rigidbody rigidbody;

    //Variables for horizental movements and controls
    public float controlsThreshold = 1f;
    public float controlsAcc = 0f;
    public float controlsSpeed = 2f;

    //Variables for vertical movements and falling
    public float startFallThreshold = 1f;
    public float stopFallThreshold = 2f;
    public float fallAcc = 0f;
    public float stopAcc = 0f;
    public VerticalMoveState verticalState = VerticalMoveState.stopState;
    
    //Variable of get input and giving up controls once y collision is detected
    public bool releaseControls = false;
    
    //Start is called before the first frame update
    void Start()
    {
        //TIP: always add component here otherwise you can't access it!
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
    }

    //Update is called once per frame
    //WARN: update is frame dependent, change distance with speed/delta time instead of depending on controls
    void Update()
    {
        //TIP:this record the passing time for you to compute speed
        float deltaTime = Time.deltaTime;
        controlsAcc += deltaTime;
        
        //WARN:Shouldn't depend on these values because it depends on every input methods
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(0, 0, 0);

        //CHECK:: release control condition might not be deal here
        //TODO: check math way to retrieve sign
        if(releaseControls == false){
            if (horizontal > 0){
                direction.x = 1;
            }else if (horizontal < 0){
                direction.x = -1;
            }

            if (vertical > 0){
                direction.z = 1;
            }else if (vertical < 0){
                direction.z = -1;
            }
        }else{
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        if(rigidbody.velocity.y > 0){
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        }

        //Controls movement of horizental
        if(controlsAcc >= controlsThreshold){
            if(direction.x != 0 || direction.z != 0){
                rigidbody.AddForce(direction*controlsSpeed, ForceMode.Impulse);
            }
            Debug.DrawLine(transform.position, 
                new Vector3(transform.position.x, 0, transform.position.z), Color.green, deltaTime);

        }
        //Set horizental velocity back to zero after control hits
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        rigidbody.angularVelocity = Vector3.zero;

        //Manage horizental movement in the state machine
        bool fallKey = Input.GetKey ("space");
        
        //State machine
        switch(verticalState){
            case VerticalMoveState.fallState:
                stopAcc += deltaTime;
                if(stopAcc >= stopFallThreshold){
                    stopAcc = 0;
                    stopFall();
                    verticalState = VerticalMoveState.stopState;                    
                }
                if(fallKey == true && releaseControls == false){
                    startDoubleFall();
                    verticalState = VerticalMoveState.doubleFallState;
                }
                break;
            case VerticalMoveState.stopState:
                fallAcc += deltaTime;
                if(fallAcc >= startFallThreshold){
                    fallAcc = 0;
                    startFall(); 
                    verticalState = VerticalMoveState.fallState;                
                }
                if(fallKey == true && releaseControls == false){
                    startDoubleFall();
                    verticalState = VerticalMoveState.doubleFallState;
                }
                break;
            case VerticalMoveState.doubleFallState:
            //CHECK::the releaseControls == false might mess up the state machine
                if(fallKey == false && releaseControls == false){
                    stopFall();
                    verticalState = VerticalMoveState.stopState;
                }
                break;
        }
    }

    //collision detection
    void OnCollisionEnter(Collision collision){
        int numOfContacts = collision.contactCount;
        Vector3 collisionDirection = new Vector3(0, 1, 0);
        foreach(ContactPoint contact in collision.contacts){
            Debug.DrawRay(contact.point, contact.normal, Color.yellow, 5f);
            if(contact.normal == collisionDirection){
                releaseControls = true;
                Debug.Log("contact.point: " + contact.point.ToString() + " Normal:" + contact.normal.ToString());
            }
        }
    }
    

    //Tip::State machine to do the falling
    Vector3 fallDirection = new Vector3(0, -1, 0);
    void startFall(){
        rigidbody.AddForce(fallDirection*fallSpeed, ForceMode.Impulse);
    }

    void stopFall(){
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        rigidbody.angularVelocity = Vector3.zero;
    }

    void startDoubleFall(){
        rigidbody.AddForce(fallDirection*doubleFallSpeed, ForceMode.Impulse);
    }
}
