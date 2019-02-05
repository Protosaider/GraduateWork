using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayMSTRoom
{
    public int index;
    public int x;
    public int y;

    public int width;
    public int height;

    public bool isMainRoom = false;
    public bool isPathRoom = false;

    //AABB inclusive
    public bool CollidesWith(DelaunayMSTRoom room)
    {
        return !((room.x >= this.x + this.width) || (room.y >= this.y + this.height) || (room.x + room.width <= this.x) || (room.y + room.height <= this.y));
    }

    public void Shift(int shiftX, int shiftY)
    {
        x += shiftX;
        y += shiftY;
    }
}
