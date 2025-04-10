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

    static class CustomDebug
    {
        private static Texture2D pixel;

        private static void CreatePixel(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

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

        public static void DrawRectangle(SpriteBatch sb, Rectangle rectangle, Color color)
        {
            if (pixel == null)
            {
                CreatePixel(sb.GraphicsDevice);
            }
            sb.Draw(pixel, rectangle, color);
        }

        public static void DrawString(SpriteBatch sb, SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            if (spriteFont == null)
            {
                throw new ArgumentNullException(nameof(spriteFont), "SpriteFont cannot be null.");
            }
            sb.DrawString(spriteFont, text, position, color);
        }
    }
}
