﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class Path_AStar 
{
    Queue<Tile> path;

   public Path_AStar(World world, Tile tileStart, Tile tileEnd)
    {
        if (world.tileGraph == null)
        {
            world.tileGraph = new Path_TileGraph(world);
        }

        Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

        if (nodes.ContainsKey(tileStart) == false)
        {
            Debug.LogError("Path_AStar: The starting tile is not in the list of nodes");
            return;
        }
        if (nodes.ContainsKey(tileEnd) == false)
        {
            Debug.LogError("Path_AStar: The ending tile is not in the list of nodes");
            return;
        }

        Path_Node<Tile> start = nodes[tileStart];
        Path_Node<Tile> goal = nodes[tileEnd];

        List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();


        SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>>();
        OpenSet.Enqueue(start,0);

        Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();
        Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();
        foreach(Path_Node<Tile> n in nodes.Values)
        {
            g_score[n] = Mathf.Infinity;
        }
        g_score[start] = 0;

        Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();
        foreach(Path_Node<Tile> n in nodes.Values)
        {
            f_score[n] = Mathf.Infinity;
        }
        f_score[start] = heuristic_cost_estimate(start,goal);

        while(OpenSet.Count > 0)
        {
            Path_Node<Tile> current = OpenSet.Dequeue();

            if (current == goal)
            {
                reconstruct_path(Came_From, current);
                return;
            }

            ClosedSet.Add(current);

            foreach(Path_Edge<Tile> edge_neighbor in current.edges)
            {
                Path_Node<Tile> neighbor = edge_neighbor.node;
                if (ClosedSet.Contains(neighbor) == true)
                {
                    continue;
                }
                float tentative_g_score = g_score[current] + dist_between(current, neighbor);

                if( OpenSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor])
                {
                    continue;
                }
                Came_From[neighbor] = current;
                g_score[neighbor] = tentative_g_score;
                f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);

                if(OpenSet.Contains(neighbor) == false)
                {
                    OpenSet.Enqueue(neighbor, f_score[neighbor]);
                }
            }//foreeach loop
        }//while loop

        //Burn our all path -- fail to walk
        
    }


    float heuristic_cost_estimate(Path_Node<Tile> a, Path_Node<Tile> b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.data.X - b.data.X, 2) + Mathf.Pow(a.data.Y - b.data.Y, 2));
    }

    float dist_between(Path_Node<Tile> a , Path_Node<Tile> b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.data.X - b.data.X, 2) + Mathf.Pow(a.data.Y - b.data.Y, 2));
    }



    void reconstruct_path(Dictionary<Path_Node<Tile>,Path_Node<Tile>> Came_From, Path_Node<Tile> current)
    {
        Queue<Tile> total_path = new Queue<Tile>();

        total_path.Enqueue(current.data);
        while (Came_From.ContainsKey(current))
        {
            current = Came_From[current];
            total_path.Enqueue(current.data);
        }

        path = new Queue<Tile> (total_path.Reverse());
    }


    public Tile Dequeue()
    {
        return path.Dequeue();
    }

    public int Length()
    {
        if(path == null)
        {
            return 0;
        }
        return path.Count();
    }
}
