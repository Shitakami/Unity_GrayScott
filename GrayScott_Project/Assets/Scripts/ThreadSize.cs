using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ThreadSize
{
    public int x;
    public int y;
    public int z;

    public ThreadSize(uint x, uint y, uint z)
    {
        this.x = (int) x;
        this.y = (int) y;
        this.z = (int) z;
    }

}