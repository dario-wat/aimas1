using UnityEngine;
using System.Collections;
using System;

public class DiscreteState : IState {

	// Coordinates in 2D discrete space
	public readonly int x;
	public readonly int y;


	// Constructor with x and y coordinates
	public DiscreteState(int x, int y) {
		this.x = x;
		this.y = y;
	}

	// Constructs with strings of coordinates
	public DiscreteState(string x, string y)
		: this(int.Parse(x), int.Parse(y)) {
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
		return Math.Abs(x) + 31 * Math.Abs(y);
	}

	// Equality
	public override bool Equals(object other) {
		if (!(other is DiscreteState)) {
			return false;
		}
		DiscreteState o = (DiscreteState) other;
		return this.x == o.x && this.y == o.y;
	}

	// For printing the state
	public override String ToString() {
		return String.Format("({0}, {1})", x, y);
	}
}
