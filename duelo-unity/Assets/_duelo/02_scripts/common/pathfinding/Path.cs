namespace Duelo.Common.Pathfinding
{
    using System.Collections;
    using System.Collections.Generic;

    public class Path<TNode> : IEnumerable<Path<TNode>>
    {
        #region Public Properties
        public TNode Data { get; private set; }
        public Path<TNode> Previous { get; private set; }
        public Path<TNode> Next { get; private set; }
        public float TotalCost { get; private set; }
        #endregion

        #region Initialization
        private Path(TNode lastStep, Path<TNode> previousSteps, float totalCost)
        {
            Data = lastStep;
            Previous = previousSteps;
            TotalCost = totalCost;
        }

        public Path(TNode start) : this(start, null, 0) { }
        #endregion

        #region Public Methods
        public Path<TNode> AddStep(TNode step, float cost)
        {
            return new Path<TNode>(step, this, TotalCost + cost);
        }

        public void SetNextNodes()
        {
            Path<TNode> next = this;
            Path<TNode> current = this.Previous;

            while (current != null)
            {
                current.Next = next;
                next = current;
                current = current.Previous;
            }
        }

        public List<TNode> AsNodeList()
        {
            var list = new List<TNode>();
            for (Path<TNode> p = this; p != null; p = p.Previous)
            {
                list.Add(p.Data);
            }

            return list;
        }

        public Stack<Path<TNode>> AsStack()
        {
            Stack<Path<TNode>> stack = new Stack<Path<TNode>>();
            for (Path<TNode> p = this; p != null; p = p.Previous)
            {
                stack.Push(p);
            }

            return stack;
        }

        public Queue<Path<TNode>> AsQueue()
        {
            Queue<Path<TNode>> queue = new Queue<Path<TNode>>();
            for (Path<TNode> p = this; p != null; p = p.Previous)
            {
                queue.Enqueue(p);
            }

            return queue;
        }
        #endregion

        #region IEnumerable Implementation
        public IEnumerator<Path<TNode>> GetEnumerator()
        {
            for (Path<TNode> p = this; p != null; p = p.Previous)
                yield return p;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}