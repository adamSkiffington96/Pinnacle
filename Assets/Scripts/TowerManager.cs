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
