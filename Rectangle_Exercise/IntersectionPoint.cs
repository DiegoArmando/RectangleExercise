using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rectangle_Exercise
{
	class IntersectionPoint
	{
		Texture2D tex;

		//Are we currently using this point? If not, we won't draw it.
		public bool active = false;
		Vector2 pos;

		SpriteBatch sb;

		float size = 10f;

		public IntersectionPoint(GraphicsDevice gd)
		{
			//When they're not being used, they are moved off screen.
			pos = new Vector2(-100.0f, -100.0f);
			//Where are we drawing the coordinates in relation to the point?

			sb = new SpriteBatch(gd);

			tex = new Texture2D(gd, 1, 1);

			//font = content

			//The texture must be sent an array of colors, each index representing one pixel. 
			//Here, we make the texture a bright orange.
			tex.SetData<Color>(new Color[] { new Color(255, 69, 0) });
		}

		public void resetPos()
		{
			pos = new Vector2(-100.0f, -100.0f);
			active = false;
		}

		public void changePos(float newX, float newY)
		{
			pos = new Vector2(newX, newY);
			active = true;
		}

		public void Draw()
		{
			sb.Begin();

			//Drawthe 25 x 25 pixel box with its center as the origin
			sb.Draw(tex, new Rectangle((int)(pos.X - size / 2), (int)(pos.Y - size / 2), (int)size, (int)size), null, Color.White, 0, new Vector2(0f, 0f), SpriteEffects.None, 0);


			sb.End();
		}

	}
}
