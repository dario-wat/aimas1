using UnityEngine;
using System.Collections;

public class ChildCar : MonoBehaviour {

	private float L;

	void Update () {
		L = transform.parent.localScale.z;
		transform.position =
			transform.parent.position + L / 2 * transform.forward;
	}
}
