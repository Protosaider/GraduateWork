using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPLeaf {

    public BoundsInt bounds;
    public BSPLeaf Left { get; set; }
    public BSPLeaf Right { get; set; }
    public int SizeX
    {
        get
        {
            return bounds.size.x;
        }
        private set { }
    }
    public int SizeZ
    {
        get
        {
            return bounds.size.z;
        }
        private set { }
    }
    public int CenterZ
    {
        get
        {
            return bounds.zMin + Mathf.FloorToInt(SizeZ * 0.5f);
        }
        private set { }
    }
    public int CenterX
    {
        get
        {
            return bounds.xMin + Mathf.FloorToInt(SizeX * 0.5f);
        }
        private set { }
    }

    public bool splitOrientationIsVertical
    {
        get;
        private set;
    }

    public BSPLeaf(Coordinate bottomLeft, int width, int height)
    {
        bounds = new BoundsInt((Vector3Int)bottomLeft, new Vector3Int(width, 2, height));       
        Left = null;
        Right = null;
    }
    public BSPLeaf(Vector3Int bottomLeft, int width, int height)
    {
        bounds = new BoundsInt(bottomLeft, new Vector3Int(width, 2, height));
        Left = null;
        Right = null;
    }
    public BSPLeaf(int x, int z, int width, int height)
    {
        bounds = new BoundsInt(new Vector3Int(x, 0, z), new Vector3Int(width, 2, height));
        Left = null;
        Right = null;
    }
    public BSPLeaf(int xMin, int zMin, int xMaxOrWidth, int zMaxOrHeight, bool isWidthHeight)
    {
        if (isWidthHeight)
        {
            bounds = new BoundsInt(new Vector3Int(xMin, 0, zMin), new Vector3Int(xMaxOrWidth, 2, zMaxOrHeight));
        }
        else
        {
            bounds = new BoundsInt(new Vector3Int(xMin, 0, zMin), new Vector3Int(xMaxOrWidth - 1, 2, zMaxOrHeight - 1));
        }
        Left = null;
        Right = null;
    }

    public void GetLeaves(ref List<BSPLeaf> list)
    {
        if (Left != null || Right != null)
        {
            if (Left != null)
            {
                Left.GetLeaves(ref list);
            }

            if (Right != null)
            {
                Right.GetLeaves(ref list);
            }
        }
        else
        {
            list.Add(this);
        }
    }

    public void GetLeaves(int depth, ref List<BSPLeaf> list)
    {
        if (list == null || depth < 1)
        {
            return;
        }
        if (depth == 1)
        {
            list.Add(this);
        }
        else
        {
            if (Left != null)
            {
                Left.GetLeaves(depth - 1, ref list);
            }
            if (Right != null)
            {
                Right.GetLeaves(depth - 1, ref list);
            }
        }
    }

    private bool CanSplit(int spaceSize, bool isVertical, ref BinarySpacePartitioningSettings settings)
    {
        if (isVertical)
        {
            return (spaceSize > (settings.minSpaceSizeVertical * settings.minSpaceSizeToSplitMiltiplier)) || (Random.value < settings.chanceToStopSpaceSplitting);
        }
        else
        {
            return (spaceSize > (settings.minSpaceSizeHorizontal * settings.minSpaceSizeToSplitMiltiplier)) || (Random.value < settings.chanceToStopSpaceSplitting);
        }
    }

    private bool AbleToSplit(ref BinarySpacePartitioningSettings settings)
    {
        bool canSplitVertical = (SizeX >= settings.minSpaceSizeVertical);
        bool canSplitHorizontal = (SizeZ >= settings.minSpaceSizeHorizontal);

        if (!(canSplitVertical || canSplitHorizontal))
        {
            return false;
        }
        else
        {
            if (canSplitVertical && canSplitHorizontal)
            {
                if (SizeX / SizeZ > settings.widthHeightRatioToSplitNotRandom) //width > height
                {
                    splitOrientationIsVertical = true;
                }
                else if (SizeZ / SizeX > settings.widthHeightRatioToSplitNotRandom)
                {
                    splitOrientationIsVertical = false;
                }
                else
                {
                    splitOrientationIsVertical = (Random.value < 0.5f);
                }
            }
            else if (canSplitVertical)
            {
                splitOrientationIsVertical = true;
            }
            else
            {
                splitOrientationIsVertical = false;
            }

            return true;
        }
    }


    public void Split(ref BinarySpacePartitioningSettings settings)
    {
        if (!AbleToSplit(ref settings))
        {
            return;
        }

        int whereToSplit;

        if (splitOrientationIsVertical)
        {
            if (settings.canShareSingleWall)
            {
                whereToSplit = Random.Range(0 + settings.minSplitSizeVertical - 1, SizeX - settings.minSplitSizeVertical + 1);
                Left = new BSPLeaf(bounds.xMin, bounds.zMin, whereToSplit, SizeZ);
                Right = new BSPLeaf(bounds.xMin + whereToSplit, bounds.zMin, SizeX - whereToSplit, SizeZ);
                if (CanSplit(whereToSplit, true, ref settings))
                {
                    Left.Split(ref settings);
                }
                if (CanSplit(SizeX - whereToSplit, true, ref settings))
                {
                    Right.Split(ref settings);
                }
            }
            else
            {
                whereToSplit = Random.Range(0 + settings.minSplitSizeVertical - 1, SizeX - settings.minSplitSizeVertical + 1);
                Left = new BSPLeaf(bounds.xMin, bounds.zMin, whereToSplit, SizeZ);
                Right = new BSPLeaf(bounds.xMin + whereToSplit + 1, bounds.zMin, SizeX - whereToSplit - 1, SizeZ);
                if (CanSplit(whereToSplit, true, ref settings))
                {
                    Left.Split(ref settings);
                }
                if (CanSplit(SizeX - whereToSplit - 1, true, ref settings))
                {
                    Right.Split(ref settings);
                }
            }
        }
        else
        {
            if (settings.canShareSingleWall)
            {
                whereToSplit = Random.Range(0 + settings.minSplitSizeHorizontal - 1, SizeZ - settings.minSplitSizeHorizontal + 1);
                Left = new BSPLeaf(bounds.xMin, bounds.zMin, SizeX, whereToSplit);
                Right = new BSPLeaf(bounds.xMin, bounds.zMin + whereToSplit, SizeX, SizeZ - whereToSplit);
                if (CanSplit(whereToSplit, false, ref settings))
                {
                    Left.Split(ref settings);
                }
                if (CanSplit(SizeZ - whereToSplit, false, ref settings))
                {
                    Right.Split(ref settings);
                }
            }
            else
            {
                whereToSplit = Random.Range(0 + settings.minSplitSizeHorizontal - 1, SizeZ - settings.minSplitSizeHorizontal + 1);
                Left = new BSPLeaf(bounds.xMin, bounds.zMin, SizeX, whereToSplit);
                Right = new BSPLeaf(bounds.xMin, bounds.zMin + whereToSplit + 1, SizeX, SizeZ - whereToSplit - 1);
                if (CanSplit(whereToSplit, false, ref settings))
                {
                    Left.Split(ref settings);
                }
                if (CanSplit(SizeZ - whereToSplit - 1, false, ref settings))
                {
                    Right.Split(ref settings);
                }
            }
        }
    }
}
