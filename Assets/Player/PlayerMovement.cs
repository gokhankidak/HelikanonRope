using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerSO _playerSO;
    [SerializeField] Animator _playerAnim;
    [SerializeField] float _smooth = 10f;
    [SerializeField] Transform _playerHand;
    [SerializeField] RopeControllerSimple _ropeController;
    int _rotateDirection;

    bool _isSwinging = false;
    bool _isDead = false;
    public Transform ropePoint;

    void Update()
    {
        if (CheckForGround() == null && _isSwinging == false)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        Debug.Log("Game over");
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        _isDead = true;
        _ropeController.DestroyRope();
        _playerAnim.SetBool("isFalling", true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
        if (_isSwinging && !_isDead)
        {
            transform.RotateAround(ropePoint.position, Vector3.up, _playerSO.swingSpeed * Time.deltaTime * _rotateDirection);
        }
    }

    private void MovePlayer()
    {
        if (!_isDead)
            transform.Translate(Vector3.right * Time.deltaTime * _playerSO.playerSpeed);
    }

    int RotationDirection(Vector3 targetDir)
    {
        Vector3 A = transform.position;

        Vector3 perp = Vector3.Cross(Vector3.forward, targetDir);
        float dir = Vector3.Dot(perp, Vector3.up);

        if (dir > 0.0f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    GameObject CheckForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    public void Swing(InputAction.CallbackContext context)
    {
        if (context.started && ropePoint != null && ropePoint.GetComponent<RopePointHandler>().IsActive)
        {
            Debug.Log("Pressed");
            _isSwinging = true;
            _playerAnim.SetBool("isSwinging", true);
            _ropeController.connectedTo = ropePoint;
            _ropeController.fromTheRope = _playerHand;
            _ropeController.CreateRope();
            _rotateDirection = RotationDirection(ropePoint.position);
            
            StartCoroutine(OverRotationControl());
        }
        else if (context.canceled)
        {
            Debug.Log("Relased");
            CheckForGround();
            _isSwinging = false;
            _playerAnim.SetBool("isSwinging", false);
            _ropeController.DestroyRope();

            GameObject ground = CheckForGround();

            if (ground != null)
            {
                StartCoroutine(FixPosition(ground));
            }
            else
            {
                OnDeath();
            }
        }
    }

    IEnumerator OverRotationControl()
    {
        while (CheckForGround()!= null && _isSwinging)
        {
            yield return new WaitForSeconds(0.05f);
        }

        while (CheckForGround() == null && _isSwinging)
        {
            yield return new WaitForSeconds(0.05f);
        }

        while (CheckForGround() != null && _isSwinging)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (_isSwinging)
        {
            yield return new WaitForSeconds(0.1f);
            OnDeath();
        }
        yield break;
    }


    IEnumerator FixPosition(GameObject ground)
    {
        float startTime = Time.time;
        while (Time.time < startTime + _smooth / 5)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, ground.transform.rotation, (Time.time - startTime) / _smooth);
            if (ground.transform.rotation.eulerAngles.y == 0 || ground.transform.rotation.eulerAngles.y == 180)
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, ground.transform.position.z), (Time.time - startTime) / _smooth);
            else
                transform.position = Vector3.Lerp(transform.position, new Vector3(ground.transform.position.x, transform.position.y, transform.position.z), (Time.time - startTime) / _smooth);

            yield return null;
        }
        yield break;
    }
}
