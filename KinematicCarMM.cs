using UnityEngine;
using System.Collections;
using System;

public class KinematicCarMM : MonoBehaviour {

	public float xs = 0.0f;
	public float zs = 0.0f;

	public float L = 5.0f;
	public float vel = 2.0f;

	public float theta = 0.0f;
	public float phi = 0.2f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dx = (float) Math.Cos(theta);
		float dz = (float) Math.Sin(theta);
		transform.Translate(dx * Time.deltaTime, 0.0f, dz * Time.deltaTime, Space.World);
		theta += (float) Math.Tan(phi) * vel / L;
		if (theta > 2*Math.PI) {
			theta -= 2* (float) Math.PI;
		}
	}
}
