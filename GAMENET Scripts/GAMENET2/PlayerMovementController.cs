using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;

    [SerializeField] private RigidbodyFirstPersonController rigidbodyFirstPersonController;

    private Animator animator;

    void Start()
    {
        rigidbodyFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(rigidbodyFirstPersonController != null && joystick != null)
        {
            rigidbodyFirstPersonController.joystickInputAxis.x = joystick.Horizontal;
            rigidbodyFirstPersonController.joystickInputAxis.y = joystick.Vertical;
            rigidbodyFirstPersonController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

            animator.SetFloat("horizontal", joystick.Horizontal);
            animator.SetFloat("vertical", joystick.Vertical);

            if (Mathf.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9)
            {
                animator.SetBool("isRunning", true);
                rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 10;
            }
            else
            {
                animator.SetBool("isRunning", false);
                rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 5;
            }
        }
    }
}
