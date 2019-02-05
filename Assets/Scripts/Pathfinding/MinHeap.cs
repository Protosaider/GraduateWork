using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IHeapItem<T> {

    private const int HEAP_DEFAULT_SIZE = 32;
    private const int HEAP_GROWTH_FACTOR = 2;

    private T[] items = null;
    private int itemsCount = 0;
    private int heapSize = HEAP_DEFAULT_SIZE;

    public int Count
    {
        get
        {
            return itemsCount;
        }
        private set { }
    }

    public MinHeap()
    {
        items = new T[HEAP_DEFAULT_SIZE];
        heapSize = HEAP_DEFAULT_SIZE;
    }

    public MinHeap(int heapSize)
    {
        if (heapSize < 1)
        {
            throw new System.ArgumentException("Heap size must be greater than zero");
        }           
        items = new T[heapSize];
        this.heapSize = heapSize;
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
        T firstItem = items[0];
        itemsCount--;
        items[0] = items[itemsCount];
        items[0].HeapIndex = 0;
        //items[itemsCount] = default(T);
        SortDown(items[0]);
        return firstItem;
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
            //int childIndexL = item.HeapIndex << 1 + 1;
            //int childIndexR = item.HeapIndex << 1 + 2;
            int childIndexL = item.HeapIndex * 2 + 1;
            int childIndexR = item.HeapIndex * 2 + 2;
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
        //int parentHeapIndex = (item.HeapIndex - 1) >> 1;
        int parentHeapIndex = (item.HeapIndex - 1) / 2;
        //Debug.Log("Index: " + parentHeapIndex + " dividing " + (item.HeapIndex - 1) / 2 + " heapIndex " + item.HeapIndex);

        while (true)
        {
            T parentItem = items[parentHeapIndex];
            if (item.CompareTo(parentItem) > 0) //item smaller then parent
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            //parentHeapIndex = (item.HeapIndex - 1) >> 1;
            parentHeapIndex = (item.HeapIndex - 1) / 2;
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
  
}

