using UnityEngine;
using System.Collections.Generic;

/*
	Move class used only for kinematic point model.
*/
public class KinematicPointMove : Move {

	// Point velocity
	private Vector3 velocity;


	// Nothing special to see
	public KinematicPointMove(Vector3 velocity, float t) : base(t) {
		this.velocity = velocity;
	}

	// Just translate the point with given velocity and time
	override protected void MoveTransform(Transform transform, float time) {
		transform.Translate(velocity * time, Space.World);
	}

	// Check is obstructed, checks line intersection with polygons
	override protected bool Obstructed(
		IEnumerable<Polygon> polys, Vector3 startPos) {
		
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);

		Edge e = new Edge(sp, np);
		foreach (Polygon p in polys) {
			if (p.Intersects(e)) {
				return true;
			}
		}
		return false;
	}

	// Predicting position, simple
	override protected Vector3 PredictPosition(Vector3 pos) {
		return pos + velocity * t;
	}

	// Create a copy
	override public Move Copy() {
		return new KinematicPointMove(velocity, t);
	}

	// For debugging
	override public string ToString() {
		return string.Format("Velocity: {0}, t: {1}", velocity, t);
	}
}
