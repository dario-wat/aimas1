using UnityEngine;

public interface IState {

	// Returns Vector3 representation of the state
	Vector3 ToVector3();

	// State to be hashable must implement these 2 methods
	int GetHashCode();
	bool Equals(object other);
}
