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
    class Paddle : DrawableSprite
    {
        //Service Dependencies
        IFloatingTextManager floatingText;

        //Dependencies
        PaddleController controller;
        Ball ball;      //Need reference to ball for collision

        const int bottomMargin = 40;    //how far the paddle sits off the bottom of the window

        //Words for the pop up text, picked at random so a long rally doesn't read the same
        static readonly string[] hitWords = { "BOP!", "POW!", "BONK!", "ZING!", "WHAM!", "BOING!" };
        static readonly Random random = new Random();

        public Paddle(Game game, Ball b)
            : base(game)
        {
            this.Speed = 200;
            this.ball = b;
            controller = new PaddleController(game, ball);

            //Lazy load the pop up text manager, it takes the place of the old console log
            floatingText = FloatingTextManager.GetOrAddService(this.Game);
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("paddleSmall");
            SetInitialLocation();
            base.LoadContent();
        }

        public void SetInitialLocation()
        {
            //Centered near the bottom of the window instead of a hard coded spot, so the
            //paddle still starts in the right place if the back buffer size changes
            Viewport vp = this.Game.GraphicsDevice.Viewport;
            this.Location = new Vector2(
                (vp.Width - (this.spriteTexture.Width * this.Scale)) / 2,
                vp.Height - (this.spriteTexture.Height * this.Scale) - bottomMargin);
        }

        Rectangle collisionRectangle;  //Rectangle for paddle collision uses just the top of the paddle instead of the whole sprite
        const int collisionBandHeight = 8;  //how far into the top of the paddle counts as a hit, a 1 pixel band lets a fast ball step right over it

        public override void Update(GameTime gameTime)
        {
            //Update Collision Rect
            collisionRectangle = new Rectangle((int)this.Location.X, (int)this.Location.Y, (int)(this.spriteTexture.Width * this.Scale), collisionBandHeight);

            //Deal with ball state
            switch (ball.State)
            {
                case BallState.OnPaddleStart:
                    //Move the ball with the paddle until launch
                    UpdateMoveBallWithPaddle();
                    break;
                case BallState.Playing:
                    UpdateCheckBallCollision();
                    break;
            }

            //Movement from controller
            controller.HandleInput(gameTime);

            this.Direction = controller.Direction;
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            KeepPaddleOnScreen();
            base.Update(gameTime);
        }

        private void UpdateMoveBallWithPaddle()
        {
            ball.Speed = 0;
            ball.Direction = Vector2.Zero;
            ball.Location = new Vector2(this.Location.X + (this.LocationRect.Width / 2 - ball.SpriteTexture.Width / 2), this.Location.Y - ball.SpriteTexture.Height);
        }

        private void UpdateCheckBallCollision()
        {
            //Ball Collsion
            //Very simple collision with ball only uses rectangles
            //Only a ball that's heading down can be reflected, a ball that's still
            //overlapping the band next update would otherwise get flipped back down
            if (ball.Direction.Y > 0 && collisionRectangle.Intersects(ball.LocationRect))
            {
                ball.Direction.Y *= -1;

                //Sit the ball back on top of the paddle so it can't stay inside the band
                ball.Location.Y = this.Location.Y - ball.SpriteTexture.Height;

                //Pop a word in the middle of the screen instead of logging the locations,
                //it's the same feedback but where the player is already watching the ball
                floatingText.Show(hitWords[random.Next(hitWords.Length)], Color.Gold);
            }
        }

        private void KeepPaddleOnScreen()
        {
            this.Location.X = MathHelper.Clamp(this.Location.X, 0, this.Game.GraphicsDevice.Viewport.Width - this.spriteTexture.Width);
        }
    }
}
