using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Node<T> : IComparable where T : IComparable<T>
    {
        public T Data { get; set; }
        public int Amount { get; set; }

        public Node(T data, int amount = 1)
        {
            Data = data;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Data.ToString()}({Amount})";
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Node<T> node:
                    return node.Data.CompareTo(Data);
                default:
                    return 1;
            }
        }
    }
}
