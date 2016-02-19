using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace Rectangle_Exercise
{
	/// This is the main type for the project. In Monogame, the Game class is what handles drawing and updating, as well 
	/// as content management.
	public class RectangleWindow : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		List<Rect> rectList;

		//Was the R key not pressed the previous frame? Prevents a refresh per frame.
		bool wasJustUp = true;

		//Is the user currently dragging a box? If so, don't drag any others.
		bool dragging = false;
		
		//The class for the window. This initalizes the graphics handling, and is called by Monogame before Initialize()
		public RectangleWindow()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// Allows the program to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content. 
		protected override void Initialize()
		{

			rectList = new List<Rect>();
			for (int i = 0; i < 2; i++)
			{
				rectList.Add(new Rect(GraphicsDevice));
			}

			this.IsMouseVisible = true;//Without this, the mouse is invisible

			base.Initialize();
		}


		/// LoadContent will be called once and is the place to load graphical content.
		/// In this case, we initialize the Spritebatch, since all textures are created in code
		/// instead of retrieved from a file.
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			
		}

		//Ordinarily in a Monogame project, there would also be an UnloadContent() function.
		//However, since the program only unloads content when it exits, in this case it is unnecessary. 



		/// Allows the program to run logic such as updating the rectangles,
		/// checking for collisions, and detecting input
		/// GameTime is the elapsed time since the last frame.
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.R))
			{
				if (wasJustUp)
				{
					foreach (Rect r in rectList)
					{
						r.Refresh(GraphicsDevice);
					}
					wasJustUp = false;
				}

			}
			else
			{
				wasJustUp = true;
			}

			foreach (Rect r in rectList)
			{
				r.Update((Mouse.GetState().LeftButton == ButtonState.Pressed), Mouse.GetState().Position, ref dragging);
			}

			for (int i = 0; i < rectList.Count; i++)
			{
				for(int j = 0; j < rectList.Count; j++)
				{
					if (i != j)
					{
						rectList[i].checkInteraction(rectList[j]);
					}
				}
			}

			

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			foreach (Rect r in rectList)
			{
				r.Draw();
			}

			base.Draw(gameTime);
		}
	}
}
