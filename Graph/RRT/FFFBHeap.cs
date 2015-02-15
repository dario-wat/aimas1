using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
	This is Binary Heap implementation of Max Priority Queue. Max is needed to
	keep smallest values in the heap.
	FFFB stands for:
		Fixed - it has the same size, each new point if it is smaller than
				maximum will push maximum value outside
		Fast -  it is very fast, fastest possible, O(logn) insertion,
				heavily optimized. they dont get any faster than this
		Float - values for inserting are floats
		Binary - it's binary :D
**/
public class FFFBHeap<Val> : IEnumerable<Val> {

	// Capacity of the heap
	private readonly int M;

	// Current size of the heap
	private int size;

	// Heap itself
	private Val[] vals;
	private float[] heap;


	// Constructor with initial size
	public FFFBHeap(int M) {
		this.M = M;
		this.vals = new Val[M+1];
		this.heap = new float[M+1];
		this.size = 0;
	}

	// Insert in fixed size heap
	public void Insert(float d, Val v) {
		if (size < M) {
			size++;
			heap[size] = d;
			vals[size] = v;
			Swim();
		} else if (d < heap[1]) {
			heap[1] = d;
			vals[1] = v;
			Sink();
		}
	}

	// Swap 2 values in heap
	private void Swap(int i, int j) {
		float tf = heap[i];
		heap[i] = heap[j];
		heap[j] = tf;
		Val tv = vals[i];
		vals[i] = vals[j];
		vals[j] = tv;
	}

	// Swim the last element
	private void Swim() {
		int i = size;
		while (i > 1 && heap[i >> 1] < heap[i]) {
			Swap(i >> 1, i);
			i >>= 1;
		}
	}

	// Sink the root
	private void Sink() {
		int i = 1;
		while (i <= size >> 1) {
			int j = i << 1;
			if (j < size && heap[j] < heap[j+1]) { j++; }
			if (heap[i] > heap[j]) { break; }
			Swap(i, j);
			i = j;
		}
	}

	// Return heap iterator
	public IEnumerator<Val> GetEnumerator() {
		return vals.Skip(1).Take(size).GetEnumerator();
	}

	// Needed to make everything compile
	IEnumerator IEnumerable.GetEnumerator() {
		return this.GetEnumerator();
	}

}
