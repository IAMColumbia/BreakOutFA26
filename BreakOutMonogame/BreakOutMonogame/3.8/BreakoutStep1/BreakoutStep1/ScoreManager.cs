using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutStep1
{
    /// <summary>
    /// Interface for the ScoreManager game service so the ball can take a life and the
    /// wall can add points without either of them holding a reference to the manager
    /// </summary>
    public interface IScoreManager
    {
        int Score { get; }
        int Lives { get; }
        bool IsGameOver { get; }

        void AddScore(int points);
        void LoseLife();
        void StartNewGame();
    }

    /// <summary>
    /// Keeps the score and the lives and draws them at the top of the screen.
    /// This is the score manager the ball was waiting on to lose a life. It only keeps
    /// the numbers, Game1 is the one that decides what to do when the lives run out.
    /// </summary>
    public class ScoreManager : DrawableGameComponent, IScoreManager
    {
        public const int StartingLives = 3;

        const string fontName = "FunFont";  //same font as the pop up text so the HUD matches
        const float hudScale = 0.55f;       //FunFont is sized for the pop up text, the HUD wants it smaller
        const int hudMargin = 6;            //gap from the top left corner
        const int hudDrawOrder = 100;       //over the sprites, under the pop up text

        public int Score { get; private set; }
        public int Lives { get; private set; }

        //Out of lives, Game1 watches this to start the next game
        public bool IsGameOver { get { return this.Lives <= 0; } }

        SpriteFont font;
        SpriteBatch spriteBatch;

        public ScoreManager(Game game)
            : base(game)
        {
            this.StartNewGame();
            this.DrawOrder = hudDrawOrder;

            game.Services.AddService(typeof(IScoreManager), this);
        }

        /// <summary>
        /// Gets the ScoreManager service and adds one to the game if there isn't one yet.
        /// Same lazy load the GameConsole uses, kept in one place so every caller is one line.
        /// </summary>
        public static ScoreManager GetOrAddService(Game game)
        {
            ScoreManager manager = (ScoreManager)game.Services.GetService(typeof(IScoreManager));
            if (manager == null)    //ohh no no score manager let's add a new one
            {
                manager = new ScoreManager(game);
                game.Components.Add(manager);
            }
            return manager;
        }

        protected override void LoadContent()
        {
            this.font = this.Game.Content.Load<SpriteFont>(fontName);
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            base.LoadContent();
        }

        /// <summary>
        /// Adds points to the score, blocks are worth more when they break than when they crack
        /// </summary>
        /// <param name="points">Points to add</param>
        public void AddScore(int points)
        {
            this.Score += points;
        }

        /// <summary>
        /// Takes a life, called when the ball gets past the paddle
        /// </summary>
        public void LoseLife()
        {
            if (this.Lives > 0)     //don't go negative, IsGameOver already went true at zero
                this.Lives--;
        }

        /// <summary>
        /// Back to a fresh game, no score and a full set of lives
        /// </summary>
        public void StartNewGame()
        {
            this.Score = 0;
            this.Lives = StartingLives;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "SCORE " + this.Score + "   LIVES " + this.Lives,
                new Vector2(hudMargin, hudMargin), Color.White,
                0, Vector2.Zero, hudScale, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
