using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    /// <summary>
    /// Used to draw debug information
    /// </summary>
    static class CustomDebug
    {
        private static Texture2D pixel;

        /// <summary>
        /// Create a single pixel 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        private static void CreatePixel(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }
       
        /// <summary>
        /// Draw line between 2 points
        /// </summary>
        /// <param name="sb">sprite batch</param>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="thickness">thickness of line</param>
        /// <param name="color">color of line</param>
        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, float thickness, Color color)
        {
            if (pixel == null)
            {
                CreatePixel(sb.GraphicsDevice);
            }

            float angle = MathF.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);
            sb.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw line with preset of direction
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="start">start point</param>
        /// <param name="length">length of line</param>
        /// <param name="direction">direction</param>
        /// <param name="thickness">thickness of line</param>
        /// <param name="color">color of line</param>
        public static void DrawLine(SpriteBatch sb, Vector2 start, float length, Direction direction, float thickness, Color color)
        {
            Vector2 end = direction switch
            {
                Direction.Up => new Vector2(start.X, start.Y - length),
                Direction.Down => new Vector2(start.X, start.Y + length),
                Direction.Right => new Vector2(start.X + length, start.Y),
                Direction.Left => new Vector2(start.X - length, start.Y),
                _ => start
            };

            DrawLine(sb, start, end, thickness, color);
        }

        /// <summary>
        /// Draw wire rectangle by creating 4 lines around
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="rectangle">sample rectangle </param>
        /// <param name="thickness">thickness of line</param>
        /// <param name="color">color of line</param>
        public static void DrawWireRectangle(SpriteBatch sb, Rectangle rectangle, float thickness, Color color)
        {
            // Top
            DrawLine(sb, new Vector2(rectangle.X, rectangle.Y), rectangle.Width, Direction.Right, thickness, color);
            // Bottom
            DrawLine(sb, new Vector2(rectangle.X, rectangle.Y + rectangle.Height), rectangle.Width, Direction.Right, thickness, color);
            // Left
            DrawLine(sb, new Vector2(rectangle.X, rectangle.Y), rectangle.Height, Direction.Down, thickness, color);
            // Right
            DrawLine(sb, new Vector2(rectangle.X + rectangle.Width, rectangle.Y), rectangle.Height, Direction.Down, thickness, color);
        }

        /// <summary>
        /// Draw rectangle by scaling a pixel large
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="rectangle">rectangle size</param>
        /// <param name="color">color of rectangle</param>
        public static void DrawRectangle(SpriteBatch sb, Rectangle rectangle, Color color)
        {
            if (pixel == null)
            {
                CreatePixel(sb.GraphicsDevice);
            }
            sb.Draw(pixel, rectangle, color);
        }

        /// <summary>
        /// Draw wire circle
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="center">Center of circle</param>
        /// <param name="radius">radius of circle</param>
        /// <param name="thickness">thickness of line</param>
        /// <param name="color">Color of circle</param>
        public static void DrawWireCircle(SpriteBatch sb, Vector2 center, float radius, float thickness, Color color)
        { 
            if (pixel == null)
            {
                CreatePixel(sb.GraphicsDevice);
            }

            for (int i = 0; i < 360; i++)
            {
                float angle = (float)(i * (Math.PI / 180));

                int x = (int)MathF.Ceiling(center.X + radius * MathF.Cos(angle));

                int y = (int)MathF.Ceiling(center.Y + radius * MathF.Sin(angle));
                
                sb.Draw(pixel, new Vector2(x, y), null,color, 0, Vector2.Zero, Vector2.One * thickness, SpriteEffects.None, 0 );
            }
        }
    }
}
