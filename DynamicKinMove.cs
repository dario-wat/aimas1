using UnityEngine;
using System.Collections.Generic;
using System;

/**
	!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	This class should NOT be used as normal move because it will not work.
	!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
**/
public class DynamicKinMove : Move {
	
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
	private DynamicCarState newState;

	
	// Computes radius of the moving circle and center offset
	public DynamicKinMove(Vector3 velocity, float speed, float acceleration,
		float turn, float r, float t) : base(t) {
		
		this.velocity = velocity.normalized;
		this.speed = speed;
		this.acceleration = acceleration;
		this.r = r;
		this.turn = turn == 0 ? 0 : (int) Mathf.Sign(turn);
		this.omega = Mathf.Sign(speed) * turn * speed / r;
	}
	// Translates and rotates, as kinematic car would want
	override protected void MoveTransform(Transform transform, float time) {
		// Translate and rotate
		transform.Translate(velocity * speed * time, Space.World);
		transform.RotateAround(transform.position, yAxis, omega * time);
		
		// Compute new velocity
		velocity = Quaternion.Euler(0, omega * time, 0) * velocity;
		speed += acceleration * time;
		omega = this.turn * speed / r * 180 / Mathf.PI;
	}

	
	// Function unnecessary
	override protected bool Obstructed(
		IEnumerable<Polygon> polys, Vector3 startPos) {
		throw new NotImplementedException();
	}
	
	// Function unnecessary
	override protected Vector3 PredictPosition(Vector3 pos) {
		throw new NotImplementedException();
	}
	
	// Not needed
	override public Move Copy() {
		throw new NotImplementedException();
	}
	
	// For debugging
	override public string ToString() {
		return string.Format("Velocity: {0}, Speed: {1}, Acceleration {2}"
		                     + " omega: {3}, turn {6}, t: {4}, r: {5}",
		                     velocity, speed, acceleration, omega, t, r, turn);
	}
}