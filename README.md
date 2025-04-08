
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

<details>
<summary><code>CameraMovement</code></summary>

```
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
```
</details>

<details>
<summary><code>FancyTowerColor</code></summary>

```
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

```
</details>

<details>
<summary><code>PlayerMovement</code></summary>

```
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

```
</details>



