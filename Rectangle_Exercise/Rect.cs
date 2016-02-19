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
		//This makes it easier to identify what's happening for the user
		public int ID; 


		//These will be our primary detection tools, and are extensible to rotated rectangle detection.
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 botLeft;
		public Vector2 botRight;

		//The rectangle's upper left corner's position on the screen
		Vector2 pos;
		Vector2 initialPos;

		public Line[] lines;

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

		//The poits at which this rectangle intersects with others. This is nullable to allow the same logic for 2 or 4 intersections.
		IntersectionPoint[] intPoints;

		//Is the box being dragged around the screen by the mouse?
		bool beingDragged = false;

		//where the mouse was when the box was clicked
		Vector2 initDragPoint;

		//This will be used to determine the initial placement and size.
		//While this may look excessive, usng the default rand() may result in duplicate values 
		//upon rapid generation.
		RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
		byte[] randomNumber = new byte[1];

		public Rect(GraphicsDevice gd, int sentID)
		{
			sb = new SpriteBatch(gd);
			ID = sentID;

			color = Color.White; //The color is set to white so that it can be shifted any way we like.
			color.A = 122;//set the alpha value to about half

			intPoints = new IntersectionPoint[4];//There can be, at most, four intersections. Usually only two.

			for(int i = 0; i < 4; i++)
			{
				intPoints[i] = new IntersectionPoint(gd);
			}

			interiorTex = new Texture2D(gd, 1, 1);
			Color[] colorData = new Color[1];

			for (int i = 0; i < colorData.Length; ++i)
			{
				colorData[i] = color;
			}

			interiorTex.SetData<Color>(colorData);

			lines = new Line[4];
			
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


			//255, 255, 0, 255 is Yellow. Lines are, in order, top, left, bottom, right.
			lines[0] = new Line(topLeft, topRight, new Color(255, 255, 0, 255), gd, false, false);
			lines[1] = new Line(topLeft, botLeft, new Color(255, 255, 0, 255), gd, true, false);
			lines[2] = new Line(botLeft, botRight, new Color(255, 255, 0, 255), gd, false, true);
			lines[3] = new Line(topRight, botRight, new Color(255, 255, 0, 255), gd, false, false);

			for(int i = 0; i < 4; i++)
			{
				intPoints[i].resetPos();
			}

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
					for(int i = 0; i < 4; i++)
					{
						lines[i].SetNewHome();
					}
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
			else if(intersecting)
			{
				ChangeColor(Color.Blue);
			}
			else if(beingDragged)
			{
				ChangeColor(new Color(70, 70, 70, color.A));
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

			//All lines are invisible until calculated to be adjacent.
			for(int i = 0; i < 4; i++)
			{
				lines[i].adjacent = false;
			}

			//All intersection points are moved offscreen until proven to be needed
			for (int i = 0; i < 4; i++)
			{
				intPoints[i].resetPos();
			}
		}

		private void UpdatePosition(Vector2 difference)
		{
			//update the position of topLeft, topRight, botLeft and botRight by the difference

			pos = initialPos + difference;
			topLeft = pos;
			topRight = new Vector2(topLeft.X + width, topLeft.Y);
			botLeft = new Vector2(topLeft.X, topLeft.Y + height);
			botRight = new Vector2(topLeft.X + width, topLeft.Y + height);

			for(int i = 0; i < 4; i++)
			{
				lines[i].UpdatePosition(difference);
			}

		}

		public void checkInteraction(Rect otherRect)
		{
			//Is the other rect inside this rect? Are all four of its points contained?
			if(ContainsPoint(otherRect.topLeft) && ContainsPoint(otherRect.topRight) && ContainsPoint(otherRect.botLeft) && ContainsPoint(otherRect.botRight))
			{
				containing = true;
				otherRect.contained = true;
			}
			//Is the other rect intersecting this rect? Is at least one corner contained?
			else if(ContainsPoint(otherRect.topLeft) || ContainsPoint(otherRect.topRight) || ContainsPoint(otherRect.botLeft) || ContainsPoint(otherRect.botRight))
			{
				intersecting = true;
				otherRect.intersecting = true;
				determineIntersections(otherRect);
			}
			//Are the two overlapping in a T shape, where no corners are contained but four intersections still occur?
			else if(CheckForT(otherRect))
			{
				intersecting = true;
				otherRect.intersecting = true;
				intPoints[0].changePos(topLeft.X, otherRect.topLeft.Y);
				intPoints[1].changePos(topRight.X, otherRect.topRight.Y);
				intPoints[2].changePos(botLeft.X, otherRect.botLeft.Y);
				intPoints[3].changePos(botRight.X, otherRect.botRight.Y);
			}

			//After all other checks, check each side for adjacency
			CheckForAdj(otherRect);
			
		}

		bool CheckForT(Rect otherRect)
		{
			//If all the points of otherRect are below the top of this one, and above the bottom of this one,
			//and all the points of this one are to the right of otherRect's left, and to the left of otherRect's right,
			//the two are colliding in a T.

			//We're checking for this:
			/*

			       _____
			_______|___|__
			|      |   |  |
			|      |   |  |
			-------|---|--|
			       |___|

			*/

			bool otherBetweenTopAndBot;
			bool thisBetweenLeftAndRight;

			otherBetweenTopAndBot = (otherRect.topLeft.Y >= topLeft.Y && otherRect.botRight.Y >= topLeft.Y && otherRect.topLeft.Y <= botRight.Y && otherRect.botRight.Y <= botRight.Y);
			thisBetweenLeftAndRight = (topLeft.X >= otherRect.topLeft.X && topRight.X >= otherRect.topLeft.X && topLeft.X <= otherRect.botRight.X && topRight.X <= otherRect.botRight.X);
			return otherBetweenTopAndBot && thisBetweenLeftAndRight;
		}

		void CheckForAdj(Rect otherRect)
		{
			//If the left is the same as or a subset of the other's right or left, color it yellow.
			//Likewise, do the same for right, top and bottom.

			//Alll sides are transparent until proven to be adjacent
			
			for(int i = 0; i < 4; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					//The top and bottom are even indices, the right and left are odd indices.
					//Only check against the indices which have the same even or odd property.
					if( (i % 2) == (j % 2) )
					{
						if(i % 2 == 0)
						{
							if (Math.Abs(lines[i].startPoint.Y - otherRect.lines[j].startPoint.Y) < float.Epsilon && lines[i].startPoint.X >= otherRect.lines[j].startPoint.X && lines[i].endPoint.X <= otherRect.lines[j].endPoint.X)
							{
								adjacent = true;
								lines[i].adjacent = true;
							}
						}
						else
						{
							//if(Math.Abs(topLeft.X - otherRect.topLeft.X) < float.Epsilon && topLeft.Y >= otherRect.topLeft.Y && botLeft.Y <= otherRect.botLeft.Y)
							if (Math.Abs(lines[i].startPoint.X - otherRect.lines[j].startPoint.X) < float.Epsilon && lines[i].startPoint.Y >= otherRect.lines[j].startPoint.Y && lines[i].endPoint.Y <= otherRect.lines[j].endPoint.Y)
							{
								adjacent = true;
								lines[i].adjacent = true;
							}
						}
					}
				}
			}

		}

		void determineIntersections(Rect otherRect)
		{
			if(ContainsPoint(otherRect.topLeft))
			{
				if (ContainsPoint(otherRect.topRight))
				{
					intPoints[0].changePos(otherRect.topLeft.X, botLeft.Y);
					intPoints[1].changePos(otherRect.topRight.X, botLeft.Y);
					
				}
				else if(ContainsPoint(otherRect.botLeft))
				{
					intPoints[0].changePos(topRight.X, otherRect.topLeft.Y);
					intPoints[1].changePos(topRight.X, otherRect.botLeft.Y);
				}
				else
				{
					intPoints[0].changePos(otherRect.topLeft.X, botRight.Y);
					intPoints[1].changePos(botRight.X, otherRect.topLeft.Y);
				}
			}
			else if(ContainsPoint(otherRect.botRight))
			{
				if(ContainsPoint(otherRect.botLeft))
				{
					intPoints[0].changePos(otherRect.botLeft.X, topLeft.Y);
					intPoints[1].changePos(otherRect.botRight.X, topLeft.Y);
				}
				else if(ContainsPoint(otherRect.topRight))
				{
					intPoints[0].changePos(topLeft.X, otherRect.topRight.Y);
					intPoints[1].changePos(topLeft.X, otherRect.botRight.Y);
				}
				else
				{
					intPoints[0].changePos(otherRect.botRight.X, topLeft.Y);
					intPoints[1].changePos(topLeft.X, otherRect.botRight.Y);
				}
			}
			else if(ContainsPoint(otherRect.botLeft))
			{
				intPoints[0].changePos(otherRect.botLeft.X, topRight.Y);
				intPoints[1].changePos(topRight.X, otherRect.botLeft.Y);
			}
			//If we have reached here, the intersecting point must be the top right.
			else
			{
				intPoints[0].changePos(otherRect.topRight.X, botLeft.Y);
				intPoints[1].changePos(botLeft.X, otherRect.topRight.Y);
			}

		}

		public bool ContainsPoint(Vector2 testPoint)
		{
			//Check if the point is in between or on the sides of the rectangle
			return (testPoint.Y > topLeft.Y && testPoint.Y < botRight.Y && testPoint.X > topLeft.X && testPoint.X < botRight.X);
		}

		void ChangeColor(Color newColor)
		{
			color = newColor;
			color.A = 122;
		}

		public void Draw()
		{
			sb.Begin();
			int width = (int)(topRight - topLeft).Length();
			int height = (int)(botLeft - topLeft).Length();

			sb.Draw(interiorTex, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, height), null, color, 0, new Vector2(0, 0), SpriteEffects.None, 1);

			//if they aren't adjacent, don't draw them
			for(int i = 0; i < 4; i++)
			{
				if(lines[i].adjacent) lines[i].Draw();
				if (intPoints[i].active) intPoints[i].Draw();
			}


			//if(top.adjacent) top.Draw();
			//if(left.adjacent) left.Draw();
			//if(right.adjacent) right.Draw();
			//if(bot.adjacent) bot.Draw();

			sb.End();
		}
	}
}
