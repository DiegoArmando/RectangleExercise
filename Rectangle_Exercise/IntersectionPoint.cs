using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
		enum placement { topLeft, topRight, botLeft, botRight };

		placement whereToPut;
		Vector2 pos;

		SpriteBatch sb;
	}
}
