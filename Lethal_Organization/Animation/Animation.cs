using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
namespace Lethal_Organization
{
    class Animation
    {
        private Rectangle _sourceImage;
        
        private int _maxIndex;
        
        private bool _isLoop;

        private bool _forcedFinish;
        
        public Rectangle SourceImage
        {
            get
            { 
                return _sourceImage; 
            }
        }

        public int MaxIndex
        {
            get 
            {
                return _maxIndex; 
            }
        }

        public bool IsLoop
        {
            get 
            { 
                return _isLoop; 
            }
        }

        public bool ForcedFinish
        {
            get 
            { 
                return _forcedFinish; 
            }
        }
        public Animation(Rectangle sourceImage, int maxIndex, bool isLoop, bool forcedFinish)
        {
            _sourceImage = sourceImage;
            _maxIndex = maxIndex;
            _isLoop = isLoop;
            _forcedFinish = forcedFinish;
        }
    }
}
