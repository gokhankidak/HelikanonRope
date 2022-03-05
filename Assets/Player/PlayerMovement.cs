using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerSO _playerSO;
    [SerializeField] Animator _playerAnim;
    [SerializeField] float _smooth = 10f;
    [SerializeField] InstantiateRope _insRope;
    Rigidbody _playerRb;
    bool _isSwinging = false;
    public Transform ropePoint;

    private void Start()
    {
        _playerRb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(CheckForGround() == null && _isSwinging == false)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        Debug.Log("Game over");
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
            StartCoroutine(_insRope.SpawnRope());

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
               StartCoroutine(FixRotation(ground));
            }
            else
            {
                OnDeath();
            }
        }
    }

    IEnumerator FixRotation(GameObject ground)
    {
        float startTime = Time.time;

        while (Time.time < startTime + _smooth)
        {
        transform.rotation = Quaternion.Slerp(transform.rotation, ground.transform.rotation
                            , (Time.time - startTime) / _smooth);
            
            yield return null;
        }
    }
}
