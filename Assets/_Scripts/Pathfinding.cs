using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets._Scripts.GameObjects;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance { get; private set; }

        private IDictionary<GridPosition, PathfindingNode> allNodes;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            allNodes = new Dictionary<GridPosition, PathfindingNode>();

            LevelLoader.Instance.LevelLoaded += PostLevelLoaded;
        }

        private void PostLevelLoaded()
        {
            LoadPathfindingNodes();
        }

        /// <summary>Gets a path of grid positions from <see cref="from"/> to <see cref="to"/>. Returns null if there is no solution.</summary>
        public IList<GridPosition> GetPath(GridPosition from, GridPosition to)
        {
            var finalNode = GetFinalNodeUsingAStar(from, to);
            if (finalNode == null)
                return null;

            var node = finalNode;
            var positions = new List<GridPosition>();
            while (node.Parent != null)
            {
                positions.Add(node.PathfindingNode.GridPosition);
                node = node.Parent;
            }

            positions.Reverse();

            return positions;
        }

        private PathfindingTempNode GetFinalNodeUsingAStar(GridPosition from, GridPosition to)
        {
            var open = new List<PathfindingTempNode>(); // List isn't best data structure.
            var closed = new List<PathfindingTempNode>();

            var startingNode = GetNode(from);
            var endingNode = GetNode(to);

            if (startingNode == null || endingNode == null)
                throw new InvalidOperationException("Either starting or ending positions aren't in the pathfinding system!");

            if (IsNodeTraversable(startingNode) == false || IsNodeTraversable(endingNode) == false)
                throw new InvalidOperationException("Starting or ending nodes aren't traversable!");

            open.Add(new PathfindingTempNode(startingNode, null, 0, 0));

            while (open.Count > 0)
            {
                int qIndex;
                var q = FindWithLowestF(open, out qIndex);
                open.RemoveAt(qIndex);

                var successors = GetNeighbors(q.PathfindingNode);
                foreach (var successor in successors)
                {
                    var successorNode = new PathfindingTempNode(successor, q, q.G + 1, GuessCost(successor.GridPosition, endingNode.GridPosition));

                    if (successorNode.PathfindingNode.GridPosition == endingNode.GridPosition)
                    {
                        return successorNode;
                    }
                    
                    var isValid = true;

                    foreach (var openNode in open)
                    {
                        if (openNode.PathfindingNode.GridPosition == successorNode.PathfindingNode.GridPosition && openNode.F < successorNode.F)
                            isValid = false;
                    }

                    foreach (var closedNode in closed)
                    {
                        if (closedNode.PathfindingNode.GridPosition == successorNode.PathfindingNode.GridPosition && closedNode.F < successorNode.F)
                            isValid = false;
                    }

                    if(isValid)
                        open.Add(successorNode);
                }
                closed.Add(q);
            }

            return null; //No solution.
        }

        private IList<PathfindingNode> GetNeighbors(PathfindingNode node)
        {
            // TODO diagonal
            var n = node.GridPosition + new GridPosition(0, 1);
            var e = node.GridPosition + new GridPosition(1, 0);
            var s = node.GridPosition + new GridPosition(0, -1);
            var w = node.GridPosition + new GridPosition(-1, 0);
            var nodes = new List<PathfindingNode>();

            foreach (var position in new[] { n, e, s, w })
            {
                var successor = GetNode(position);
                if (successor != null && IsNodeTraversable(successor))
                {
                    nodes.Add(successor);
                }
            }

            return nodes;
        } 

        private static PathfindingTempNode FindWithLowestF(IList<PathfindingTempNode> nodes, out int index)
        {
            PathfindingTempNode withLowestF = null;
            index = -1;
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (withLowestF == null || node.F < withLowestF.F)
                {
                    withLowestF = node;
                    index = i;
                }
            }
            return withLowestF;
        } 

        private int GuessCost(GridPosition from, GridPosition to)
        {
            return Mathf.Abs(from.X - to.X) + Mathf.Abs(from.Y - to.Y);
        }

        private PathfindingNode GetNode(GridPosition position)
        {
            PathfindingNode node;
            allNodes.TryGetValue(position, out node);
            return node;
        }

        private static bool IsNodeTraversable(PathfindingNode node)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var obj in node.GameObjects)
            {
                if (obj.Layer == 2)
                {
                    if (obj.IsDynamic == false)
                        return false;

                    if (obj.IsTraversableAt(node.GridPosition) == false)
                        return false;
                }
            }
            return true;
        }

        private void LoadPathfindingNodes()
        {
            foreach (var obj in LevelLoader.Instance.AllLevelObjects)
            {
                var inGameObject = obj.GetInterfaceComponent<IInGameObject>();
                if (inGameObject == null)
                    throw new InvalidOperationException("Missing IInGameObject component.");

                if (inGameObject.StartGridPosition == null)
                    continue;

                var position = inGameObject.StartGridPosition.Value;
                PathfindingNode node;
                if (!allNodes.TryGetValue(position, out node))
                {
                    allNodes[position] = node = new PathfindingNode { GridPosition = position };
                }

                node.GameObjects.Add(inGameObject);
            }
        }
    }

    public class PathfindingNode
    {
        public IList<IInGameObject> GameObjects { get; set; }

        public GridPosition GridPosition { get; set; }

        public PathfindingNode()
        {
            GameObjects = new List<IInGameObject>();
        }
    }

    public class PathfindingTempNode
    {
        public PathfindingNode PathfindingNode { get; private set; }

        public PathfindingTempNode Parent { get; private set; }

        /// <summary>G + H</summary>
        public float F { get; private set; }

        /// <summary>Cost that it took to reach this node.</summary>
        public float G { get; private set; }

        /// <summary>Guess to how much it will cost to read the target node.</summary>]
        public float H { get; private set; }

        public PathfindingTempNode(PathfindingNode node, PathfindingTempNode parent, float g, float h)
        {
            PathfindingNode = node;
            Parent = parent;
            G = g;
            H = h;
            F = g + h;
        }
    }
}