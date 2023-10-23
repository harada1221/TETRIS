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
    public int[,] set(MinoScript minoscript)
    {
        if (minoscript.GetminoType == MinoScript.MinoType.TMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.TOP) return TMinoTop;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return TMinoRight;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.BOTTOM) return TMinoBottom;
            else return TMinoLeft;
        }
        else if (minoscript.GetminoType == MinoScript.MinoType.JMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return JMInoRight;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.BOTTOM) return JMinoBottom;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.TOP) return JMinoTop;
            else return JMinoLeft;
        }
        else if (minoscript.GetminoType == MinoScript.MinoType.LMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.LEFT) return LMinoLeft;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.TOP) return LMinoTop;
            else if (minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return LMinoRight;
            else return LMinoBottom;
        }
        else if (minoscript.GetminoType == MinoScript.MinoType.SMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.LEFT | minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return SMinoSide;
            else return SMinoTop;

        }
        else if (minoscript.GetminoType == MinoScript.MinoType.ZMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.LEFT | minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return ZMinoSide;
            else return ZMinoTop;

        }
        else if (minoscript.GetminoType == MinoScript.MinoType.IMINO)
        {
            if (minoscript.GetRotationType == MinoScript.RotationMinoType.LEFT | minoscript.GetRotationType == MinoScript.RotationMinoType.RIGHT) return IMinoSide;
            else return IMinoTop;

        }
        else if (minoscript.GetminoType == MinoScript.MinoType.OMINO)
        {
            return OMinoTop;
        }
        else
        {
            return TMinoTop;
        }
    }
}
