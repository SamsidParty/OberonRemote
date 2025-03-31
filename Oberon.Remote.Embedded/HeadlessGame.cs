using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Oberon.Remote.Embedded {
    public class HeadlessGame : Game
    {
        private GraphicsDeviceManager Graphics;

        public HeadlessGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1; 
            Graphics.PreferredBackBufferHeight = 1;
            Graphics.ApplyChanges(); 
            Window.IsBorderless = true;
            Window.AllowUserResizing = false;
            Window.Position = new Point(9999, 9999);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

