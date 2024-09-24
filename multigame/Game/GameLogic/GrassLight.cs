using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multigame.GameLogic
{
     class GrassLight : GrassAndFlags
    {
        public override BlockType Type => BlockType.grass_light;

        private bool _isClicked;
        private bool _isFlagged;

        public override bool isClicked
        {
            get { return _isClicked; }
            set { _isClicked = value; }
        }
        public override bool isFlagged
        {
            get { return _isFlagged; }
            set { _isFlagged = value; }
        }

        public GrassLight()
        {
            _isClicked = false;
            _isFlagged = false;
        }
    }
}
