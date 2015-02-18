using UnityEngine;
using System.Collections.Generic;

public class DynamicCarMove : Move {
	
	// Angular velocity
	private float omega;
	
	// Normalized velocity, direction of moving
	public Vector3 velocity { get; private set; }
	
	// Speed, can be negative when moving backwards
	public float speed { get; private set; }
	
	// Acceleration, negative for deccelerating
	private float acceleration;
	
	private int turn;
	
	// Rotation radius
	private float r;
	private DynamicCarState newState;
	
	// Center offset for faster and easier center finding
	//private Vector3 centerOff;
	float toDeg = 180/Mathf.PI;
	//float toRad = Mathf.PI/180;
	
	
	// Computes radius of the moving circle and center offset
	public DynamicCarMove(Vector3 velocity, float speed, float acceleration,
		float phi, float r, DynamicCarState newState, float t) : base(t) {
		
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.acceleration = acceleration;
		this.r = r;
		this.turn = (int)Mathf.Sign (phi);
		this.omega = turn * (speed/r) * toDeg;
		this.newState = newState;
		//Debug.Log("created");
		
		// Setting centerOff
		//if (turn != 0) {
		//	Vector3 normal = Quaternion.Euler(0, Mathf.Sign(turn)*90, 0) * velocity;
		//	this.centerOff = normal.normalized * r;
		//}
	}
	
	// Translates and rotates, as kinematic car would want
	override protected void MoveTransform(Transform transform, float time) {
		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		transform.RotateAround(transform.position, yAxis, omega * time);
		
		// Compute new velocity
		velocity = Quaternion.Euler(0, omega * time, 0) * velocity;
		speed += acceleration * time;
		omega = turn * (speed / r) * toDeg;
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
		//if (turn == 0) {
		//	return pos + velocity * speed * t
		//		+ 0.5f * velocity * acceleration * t * t;
		//}
		return newState.position;
		
		/*Vector3 center = pos + centerOff;
		float endSpeed = speed + acceleration * t;
		float angle = ( (speed + endSpeed) / (2 * r) )* t;
		Vector3 endVector = - (Quaternion.Euler(0, angle * toDeg, 0) * centerOff);
		return center + endVector;*/
	}
	
	// Creates a copy
	override public Move Copy() {
		return new DynamicCarMove(velocity, speed, acceleration, omega, r, newState, t);
	}
	
	// For debugging
	override public string ToString() {
		return string.Format("Velocity: {0}, Speed: {1}, Acceleration {2}"
		                     + " omega: {3}, t: {4}, r: {5}",
		                     velocity, speed, acceleration, omega, t, r);
	}
}