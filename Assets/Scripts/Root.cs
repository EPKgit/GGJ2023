using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    public Sprite Straight;
    public Sprite StraightEnd;
    public Sprite Angle;
    public Sprite AngleEnd;
    public Vector2 gridPosition;

    void SetSprite(Direction inDir, Direction outDir, bool ending)
    {
        var spriteComp = GetComponent<SpriteRenderer>();
        if(inDir == outDir)
        {
            spriteComp.sprite = (ending ? StraightEnd : Straight);
            spriteComp.flipX = false;
        }
        else
        {
            spriteComp.sprite = (ending ? AngleEnd : Angle);

            int deltaDir = ((int)outDir - (int)inDir + 4) % 4;
            spriteComp.flipX = (deltaDir > 0);
        }

        switch(inDir)
        {
            case Direction.DOWN:  transform.rotation = Quaternion.Euler(0, 0,   0); break;
            case Direction.RIGHT:  transform.rotation = Quaternion.Euler(0, 0,  90); break;
            case Direction.UP:    transform.rotation = Quaternion.Euler(0, 0, 180); break;
            case Direction.LEFT: transform.rotation = Quaternion.Euler(0, 0, 270); break;
        }
    }

    // Adjust sprite to match the shape of the roots
    public void SetConnection(Direction inDir, Direction outDir)
    {
        SetSprite(inDir, outDir, false);
    }

    // Adjust sprite to match the shape of the roots, and indicate the direction of growth
    public void SetEnding(Direction inDir, Direction outDir)
    {
        SetSprite(inDir, outDir, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
