using System.Collections.Generic;
using UnityEngine;
using C5;
using System.Linq;
//using System;
public class PathfinderScript : MonoBehaviour
{

	TileMap map;

	bool pathfindingMode = false;
	Dictionary<Vector2, Vector2> pathingData;
	LineRenderer pathRender;



	private void Start()
	{
		generatePathfinder();
	}

	void generatePathfinder()
	{
		pathRender = gameObject.AddComponent<LineRenderer>();
		pathRender.enabled = false;
		pathRender.startWidth = pathRender.endWidth = 0.1f;
		pathRender.material = (Material)Resources.Load("SelectorMaterial");
	}
	
	public void setMap(TileMap m) { map = m; }
	public void getMovementPaths(Vector2 startPosition,int moveDistance, bool highlight)
	{
		//Get no go areas from player controllers
		var unitPositions = Controller.getUnitPositions();
		// Pathfinding stuff
		IntervalHeap<PathfinderNode> frontier = new IntervalHeap<PathfinderNode>(new PathfinderNode(new Vector2(), 0));
		frontier.Add(new PathfinderNode(startPosition, 0));
		Dictionary<Vector2,Vector2> cameFrom = new Dictionary<Vector2,Vector2>();
		Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
		cameFrom.Add(startPosition, new Vector2(-1,-1));
		costSoFar.Add(startPosition, 0);

		while(frontier.Count > 0)
		{
			//Get current
			PathfinderNode current = frontier.FindMin();
			frontier.DeleteMin();

			// iterate through neighbours
			foreach (Vector2 next in map.getNeibour(current.position))
			{
				if (unitPositions.Contains(next))
					continue;
				int newCost = map.tileTypes[map.tiles[(int)next.x, (int)next.y]].cost + costSoFar[current.position];
				if (newCost <= moveDistance && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]))
				{

					int priority = newCost;
					if (costSoFar.ContainsKey(next))
					{
						costSoFar[next] = newCost;
						
						PathfinderNode newNode = new PathfinderNode(next, priority);
						frontier.Add(newNode);

						cameFrom[next] = current.position;
					}
					else
					{ 	
						costSoFar.Add(next, newCost);
						PathfinderNode newNode = new PathfinderNode(next, priority);
						frontier.Add(newNode);

						cameFrom.Add(next, current.position);
					}
					
					
				}
			}

		}

		if(highlight)
		{
			map.highlightArea(new List<Vector2>(costSoFar.Keys), new Color(0,1,1));
		}
		pathingData = cameFrom;
	}
	public void setPathfinding(bool value)
	{
		if (value)
		{
			pathfindingMode = true;
			pathRender.enabled = true;
			pathRender.SetPositions(new Vector3[0]);
		}
		else
		{
			pathfindingMode = false;
			pathRender.enabled = false;
		}
	}
	public bool getPathfinding()
	{
		return pathfindingMode;
	}
	public Dictionary<Vector2, Vector2>.KeyCollection getReachableLocations() { return pathingData.Keys; }
    public Vector2[] getReachableLocationsWithin(int moveDistance) { return pathingData.Keys.Where(pos => {print(pos + " is " + getPath(pos).Length); return getPath(pos).Length < moveDistance; }).ToArray<Vector2>(); } // TODO - check length of path and only return paths shorter than move distance
    public Vector2[] getPath(Vector2 position)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 current = position;
        while (current != new Vector2(-1, -1))
        {
            path.Add(current);
            current = pathingData[current];
        }
        path.Reverse();
        return path.ToArray();
    }

    public Vector2[] drawPath(Vector2 end, bool visible = true, bool includeTarget = true)
	{

		// Generate path
		if (!pathingData.ContainsKey(end) && includeTarget == true)
		{

			setPathfinding(false);
			return null;
		}

		List<Vector2> path = new List<Vector2>();
		List<Vector3> linePath = new List<Vector3>();
		Vector2 current = end;
		while (current != new Vector2(-1, -1))
		{
			linePath.Add(new Vector3(current.x, 0.7f, current.y));
			path.Add(current);
			current = pathingData[current];
		}
		//linePath.Reverse();
		path.Reverse();

		//if not include target remove 1 
		if (!includeTarget)
			path.RemoveAt(path.Count - 1);


		//Render path
		setPathfinding(false);

		if (visible)
		{
			pathRender.numPositions = linePath.Count;
			pathRender.SetPositions(linePath.ToArray());
		}
		else
		{
			pathRender.numPositions = 0;
		}
		setPathfinding(true);
		return path.ToArray();

		//return null;
	}

}

