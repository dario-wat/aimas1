using UnityEngine;
using System.Collections;

public class Move {

	// Acceleration
	private float acc = 0.0f;

	// Velocity
	private float vel = 0.0f;

	// Turning angle in degrees
	private float theta = 0.0f;

	// Velocity in directions
	private float dx = 0.0f;
	private float dz = 0.0f;

	// How long move has to be done
	public float t = 0.0f;

	public Move(float dx, float dz, float t) {
		this.dx = dx;
		this.dz = dz;
		this.t = t;
	}

	public float MoveMe(Transform tranform, float dt) {
		return 0.0f;
	}
}
