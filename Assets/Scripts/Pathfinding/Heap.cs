using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Heap<T> where T : IHeapItem<T>
{
    private const int HEAP_DEFAULT_SIZE = 32;
    private const int HEAP_GROWTH_FACTOR = 2;

    private const int INDEX = 1;

    private T[] items = null;
    private int itemsCount = INDEX;
    private int heapSize = HEAP_DEFAULT_SIZE;

    public int Count
    {
        get
        {
            return itemsCount - INDEX;
        }
        private set { }
    }

    public Heap()
    {
        items = new T[HEAP_DEFAULT_SIZE + INDEX];
        heapSize = HEAP_DEFAULT_SIZE + INDEX;
    }

    public Heap(int heapSize)
    {
        if (heapSize < 1)
        {
            throw new System.ArgumentException("Heap size must be greater than zero");
        }
        items = new T[heapSize + INDEX];
        this.heapSize = heapSize + INDEX;
    }

    public void Add(T item)
    {
        item.HeapIndex = itemsCount;
        items[itemsCount] = item;
        SortUp(item);
        itemsCount++;
    }

    public T Remove()
    {
        T firstItem = items[INDEX];
        itemsCount--;
        items[INDEX] = items[itemsCount];
        items[INDEX].HeapIndex = INDEX;
        //items[itemsCount] = default(T);
        SortDown(items[INDEX]);
        return firstItem;
    }

    public T Peek()
    {
        return items[INDEX];
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexL = item.HeapIndex << 1;
            int childIndexR = childIndexL + 1;

            int swapIndex = 0;

            if (childIndexL < itemsCount)
            {
                swapIndex = childIndexL;
                if (childIndexR < itemsCount)
                {
                    if (items[childIndexL].CompareTo(items[childIndexR]) < 0)
                    {
                        swapIndex = childIndexR;
                    }
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                    //Swap(ref item, ref items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentHeapIndex = item.HeapIndex >> 1;

        while (parentHeapIndex > 0)
        {

            T parentItem = items[parentHeapIndex];
            if (item.CompareTo(parentItem) > 0) //item smaller then parent
            {
                Swap(item, parentItem);
                //Swap(ref item, ref parentItem);
            }
            else
            {
                break;
            }
            parentHeapIndex = item.HeapIndex >> 1;
        }
    }

    private void Swap(T lItem, T rItem)
    {
        items[lItem.HeapIndex] = rItem;
        items[rItem.HeapIndex] = lItem;

        int itemIndex = lItem.HeapIndex;
        lItem.HeapIndex = rItem.HeapIndex;
        rItem.HeapIndex = itemIndex;
    }

    private void Swap(ref T lItem, ref T rItem)
    {
        T item = lItem;
        items[lItem.HeapIndex] = rItem;
        items[rItem.HeapIndex] = item;

        lItem.HeapIndex = rItem.HeapIndex;
        rItem.HeapIndex = item.HeapIndex;
    }

}
