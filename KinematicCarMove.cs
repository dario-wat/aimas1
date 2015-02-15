using UnityEngine;
using System.Collections.Generic;

public class KinematicCarMove : Move {

	protected float omega;
	protected Vector3 velocity;
	protected float speed;
	override public float t { get; private set; }

	private float r;
	private Vector3 centerOff;

	public KinematicCarMove(Vector3 velocity, float speed, float omega, float t) {
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.omega = Mathf.Sign(speed) * omega;
		this.t = t;
		this.r = Mathf.Abs(speed) / (Mathf.Abs(omega) * Mathf.PI / 180.0f);

		if (omega != 0) {
			Vector3 normal;
			if (omega < 0) {
				// Turning in left circle
				normal = Quaternion.Euler(0, -90, 0) * velocity;
			} else {
				// Turning in right circle
				normal = Quaternion.Euler(0, 90, 0) * velocity;
			}
			this.centerOff = normal.normalized * r;
		}
	}

	override public float MoveMe(Transform transform, float dt) {
		// Time used for transforming the vehicle
		float time = Mathf.Min(dt, t);

		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		float rot = (speed < 0 ? -omega : omega);
		rot = omega;
		transform.RotateAround(transform.position, yAxis, rot * time);
		
		// Compute new velocities, accelerations, anggles and what not
		velocity = Quaternion.Euler(0, rot * time, 0) * velocity;

		t -= time;
		return dt - time;
	}

	override protected bool Obstructed(IEnumerable<Polygon> polys, Vector3 startPos) {
		Vector3 newPoint = this.PredictPosition(startPos);
		Vector2 sp = new Vector2(startPos.x, startPos.z);
		Vector2 np = new Vector2(newPoint.x, newPoint.z);

		if (omega == 0.0f) {		// Check straight line intersection
			Edge e = new Edge(sp, np);
			foreach (Polygon p in polys) {
				if (p.Intersects(e)) {
					return true;
				}
			}
		} else {				// Check arc intersection
			// Center of turning circle
			Vector3 center = startPos + centerOff;
			Vector2 cp = new Vector2(center.x, center.z);
			Vector2 cenToS = sp - cp;
			Vector2 cenToN = np - cp;

			// Angles of the arc
			float a1, a2;
			if (omega < 0) {
				a1 = Arc.Angle(Vector2.right, cenToS);
				a2 = Arc.Angle(Vector2.right, cenToN);
			} else {
				a1 = Arc.Angle(Vector2.right, cenToN);
				a2 = Arc.Angle(Vector2.right, cenToS);
			}
			
			// Check if arc intersects with any of the polygons
			Arc arc = new Arc(cp, r, a1, a2);
			foreach (Polygon p in polys) {
				if (p.Intersects(arc)) {
					return true;
				}
			}
		}
		return false;
	}

	override protected Vector3 PredictPosition(Vector3 pos) {
		if (omega == 0) {
			return pos + velocity * speed * t;
		}
		Vector3 center = pos + centerOff;
		float angle = omega * t;
		Vector3 endVector = - (Quaternion.Euler(0, angle, 0) * centerOff);
		return center + endVector;
	}

	override public Move Copy() {
		return new KinematicCarMove(velocity, speed, Mathf.Sign(speed) * omega, t);
	}

	override public string ToString() {
		return string.Format("{0} {1} {2} {3} {4} {5}",
			velocity, speed, Mathf.Sign(speed) * omega, t, omega < 0, r);
	}
}
