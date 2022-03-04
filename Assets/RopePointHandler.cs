using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePointHandler : MonoBehaviour
{
    [SerializeField] Material _pointMat,_defMaterial;
    Transform _playerTransform;

    public bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PointTrigger")
        {
            isActive = true;
            other.GetComponentInParent<PlayerMovement>().ropePoint = this.transform;
            _playerTransform = other.gameObject.transform;
            AnimatePoint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
        other.GetComponentInParent<PlayerMovement>().ropePoint = null;
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().material = _defMaterial;
    }

    // Update is called once per frame
    void AnimatePoint()
    {
        gameObject.GetComponent<MeshRenderer>().material = _pointMat;
        gameObject.GetComponent<Animator>().enabled = true;
    }
}
