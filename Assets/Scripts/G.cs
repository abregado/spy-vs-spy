using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class G {

    public enum GridDir {
        None,
        North,
        South,
        West,
        East,
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest,
    }
    
    public enum ItemType {
        None,
        Briefcase,
        Rope,
        Disguise,
        Money,
        Bomb,
    }

    public enum FurnitureTypes {
        None,
        Cabinet,
        Desk,
        Drawer,
        Rug,
        Lamp,
        TrashCan,
        WallSafe
    }

}
