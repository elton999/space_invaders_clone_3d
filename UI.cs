using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space_inveders_clone_3d
{
    public class UI
    {
        public SpriteFont Font;
        private Effect SpriteEffect;
        public UI(GraphicsDevice graphics)
        {
            this.SpriteEffect = Game1.Instance.Content.Load<Effect>("SpriteWave");
            this.Font = Game1.Instance.Content.Load<SpriteFont>("font");
            this._BackBuffer = new RenderTarget2D(graphics, 1500, 1000);
        }

        float amount = 0;
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            amount = (float)gameTime.TotalGameTime.TotalSeconds * 10;
            this.SpriteEffect.Parameters["amount"].SetValue(amount);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, this.SpriteEffect, null);
            spriteBatch.Draw((Texture2D)this._BackBuffer, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }

        private RenderTarget2D _BackBuffer;
        public void DrawTexts(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            graphics.SetRenderTarget(this._BackBuffer);
            graphics.Clear(Color.Transparent);
            spriteBatch.Begin();
            if (Game1.Instance.CurrentStatus == Game1.Status.START)
                spriteBatch.DrawString(this.Font, "Press Any Key To Start", new Vector2(450, 400), Color.White);
            if (Game1.Instance.CurrentStatus == Game1.Status.GAMEOVER)
                spriteBatch.DrawString(this.Font, "Game Over", new Vector2(570, 200), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);
        }
    }
}