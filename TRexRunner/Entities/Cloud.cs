using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class Cloud : SkyObject
{

    private const int TEXTURE_CORRDS_X = 87;
    private const int TEXTURE_CORRDS_Y = 0;
    private const int SPRITE_WIDTH = 46;
    private const int SPRITE_HEIGHT = 17;
    
    
    public override float Speed => _trex.Speed * 0.5f;

    private Sprite _sprite;

    public Cloud(TRex trex, Vector2 position, Texture2D spriteSheet) : base(trex, position)
    {
        _sprite = new Sprite(spriteSheet, TEXTURE_CORRDS_X, TEXTURE_CORRDS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
    }

    public override void Draw(SpriteBatch batch, GameTime time)
    {
        _sprite.Draw(batch, Position);
    }
}