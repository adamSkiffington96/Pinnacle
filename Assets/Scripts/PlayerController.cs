using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float _rayMaxDistance = 10f;

    private LayerMask _rayLayerMask;

    private Outline _lastOutline;

    private Material _lastMaterial;

    public Color _emissionColorChange;

    private Transform _highlightedBlock;

    private Transform _selectedBlock;

    private List<Rigidbody> _nearbySelectedBlocks;

    private int _skipNearbyIterations = 2;
    private int _nearbyIterations = 0;

    private bool _holdingBlock = false;

    public float _blockMoveSpeed = 1f;
    public float _nearBlockMoveSpeed = 1f;

    public float _blockChangeHeightSpeed = 1f;

    private PlayerMovement _playerMovement;

    private CameraController _cameraController;

    private Rigidbody _lastBlockRigidbody;

    private float _resetCounter = 0f;

    public Transform _tower;

    private Transform _newTower;

    private Transform _cam;

    private bool _resetTower = false;

    public TowerManager _towerManager;

    public Text _debugText;

    private float _pushPower = 0f;


    void Start()
    {
        _cam = Camera.main.transform;

        _tower.gameObject.SetActive(false);
        _newTower = Instantiate(_tower);
        _newTower.gameObject.SetActive(true);

        _cameraController = GetComponent<CameraController>();
        _playerMovement = GetComponent<PlayerMovement>();
        _rayLayerMask = LayerMask.GetMask("Raycasting");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool raycasting = HighlightSystem();

        if (Input.GetMouseButtonDown(0)) {
            if (_holdingBlock == false) {
                if (raycasting) {
                    _selectedBlock = _highlightedBlock;
                    print("Selected block: " + _selectedBlock.name + " " + _selectedBlock.GetSiblingIndex() + " in layer " + _selectedBlock.parent.GetSiblingIndex());

                    _lastBlockRigidbody = _selectedBlock.GetComponent<Rigidbody>();

                    _towerManager.ToggleHolding();

                    _cameraController.ChangeMouseMovement(false);

                    _holdingBlock = true;

                    //_playerMovement.ToggleMovement();

                    //_cameraController.ToggleMouseMovement();
                }
            }
            else
            {
                _cameraController.ChangeMouseMovement(false);
            }
        }

        if (_holdingBlock) {
            MoveBlock();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _cameraController.ChangeMouseMovement(true);
        }

        //_debugText.text = "Holding state: " + _holdingBlock;

        if (Input.GetKey(KeyCode.R)) {
            if (_resetTower == false) {
                if (_resetCounter < 3f) {
                    _resetCounter += Time.deltaTime;
                }
                else {
                    _newTower.gameObject.SetActive(false);
                    _newTower = null;
                    _newTower = Instantiate(_tower);
                    _newTower.gameObject.SetActive(true);

                    _resetTower = true;
                    _resetCounter = 0f;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.R)) {
            _resetTower = false;
        }
    }

    public void DeselectBlock()
    {
        if (_selectedBlock != null) {
            print("Un-selected block: " + _selectedBlock.name + " " + _selectedBlock.GetSiblingIndex() + " in layer " + _selectedBlock.parent.GetSiblingIndex());

            _selectedBlock = null;
        }

        _towerManager.ToggleHolding();

        if (_lastBlockRigidbody != null)
            _lastBlockRigidbody = null;


        if(_holdingBlock == true)
            _holdingBlock = false;

        ClearHighlighted();
    }

    private void MoveBlock()
    {
        if(_selectedBlock == null)
            return;

        float _mouseXInput = Input.GetAxis("Mouse X");
        float _mouseYInput = Input.GetAxis("Mouse Y");

        _pushPower = Mathf.Clamp(_mouseYInput, 0f, 1f);
        //_debugText.text = "Pushing power: " + _pushPower;

        //float _heightInput = Input.GetAxis("ControlHeight");

        float _heightInput = 0f;

        if (Input.GetMouseButton(0))
            _heightInput++;

        if (Input.GetMouseButton(1))
            _heightInput--;

        if (_heightInput == 1) {
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

    private bool HighlightSystem()
    {
        bool raycasting = false;

        if (_holdingBlock)
            return false;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, _rayMaxDistance, _rayLayerMask)) {
            if (_highlightedBlock != null) {
                ClearHighlighted();
            }

            _lastMaterial = hit.transform.GetComponent<Renderer>().material;
            _lastMaterial.SetColor("_EmissionColor", _emissionColorChange);
            _lastOutline = hit.transform.GetComponent<Outline>();
            _lastOutline.enabled = true;

            _highlightedBlock = hit.transform;

            Debug.Log(hit.collider.gameObject.name);

            raycasting = true;
        }
        else {
            if (_highlightedBlock != null) {
                ClearHighlighted();
            }
        }

        return raycasting;
    }

    private void ClearHighlighted()
    {
        if(_lastOutline.enabled)
            _lastOutline.enabled = false;
        _lastOutline = null;

        _lastMaterial.SetColor("_EmissionColor", Color.black);
        _lastMaterial = null;

        _highlightedBlock = null;
    }
}
