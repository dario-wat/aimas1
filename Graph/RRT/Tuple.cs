using UnityEngine;
using System.Collections;

// Just to help us return pairs
public class Tuple<T1, T2> {

	public readonly T1 _1;
	public readonly T2 _2;
	
	public Tuple(T1 _1, T2 _2) {
		this._1 = _1;
		this._2 = _2;
	}
}
