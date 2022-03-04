using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerSO _playerSO;
    [SerializeField] Animator _playerAnim;
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

    GameObject CheckForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return hit.transform.gameObject;
            }
        }
        
        return null;
    }

    public void Swing(InputAction.CallbackContext context)
    {
        if (context.started && ropePoint.GetComponent<RopePointHandler>().IsActive)
        {
            Debug.Log("Pressed");
            _isSwinging = true;
            _playerAnim.SetBool("isSwinging", true);
        }
        else if (context.canceled)
        {
            Debug.Log("Relased");
            CheckForGround();
            _isSwinging = false;
            _playerAnim.SetBool("isSwinging", false);

            GameObject ground = CheckForGround();

            if(ground != null)
            {
                transform.rotation =  Quaternion.Euler(ground.transform.rotation.eulerAngles);
            }
        }
    }
}
