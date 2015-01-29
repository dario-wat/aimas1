using UnityEngine;
using System.Collections;

public class DinamicMM : MonoBehaviour {

	public float xs = 0.0f;
	public float zs = 0.0f;

	public float dx = 0.0f;
	public float dz = 0.0f;

	public float ax = 0.2f;
	public float az = 0.2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(dx * Time.deltaTime, 0.0f, dz * Time.deltaTime, Space.World);
		dx += ax * Time.deltaTime;
		dz += az * Time.deltaTime;
	}
}
