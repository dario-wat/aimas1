using System.Collections.Generic;
using UnityEngine;

public interface IVehicleState<T> where T : IVehicleState<T> {

	// Vector2 representation of the state's location
	Vector2 vec2 { get; }

	// Vector3 representation of the state's location
	Vector3 vec3 { get; }

	// Computes distance in state space, returns null if the
	// State t is not reachable
	// Function is comutative, a.Distance(b) == b.Distance(a)
	float Distance(T other);

	// Returns a list of moves that are needed to perform in order to reach
	// the other state, and the state it has actually reached
	Tuple<List<Move>, T> MovesTo(T other);

	bool Equals(object other);
}