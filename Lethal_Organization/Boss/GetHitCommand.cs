using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Lethal_Organization
{
    class GetHitCommand : ICommand<Boss>
    {
        public bool Finished { get ; set ; }

        private Action<Boss.State> _setAnim;

        private Func<Boss.State, int, bool> _checkFinishAnimation;

        private Func<Boss.State, int> _getMaxIndex;

        public GetHitCommand(Action<Boss.State> setAnim, Func<Boss.State, int, bool> checkFinishAnimation, Func<Boss.State, int> getMaxIndex)
        {
            _setAnim= setAnim;

            _checkFinishAnimation= checkFinishAnimation;
            
            _getMaxIndex= getMaxIndex;
        }
        void ICommand<Boss>.Execute(Boss gameObject)
        {
            _setAnim(Boss.State.TakeHit);
        }

        void ICommand<Boss>.Update(Boss gameObject, GameTime gameTime)
        {
            if (_checkFinishAnimation(Boss.State.TakeHit, _getMaxIndex(Boss.State.TakeHit) - 1))
            {
                Finished = true;    
            }
        }
    }
}
