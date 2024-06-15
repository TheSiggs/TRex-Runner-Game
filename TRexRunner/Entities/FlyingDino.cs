using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class FlyingDino : Obstacle
{

    public static readonly int[] FlyingPositions = new int[] { 80, 62, 24 };
    
    private const int TEXTURE_COORDS_X = 134;
    private const int TEXTURE_COORDS_y = 0;
    private const int SPRITE_WIDTH = 46;
    private const int SPRITE_HEIGHT = 42;
    private const float ANIAMTION_FRAME_LENGTH = 0.3f;
    private const int VERTICAL_COLLISION_INSET = 10;
    private const int HORIZONTAL_COLLISION_INSET = 6;

    private const float SPEED_PPS = 80f;

    private TRex _trex;

    public override Rectangle CollisionBox
    {
        get
        {
            Rectangle collisionBox = new Rectangle((int) Math.Round(Position.X), (int) Math.Round(Position.Y), SPRITE_WIDTH, SPRITE_HEIGHT);
            collisionBox.Inflate(-HORIZONTAL_COLLISION_INSET, -VERTICAL_COLLISION_INSET);
            return collisionBox;
        }
    }
    
    public int DrawOrder { get; }
    
    private SpriteAnimation _animation;
    
    public FlyingDino(Texture2D spriteSheet, TRex trex, Vector2 position) : base(trex, position)
    {
        Sprite spriteA = new Sprite(spriteSheet, TEXTURE_COORDS_X, TEXTURE_COORDS_y, SPRITE_WIDTH, SPRITE_HEIGHT);
        Sprite spriteB = new Sprite(spriteSheet, TEXTURE_COORDS_X + SPRITE_WIDTH, TEXTURE_COORDS_y, SPRITE_WIDTH, SPRITE_HEIGHT);

        _trex = trex;

        _animation = new SpriteAnimation();
        _animation.AddFrame(spriteA, 0);
        _animation.AddFrame(spriteB, ANIAMTION_FRAME_LENGTH);
        _animation.AddFrame(spriteA, ANIAMTION_FRAME_LENGTH * 2);
        _animation.ShouldLoop = true;
        _animation.Play();

    }

    
    public override void Update(GameTime time)
    {
        base.Update(time);

        
        if (_trex.IsAlive)
        {
            _animation.Update(time);
            Position = new Vector2(Position.X - SPEED_PPS * (float) time.ElapsedGameTime.TotalSeconds, Position.Y);
        }
    }

    public override void Draw(SpriteBatch batch, GameTime time)
    {
        _animation.Draw(batch, Position);
    }
}