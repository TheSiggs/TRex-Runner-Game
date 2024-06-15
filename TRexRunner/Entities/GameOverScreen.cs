using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class GameOverScreen : IGameEntity
{
    public const int GAME_OVER_TEXTURE_COORDS_POS_X = 655;
    public const int GAME_OVER_TEXTURE_COORDS_POS_Y = 14;
    public const int GAME_OVER_TEXTURE_HEIGHT = 14;
    public const int GAME_OVER_TEXTURE_WIDTH = 192;

    public const int BUTTON_SPRITE_TEXTURE_COORDS_POS_X = 1;
    public const int BUTTON_SPRITE_TEXTURE_COORDS_POS_Y = 1;
    public const int BUTTON_SPRITE_HEIGHT = 34;
    public const int BUTTON_SPRITE_WIDTH = 38;

    private Sprite _textSprite;
    private Sprite _buttonSprite;

    private KeyboardState _previousKeyboardState;

    public Vector2 Position { get; set; }

    private Vector2 ButtonPosition => Position + new Vector2((GAME_OVER_TEXTURE_WIDTH / 2 - BUTTON_SPRITE_WIDTH / 2),
        (GAME_OVER_TEXTURE_HEIGHT + 20));

    public bool IsEnabled = false;

    public int DrawOrder => 100;

    private Rectangle ButtonBounds => new Rectangle(((ButtonPosition * _game.ZoomFactor)).ToPoint(), new Point((int)
        (BUTTON_SPRITE_WIDTH * _game.ZoomFactor), (int)(BUTTON_SPRITE_HEIGHT * _game.ZoomFactor)));

    private TRexRunnerGame _game;

    public GameOverScreen(Texture2D spriteSheet, TRexRunnerGame game)
    {
        _textSprite = new Sprite(spriteSheet, GAME_OVER_TEXTURE_COORDS_POS_X, GAME_OVER_TEXTURE_COORDS_POS_Y,
            GAME_OVER_TEXTURE_WIDTH, GAME_OVER_TEXTURE_HEIGHT);

        _buttonSprite = new Sprite(spriteSheet, BUTTON_SPRITE_TEXTURE_COORDS_POS_X, BUTTON_SPRITE_TEXTURE_COORDS_POS_Y,
            BUTTON_SPRITE_WIDTH, BUTTON_SPRITE_HEIGHT);
        _game = game;
    }

    public void Update(GameTime time)
    {
        if (!IsEnabled)
        {
            return;
        }

        MouseState mouseState = Mouse.GetState();
        KeyboardState keyboardState = Keyboard.GetState();

        bool isJumpKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
        bool wasJumpKeyPressed =
            _previousKeyboardState.IsKeyDown(Keys.Up) || _previousKeyboardState.IsKeyDown(Keys.Space);

        if ((ButtonBounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed) ||
            (wasJumpKeyPressed && !isJumpKeyPressed))
        {
            _game.Replay();
        }

        _previousKeyboardState = keyboardState;
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        if (!IsEnabled)
        {
            return;
        }

        _textSprite.Draw(batch, Position);
        _buttonSprite.Draw(batch, ButtonPosition);
    }
}