using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rectangle_Exercise
{
	//NOTE: We will not be using any of the functions of the built in Rectangle object, as that would defeat the purpose of the exercise.
	class Rect
	{
		//These will be our primary detection tools, and are extensible to rotated rectangle detection.
		Vector2 topLeft;
		Vector2 topRight;
		Vector2 botLeft;
		Vector2 botRight;

		Line top;
		Line right;
		Line left;
		Line bot;

		Color color;

		Texture2D interiorTex;

		SpriteBatch sb;

		//This will be used to determine the initial placement and size.
		Random rand = new Random();

		public Rect(GraphicsDevice gd)
		{
			color = Color.Green; //The rectangle is green when no intersections, adjacencies, or containments are occuring.
			int height = rand.Next(1, 150);
			int width = rand.Next(1, 150);

			topLeft = new Vector2(rand.Next(10, 400), rand.Next(10, 400));
			topRight = new Vector2(topLeft.X + width, topLeft.Y);
			botLeft = new Vector2(topLeft.X, topLeft.Y + height);
			botRight = new Vector2(topLeft.X + width, topLeft.Y + height);

			interiorTex = new Texture2D(gd, width, height);
			Color[] colorData = new Color[width * height];

			for (int i = 0; i < colorData.Length; ++i)
			{
				colorData[i] = color;
			}

			interiorTex.SetData<Color>(colorData);
		}
	}
}
