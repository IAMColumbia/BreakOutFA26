using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace BreakoutStep1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Ball ball;
        Paddle paddle;
        BlockWall wall;

        //Managers, these are game services so the game objects above look them up
        //instead of being handed a reference to them
        FloatingTextManager floatingText;
        ScoreManager score;

        const int wallClearBonus = 500;     //points for breaking every block in the wall

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Order matters here, a component that depends on a service has to be added
            //after the service it needs. InputHandler goes in first because the paddle
            //controller reads it, it used to get added as a side effect of the GameConsole.
            this.Components.Add(new InputHandler(this));

            floatingText = new FloatingTextManager(this);
            this.Components.Add(floatingText);

            score = new ScoreManager(this);
            this.Components.Add(score);

            ball = new Ball(this);
            this.Components.Add(ball);

            paddle = new Paddle(this, ball);
            this.Components.Add(paddle);

            //The wall adds its own blocks to Components, so they land in the list ahead
            //of the wall itself and have their textures loaded before it lays them out
            wall = new BlockWall(this, ball);
            this.Components.Add(wall);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.Title = "Breakout";

            //Nothing to place by hand here, the paddle, the ball and the wall all size
            //themselves off the viewport once their content is loaded

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Every sprite and manager loads its own content in its own LoadContent,
            //so there's nothing for the game itself to load here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            //All of our content came from the ContentManager, it unloads itself,
            //so there's nothing hand made here that needs cleaning up
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //The round starting and ending lives here so the score manager and the wall
            //don't have to know about each other, they just report what they know

            //Broke the whole wall, bonus points and a fresh wall to keep the ball busy
            if (wall.IsCleared)
            {
                score.AddScore(wallClearBonus);
                floatingText.Show("WALL CLEARED!", Color.LimeGreen);
                wall.ResetWall();
            }

            //Out of lives, start over. The ball already parked itself on the paddle
            //when it was missed so the player gets to launch the next game themselves
            if (score.IsGameOver)
            {
                floatingText.Show("GAME OVER", Color.OrangeRed);
                score.StartNewGame();
                wall.ResetWall();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Nothing is drawn here, every sprite and manager draws itself in base.Draw.
            //They come out in DrawOrder order, so the score HUD lands over the sprites
            //and the pop up text lands over everything.

            base.Draw(gameTime);
        }
    }
}
