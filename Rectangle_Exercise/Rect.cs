using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Rectangle_Exercise
{
	//NOTE: We will not be using any of the functions of the built in Rectangle object, as that would defeat the purpose of the exercise.
	class Rect
	{
		//These will be our primary detection tools, and are extensible to rotated rectangle detection.
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 botLeft;
		public Vector2 botRight;

		//The rectangle's upper left corner's position on the screen
		Vector2 pos;
		Vector2 initialPos;

		Line top;
		Line right;
		Line left;
		Line bot;

		Color color;

		Texture2D interiorTex;

		SpriteBatch sb;

		int height;
		int width;

		//This is a series of bools instead of an enumeration so that combinations of states can be tracked.
		public bool intersecting = false;
		public bool containing = false;
		public bool contained = false;
		public bool adjacent = false;

		//Is the box being dragged around the screen by the mouse?
		bool beingDragged = false;
		Vector2 initDragPoint;//where the mouse was when the box was clicked

		//This will be used to determine the initial placement and size.
		//While this may look excessive, usng the default rand() may result in duplicate values 
		//upon rapid generation.
		RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
		byte[] randomNumber = new byte[1];

		public Rect(GraphicsDevice gd)
		{
			sb = new SpriteBatch(gd);

			color = Color.White; //The color is set to white so that it can be shifted any way we like.
			color.A = 122;//set the alpha value to about half


			interiorTex = new Texture2D(gd, 1, 1);
			Color[] colorData = new Color[1];

			for (int i = 0; i < colorData.Length; ++i)
			{
				colorData[i] = color;
			}

			interiorTex.SetData<Color>(colorData);

			Console.Write("sb test in initialize: " + sb.ToString());

			//Sets the size and position
			Refresh(gd);
			
		}

		public void Refresh(GraphicsDevice gd)
		{
			rand.GetBytes(randomNumber);
			height = Convert.ToInt32(randomNumber[0]) % 225 + 25;
			rand.GetBytes(randomNumber);
			width = Convert.ToInt32(randomNumber[0]) % 225 + 25;

			color = Color.White;
			color.A = 122;

			topLeft = new Vector2();
			rand.GetBytes(randomNumber);
			topLeft.X = Convert.ToInt32(randomNumber[0]) % 500 + 1;

			rand.GetBytes(randomNumber);
			topLeft.Y = Convert.ToInt32(randomNumber[0]) % 500 + 1;

			topRight = new Vector2(topLeft.X + width, topLeft.Y);
			botLeft = new Vector2(topLeft.X, topLeft.Y + height);
			botRight = new Vector2(topLeft.X + width, topLeft.Y + height);

			top = new Line(topLeft, topRight, color, gd, false, true);
			left = new Line(topLeft, botLeft, color, gd, false, false);
			right = new Line(topRight, botRight, color, gd, true, false);
			bot = new Line(botLeft, botRight, color, gd, false, false);

			pos = topLeft;
			initialPos = pos;

		}

		public void Update(bool mouseDown, Point mousePos, ref bool dragging)
		{
			Vector2 mouseVector = new Vector2(mousePos.X, mousePos.Y);
			//If the mouse is down and over the rectangle:
			if (mouseDown)
			{
				if (ContainsPoint(mouseVector))
				{
					if (!beingDragged && !dragging)
					{
						beingDragged = true;
						initDragPoint = mouseVector;
						dragging = true;
					}
				}

				if (beingDragged)
				{
					UpdatePosition(mouseVector - initDragPoint);
				}
			}
			else
			{
				if (beingDragged)
				{
					beingDragged = false;
					dragging = false;
					initialPos = pos;
					top.SetNewHome();
					left.SetNewHome();
					right.SetNewHome();
					bot.SetNewHome();
				}
			}


			//Color is priorized based on state
			if (contained)
			{
				ChangeColor(Color.Black);
			}
			else if (containing)
			{
				ChangeColor(Color.Purple);
			}
			else if(adjacent)
			{
				ChangeColor(Color.Yellow);
			}
			else if(intersecting)
			{
				ChangeColor(Color.Blue);
			}
			else if(beingDragged)
			{
				ChangeColor(new Color(color.R + 30, color.G + 30, color.B + 30, color.A));
			}
			else
			{
				ChangeColor(Color.Green);
			}

			//Set all flags as false at the end of update to avoid costly and cumbersome calculations.
			//These will be updated as needed in CheckInteractions, which is called after update.
			intersecting = false;
			containing = false;
			contained = false;
			adjacent = false;
		}

		private void UpdatePosition(Vector2 difference)
		{
			//update the position of topLeft, topRight, botLeft and botRight by the difference

			pos = initialPos + difference;
			topLeft = pos;
			topRight = new Vector2(topLeft.X + width, topLeft.Y);
			botLeft = new Vector2(topLeft.X, topLeft.Y + height);
			botRight = new Vector2(topLeft.X + width, topLeft.Y + height);

			top.UpdatePosition(difference);
			left.UpdatePosition(difference);
			right.UpdatePosition(difference);
			bot.UpdatePosition(difference);


		}

		public void checkInteraction(Rect otherRect)
		{
			if(ContainsPoint(otherRect.topLeft) && ContainsPoint(otherRect.topRight) && ContainsPoint(otherRect.botLeft) && ContainsPoint(otherRect.botRight))
			{
				containing = true;
				otherRect.contained = true;
			}
		}

		public bool ContainsPoint(Vector2 testPoint)
		{
			//Check if the point is in between the sides of the rectangle
			return (testPoint.Y > topLeft.Y && testPoint.Y < botRight.Y && testPoint.X > topLeft.X && testPoint.X < botRight.X);
		}

		void ChangeColor(Color newColor)
		{
			color = newColor;
			color.A = 122;
			top.color = newColor;
			left.color = newColor;
			right.color = newColor;
			bot.color = newColor;
		}

		public void Draw()
		{
			sb.Begin();
			int width = (int)(topRight - topLeft).Length();
			int height = (int)(botLeft - topLeft).Length();

			sb.Draw(interiorTex, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, height), null, color, 0, new Vector2(0, 0), SpriteEffects.None, 0);

			//top.Draw();
			//left.Draw();
			//right.Draw();
			//bot.Draw();
			
			sb.End();
		}
	}
}
