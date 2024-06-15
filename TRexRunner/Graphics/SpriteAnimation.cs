using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TRexRunner.Graphics;

public class SpriteAnimation
{
    private List<SpriteAnimationFrame> _frames = new List<SpriteAnimationFrame>();

    public bool IsPlaying { get; private set; }
    public float PlaybackProgress { get; private set; }

    public SpriteAnimationFrame this[int index] => GetFrame(index);

    public SpriteAnimationFrame CurrentFrame => _frames
        .Where(f => f.TimeStamp <= PlaybackProgress)
        .OrderBy(f => f.TimeStamp)
        .LastOrDefault();

    public int FrameCount => _frames.Count;
    
    public float Duration => _frames.Any() ? _frames.Max(f => f.TimeStamp) : 0;

    public bool ShouldLoop { get; set; } = true;

    public void AddFrame(Sprite sprite, float timestamp)
    {
        SpriteAnimationFrame frame = new SpriteAnimationFrame(sprite, timestamp);

        _frames.Add(frame);
    }

    public void Update(GameTime time)
    {
        if (IsPlaying)
        {
            PlaybackProgress += (float)time.ElapsedGameTime.TotalSeconds;

            if (PlaybackProgress > Duration)
            {
                if (ShouldLoop)
                {
                    PlaybackProgress -= Duration;
                }
                else
                {
                    Stop();
                }
            }
        }
    }

    public void Play()
    {
        IsPlaying = true;
    }

    public void Stop()
    {
        IsPlaying = false;
        PlaybackProgress = 0;
    }

    public SpriteAnimationFrame GetFrame(int index)
    {
        if (index < 0 || index >= _frames.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                $"A frame with index {index} is does not exist in animation");
        }

        return _frames[index];
    }

    public void Draw(SpriteBatch batch, Vector2 position)
    {
        SpriteAnimationFrame frame = CurrentFrame;
        if (frame != null)
        {
            frame.Sprite.Draw(batch, position);
        }
    }

    public void Clear()
    {
        Stop();
        _frames.Clear();
    }

    public static SpriteAnimation CreateSimpleAnimation(Texture2D texture, Point startPos, int width, int height, Point offset, int frameCount, float frameLength)
    {
        if (texture == null)
        {
            throw new ArgumentNullException(nameof(texture), "Texture cannot be null");
        }

        SpriteAnimation anim = new SpriteAnimation();

        for (int i = 0; i < frameCount; i++)
        {
            Sprite sprite = new Sprite(texture, (startPos.X + i * offset.X), (startPos.Y + i * offset.Y), width, height);
            anim.AddFrame(sprite, frameLength * i);
            if (i == frameCount - 1)
            {
                anim.AddFrame(sprite, frameLength * (i + 1));
            }
        }

        return anim;
    }
}