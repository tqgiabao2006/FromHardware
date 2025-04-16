using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    //T is State Enum
    internal class Animator<T> where T: Enum
    {

        Dictionary<T, Animation> _animations;

        private Texture2D _spriteSheet;

        private Rectangle _imageSource;

        private int _currentFrame;

        private float _secondPerFrame;

        private float _timeCounter;

        private int _frameWidth;

        private int _frameHeight;

        private T _currentState;

        /// <summary>
        /// Used for object with multiply animation need to read from file
        /// </summary>
        /// <param name="spriteFile"></param>

        public Animator(Texture2D spriteSheet, T state,string spriteText, float secondPerFrame)
        {
            _spriteSheet = spriteSheet;

            _animations = new Dictionary<T, Animation>();

            LoadTexture(spriteText);

            _currentState = state;
            
            _secondPerFrame = secondPerFrame;

            _timeCounter = 0;

            _currentFrame = 0;
        }

        public Animator(Texture2D spriteSheet, int frameWidth, int frameHeight)
        {

        }

        public void Update(GameTime gameTime)
        {
            if(_animations.ContainsKey(_currentState))
            {
                Animation animation = _animations[_currentState];
                if(_timeCounter <= 0)
                {
                    if (animation.IsLoop)
                    {
                        if (_currentFrame >= animation.MaxIndex - 1)
                        {
                            _currentFrame = 0;
                        }
                        else
                        {
                            _currentFrame++;
                        }
                    }
                    else
                    {
                        if (_currentFrame < animation.MaxIndex - 1)
                        {
                            _currentFrame++;
                        }
                    }
                    Rectangle animSequence = animation.SourceImage;
                    _imageSource = new Rectangle( _currentFrame * _frameWidth, animSequence.Y, _frameWidth, _frameHeight);
                    _timeCounter = _secondPerFrame;
                }else
                {
                    _timeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public void Draw(SpriteBatch sb, Rectangle displayPos, SpriteEffects effect)
        {
            //Scale to two
            sb.Draw(_spriteSheet, displayPos, _imageSource,Color.White, 0,Vector2.Zero,effect, 0);
        }

        public void SetState(T  state)
        {
            if(_currentState.Equals(state))
            {
                return;
            }

            _currentState = state;
            _currentFrame = 0;
            _timeCounter = 0;
        }

        private void LoadTexture(string filePath)
        {
            string line = "";
            StreamReader reader = null;
            int spriteWidth = 0;
            int spriteHeight = 0;
            try
            {
                reader = new StreamReader(filePath);
                while((line = reader.ReadLine()) != null)
                {
                    if (line[0] == '=' || line[0] == '/')
                    {
                        continue;
                    }

                    string[] data = line.Split(',');
                    if(data.Length == 3)
                    {
                        _frameWidth = int.Parse(data[1]);
                        _frameHeight = int.Parse(data[2]);
                    }else if(data.Length == 6)
                    {
                        T state = (T)Enum.Parse(typeof(T), data[0]);
                        int rowIndex = int.Parse(data[1]);
                        int colIndex = int.Parse(data[2]);
                        int height = int.Parse(data[3]);
                        int width = int.Parse(data[4]);
                        bool isLoop = int.Parse(data[5]) == 1? true : false;    

                        _animations.Add(state,
                            new Animation()
                            {
                                SourceImage = new Rectangle(_frameWidth * colIndex, _frameHeight * rowIndex, width , height),
                                MaxIndex = width / _frameWidth,
                                IsLoop = isLoop,
                            });
                    }
                                
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }finally
            {
                if(reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
