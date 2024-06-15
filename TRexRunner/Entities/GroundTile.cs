using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class GroundTile : IGameEntity
{

    private float _positionY;
    public float PositionX { get; set; }
    public Sprite Sprite { get;}
    
    public int DrawOrder { get; set; }


    public GroundTile(float positionX, float positionY, Sprite sprite)
    {
        PositionX = positionX;
        _positionY = positionY;
        Sprite = sprite;
    }

    public void Update(GameTime time)
    {
        
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        Sprite.Draw(batch, new Vector2(PositionX, _positionY));
    }
}