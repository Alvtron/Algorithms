using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Library
{
    public enum Traversal
    {
        PreOrder,
        InOrder,
        PostOrder
    }

    public abstract class Tree<T> : IEnumerable<T> where T : IComparable<T>
    {
        public Node<T> Node { get; set; }
        public Tree<T> Left { get; set; }
        public Tree<T> Right { get; set; }
        public bool IsReadOnly { get; set; } = false;
        public int Count => (Node?.Amount ?? 0) + (Left?.Count ?? 0) + (Right?.Count ?? 0);
        public int CountUnique => (Node != null ? 1 : 0) + (Left?.CountUnique ?? 0) + (Right?.CountUnique ?? 0);
        public double Balance => -1.00 * (((Left?.Count ?? 0.00) - (Right?.Count ?? 0.00)) / Count);
        public double BalanceUnique => -1.00 * (((Left?.CountUnique ?? 0.00) - (Right?.CountUnique ?? 0.00)) / CountUnique);

        private readonly Action<Node<T>> _printNode = node => Debug.Write(node + ",");

        public abstract void Add(T item);

        public void Add(IEnumerable<T> list)
        {
            if (list == null) return;
            foreach (var item in list) Add(item);
        }

        public bool Remove(T item)
        {
            if (IsReadOnly)
                throw new InvalidOperationException("This tree is read-only.");

            var rest = RemoveAndReturnRest(item);
            if (rest == null) return true;

            foreach (var node in rest) Add(node);
            return true;
        }

        private IList<Tree<T>> RemoveAndReturnRest(T item)
        {
            if (item == null)
                return null;

            if (item.Equals(Node.Data))
            {
                var list = new List<Tree<T>>();
                if (Left != null) list.Add(Left);
                if (Right != null) list.Add(Right);
                Clear();
                return list;
            }

            // Contains left side
            if (item.CompareTo(Node.Data) > 0 && Left != null)
                return Left.RemoveAndReturnRest(item);

            // Contains right side
            if (item.CompareTo(Node.Data) < 0 && Right != null)
                return Right.RemoveAndReturnRest(item);

            return null;
        }

        public bool Contains(T item)
        {
            if (item == null || Node == null)
                return false;

            // Contains left side
            if (item.CompareTo(Node.Data) > 0 && Left != null)
                return Left.Contains(item);

            // Contains right side
            if (item.CompareTo(Node.Data) < 0 && Right != null)
                return Right.Contains(item);

            return true;
        }

        public int CountItem(T item)
        {
            if (item == null || Node == null)
                return 0;

            // Contains left side
            if (item.CompareTo(Node.Data) > 0 && Left != null)
                return Left.CountItem(item);

            // Contains right side
            if (item.CompareTo(Node.Data) < 0 && Right != null)
                return Right.CountItem(item);

            return Node.Amount;
        }

        public void Clear()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("This tree is read-only.");

            Node = null;
            Left = null;
            Right = null;
        }

        public void Print(Traversal traversal)
        {
            switch (traversal)
            {
                case Traversal.PreOrder:
                    PreOrderTraversal(_printNode, this);
                    break;
                case Traversal.InOrder:
                    InOrderTraversal(_printNode, this);
                    break;
                case Traversal.PostOrder:
                    PostOrderTraversal(_printNode, this);
                    break;
                default:
                    return;
            }
        }

        private static void PreOrderTraversal(Action<Node<T>> action, Tree<T> tree)
        {
            if (tree == null) return;

            action(tree.Node);
            PreOrderTraversal(action, tree.Left);
            PreOrderTraversal(action, tree.Right);
        }

        private static void InOrderTraversal(Action<Node<T>> action, Tree<T> tree)
        {
            if (tree == null) return;

            InOrderTraversal(action, tree.Left);
            action(tree.Node);
            InOrderTraversal(action, tree.Right);
        }
        private static void PostOrderTraversal(Action<Node<T>> action, Tree<T> tree)
        {
            if (tree == null) return;

            PostOrderTraversal(action, tree.Left);
            PostOrderTraversal(action, tree.Right);
            action(tree.Node);
        }

        public IList<T> ToList(Traversal traversal = Traversal.InOrder, bool uniqueOnly = false)
        {
            switch (traversal)
            {
                case Traversal.PreOrder:
                    return PreOrderList(this, uniqueOnly);
                case Traversal.InOrder:
                    return InOrderList(this, uniqueOnly);
                case Traversal.PostOrder:
                    return PostOrderList(this, uniqueOnly);
                default:
                    return null;
            }
        }

        private static List<T> PreOrderList(Tree<T> tree, bool uniqueOnly)
        {
            if (tree == null) return null;

            var list = new List<T>();

            if (uniqueOnly) list.Add(tree.Node.Data);
            else list.AddRange(Enumerable.Repeat(tree.Node.Data, tree.Node.Amount).ToList());

            if (tree.Left != null)
                list.AddRange(PostOrderList(tree.Left, uniqueOnly));

            if (tree.Right != null)
                list.AddRange(PostOrderList(tree.Right, uniqueOnly));

            return list;
        }

        private static List<T> InOrderList(Tree<T> tree, bool uniqueOnly)
        {
            if (tree == null) return null;

            var list = new List<T>();

            if (tree.Left != null)
                list.AddRange(PostOrderList(tree.Left, uniqueOnly));

            if (uniqueOnly) list.Add(tree.Node.Data);
            else list.AddRange(Enumerable.Repeat(tree.Node.Data, tree.Node.Amount).ToList());

            if (tree.Right != null)
                list.AddRange(PostOrderList(tree.Right, uniqueOnly));

            return list;
        }
        private static List<T> PostOrderList(Tree<T> tree, bool uniqueOnly)
        {
            if (tree == null) return null;

            var list = new List<T>();

            if (tree.Left != null)
                list.AddRange(PostOrderList(tree.Left, uniqueOnly));

            if (tree.Right != null)
                list.AddRange(PostOrderList(tree.Right, uniqueOnly));

            if (uniqueOnly) list.Add(tree.Node.Data);
            else list.AddRange(Enumerable.Repeat(tree.Node.Data, tree.Node.Amount).ToList());

            return list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ToList().GetEnumerator();
        }
    }
}
