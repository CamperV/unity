using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPriorityQueue<T>
{
	void Enqueue(int priority, T item);
	T Dequeue();
}

public class PriorityNode<T>
{
	public int priority;
	public T item;
	
	public PriorityNode(int p, T i) {
		priority = p;
		item = i;
	}
}

public class PriorityQueue<T> : IPriorityQueue<T>
{	
	PriorityHeap<T> queue = new PriorityHeap<T>();
	
	public int Count {
		get { return queue.Count; }
	}
	
	public void Enqueue(int priority, T item) {
		PriorityNode<T> node = new PriorityNode<T>(priority, item);
		queue.Insert(node);
	}
	
	public T Dequeue() {
		return queue.Pop().item;
	}
}


public class PriorityHeap<T>
{
	List<PriorityNode<T>> heapArray = new List<PriorityNode<T>>();
	int heapSize = -1;
	
	public int Count {
		get { return heapArray.Count; }
	}
	
	public void Insert(PriorityNode<T> node) {
		heapArray.Add(node);
		heapSize++;
		BuildMinHeap(heapSize);
	}
	
	public PriorityNode<T> Pop() {
		var node = heapArray[0];
		
		heapArray[0] = heapArray[heapSize];
		heapArray.RemoveAt(heapSize);
		heapSize--;
		RebalanceMinHeap(0);
		
		return node;
	}
	
	private void Swap(int i, int j) {
		var tmp = heapArray[i];
		heapArray[i] = heapArray[j];
		heapArray[j] = tmp;
	}
	
	// keep track of heap node's children via index
	private int LeftChild(int i) {
		return (i * 2) + 1;
	}
	// keep track of heap node's children via index
	private int RightChild(int i) {
		return (i * 2) + 2;
	}
	private int Parent(int i) {
		return (i - 1) / 2;
	}
	
	private void BuildMinHeap(int i) {
		while (i >= 0 && heapArray[Parent(i)].priority > heapArray[i].priority) {
			Swap(i, Parent(i));
			i = Parent(i);
		}
	}
	
	private void RebalanceMinHeap(int i) {
		int left  = LeftChild(i);
		int right = RightChild(i);
		
		int min = i;
		if (left <= heapSize  && heapArray[min].priority > heapArray[left].priority)  min = left;
		if (right <= heapSize && heapArray[min].priority > heapArray[right].priority) min = right;
		
		if (min != i) {
			Swap(min, i);
			RebalanceMinHeap(min);
		}
	}
}