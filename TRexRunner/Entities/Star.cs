using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class Star : SkyObject
{
    private readonly IDayNightCycle _dayNightCycle;
    private const int TEXTURE_INITIAL_FRAME_COORDS_X = 644;
    private const int TEXTURE_INITIAL_FRAME_COORDS_Y = 2;
    private const int SPRITE_WIDTH = 9;
    private const int SPRITE_HEIGHT = 9;
    private const float ANIAMTION_FRAME_LENGTH = 0.4f;

    private SpriteAnimation _animation;

    public override float Speed => _trex.Speed * 0.2f;


    public Star(TRex trex, Vector2 position, Texture2D spriteSheet, IDayNightCycle dayNightCycle) : base(trex, position)
    {
        _dayNightCycle = dayNightCycle;
        _animation = SpriteAnimation.CreateSimpleAnimation(spriteSheet,
            new Point(TEXTURE_INITIAL_FRAME_COORDS_X, TEXTURE_INITIAL_FRAME_COORDS_Y), SPRITE_WIDTH, SPRITE_HEIGHT,
            new Point(0, SPRITE_HEIGHT), 3, ANIAMTION_FRAME_LENGTH);

        _animation.ShouldLoop = true;
        _animation.Play();
    }

    public override void Draw(SpriteBatch batch, GameTime time)
    {
        if (_dayNightCycle.IsNight)
        {
            _animation.Draw(batch, Position);
        }
    }

    public override void Update(GameTime time)
    {
        base.Update(time);
        if (_trex.IsAlive)
        {
            _animation.Update(time);
        }
    }
}