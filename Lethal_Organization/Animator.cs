using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    //T is State Enum
    internal class Animator<T> where T: Enum
    {
        private T _curState;

        private T _prevState;

        private Dictionary<T, Animation> _animMap;

        private Queue<T> _stateQueue;

        private Animation _curAnim;

        private Texture2D _spriteSheet;

        private Rectangle _imageSource;

        private int _currentFrame;

        private float _secondPerFrame;

        private float _timeCounter;

        private int _frameWidth;

        private int _frameHeight;



        /// <summary>
        /// Used for object with multiply animation need to read from file
        /// </summary>
        /// <param name="spriteFile"></param>

        public Animator(Texture2D spriteSheet, T state, string spriteText, float secondPerFrame)
        {
            _spriteSheet = spriteSheet;

            _animMap = new Dictionary<T, Animation>();

            _stateQueue = new Queue<T>();

            LoadTexture(spriteText);

            _curState = state;
            
            _secondPerFrame = secondPerFrame;

            _timeCounter = 0;

            _currentFrame = 0; 
        }

        public Animator(Texture2D spriteSheet, T defaultState, int frameWidth, int frameHeight, float secondPerFrame)
        {
            _spriteSheet = spriteSheet;

            _animMap = new Dictionary<T, Animation>();

            _stateQueue = new Queue<T>();

            _curState = defaultState;

            _secondPerFrame = secondPerFrame;

            _timeCounter = 0;

            _currentFrame = 0;

            _frameHeight = frameHeight;

            _frameWidth = frameWidth;

            _animMap.Add(_curState, new Animation(
                new Rectangle(0, 0, spriteSheet.Width, spriteSheet.Height),
                spriteSheet.Width / frameWidth,
                true,
                false));

            SwitchToState(defaultState);
        }

        public void Update(GameTime gameTime)
        {
            if(_curAnim == null)
            {
                return;
            }

            _timeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Set wait time for each frame
            if(_timeCounter > 0)
            {
                return;
            }

            //Change frame
            if (_curAnim.IsLoop)
            {
                _currentFrame = (_currentFrame + 1) % _curAnim.MaxIndex;  //Wrap around the max Index
            }
            else
            {
                if (_currentFrame < _curAnim.MaxIndex - 1)
                {
                    _currentFrame++;
                }
            }


            //Calculate sourece image for frame
            Rectangle animSequence = _curAnim.SourceImage;
            _imageSource = new Rectangle( _currentFrame * _frameWidth, animSequence.Y, _frameWidth, _frameHeight);
            
            _timeCounter = _secondPerFrame;

            //If animation, forced to finish, reach its last frame => switch next state on the queue
            if(!_curAnim.IsLoop                                
                && _curAnim.ForcedFinish
                && _currentFrame == _curAnim.MaxIndex - 1  
                && _stateQueue.Count > 0)
            {
                T nextState = _stateQueue.Dequeue();
                SwitchToState(nextState);
            }
        }

        public void Draw(SpriteBatch sb, Rectangle displayPos, SpriteEffects effect)
        {
            //Scale to two
            sb.Draw(_spriteSheet, displayPos, _imageSource,Color.White, 0,Vector2.Zero,effect, 0);
        }

        public void Draw(SpriteBatch sb, Rectangle displayPos, float angle)
        {
            Vector2 origin = new Vector2(_imageSource.Width / 2f, _imageSource.Height / 2f); //Rotate around the center of texture to avoid go outside hitbox
            sb.Draw(_spriteSheet, displayPos, _imageSource, Color.White, angle, origin, SpriteEffects.None, 0);

        }

        public void SetState(T  state)
        {    
            //State not change ignore
            if(_curState.Equals(state) || !_animMap.ContainsKey(state))
            {
                return;
            }

            //If there is an animation that force to finish => wait
            if(_curAnim != null 
                && _curAnim.ForcedFinish 
                && _currentFrame <  _curAnim.MaxIndex -1)
            {
                _stateQueue.Enqueue(state);
            }else //Immidately switch state
            {
                SwitchToState(state);
            }            
        }

        private void SwitchToState(T state)
        {
            _prevState = _curState;
            _curState = state;
            _curAnim = _animMap[state];
            _currentFrame = 0;
            _timeCounter = 0;

            //Set source image to first frame
            Rectangle animSequence = _curAnim.SourceImage;
            _imageSource = new Rectangle(0, animSequence.Y, _frameWidth, _frameHeight);
        }

        public bool CheckAnimationFinish(T state, int index)
        {
            return _curState.Equals(state) && _currentFrame >= index;
        }

        public int GetMaxIndex(T state)
        {
           if(!_animMap.ContainsKey(state)) 
           {
                return -1;
           }

           return _animMap[state].MaxIndex;
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
                    }else if(data.Length == 7)
                    {
                        T state = (T)Enum.Parse(typeof(T), data[0]);
                        int rowIndex = int.Parse(data[1]);
                        int colIndex = int.Parse(data[2]);
                        int height = int.Parse(data[3]);
                        int width = int.Parse(data[4]);
                        bool isLoop = data[5] == "1";
                        bool forceFinish = data[6] == "1";


                        _animMap.Add(state,
                            new Animation(
                                new Rectangle(_frameWidth * colIndex, _frameHeight * rowIndex, width, height), //Source image
                                width / _frameWidth,   //Max index
                                isLoop,
                                forceFinish));
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
