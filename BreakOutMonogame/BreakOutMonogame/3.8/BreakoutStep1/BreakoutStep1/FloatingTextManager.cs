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
    /// Interface for the FloatingTextManager game service so anything in the game can
    /// pop text on the screen the same way it writes to the GameConsole
    /// </summary>
    public interface IFloatingTextManager
    {
        void Show(string text);
        void Show(string text, Color color);
    }

    /// <summary>
    /// Draws every live FloatingText in the fun font, starting in the middle of the
    /// screen and floating up. This replaces logging hits and locations to the console,
    /// the text shows up where the player is already looking instead of in a corner.
    /// </summary>
    public class FloatingTextManager : DrawableGameComponent, IFloatingTextManager
    {
        const string fontName = "FunFont";  //built by the pipeline from Content/FunFont.spritefont
        const int maxOnScreen = 6;          //oldest text gets dropped past this so the middle stays readable
        const int spawnSpread = 70;         //random left right spread so two hits in a row don't stack up
        const int lineHeight = 44;          //a line of FunFont, used to stack text that spawns together
        const float startScale = 0.8f;      //text starts a little small and grows as it rises
        const float growScale = 0.4f;
        const int textDrawOrder = 200;      //draw over the sprites and the score HUD

        SpriteFont font;
        SpriteBatch spriteBatch;

        protected List<FloatingText> floatingText;

        static readonly Random random = new Random();   //shared so the spread doesn't repeat per manager

        public FloatingTextManager(Game game)
            : base(game)
        {
            this.floatingText = new List<FloatingText>();
            this.DrawOrder = textDrawOrder;

            game.Services.AddService(typeof(IFloatingTextManager), this);
        }

        /// <summary>
        /// Gets the FloatingTextManager service and adds one to the game if there isn't one yet.
        /// Same lazy load the GameConsole uses, kept in one place so every caller is one line.
        /// </summary>
        public static FloatingTextManager GetOrAddService(Game game)
        {
            FloatingTextManager manager = (FloatingTextManager)game.Services.GetService(typeof(IFloatingTextManager));
            if (manager == null)    //ohh no no manager let's add a new one
            {
                manager = new FloatingTextManager(game);
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
        /// Pops text in the middle of the screen in the default color
        /// </summary>
        /// <param name="text">What to show, short words read best while the ball is moving</param>
        public void Show(string text)
        {
            this.Show(text, Color.White);
        }

        /// <summary>
        /// Pops text in the middle of the screen
        /// </summary>
        /// <param name="text">What to show, short words read best while the ball is moving</param>
        /// <param name="color">Color to draw it in, it fades out from here</param>
        public void Show(string text, Color color)
        {
            Viewport vp = this.Game.GraphicsDevice.Viewport;

            //Start in the middle of the screen and drop a line for anything still sitting
            //there. Text that spawns on the same update, like MISS! and GAME OVER, would
            //otherwise draw right on top of each other and read as one scribble.
            //The list is oldest first and old text has already floated up, so one pass down
            //the list leaves the new line under everything it would have covered.
            float spawnY = vp.Height / 2f;
            foreach (FloatingText onScreen in this.floatingText)
            {
                if (Math.Abs(onScreen.Location.Y - spawnY) < lineHeight)
                    spawnY = onScreen.Location.Y + lineHeight;
            }

            //Nudged sideways too so a fast rally doesn't draw a straight column
            Vector2 location = new Vector2(
                vp.Width / 2f + random.Next(-spawnSpread, spawnSpread),
                spawnY);

            this.floatingText.Add(new FloatingText(text, location, color));

            if (this.floatingText.Count > maxOnScreen)
                this.floatingText.RemoveAt(0);  //oldest text goes first
        }

        public override void Update(GameTime gameTime)
        {
            //Walk the list backwards so finished text can be removed inside the loop
            for (int i = this.floatingText.Count - 1; i >= 0; i--)
            {
                this.floatingText[i].Update(gameTime);

                if (this.floatingText[i].IsDone)
                    this.floatingText.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (FloatingText text in this.floatingText)
            {
                float lifeLeft = text.LifeLeft;
                float scale = startScale + (1 - lifeLeft) * growScale;

                //Measure so the text is centered on its location instead of hanging off to the right
                Vector2 size = font.MeasureString(text.Text) * scale;

                //The font is built with premultiplied alpha so scaling the whole color fades it out
                spriteBatch.DrawString(font, text.Text, text.Location - size / 2f, text.Color * lifeLeft,
                    0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
