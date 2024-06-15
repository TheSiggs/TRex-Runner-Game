using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRexRunner.Entities;

public interface IGameEntity
{
    public int DrawOrder { get; }

    public void Update(GameTime time);
    public void Draw(SpriteBatch batch, GameTime time);
}