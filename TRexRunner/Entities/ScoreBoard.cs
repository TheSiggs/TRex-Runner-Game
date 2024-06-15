using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace TRexRunner.Entities;

public class ScoreBoard : IGameEntity
{
    private const int TEXTURE_COORDS_NUMBER_X = 655;
    private const int TEXTURE_COORDS_NUMBER_Y = 0;
    private const int TEXTURE_COORDS_NUMBER_WIDTH = 10;
    private const int TEXTURE_COORDS_NUMBER_HEIGHT = 13;

    private const byte NUMBER_DIGITS_TO_DRAW = 5;
    private const float SCORE_MARGIN = 70;

    private const int TEXTURE_COORDS_HI_X = 755;
    private const int TEXTURE_COORDS_HI_Y = 0;
    private const int TEXTURE_COORDS_HI_WIDTH = 20;
    private const int TEXTURE_COORDS_HI_HEIGHT = 13;
    
    private const int HI_TEXT_MARGIN = 28;
    private const float SCORE_INCREMENT_MULTIPLIER = 0.025f;

    private const float FLASH_ANIMATION_FRAME_LENGTH = 0.2f;
    private const int FLASH_ANIMATION_FLASH_COUNT = 4;

    private const int MAX_SCORE = 99_999;

    private SoundEffect _scoreSfx;

    public bool HasHighScore => HighSore > 0;

    public Vector2 Position { get; set; }
    
    private Texture2D _texture;

    public double Score
    {
        get => _score;
        set => _score = Math.Max(0, Math.Min(MAX_SCORE, value));
    }

    public int DisplayScore => (int) Math.Floor(Score);
    public int HighSore { get; set; }

    public int DrawOrder => 100;

    public TRex _trex { get; set; }

    private bool _isPlayingFlashAnimation;
    private float _flashAnimationTime;
    private double _score;

    public ScoreBoard(Texture2D texture, Vector2 position, TRex trex, SoundEffect sfx)
    {
        _texture = texture;
        Position = position;
        _trex = trex;
        _scoreSfx = sfx;
    }
    
    public void Update(GameTime time)
    {
        int oldScore = DisplayScore;
        Score += _trex.Speed * SCORE_INCREMENT_MULTIPLIER * time.ElapsedGameTime.TotalSeconds;

        if (!_isPlayingFlashAnimation && (DisplayScore / 100 != oldScore / 100))
        {
            _isPlayingFlashAnimation = true;
            _flashAnimationTime = 0;
            _scoreSfx.Play(0.8f, 0, 0);
        }

        if (_isPlayingFlashAnimation)
        {
            _flashAnimationTime += (float)time.ElapsedGameTime.TotalSeconds;
            
            

            if (_flashAnimationTime >= FLASH_ANIMATION_FRAME_LENGTH * FLASH_ANIMATION_FLASH_COUNT * 2)
            {
                _isPlayingFlashAnimation = false;
            }
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        
        if (HasHighScore)
        {
            batch.Draw(_texture, new Vector2(Position.X - HI_TEXT_MARGIN, Position.Y), new Rectangle(TEXTURE_COORDS_HI_X, TEXTURE_COORDS_HI_Y, TEXTURE_COORDS_HI_WIDTH, TEXTURE_COORDS_HI_HEIGHT), Color.White);
            DrawScore(batch, HighSore, Position.X);
        }

        if (!_isPlayingFlashAnimation || (int)(_flashAnimationTime / FLASH_ANIMATION_FRAME_LENGTH) % 2 != 0)
        {
            int score = !_isPlayingFlashAnimation ? DisplayScore : (DisplayScore / 100 * 100);
            DrawScore(batch, score, Position.X + SCORE_MARGIN);
        }
    }

    private void DrawScore(SpriteBatch batch, int score, float posX)
    {
        int[] scoreDigits = SplitDigits(score);
        foreach (int digit in scoreDigits)
        {
            Rectangle textureCoords = GetDigitTextureBounds(digit);

            Vector2 screenPos = new Vector2(posX, Position.Y);
            
            batch.Draw(_texture, screenPos, textureCoords, Color.White);
            
            posX += TEXTURE_COORDS_NUMBER_WIDTH;
        }
    }

    private int[] SplitDigits(int input)
    {
        string inputStr = input.ToString().PadLeft(NUMBER_DIGITS_TO_DRAW, '0');

        int[] res = new int[inputStr.Length];

        for (int i = 0; i < res.Length; i++)
        {
            res[i] = (int)char.GetNumericValue(inputStr[i]);
        }

        return res;
    }

    private Rectangle GetDigitTextureBounds(int digit)
    {
        if (digit is < 0 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(digit), "The value of the digit must be between 0 and 9");
        }
        int posX = TEXTURE_COORDS_NUMBER_X + digit * TEXTURE_COORDS_NUMBER_WIDTH;
        int posY = TEXTURE_COORDS_NUMBER_Y;

        return new Rectangle(posX, posY, TEXTURE_COORDS_NUMBER_WIDTH, TEXTURE_COORDS_NUMBER_HEIGHT);
    }
    
    
}