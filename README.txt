Click and drag the rectangles to test various cases.

Press the R key to set the rectangles to be at a random position and of a random size.

Intersections are indicated with an orange square, and intersecting rectangles are blue.

Adjacencies are indicated by lighting up the adjacent side. 

Containment is indicated as such:
	Black: This rectangle is being contained by another rectangle.
	Purple: This rectangle contains another rectangle.

Edge cases:
A rectangle which has an adjacent edge is not considered to be contained, as the points of 
adjacent edge are on the rectangle, but not inside it.

Adjacency is not considered to be an intersection unless the other two corners of the rectangle with
the adjacent edge are contained.