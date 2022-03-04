using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerSO _playerSO;
    Rigidbody _playerRb;
    bool _isSwinging = false;
    public Transform ropePoint;

    private void Start()
    {
        _playerRb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _playerSO.playerSpeed);

        if(_isSwinging)
        {
            transform.RotateAround(ropePoint.position, Vector3.up, _playerSO.swingSpeed * Time.deltaTime);
        }
    }

    public void Swing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Pressed");
            _isSwinging = true;
        }
        else if (context.canceled)
        {
            Debug.Log("Relased");
            _isSwinging = false;
        }
    }
}
