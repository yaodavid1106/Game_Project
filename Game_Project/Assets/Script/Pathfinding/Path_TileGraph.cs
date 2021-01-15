﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph
{


	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph(World world)
	{


		nodes = new Dictionary<Tile,Path_Node<Tile>>();
		for (int x = 0; x < world.Width; x++)
		{
			for (int y = 0; y < world.Height; y++)
			{

				Tile t = world.GetTileAt(x,y);

				if (t.movementCost > 0)
				{
					Path_Node<Tile> n = new Path_Node<Tile>();
					n.data = t;
					nodes.Add(t,n);
				}
			}
		}

		Debug.Log("Path_TileGraph: Created" + nodes.Count);

		int edgeCount = 0; 
		foreach(Tile t in nodes.Keys)
		{
			Path_Node<Tile> n = nodes[t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

			Tile[] neighbours = t.GetNeighbours(true);

			for (int i = 0; i<neighbours.Length; i ++)
			{
				if(neighbours[i] != null && neighbours[i].movementCost>0)
				{
					//Cutting Corner
					if(ClippingCorner(t, neighbours[i]))
                    {
						continue;
                    }

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbours[i].movementCost;
					e.node = nodes[neighbours[i]];
					edges.Add(e);
					edgeCount++;
				}
			}

			n.edges = edges.ToArray();
		}
		Debug.Log("Path_TileGraph: Created" + edgeCount);


	}



	bool ClippingCorner(Tile curr, Tile neigh)
    {
		if(Mathf.Abs(curr.X-neigh.X) + Mathf.Abs(curr.Y-neigh.Y) == 2)
        {
			int dx = curr.X - neigh.X;
			int dy = curr.Y - neigh.Y;

			if (curr.world.GetTileAt(curr.X-dx,curr.Y).movementCost == 0)
            {
				return true;
            }

			if (curr.world.GetTileAt(curr.X, curr.Y - dy).movementCost == 0)
			{
				return true;
			}
		}

		return false;
    }

}
