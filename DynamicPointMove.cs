using UnityEngine;
using System.Collections.Generic;

public class DynamicPointMove : Move {

	// Velocity and acceleration vectors, both not normalized
	public Vector3 velocity { get; private set; }
	private Vector3 acceleration;

	// Base t
	public DynamicPointMove(Vector3 velocity, Vector3 acceleration,
		float t) : base(t){
		
		this.velocity = velocity;
		this.acceleration = acceleration;
	}

	// Translates and updates velocity
	override protected void MoveTransform(Transform transform, float time) {
		// Translate and rotate
		transform.Translate(velocity * time, Space.World);
		velocity += acceleration * time;
	}

	// Checks obstructions by estimating a line and checking if the
	// line clashes with polygons, this is fine thing to do since
	// steps are really small in dynamic point moves
	override protected bool Obstructed(IEnumerable<Polygon> polys, Vector3 startPos) {
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

	// Simple physics
	override protected Vector3 PredictPosition(Vector3 pos) {
		return pos + velocity * t + 0.5f * acceleration * t * t;
	}

	// Copy right, not really needed
	override public Move Copy() {
		return new DynamicPointMove(velocity, acceleration, t);
	}

	// For debugging
	override public string ToString() {
		return string.Format("{0} {1} {2}", velocity, acceleration, t);
	}
}
