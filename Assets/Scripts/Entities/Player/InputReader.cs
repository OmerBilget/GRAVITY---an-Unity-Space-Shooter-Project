using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputReader : MonoBehaviour
{

    public PlayerMovement playerMovement;
    public BulletHandler bulletHandler;
    public ForceField forceField;
    public MineHandler mineHandler;
    public Cam camera;
    public bool keyboard = true;
    Vector2 mouseCoordinates=Vector2.zero;
    public Rigidbody2D rb;
    float delta;
    Vector2 worldPos;
    Vector2 targetHeading;
    Camera cam;
    private void Start()
    {
        
        cam = Camera.main;
    }
    // Start is called before the first frame update


    public void GamepadThrust(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.started)
        {
            playerMovement.EnableThrust();
        }
        else if (context.canceled)
        {
            playerMovement.DisableThrust();
        }
    }

    public void GamepadHyperflight(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.started)
        {
            playerMovement.EnableHyperFlight();
        }
        else if (context.canceled)
        {
            playerMovement.DisableHyperflight();
        }
    }

    public void GamepadFireWeapon(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.started)
        {
            bulletHandler.enableWeapon();
        }
        else if (context.canceled)
        {
            bulletHandler.disableWeapon();
        }
    }
    public void GamepadDeployMine(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.started)
        {
            mineHandler.DeployMine();
        }
    }

    public void GamepadPulse(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.started)
        {
            forceField.activatePulse();
        }
       
    }

    public void GamepadChangeMoveDirection(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }

        if (context.performed)
        {
            playerMovement.setTargetHeading(context.ReadValue<Vector2>().normalized);
        }
    }

    public void GamePadZoomOut(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.performed)
        {
            camera.ZoomOut();
        }
    }

    public void GamePadZoomIn(InputAction.CallbackContext context)
    {
        if (keyboard)
        {
            return;
        }
        if (context.performed)
        {
            camera.ZoomIn();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (keyboard)
        {
            if (Input.GetKeyDown(KeyCode.W)) { 
            
                playerMovement.EnableThrust();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                playerMovement.DisableThrust();
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerMovement.EnableHyperFlight();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                playerMovement.DisableHyperflight();
            }


            if (Input.GetMouseButtonDown(0))
            {
                bulletHandler.enableWeapon();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                bulletHandler.disableWeapon();
            }


            if (Input.GetMouseButtonDown(1))
            {
                forceField.activatePulse();
            }

            if (Input.GetMouseButtonDown(2))
            {
                mineHandler.DeployMine();
            }


            delta = Input.mouseScrollDelta.y;
            if (delta < 0)
            {
                camera.ZoomIn();

            }else if (delta > 0)
            {
                camera.ZoomOut();
            }
            Plane plane = new Plane(Vector3.back, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(plane.Raycast(ray,out float enter))
            {
                worldPos = ray.GetPoint(enter);
            }
            //worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            targetHeading = worldPos - (Vector2)rb.transform.position;
            targetHeading.Normalize();
            if (targetHeading.magnitude != 0)
            {
                playerMovement.setTargetHeading(targetHeading);
            }
            
        }
   
    }
}
