using UnityEngine;
using System.Collections;

public class ChildCar : MonoBehaviour {

	private float L;

	void Start () {
		this.L = transform.parent.localScale.z;
	}
	
	void Update () {
		transform.position =
			transform.parent.position + L / 2 * transform.forward;
	}
}
