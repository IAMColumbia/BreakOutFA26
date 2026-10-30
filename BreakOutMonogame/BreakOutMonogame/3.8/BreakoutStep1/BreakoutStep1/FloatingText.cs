using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutStep1
{
    /// <summary>
    /// One line of pop up text that drifts up the screen and fades out.
    /// Same split as Block and MonogameBlock, this class just holds the state and
    /// FloatingTextManager is the game component that loads the font and draws it.
    /// </summary>
    public class FloatingText
    {
        public string Text { get; private set; }
        public Color Color { get; private set; }
        public Vector2 Location;    //center of the text, moves up as the life runs down

        const float startingLife = 1.1f;    //seconds the text stays on screen
        const float floatSpeed = 90;        //pixels per second the text drifts up

        protected float life;   //seconds of life left

        public FloatingText(string text, Vector2 location, Color color)
        {
            this.Text = text;
            this.Location = location;
            this.Color = color;
            this.life = startingLife;
        }

        //Done means the manager can stop drawing this text and drop it
        public bool IsDone { get { return this.life <= 0; } }

        //1 when the text spawns down to 0 when it's done, used to fade and grow the text
        public float LifeLeft { get { return MathHelper.Clamp(this.life / startingLife, 0, 1); } }

        public void Update(GameTime gameTime)
        {
            //Same seconds from milliseconds the sprites use so the drift matches the ball speed
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            this.life -= elapsedSeconds;
            this.Location.Y -= floatSpeed * elapsedSeconds;
        }
    }
}
