// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// çÏê¨ì˙10åé18ì˙:
// çÏê¨é“:å¥ìc
// ---------------------------------------------------------using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlockSet
{
    private MinoScript _minoScript = default;
    private int[,] TMinoTop = new int[4, 4]{
        {0,2,0,0},
        {2,2,2,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] TMinoRight = new int[4, 4]{
        {0,2,0,0},
        {0,2,2,0},
        {0,2,0,0},
        {0,0,0,0}
    };

    private int[,] TMinoBottom = new int[4, 4]{
        {2,2,2,0},
        {0,2,0,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] TMinoLeft = new int[4, 4]{
        {0,2,0,0},
        {2,2,0,0},
        {0,2,0,0},
        {0,0,0,0}
    };

    private int[,] JMInoRight = new int[4, 4]{
        {0,2,2,0},
        {0,2,0,0},
        {0,2,0,0},
        {0,0,0,0}
    };

    private int[,] JMinoBottom = new int[4, 4]{
        {0,0,0,0},
        {2,2,2,0},
        {0,0,2,0},
        {0,0,0,0}
    };

    private int[,] JMinoLeft = new int[4, 4]{
        {0,2,0,0},
        {0,2,0,0},
        {2,2,0,0},
        {0,0,0,0}
    };

    private int[,] JMinoTop = new int[4, 4]{
        {2,0,0,0},
        {2,2,2,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] LMinoLeft = new int[4, 4]{
        {2,2,0,0},
        {0,2,0,0},
        {0,2,0,0},
        {0,0,0,0}
    };

    private int[,] LMinoTop = new int[4, 4]{
        {0,0,2,0},
        {2,2,2,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] LMinoRight = new int[4, 4]{
        {0,2,0,0},
        {0,2,0,0},
        {0,2,2,0},
        {0,0,0,0}
    };

    private int[,] LMinoBottom = new int[4, 4]{
        {0,0,0,0},
        {2,2,2,0},
        {2,0,0,0},
        {0,0,0,0}
    };

    private int[,] SMinoSide = new int[4, 4]{
        {2,0,0,0},
        {2,2,0,0},
        {0,2,0,0},
        {0,0,0,0}
    };

    private int[,] SMinoTop = new int[4, 4]{
        {0,2,2,0},
        {2,2,0,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] ZMinoSide = new int[4, 4]{
        {0,2,0,0},
        {2,2,0,0},
        {2,0,0,0},
        {0,0,0,0}
    };

    private int[,] ZMinoTop = new int[4, 4]{
        {2,2,0,0},
        {0,2,2,0},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] IMinoTop = new int[4, 4]{
        {0,2,0,0},
        {0,2,0,0},
        {0,2,0,0},
        {0,2,0,0}
    };

    private int[,] IMinoSide = new int[4, 4]{
        {0,0,0,0},
        {2,2,2,2},
        {0,0,0,0},
        {0,0,0,0}
    };

    private int[,] OMinoTop = new int[4, 4]{
        {2,2,0,0},
        {2,2,0,0},
        {0,0,0,0},
        {0,0,0,0}
    };
    public int[,] set(int blockNum, int rot)
    {
        if (blockNum == 0)
        {
            if (rot == 0) return TMinoTop;
            else if (rot == 1) return TMinoRight;
            else if (rot == 2) return TMinoBottom;
            else return TMinoLeft;
        }
        else if (blockNum == 1)
        {
            if (rot == 0) return JMInoRight;
            else if (rot == 1) return JMinoBottom;
            else if (rot == 2) return JMinoLeft;
            else return JMinoTop;
        }
        else if (blockNum == 2)
        {
            if (rot == 0) return LMinoLeft;
            else if (rot == 1) return LMinoTop;
            else if (rot == 2) return LMinoRight;
            else return LMinoBottom;
        }
        else if (blockNum == 3)
        {
            if (rot == 0 | rot == 2) return SMinoSide;
            else return SMinoTop;

        }
        else if (blockNum == 4)
        {
            if (rot == 0 | rot == 2) return ZMinoSide;
            else return ZMinoTop;

        }
        else if (blockNum == 5)
        {
            if (rot == 0 | rot == 2) return IMinoTop;
            else return IMinoSide;

        }
        else if (blockNum == 6)
        {
            return OMinoTop;
        }
        else
        {
            return TMinoTop;
        }
    }
}
