
# Project Title

Play jenga, and try to pull blocks from the tower without toppling it all!


## Screenshots

![App Screenshot](https://i.imgur.com/8BaeHzL.jpeg)


## Features

- Select blocks and push or pull them with your mouse
- Live previews
- Lower blocks are under pressure and experience more friction
- Reset the tower


## Snippets

</details>

<details>
<summary><code>Move Block</code></summary>

```
private void MoveBlock()
{
    if(_selectedBlock == null)
        return;

    float _mouseXInput = Input.GetAxis("Mouse X");
    float _mouseYInput = Input.GetAxis("Mouse Y");

    _pushPower = Mathf.Clamp(_mouseYInput, 0f, 1f);

    // If we try to push (mouse forward) and are looking at a block,
    //  - push that block in our lookDirection
    //  - push harder the faster you shoot the mouse forward
    //  - add a resistant force the lower down the block is in the tower

    if (Input.GetMouseButton(0)) {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, _rayMaxDistance, _rayLayerMask)) {
            if(hit.collider.transform == _selectedBlock) {

                Vector3 _inputVector = 100f * _mouseYInput * _cam.forward.normalized;

                _lastBlockRigidbody.AddForceAtPosition(_blockMoveSpeed * Time.smoothDeltaTime * _inputVector, hit.point);

                if(_nearbyIterations >= _skipNearbyIterations)
                {
                    _nearbySelectedBlocks = _selectedBlock.GetComponent<NearbyObjects>().GetObjects();

                    if (_nearbySelectedBlocks.Count > 0)
                    {
                        float blockLayerModifier = 1f - (_selectedBlock.parent.GetSiblingIndex() / 17f);

                        foreach (Rigidbody nearBlock in _nearbySelectedBlocks)
                        {
                            _debugText.text = "Layer power modifier: " + blockLayerModifier;

                            nearBlock.AddForce((blockLayerModifier * _nearBlockMoveSpeed * _pushPower) * Time.smoothDeltaTime * _inputVector);
                        }
                    }
                }
                else
                {
                    _nearbyIterations++;
                }
            }
        }
    }
}

```
</details>

</details>

<details>
<summary><code>CameraController.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private Camera _camera;

    private Vector3 _movementInput = Vector3.zero;

    private Vector3 _camRotation = Vector3.zero;

    public float _panCameraSpeed = 100f;

    public float _smoothingCamSpeed = 4f;

    private bool _useMouseInput = true;

    private int _initializedCamera = 10;

    private void Start()
    {
        _movementInput = Vector3.zero;
        _camRotation = transform.rotation.eulerAngles;
    }

    private void OnEnable()
    {
        _movementInput = Vector3.zero;
        _camRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        CameraMovement();
    }

    public void ChangeMouseMovement(bool state)
    {
        _useMouseInput = state;
    }

    private void CameraMovement()
    {
        // Generic 1st person camera controller, follows player
        //  - added smoothing

        Vector3 input = Vector3.zero;
        if(_useMouseInput == true) {
            if(_initializedCamera <= 0) {
                input = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
            }
            else {
                _initializedCamera--;
            }
        }

        _movementInput = Vector3.Lerp(_movementInput, input, _smoothingCamSpeed * Time.deltaTime);

        float _pitch = Mathf.Clamp(_camRotation.x + (-_movementInput.y * _panCameraSpeed * Time.deltaTime * 10f), -90f, 45f);

        float _yaw = _camRotation.y + (_movementInput.x * _panCameraSpeed * Time.deltaTime * 10f);

        _camRotation = new Vector3(_pitch, _yaw, 0f);

        transform.eulerAngles = _camRotation;
    }
}

```
</details>

</details>

<details>
<summary><code>TowerManager.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public PlayerController _playerController;

    private bool _holdingBlock = false;

    public Transform _tower;

    public Color _lighterColor;

    private float lighterColorChance = 0.33f;


    private void Start()
    {
        ModifyColors();
    }

    public void ToggleHolding()
    {
        if (_holdingBlock == true)
            _holdingBlock = false;
        else
            _holdingBlock = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deselect blocks once they drop below ground level

        if (other.CompareTag("Block"))
        {
            if (_holdingBlock)
            {
                _playerController.DeselectBlock();
            }
        }
    }

    private void ModifyColors()
    {
        //Overlay tower with color
        foreach(Transform layer in _tower)
        {
            foreach (Transform block in layer)
            {
                if (Random.Range(0f, 1f) <= lighterColorChance)
                {
                    Material altMaterial = block.GetComponent<Renderer>().material;
                    altMaterial.color = _lighterColor;
                }
            }
        }
    }
}

```
</details>

</details>

<details>
<summary><code>PlayerMovement.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float _moveSpeed = 1f;

    private bool _suspendMovement = false;


    public void ToggleMovement()
    {
        if(_suspendMovement == false)
            _suspendMovement = true;
        else
            _suspendMovement = false;
    }

    private void Update()
    {
        // WASD - Horizontal movement
        // CTRL/SHIFT - Vertical movement

        if (!_suspendMovement)
        {
            Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("ControlHeight"), Input.GetAxis("Vertical"));

            Vector3 newMovement = (transform.right.normalized * movementInput.x) + (transform.up.normalized * movementInput.y) + (transform.forward.normalized * movementInput.z);

            transform.position += (_moveSpeed * Time.smoothDeltaTime * newMovement);
        }
    }
}

```
</details>



