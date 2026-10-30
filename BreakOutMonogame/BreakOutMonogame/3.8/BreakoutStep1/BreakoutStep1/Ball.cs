using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutStep1
{
    // what the ball is currently doing.
    public enum BallState { OnPaddleStart, Playing }

    public class Ball : DrawableSprite
    {
        public BallState State { get; private set; }

        //Launch settings, these used to be magic numbers sitting inside LaunchBall
        const float launchSpeed = 260;      //pixels per second, Direction is normalized so this is the real speed
        const float launchLean = 0.6f;      //how far off straight up a launch can lean, in X per 1 of Y

        //Service Dependencies
        IScoreManager score;
        IFloatingTextManager floatingText;

        static readonly Random random = new Random();   //shared, every launch leans a different way

        public Ball(Game game)
            : base(game)
        {
            this.State = BallState.OnPaddleStart;

            //Lazy load the services, same idea as the old GameConsole lookup but the
            //managers hold the lazy load so this is one line each
            score = ScoreManager.GetOrAddService(this.Game);
            floatingText = FloatingTextManager.GetOrAddService(this.Game);
        }

        public void SetInitialLocation()
        {
                Location = new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f,
                    Game.GraphicsDevice.Viewport.Height / 2f);
        }

        /// <summary>
        /// Launches ball
        /// </summary>
        /// <param name="gameTime"></param>
        public void LaunchBall(GameTime gameTime)
        {
            this.Speed = launchSpeed;

            //Lean the launch left or right a little so the ball doesn't take the same path
            //every time. Normalizing keeps Speed honest, a raw (1,-1) is 1.41 units long so
            //the ball used to travel 40% faster than Speed said it would.
            float lean = (((float)random.NextDouble() * 2) - 1) * launchLean;
            this.Direction = Vector2.Normalize(new Vector2(lean, -1));

            this.State = BallState.Playing;
            this.floatingText.Show("GO!");
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("ballSmall");
            SetInitialLocation();
            base.LoadContent();
        }

        private void resetBall()
        {
            this.Speed = 0;
            this.State = BallState.OnPaddleStart;
            //Back on the paddle, Paddle parks it there on its next update
        }

        public override void Update(GameTime gameTime)
        {
            switch (this.State)
            {
                case BallState.OnPaddleStart:
                    break;

                case BallState.Playing:
                    UpdateBall(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateBall(GameTime gameTime)
        {
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            //bounce off wall
            //Left and Right
            if ((this.Location.X + this.spriteTexture.Width > this.Game.GraphicsDevice.Viewport.Width)
                ||
                (this.Location.X < 0))
            {
                this.Direction.X *= -1;
            }
            //bottom Miss
            if (this.Location.Y + this.spriteTexture.Height > this.Game.GraphicsDevice.Viewport.Height)
            {
                //The floor is not a wall, reflecting here gives the ball a full width
                //safety net and the paddle stops mattering at all
                this.score.LoseLife();
                this.floatingText.Show("MISS!", Color.OrangeRed);
                this.resetBall();
            }

            //Top
            if (this.Location.Y < 0)
            {
                this.Direction.Y *= -1;
            }
        }
    }
}
