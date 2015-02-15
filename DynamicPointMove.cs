using UnityEngine;
using System.Collections.Generic;

public class DynamicPointMove : Move {

	public Vector3 velocity;
	private Vector3 acceleration;
	override public float t { get; private set; }

	public DynamicPointMove(Vector3 velocity, Vector3 acceleration, float t) {
		this.velocity = velocity;
		this.acceleration = acceleration;
		this.t = t;
	}

	override public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Translate and rotate
		transform.Translate(velocity * time, Space.World);
		
		velocity += acceleration * time;

		t -= time;
		return dt - time;
	}

	override protected bool Obstructed(IEnumerable<Polygon> polys, Vector3 startPos) {
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);

		List<Vector2> points = new List<Vector2>();
			Vector2 diff = (np - sp) / (DynamicPointState.collisionPoints - 1);
			Vector2 point = sp;
			for (int i = 0; i < DynamicPointState.collisionPoints; i++) {
				points.Add(point + i * diff);
				//point += diff;
			}
			
			foreach (Vector2 v in points) {
				foreach (Polygon p in polys) {
					if (p.IsInside(v)) {
						return true;
					}
				}
			}
		return false;
	}

	override protected Vector3 PredictPosition(Vector3 pos) {
		return pos + velocity * t
				+ 0.5f * acceleration * t * t;
	}
}
