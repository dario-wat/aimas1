using UnityEngine;
using System.Collections;
using System;

public class ContinuousState : IState {

	// Coordinates in 2D discrete space
	public readonly float x;
	public readonly float y;


	// Constructor with x and y coordinates
	public ContinuousState(float x, float y) {
		this.x = x;
		this.y = y;
	}

	// Constructs with strings of coordinates
	public ContinuousState(string x, string y)
		: this(float.Parse(x), float.Parse(y)) {
	}

	// Converts coordinates to vector
	public Vector3 ToVector3() {
		return new Vector3(x, 0.0f, y);
	}

	// Converts to Vector2
	public Vector2 ToVector2() {
		return new Vector2(x, y);
	}

	// Hashcode of the state
	public override int GetHashCode() {
		return x.GetHashCode() + 31 * y.GetHashCode();
	}

	// Equality
	public override bool Equals(object other) {
		if (!(other is ContinuousState)) {
			return false;
		}
		ContinuousState o = (ContinuousState) other;
		return this.x.Equals(o.x) && this.y.Equals(o.y);
	}

	// For printing the state
	public override String ToString() {
		return String.Format("({0}, {1})", x, y);
	}
}
