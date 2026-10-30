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
    public class MonogameBlock : DrawableSprite
    {
        protected Block block;

        protected string NormalTextureName, HitTextureName;
        protected Texture2D NormalTexture, HitTexture;

        private BlockState blockstate;
        public BlockState BlockState
        {
            get { return this.block.BlockState; } //encapulsate block.BlockState
            set { this.block.BlockState =  value; }
        }

        public MonogameBlock(Game game)
        : base(game)
        {
            this.block = new Block();
            NormalTextureName = "block_blue";
            HitTextureName = "block_bubble";
        }

        /// <summary>
        /// Block with its own textures, lets BlockWall give each row a different color
        /// </summary>
        /// <param name="game"></param>
        /// <param name="normalTextureName">Texture for a block that hasn't been hit</param>
        /// <param name="hitTextureName">Texture for a block that has been hit once</param>
        public MonogameBlock(Game game, string normalTextureName, string hitTextureName)
        : this(game)    //chain the default constructor then swap the texture names
        {
            NormalTextureName = normalTextureName;
            HitTextureName = hitTextureName;
        }

        protected virtual void updateBlockTexture()
        {

            this.blockstate = this.block.BlockState;
            switch (block.BlockState)
            {
                case BlockState.Normal:
                    this.Visible = true;
                    this.spriteTexture = NormalTexture;
                    break;
                case BlockState.Hit:
                    this.spriteTexture = HitTexture;
                    break;
                case BlockState.Broken:
                    this.spriteTexture = NormalTexture;
                    //this.enabled = false;
                    this.Visible = false; //don't show block

                    break;
            }
        }

        protected override void LoadContent()
        {
            this.NormalTexture = this.Game.Content.Load<Texture2D>(NormalTextureName);
            this.HitTexture = this.Game.Content.Load<Texture2D>(HitTextureName);
            updateBlockTexture(); //notice this is in loadcontent not the constuctor
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            block.UpdateBlockState();
            UnityBlockUpdate();
        }

        protected virtual void UnityBlockUpdate()
        {
            updateBlockTexture();
        }

        public void HitByBall(Ball ball)
        {
            this.block.Hit();

        }

        /// <summary>
        /// Puts the block back to new and shows it again, called by BlockWall.ResetWall
        /// </summary>
        public void Reset()
        {
            this.block.Reset();
            updateBlockTexture();   //back to the normal texture and Visible again
        }
    }
}
