using System.Collections.Generic;
using UnityEngine;

public interface IVehicleState<T> where T : IVehicleState<T> {

	// Computes distance in state space, returns null if the
	// State t is not reachable
	// Function is comutative, a.Distance(b) == b.Distance(a)
	float Distance(T other);

	// Returns a list of moves that are needed to perform in order to reach
	// the other state
	List<Move> MovesTo(T other);

	// Returns location of the state
	Vector3 Location();

	bool Equals(object other);
}