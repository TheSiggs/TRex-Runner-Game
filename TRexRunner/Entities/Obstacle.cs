using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public abstract class Obstacle : IGameEntity, ICollidable
{
    
    public int DrawOrder { get; set; }
    public Vector2 Position { get; protected set; }
    private TRex _trex;
    public abstract Rectangle CollisionBox { get; }
    protected Obstacle(TRex trex, Vector2 position)
    {
        _trex = trex;
        Position = position;
    }

    public virtual void Update(GameTime time)
    {
        float posX = Position.X - _trex.Speed * (float) time.ElapsedGameTime.TotalSeconds;
        Position = new Vector2(posX, Position.Y);
        CheckCollisions();

    }

    public abstract void Draw(SpriteBatch batch, GameTime time);

    private void CheckCollisions()
    {
        Rectangle obstacleCollisionBox = CollisionBox;
        Rectangle trexCollisionBox = _trex.CollisionBox;

        if (obstacleCollisionBox.Intersects(trexCollisionBox))
        {
            _trex.Die();
        }
    }
}