using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class G {

    public const int MAX_HEALTH = 30;
    public const float RESPAWN_TIME = 15f;
    public const float TIME_BEFORE_DEATH_ROOM_CHANGE = 2f;
    
    public const float ATTACK_MOVE_SPEED = 1.0f;
    public const float DAMAGED_MOVE_SPEED = 1.0f;
    public const float MAX_MOVE_SPEED = 3.0f;
    public const float SPEED_RESET_TIME = 0.5f;

    public const float TIME_LIMIT = 300f;

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
