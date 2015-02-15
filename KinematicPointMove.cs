using UnityEngine;
using System.Collections.Generic;

/*
	Move class used only for kinematic point model.
*/
public class KinematicPointMove : Move {

	Vector3 velocity;
	float speed;
	override public float t { get; private set; }

	public KinematicPointMove(Vector3 velocity, float speed, float t) {
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.t = t;
	}

	override public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Translate
		transform.Translate(velocity * speed * time, Space.World);

		t -= time;
		return dt - time;
	}

	override protected bool Obstructed(IEnumerable<Polygon> polys, Vector3 startPos) {
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

	override protected Vector3 PredictPosition(Vector3 pos) {
		return pos + velocity * speed * t;
	}
}
