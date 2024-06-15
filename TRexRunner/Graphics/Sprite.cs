using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRexRunner.Graphics;

public class Sprite
{
    public Texture2D Texture { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Color TintColor { get; set; } = Color.White;

    public Sprite(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void Draw(SpriteBatch batch, Vector2 position)
    {
        batch.Draw(Texture, position, new Rectangle(X, Y, Width, Height), TintColor);
    }
}