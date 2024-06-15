using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TRexRunner.Entities;

public class CactusGroup : Obstacle
{

    private const int SMALL_CACTUS_SPRITE_HEIGHT = 36;
    private const int SMALL_CACTUS_SPRITE_WIDTH = 17;
    private const int SMALL_CACTUS_TEXTURE_POS_X = 228;
    private const int SMALL_CACTUS_TEXTURE_POS_Y = 0;
    
    private const int LARGE_CACTUS_SPRITE_HEIGHT = 51;
    private const int LARGE_CACTUS_SPRITE_WIDTH = 25;
    private const int LARGE_CACTUS_TEXTURE_POS_X = 332;
    private const int LARGE_CACTUS_TEXTURE_POS_Y = 0;

    private const int COLLISION_BOX_INSET = 5;

    public override Rectangle CollisionBox
    {
        get
        {
            Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Sprite.Width, Sprite.Height);
            box.Inflate(-COLLISION_BOX_INSET, -COLLISION_BOX_INSET);
            return box;
        }
    }

    public bool IsLarge { get; }
    
    public enum GroupSize
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }

    public GroupSize Size { get; }

    public Sprite Sprite { get; set; }
    
    public CactusGroup(Texture2D spriteSheet, bool isLarge, GroupSize size, TRex trex, Vector2 position) : base(trex, position)
    {
        IsLarge = isLarge;
        Size = size;
        Sprite = GenerateSprite(spriteSheet);
    }

    public override void Draw(SpriteBatch batch, GameTime time)
    {
        Sprite.Draw(batch, Position);
    }

    private Sprite GenerateSprite(Texture2D spriteSheet)
    {
        Sprite sprite = null;
        int spriteWidth = 0;
        int spriteHeight = 0;
        int posX = 0;
        int posY = 0;
        if (!IsLarge)
        {
            // Create small
            spriteWidth = SMALL_CACTUS_SPRITE_WIDTH;
            spriteHeight = SMALL_CACTUS_SPRITE_HEIGHT;
            posX = SMALL_CACTUS_TEXTURE_POS_X;
            posY = SMALL_CACTUS_TEXTURE_POS_Y;
        }
        else
        {
            // Create large
            spriteWidth = LARGE_CACTUS_SPRITE_WIDTH;
            spriteHeight = LARGE_CACTUS_SPRITE_HEIGHT;
            posX = LARGE_CACTUS_TEXTURE_POS_X;
            posY = LARGE_CACTUS_TEXTURE_POS_Y;
        }
        
        int offsetX = 0;
        int width = spriteWidth;
        switch (Size)
        {
            case GroupSize.Small:
                offsetX = 0;
                width = spriteWidth;
                break;
            case GroupSize.Medium:
                offsetX = 1;
                width = spriteWidth * 2;
                break;
            case GroupSize.Large:
                offsetX = 3;
                width = spriteWidth * 3;
                break;
        }

        sprite = new Sprite(
            spriteSheet, 
            (posX + offsetX * spriteWidth),
            posY, 
            width, 
            spriteHeight
        );

        return sprite;
    }
}