using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace Rectangle_Exercise
{
	/// <summary>
	/// This is the main type for the project. In Monogame, the Game class is what handles drawing and updating, as well 
	/// as content management.
	/// </summary>
	public class RectangleWindow : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		List<Rect> rectList;

		//Was the R key not pressed the previous frame? Prevents a refresh per frame.
		bool wasJustUp = true;

		//Is the user currently dragging a box? If so, don't drag any others.
		bool dragging = false;
		
		public RectangleWindow()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			rectList = new List<Rect>();
			for (int i = 0; i < 5; i++)
			{
				rectList.Add(new Rect(GraphicsDevice, i));
			}

			this.IsMouseVisible = true;//Without this, the mouse is invisible

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
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
