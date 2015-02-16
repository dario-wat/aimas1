using UnityEngine;
using System.Collections.Generic;

public class DynamicCarMove : Move {

	// Angular velocity
	private float omega;

	// Normalized velocity, direction of moving
	private Vector3 velocity;

	// Speed, can be negative when moving backwards
	private float speed;

	// Acceleration, negative for deccelerating
	private float acceleration;

	private int turn;

	// Rotation radius
	private float r;

	// Center offset for faster and easier center finding
	private Vector3 centerOff;


	// Computes radius of the moving circle and center offset
	public DynamicCarMove(Vector3 velocity, float speed, float acceleration,
		float turn, float t) : base(t) {
		
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.acceleration = acceleration;
		this.r = DynamicCarState.radius;
		this.turn = (int) Mathf.Sign(turn);
		this.omega = Mathf.Sign(speed) * turn * speed / r;
		//Debug.Log("created");

		// Setting centerOff
		if (turn != 0) {
			Vector3 normal;
			if (turn < 0) {	// Turning in left circle
				normal = Quaternion.Euler(0, -90, 0) * velocity;
			} else {			// Turning in right circle
				normal = Quaternion.Euler(0, 90, 0) * velocity;
			}
			this.centerOff = normal.normalized * r;
		}
	}

	// Translates and rotates, as kinematic car would want
	override protected void MoveTransform(Transform transform, float time) {
		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		transform.RotateAround(transform.position, yAxis, omega * time);

		// Compute new velocity
		velocity = Quaternion.Euler(0, omega * time, 0) * velocity;
		speed += acceleration * time;
		omega = turn * speed / r * 180 / Mathf.PI;
	}

	// Obstructions, must check line and arc intersection
	override protected bool Obstructed(
		IEnumerable<Polygon> polys, Vector3 startPos) {
		
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);

		// Check line - polygon intersection
		Edge e = new Edge(sp, np);
		foreach (Polygon p in polys) {
			if (p.Intersects(e)) {
				return true;
			}
		}
		return false;
	}

	// Predict the point, depends if its arc or line
	override protected Vector3 PredictPosition(Vector3 pos) {
		// TODO this if maybe unnecessary
		if (turn == 0) {
			return pos + velocity * speed * t
				+ 0.5f * velocity * acceleration * t * t;
		}

		Vector3 center = pos + centerOff;
		float endSpeed = speed + acceleration * t;
		float angle = (speed + endSpeed) / (2 * r) * t;
		Vector3 endVector = - (Quaternion.Euler(0, angle, 0) * centerOff);
		Debug.Log(ToString() + " " + pos + " " + (center + endVector));
		return center + endVector;
	}

	// Creates a copy
	override public Move Copy() {
		return new DynamicCarMove(velocity, speed, acceleration, turn, t);
	}

	// For debugging
	override public string ToString() {
		return string.Format("Velocity: {0}, Speed: {1}, Acceleration {2}"
			+ " Turn: {3}, t: {4}, r: {5}",
			velocity, speed, acceleration, turn, t, r);
	}
}
