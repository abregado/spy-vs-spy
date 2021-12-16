using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class G {

    public const float RESPAWN_TIME = 3f;

    public enum GridDir {
        None,
        North,
        South,
        West,
        East,
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
