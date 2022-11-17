﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCS : MonoBehaviour
{
    public static LinkedList<Step> steps = new();
    public static PriorityQueue<SearchEntry> open;
    public static Dictionary<TileBlock, TileBlock> parent = new();

    public static void FindPath(TileBlock startBlock, TileBlock endBlock)
    {
        PathFinder.isIterativeDepending = false;
        steps.Clear();
        parent.Clear();

        Vector2 startPos = startBlock.TileNode.Index();
        int startX = Mathf.RoundToInt(startPos.x);
        int startY = Mathf.RoundToInt(startPos.y);
        steps.AddLast(new Step($"*** Bắt đầu tại ({startX}, {startY}) ***"));
        open = new();
        open.Enqueue(new SearchEntry(startBlock, 0));
        parent[startBlock] = startBlock;
        while (open.Count > 0)
        {
            SearchEntry curNode = open.Dequeue();
            if (curNode.Block != startBlock)
            {
                steps.AddLast(new PriorityStep(curNode.Block, TileStatus.SEEN, TileStatus.VISITED, curNode.Value));
            }
            if (curNode.Block == endBlock)
            {
                return;
            }
            foreach (TileMap.TileNode tileNode in curNode.Block.TileNode.Neighbours)
            {
                int costToNext = curNode.Value + tileNode.Cost;
                if (!parent.ContainsKey(tileNode.TileBlock))
                {
                    steps.AddLast(new PriorityStep(tileNode.TileBlock, TileStatus.NORMAL, TileStatus.SEEN, costToNext));
                    parent[tileNode.TileBlock] = curNode.Block;
                    open.Enqueue(new(tileNode.TileBlock, costToNext));
                }
                else if (tileNode.TileBlock.Node.Value > costToNext)
                {
                    parent[tileNode.TileBlock] = curNode.Block;
                    open.Remove(tileNode.TileBlock.Node);
                    steps.AddLast(new PriorityStep(tileNode.TileBlock,
                        open.Contains(tileNode.TileBlock.Node) ? TileStatus.SEEN : TileStatus.VISITED, 
                        TileStatus.SEEN, costToNext));
                    open.Enqueue(new(tileNode.TileBlock, costToNext));
                }
            }
        }
        parent.Clear();
    }
}
