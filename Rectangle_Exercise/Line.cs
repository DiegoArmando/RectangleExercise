using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rectangle_Exercise
{
	class Line
	{
		//In a rendering engine, a texture referes to a collection of pixel data used to render an object.
		Texture2D lineTex;
		Vector2 startPoint;
		Vector2 endPoint;

		
		public Color color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		} 

		//The Sprite Batch is a way of communicating with the renderer.
		SpriteBatch sb;

		public Line(Vector2 startPointArg, Vector2 endPointArg, Color rectColor, GraphicsDevice gd)
		{
			startPoint = startPointArg;
			endPoint = endPointArg;
			color = rectColor;

			//Here, we are making a texture which is a single pixel. This will be stretched to form our lines.
			lineTex = new Texture2D(gd, 1, 1);

			//The texture must be sent an array of colors, each index representing one pixel. 
			lineTex.SetData<Color>(new Color[] { color });
			sb = new SpriteBatch(gd);//This points the spritebatch to the renderer to allow us to make draw calls.
		}

		public void Draw()
		{
			Vector2 edge = endPoint - startPoint;
			float angle = (float)Math.Atan2(edge.Y, edge.X);

			//We are using the rectangle here as an arugment to render the line. This is necessary to achieve the solid lines
			//and transparent interior to each rectangle which these lines are a part of.
			sb.Draw(lineTex, new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)edge.Length(), 1), null, color, angle, new Vector2(0, 0), SpriteEffects.None, 0);

		}
	}
}
