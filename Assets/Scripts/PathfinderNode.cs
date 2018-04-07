using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderNode : IComparer<PathfinderNode>
{
	public Vector2 position;
	public float weight;
	public PathfinderNode(Vector2 p, float w)
	{
		position = p;
		weight = w;
	}
		
	public override bool Equals(object obj)
	{
		if (obj == null)
			return false;
		PathfinderNode other = obj as PathfinderNode;

		if (other == null)
			return false;

		return position == other.position;
	}
	public override int GetHashCode()
	{
		return position.x.GetHashCode() + position.y.GetHashCode();
	}

	public int Compare(PathfinderNode a, PathfinderNode b)
	{
		if (a.weight > b.weight)
			return 1;
		else if (a.weight < b.weight)
			return -1;
		else
			return 0;
	}
}