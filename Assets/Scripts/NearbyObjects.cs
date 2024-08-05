using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyObjects : MonoBehaviour
{
    private List<Rigidbody> _nearbyBlocks = new List<Rigidbody>();

    private bool _searchingEnabled = true;

    public void ToggleSearching()
    {
        if(_searchingEnabled == false)
            _searchingEnabled = true;
        else
            _searchingEnabled = false;
    }

    public List<Rigidbody> GetObjects()
    {
        if (_searchingEnabled == false)
            _searchingEnabled = true;

        return _nearbyBlocks;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_searchingEnabled)
        {
            if (collision.collider.transform.CompareTag("Block"))
            {
                _nearbyBlocks.Add(collision.transform.GetComponent<Rigidbody>());
            }
        }
    }
}
