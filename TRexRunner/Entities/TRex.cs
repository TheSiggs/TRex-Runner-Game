using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class TRex : IGameEntity, ICollidable
{
    public const int TREX_DEFAULT_SPRITE_POS_X = 848;
    public const int TREX_DEFAULT_SPRITE_POS_Y = 0;
    public const int TREX_DEFAULT_SPRITE_POS_WIDTH = 44;
    public const int TREX_DEFAULT_SPRITE_POS_HEIGHT = 52;

    private const int TREX_IDLE_BACKGROUND_SPRITE_POS_X = 40;
    private const int TREX_IDLE_BACKGROUND_SPRITE_POS_Y = 0;

    private const int TREX_RUNNING_SPRITE_ONE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_POS_WIDTH * 2;
    private const int TREX_RUNNING_SPRITE_ONE_POS_Y = 0;

    private const int TREX_RUNNING_SPRITE_TWO_POS_X = TREX_RUNNING_SPRITE_ONE_POS_X + TREX_DEFAULT_SPRITE_POS_WIDTH;
    private const int TREX_RUNNING_SPRITE_TWO_POS_Y = 0;

    private const int TREX_DEAD_SPRITE_POS_X = 1068;
    private const int TREX_DEAD_SPRITE_POS_Y = 0;

    private const int TREX_DUCKING_SPRITE_WIDTH = 59;

    private const int TREX_DUCKING_SPRITE_ONE_POS_X = TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_POS_WIDTH * 6;
    private const int TREX_DUCKING_SPRITE_ONE_POS_Y = 0;

    private const int TREX_DUCKING_SPRITE_TWO_POS_X = TREX_DUCKING_SPRITE_ONE_POS_X + TREX_DUCKING_SPRITE_WIDTH;
    private const int TREX_DUCKING_SPRITE_TWO_POS_Y = 0;


    private const double BLINK_ANIMATION_RANDOM_MIN = 2f;
    private const double BLINK_ANIMATION_RANDOM_MAX = 8f;
    private const float BLINK_ANIMATION_DURATION = 0.2f;

    private const float JUMP_START_VELOCITY = -480;
    private const float GRAVITY = 1600f;
    private const float CANCEL_JUMP_VELOCITY = -100f;
    private const float MIN_JUMP_HEIGHT = 40;
    private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 10f;
    private const float DROP_VELOCITY = 600f;
    public const float START_SPEED = 280f;
    public const float MAX_SPEED = 900f;

    private const float ACCELERATION_PPS_PER_SECOND = 5f;
    private const int COLLISION_BOX_INSET = 3;
    private const int DUCK_COLLISION_REDUCTION = 20;

    private float _startPosY;
    private float _dropVelocity;

    public event EventHandler JumpComplete;
    public event EventHandler Died;


    public int DrawOrder { get; set; }

    public float Speed { get; set; }
    public bool IsAlive { get; set; }

    private Sprite _idleSprite;
    private Sprite _idleBlinkSprite;
    private Sprite _idleBackgroundSprite;

    public Vector2 Position { get; set; }
    public TRexState State { get; private set; }

    private SpriteAnimation _blinkAnimation;

    private Random _random;

    private SoundEffect _jumpSound;

    private float _verticalVelocity;

    private SpriteAnimation _runAnimation;

    private SpriteAnimation _duckAnimation;

    private Sprite _deadSprite;

    public Rectangle CollisionBox
    {
        get
        {
            Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y),
                TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT);
            box.Inflate(-COLLISION_BOX_INSET, -COLLISION_BOX_INSET);

            if (State == TRexState.Ducking)
            {
                box.Y += DUCK_COLLISION_REDUCTION;
                box.Height -= DUCK_COLLISION_REDUCTION;
            }
            return box;
        }
    }

    public TRex(Texture2D spriteSheet, Vector2 position, SoundEffect jumpSound)
    {
        _idleBackgroundSprite = new Sprite(spriteSheet, TREX_IDLE_BACKGROUND_SPRITE_POS_X,
            TREX_IDLE_BACKGROUND_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT);

        Position = position;
        State = TRexState.Idle;

        _jumpSound = jumpSound;
        _random = new Random();

        _idleSprite = new Sprite(spriteSheet, TREX_DEFAULT_SPRITE_POS_X, TREX_DEFAULT_SPRITE_POS_Y,
            TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT);
        _idleBlinkSprite = new Sprite(spriteSheet, TREX_DEFAULT_SPRITE_POS_X + TREX_DEFAULT_SPRITE_POS_WIDTH,
            TREX_DEFAULT_SPRITE_POS_Y, TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT);

        _blinkAnimation = new SpriteAnimation
        {
            ShouldLoop = false
        };

        CreateBlinkAnimation();
        _blinkAnimation.Play();

        _startPosY = position.Y;

        _runAnimation = new SpriteAnimation();
        _runAnimation.AddFrame(
            new Sprite(spriteSheet, TREX_RUNNING_SPRITE_ONE_POS_X, TREX_RUNNING_SPRITE_ONE_POS_Y,
                TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT), 0);
        _runAnimation.AddFrame(
            new Sprite(spriteSheet, TREX_RUNNING_SPRITE_TWO_POS_X, TREX_RUNNING_SPRITE_TWO_POS_Y,
                TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
        _runAnimation.AddFrame(_runAnimation[0].Sprite, RUN_ANIMATION_FRAME_LENGTH * 2);
        _runAnimation.Play();

        _duckAnimation = new SpriteAnimation();
        _duckAnimation.AddFrame(
            new Sprite(spriteSheet, TREX_DUCKING_SPRITE_ONE_POS_X, TREX_DUCKING_SPRITE_ONE_POS_Y,
                TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT), 0);
        _duckAnimation.AddFrame(
            new Sprite(spriteSheet, TREX_DUCKING_SPRITE_TWO_POS_X, TREX_DUCKING_SPRITE_TWO_POS_Y,
                TREX_DUCKING_SPRITE_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT), RUN_ANIMATION_FRAME_LENGTH);
        _duckAnimation.AddFrame(_duckAnimation[0].Sprite, RUN_ANIMATION_FRAME_LENGTH * 2);
        _duckAnimation.Play();

        _deadSprite = new Sprite(spriteSheet, TREX_DEAD_SPRITE_POS_X, TREX_DEAD_SPRITE_POS_Y,
            TREX_DEFAULT_SPRITE_POS_WIDTH, TREX_DEFAULT_SPRITE_POS_HEIGHT);
        IsAlive = true;
    }

    public void Update(GameTime time)
    {
        if (State == TRexState.Idle)
        {
            if (!_blinkAnimation.IsPlaying)
            {
                CreateBlinkAnimation();
                _blinkAnimation.Play();
            }

            _blinkAnimation.Update(time);
        }
        else if (State is TRexState.Jumping or TRexState.Falling)
        {
            Position = new Vector2(Position.X,
                Position.Y + _verticalVelocity * (float)time.ElapsedGameTime.TotalSeconds +
                _dropVelocity * (float)time.ElapsedGameTime.TotalSeconds);
            _verticalVelocity += GRAVITY * (float)time.ElapsedGameTime.TotalSeconds;

            if (_verticalVelocity >= 0)
            {
                State = TRexState.Falling;
            }

            if (Position.Y >= _startPosY)
            {
                Position = new Vector2(Position.X, _startPosY);
                _verticalVelocity = 0;
                State = TRexState.Running;
                
                OnJumpComplete();
            }
        }
        else if (State == TRexState.Running)
        {
            _runAnimation.Update(time);
        }
        else if (State == TRexState.Ducking)
        {
            _duckAnimation.Update(time);
        }

        if (State != TRexState.Idle)
        {
            Speed += ACCELERATION_PPS_PER_SECOND * (float) time.ElapsedGameTime.TotalSeconds;
            if (Speed > MAX_SPEED)
            {
                Speed = MAX_SPEED;
            }
        }

        _dropVelocity = 0;
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        if (IsAlive)
        {
            if (State == TRexState.Idle)
            {
                _idleBackgroundSprite.Draw(batch, Position);
                _blinkAnimation.Draw(batch, Position);
            }
            else if (State is TRexState.Jumping or TRexState.Falling)
            {
                _idleSprite.Draw(batch, Position);
            }
            else if (State == TRexState.Running)
            {
                _runAnimation.Draw(batch, Position);
            }
            else if (State == TRexState.Ducking)
            {
                _duckAnimation.Draw(batch, Position);
            }
        }
        else
        {
            _deadSprite.Draw(batch, Position);
        }
    }

    private void CreateBlinkAnimation()
    {
        _blinkAnimation.Clear();
        double blinkTimeStamp = BLINK_ANIMATION_RANDOM_MIN + _random.NextDouble() * BLINK_ANIMATION_RANDOM_MAX;

        _blinkAnimation.AddFrame(_idleSprite, 0);
        _blinkAnimation.AddFrame(_idleBlinkSprite, (float)blinkTimeStamp);
        _blinkAnimation.AddFrame(_idleBlinkSprite, (float)blinkTimeStamp + BLINK_ANIMATION_DURATION);
    }

    public bool BeginJump()
    {
        if (State is TRexState.Jumping or TRexState.Falling)
            return false;

        _jumpSound.Play();

        State = TRexState.Jumping;

        _verticalVelocity = JUMP_START_VELOCITY;

        return true;
    }

    public bool CancelJump()
    {
        if (State != TRexState.Jumping || (_startPosY - Position.Y) < MIN_JUMP_HEIGHT)
        {
            return false;
        }

        _verticalVelocity = _verticalVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;

        return true;
    }

    public bool Duck()
    {
        if (State is TRexState.Jumping or TRexState.Falling)
        {
            return false;
        }

        State = TRexState.Ducking;

        return true;
    }

    public bool GetUp()
    {
        if (State != TRexState.Ducking)
        {
            return false;
        }

        State = TRexState.Running;
        return true;
    }

    public bool Drop()
    {
        if (State != TRexState.Falling && State != TRexState.Jumping)
        {
            return false;
        }

        _dropVelocity = DROP_VELOCITY;
        State = TRexState.Falling;
        return true;
    }

    protected virtual void OnJumpComplete()
    {
        EventHandler handler = JumpComplete;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void Initialize()
    {
        Speed = START_SPEED;
        State = TRexState.Running;
        IsAlive = true;
        Position = new Vector2(Position.X, _startPosY);
    }

    protected virtual void OnDied()
    {
        EventHandler handler = Died;
        handler?.Invoke(this, EventArgs.Empty);
    }
    
    public bool Die()
    {
        if (!IsAlive)
        {
            return false;
        }

        State = TRexState.Idle;
        IsAlive = false;
        Speed = 0;

        OnDied();
        
        return true;
    }

}