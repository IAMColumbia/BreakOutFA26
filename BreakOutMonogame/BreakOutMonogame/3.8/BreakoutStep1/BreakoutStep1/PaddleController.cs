using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutStep1
{
    class PaddleController
    {
        InputHandler input;
        Ball ball; //maybe should delegate to parent
        public Vector2 Direction { get; private set; }

        public PaddleController(Game game, Ball ball)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));
            this.Direction = Vector2.Zero;
            this.ball = ball;   //need refernce to ball to be able to lanch ball could possibly use delegate here
        }

        public void HandleInput(GameTime gametime)
        {
            this.Direction = Vector2.Zero;  //Start with no direction on each new upafet

            //No need to sum input only uses left and right
            if (input.KeyboardState.IsKeyDown(Keys.Left))
            {
                this.Direction = new Vector2(-1, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right))
            {
                this.Direction = new Vector2(1, 0);
            }

            //Mouse control, steers the same way the arrow keys do.
            //This uses how far the mouse moved and not where it is, so the paddle keeps
            //its own Speed instead of teleporting to the cursor. Checked after the keys
            //so the mouse wins if someone is somehow using both at once.
            float mouseMovedX = input.MouseState.X - input.PreviousMouseState.X;
            if (mouseMovedX != 0)
            {
                this.Direction = new Vector2(Math.Sign(mouseMovedX), 0);
            }

            //Up or left click launches ball
            if (input.KeyboardState.WasKeyPressed(Keys.Up) || WasLeftClick())
            {
                if (ball.State == BallState.OnPaddleStart) //Only Launch Ball is it's on paddle
                    this.ball.LaunchBall(gametime);
            }
        }

        /// <summary>
        /// True on the update the left button goes down, so holding it only launches once
        /// </summary>
        private bool WasLeftClick()
        {
            return input.MouseState.LeftButton == ButtonState.Pressed
                && input.PreviousMouseState.LeftButton == ButtonState.Released;
        }
    }
}
