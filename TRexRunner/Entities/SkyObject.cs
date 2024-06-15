using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TRexRunner.Entities;

public abstract class SkyObject : IGameEntity
{
    protected readonly TRex _trex;
    public int DrawOrder { get; set; }
    public abstract float Speed { get;  }
    public Vector2 Position { get; set; }

    protected SkyObject(TRex trex, Vector2 position)
    {
        _trex = trex;
        Position = position;
    }
    
    public abstract void Draw(SpriteBatch batch, GameTime time);

    public virtual void Update(GameTime time)
    {
        if (_trex.IsAlive)
        {
            Position = new Vector2((Position.X - Speed * (float)time.ElapsedGameTime.TotalSeconds), Position.Y);
        }
    }
}