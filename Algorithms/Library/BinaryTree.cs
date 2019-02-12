using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class BinaryTree<T> : Tree<T> where T : IComparable<T>
    {
        public override void Add(T item)
        {
            if (IsReadOnly)
                throw new InvalidOperationException("This tree is read-only.");

            if (item == null) return;

            // This node is available
            if (Node == null) Node = new Node<T>(item);

            // item is a descendant of this node
            else if (item.CompareTo(Node.Data) > 0)
            {
                if (Left == null)
                {
                    Left = new BinaryTree<T> { Node = new Node<T>(item) };
                    return;
                }

                Left.Add(item);
            }

            // item is a ascendant of this node
            else if (item.CompareTo(Node.Data) < 0)
            {
                if (Right == null)
                {
                    Right = new BinaryTree<T> { Node = new Node<T>(item) };
                    return;
                }

                Right.Add(item);
            }

            // Item is equal of this node
            else Node.Amount++;
        }
    }
}
