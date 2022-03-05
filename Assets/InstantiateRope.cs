using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InstantiateRope : MonoBehaviour
{
	public float speed = 1;

	[SerializeField] GameObject _ropePoint;
	[SerializeField] float distance = 2;
	[SerializeField] GameObject _nodePrefab;
	[SerializeField] GameObject _playerHand;
	[SerializeField] GameObject lastNode;

	bool done = false;

	// Use this for initialization
	void Start()
	{
		lastNode = _ropePoint;
	}

	// Update is called once per frame

	public IEnumerator SpawnRope()
    {
		while ((Vector3)transform.position != _ropePoint.transform.position)
		{
			if (Vector3.Distance(_playerHand.transform.position, lastNode.transform.position) > distance)
			{
				CreateNode();
				yield return null;
			}
		}

		if (done == false)
		{
			done = true;

			lastNode.GetComponent<HingeJoint>().connectedBody = _playerHand.GetComponent<Rigidbody>();
		}
		yield return null;
	}

	void CreateNode()
	{

		Vector3 pos2Create = _playerHand.transform.position - lastNode.transform.position;
		pos2Create.Normalize();
		pos2Create *= distance;
		pos2Create += (Vector3)lastNode.transform.position;

		GameObject go = (GameObject)Instantiate(_nodePrefab, pos2Create, Quaternion.identity);

		go.transform.SetParent(transform);
		lastNode.GetComponent<HingeJoint>().connectedBody = go.GetComponent<Rigidbody>();
		lastNode = go;
	}
}