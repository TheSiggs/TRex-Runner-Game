using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TRexRunner.Entities;

public interface ICollidable
{
    Rectangle CollisionBox { get;  }
}