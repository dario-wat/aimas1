using UnityEngine;
using System.Collections;

/*
	Move class used only for kinematic point model.
*/
public class KinematicPointMove : Move {

	Vector3 velocity;
	float speed;
	float t;

	public KinematicPointMove(Vector3 velocity, float speed, float t) {
		this.velocity = velocity;
		this.speed = speed;
		this.t = t;
	}

	public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Translate
		transform.Translate(velocity * speed * time, Space.World);

		t -= time;
		return dt - time;
	}

	private bool Obstructed()
}
