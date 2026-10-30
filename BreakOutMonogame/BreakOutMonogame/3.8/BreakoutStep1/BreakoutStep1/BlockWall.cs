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
    /// <summary>
    /// The wall of blocks at the top of the screen.
    /// It builds the blocks, lays them out from the viewport size, and is the one place
    /// that checks the ball against them. Each block is still its own game component so
    /// it keeps drawing and updating itself, the wall only owns the layout and the hits.
    /// </summary>
    class BlockWall : GameComponent
    {
        //20 columns of 32 pixel blocks across an 800 wide window leaves an 8 pixel gap
        //between them, which is narrower than the ball. Spread the same blocks wider and
        //the ball flies up through the gaps clipping the sides of blocks instead of
        //bouncing off their faces.
        const int blockRows = 4;
        const int blockColumns = 20;
        const int wallTopMargin = 56;       //leaves room for the score HUD above the wall
        const int blockRowSpacing = 8;      //gap between the rows

        const int crackPoints = 10;         //first hit on a block
        const int breakPoints = 100;        //hit that finishes it off

        //One texture per row, the colors repeat if there are ever more rows than textures
        static readonly string[] rowTextureNames = { "block_red", "block_yellow", "block_blue", "block_greenWaveSmall" };
        const string hitTextureName = "block_bubble";   //what every block switches to once it's been hit

        //Service Dependencies
        IScoreManager score;
        IFloatingTextManager floatingText;

        //Dependencies
        Ball ball;      //Need reference to ball for collision
        protected List<MonogameBlock> blocks;

        public BlockWall(Game game, Ball b)
            : base(game)
        {
            this.ball = b;

            this.score = ScoreManager.GetOrAddService(game);
            this.floatingText = FloatingTextManager.GetOrAddService(game);

            this.blocks = new List<MonogameBlock>();
            for (int row = 0; row < blockRows; row++)
            {
                for (int column = 0; column < blockColumns; column++)
                {
                    MonogameBlock block = new MonogameBlock(game,
                        rowTextureNames[row % rowTextureNames.Length], hitTextureName);

                    this.blocks.Add(block);
                    game.Components.Add(block);     //added here so each block loads and draws itself
                }
            }
        }

        //Every block is broken, Game1 watches this to put up a fresh wall
        public bool IsCleared
        {
            get
            {
                foreach (MonogameBlock block in this.blocks)
                {
                    if (block.BlockState != BlockState.Broken)
                        return false;
                }
                return this.blocks.Count > 0;   //an empty wall is not a cleared wall
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            //The blocks went into Components in the constructor so they come before this
            //component and already have their textures, which is what the layout measures
            LayOutWall();
        }

        /// <summary>
        /// Spreads the blocks across the top of the window.
        /// Each block gets a cell of the viewport and is centered in it, so nothing here
        /// is a hard coded position and the wall still fits if the back buffer changes.
        /// </summary>
        private void LayOutWall()
        {
            Viewport vp = this.Game.GraphicsDevice.Viewport;

            int cellWidth = vp.Width / blockColumns;
            int wallLeft = (vp.Width - (cellWidth * blockColumns)) / 2;     //center any leftover pixels

            for (int row = 0; row < blockRows; row++)
            {
                for (int column = 0; column < blockColumns; column++)
                {
                    MonogameBlock block = this.blocks[(row * blockColumns) + column];

                    int blockWidth = (int)(block.SpriteTexture.Width * block.Scale);
                    int blockHeight = (int)(block.SpriteTexture.Height * block.Scale);

                    block.Location = new Vector2(
                        wallLeft + (column * cellWidth) + ((cellWidth - blockWidth) / 2),
                        wallTopMargin + (row * (blockHeight + blockRowSpacing)));
                }
            }
        }

        /// <summary>
        /// Puts every block back to new, used when the wall is cleared or the game restarts
        /// </summary>
        public void ResetWall()
        {
            foreach (MonogameBlock block in this.blocks)
            {
                block.Reset();
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Nothing to hit while the ball is still sitting on the paddle
            if (this.ball.State == BallState.Playing)
                UpdateCheckBallCollision();

            base.Update(gameTime);
        }

        private void UpdateCheckBallCollision()
        {
            foreach (MonogameBlock block in this.blocks)
            {
                if (block.BlockState == BlockState.Broken)
                    continue;   //broken blocks are off the board, the ball goes right through

                //Very simple collision with ball only uses rectangles
                //Intersects first because Intersection hands back a zero width or zero
                //height rectangle for two blocks that are only touching, and bouncing off
                //a touch pushes the ball back out by nothing and it sticks
                if (!Sprite.Intersects(block.LocationRect, this.ball.LocationRect))
                    continue;

                Rectangle overlap = Sprite.Intersection(block.LocationRect, this.ball.LocationRect);

                BounceBallOffBlock(overlap);
                HitBlock(block);

                //Only one block per update, bouncing off two in the same frame flips the
                //ball twice and it carries on through the wall like nothing happened
                break;
            }
        }

        /// <summary>
        /// Reflects the ball off the side of the block it actually came in on
        /// </summary>
        /// <param name="overlap">Where the ball and the block overlap</param>
        private void BounceBallOffBlock(Rectangle overlap)
        {
            //A wide flat overlap means the ball came in from above or below, a tall thin
            //one means it came in from a side, so flip the axis it poked in the least
            if (overlap.Width >= overlap.Height)
            {
                this.ball.Direction.Y *= -1;

                //Push the ball back out by how far it sank in so it can't hit the same block twice
                this.ball.Location.Y += Math.Sign(this.ball.Direction.Y) * overlap.Height;
            }
            else
            {
                this.ball.Direction.X *= -1;
                this.ball.Location.X += Math.Sign(this.ball.Direction.X) * overlap.Width;
            }
        }

        /// <summary>
        /// Hits a block and scores it, two hits break a block
        /// </summary>
        /// <param name="block">The block the ball just hit</param>
        private void HitBlock(MonogameBlock block)
        {
            block.HitByBall(this.ball);

            if (block.BlockState == BlockState.Broken)
            {
                this.score.AddScore(breakPoints);
                this.floatingText.Show("+" + breakPoints, Color.Gold);
            }
            else
            {
                this.score.AddScore(crackPoints);   //just cracked it, the block changes texture
            }
        }
    }
}
