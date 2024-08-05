using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float _moveSpeed = 1f;

    private bool _suspendMovement = false;

    private void Start()
    {
        
    }

    public void ToggleMovement()
    {
        if(_suspendMovement == false)
            _suspendMovement = true;
        else
            _suspendMovement = false;
    }

    private void Update()
    {
        if (!_suspendMovement)
        {
            Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("ControlHeight"), Input.GetAxis("Vertical"));

            Vector3 newMovement = (transform.right.normalized * movementInput.x) + (transform.up.normalized * movementInput.y) + (transform.forward.normalized * movementInput.z);

            transform.position += (_moveSpeed * Time.smoothDeltaTime * newMovement);
        }
    }
}
