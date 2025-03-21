using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization
{
    static class CustomDebug
    {
        private static Texture2D pixel;
        private static void CreatePixel(SpriteBatch sb)
        {
            pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        #region // Line drawing

        /// <summary>
        /// Draws a singular line between 2 points, centered so
        /// rotation doesn't surround top-left pixel position
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="point1">Vector2 point to start line drawing</param>
        /// <param name="point2">Vector2 point to end line drawing</param>
        /// <param name="thickness">Line thickness (px)</param>
        /// <param name="color">Color of line to draw</param>
        public static void DrawLineCentered(
            this SpriteBatch sb,
            Vector2 point1,
            Vector2 point2,
            float thickness,
            Color color
        )
        {
            // make angle that faces perpendicular to line between two points
            float angle = MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            angle -= MathF.PI / 2;

            // offset is halfway across thickness in perpendicular direction
            Vector2 offset = new(
                MathF.Cos(angle) * thickness / 2,
                MathF.Sin(angle) * thickness / 2
            );

            // draw with point offset applied
            DrawLine(sb, point1 + offset, point2 + offset, thickness, color);
        }

        /// <summary>
        /// Draws a line to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="point">Starting point of line</param>
        /// <param name="length">Length of line</param>
        /// <param name="angle">Angle (in radians) of line (0 facing right)</param>
        /// <param name="thickness">Thickness of line (in pixels)</param>
        /// <param name="color">Color of line to draw</param>
        public static void DrawLine(
            SpriteBatch sb,
            Vector2 point,
            float length,
            float angle,
            float thickness,
            Color color
        )
        {
            if (pixel == null)
            {
                CreatePixel(sb);
            }

            sb.Draw(
                pixel,
                point,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0
            );
        }

        /// <summary>
        /// Draws a line to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="point1">Start point of line</param>
        /// <param name="point2">End point of line</param>
        /// <param name="thickness">Thickness of line (in pixels)</param>
        /// <param name="color">Color of line to draw</param>
        public static void DrawLine(
            SpriteBatch sb,
            Vector2 point1,
            Vector2 point2,
            float thickness,
            Color color
        )
        {
            float distance = Vector2.Distance(point1, point2);
            float angle = MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(sb, point1, distance, angle, thickness, color);
        }

        /// <summary>
        /// Draws a line to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="thickness">Thickness of line (in pixels)</param>
        /// <param name="color">Color of line to draw</param>
        public static void DrawLine(
            SpriteBatch sb,
            int x1,
            int y1,
            int x2,
            int y2,
            float thickness,
            Color color
        )
        {
            Vector2 point1 = new(x1, y1);
            Vector2 point2 = new(x2, y2);
            DrawLine(sb, point1, point2, thickness, color);
        }

        #endregion

        #region // Rectangle drawing

        /// <summary>
        /// Draws a rectangle outline/border
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="thickness">Thickness of rectangle's lines</param>
        /// <param name="color">Color of rectangle</param>
        public static void DrawRectOutline(
            SpriteBatch sb,
            Rectangle rect,
            float thickness,
            Color color
        )
        {
            // top line
            DrawLine(sb,
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                thickness,
                color
            );

            // right line
            DrawLine(sb,
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                thickness,
                color
            );

            // bottom line
            DrawLine(sb,
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height),
                thickness,
                color
            );

            // left line
            DrawLine(sb,
                new Vector2(rect.X, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y),
                thickness,
                color
            );
        }

        /// <summary>
        /// Draws a rectangle outline/border
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="x">X coordinate of rectangle</param>
        /// <param name="y">Y coordinate of rectangle</param>
        /// <param name="width">Width of rectangle</param>
        /// <param name="height">Height of rectangle</param>
        /// <param name="thickness">Thickness of rectangle's lines</param>
        /// <param name="color">Color of rectangle</param>
        public static void DrawRectOutline(
            SpriteBatch sb,
            int x,
            int y,
            int width,
            int height,
            float thickness,
            Color color
        )
        {
            Rectangle rectToDraw = new(x, y, width, height);
            DrawRectOutline(sb, rectToDraw, thickness, color);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="rect">Rectangle itself that is drawn</param>
        /// <param name="color">Color of rectangle</param>
        public static void DrawRectFill(SpriteBatch sb, Rectangle rect, Color color)
        {
            if (pixel == null)
            {
                CreatePixel(sb);
            }

            sb.Draw(pixel, rect, color);
        }


        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="x">X coordinate of rectangle</param>
        /// <param name="y">Y coordinate of rectangle</param>
        /// <param name="width">Width of rectangle</param>
        /// <param name="height">Height of rectangle</param>
        /// <param name="color">Color of rectangle</param>
        public static void DrawRectFill(
            SpriteBatch sb,
            int x,
            int y,
            int width,
            int height,
            Color color
        )
        {
            Rectangle rectToDraw = new(x, y, width, height);
            DrawRectFill(sb, rectToDraw, color);
        }

        #endregion

        #region // Circle drawing

        /// <summary>
        /// Draws a circle outline to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="centerPoint">Center of circle</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="divisions">
        /// Number of subdivisions of circle, higher value means
        /// higher number of sides (and more circle-y circle)
        /// </param>
        /// <param name="thickness">Thickness of circle outline line</param>
        /// <param name="color">Color of circle</param>
        public static void DrawCircleOutline(
            SpriteBatch sb,
            Vector2 centerPoint,
            float radius,
            int divisions,
            float thickness,
            Color color
        )
        {
            // set up variables
            float angleStep = 2 * MathF.PI / divisions;
            float angle = 0;
            Vector2 thisPos = new Vector2(
                MathF.Cos(angle) * radius,
                MathF.Sin(angle) * radius
            ) + centerPoint;

            // iterate for number of divisions, rotate around
            //   360 degrees and draw line at each iteration
            for (int i = 0; i < divisions; i++)
            {
                // increase angle
                angle += angleStep;

                // set new vector positions based on angle
                Vector2 prevPos = thisPos;
                thisPos = new Vector2(
                    MathF.Cos(angle) * radius,
                    MathF.Sin(angle) * radius
                ) + centerPoint;

                // draw line with these positions
                DrawLine(sb, prevPos, thisPos, thickness, color);
            }
        }

        /// <summary>
        /// Draws a circle outline to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="centerX">X coordinate of circle center</param>
        /// <param name="centerY">Y coordinate of circle center</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="divisions">
        /// Number of subdivisions of circle, higher value means
        /// higher number of sides (and more circle-y circle)
        /// </param>
        /// <param name="thickness">Thickness of circle outline line</param>
        /// <param name="color">Color of circle</param>
        public static void DrawCircleOutline(
            SpriteBatch sb,
            int centerX,
            int centerY,
            float radius,
            int divisions,
            float thickness,
            Color color
        )
        {
            DrawCircleOutline(
                sb,
                new Vector2(centerX, centerY),
                radius,
                divisions,
                thickness,
                color
            );
        }

        /// <summary>
        /// Draws a filled circle to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="centerPoint">Center of circle</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="divisions">
        /// Number of subdivisions of circle, higher value means
        /// higher number of sides (and more circle-y circle)
        /// </param>
        /// <param name="color">Color of circle</param>
        public static void DrawCircleFill(
            SpriteBatch sb,
            Vector2 centerPoint,
            float radius,
            int divisions,
            Color color
        )
        {
            // set up variables
            float angleStep = 2 * MathF.PI / divisions;
            float angle = 0;
            Vector2 edgePos = new Vector2(
                MathF.Cos(angle) * radius,
                MathF.Sin(angle) * radius
            ) + centerPoint;

            // thickness equals the distance between two edge points
            float thickness = Vector2.Distance(
                edgePos,
                new Vector2(
                    MathF.Cos(angle + angleStep) * radius,
                    MathF.Sin(angle + angleStep) * radius
                ) + centerPoint
            );

            // iterate for number of divisions, rotate around
            //   360 degrees and draw line between center point
            //   an edge point in the circle
            for (int i = 0; i < divisions; i++)
            {
                DrawLineCentered(sb, centerPoint, edgePos, thickness, color);

                angle += angleStep;

                edgePos = new Vector2(
                    MathF.Cos(angle) * radius,
                    MathF.Sin(angle) * radius
                ) + centerPoint;
            }
        }

        /// <summary>
        /// Draws a filled circle to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="centerX">X coordinate of circle center</param>
        /// <param name="centerY">Y coordinate of circle center</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="divisions">
        /// Number of subdivisions of circle, higher value means
        /// higher number of sides (and more circle-y circle)
        /// </param>
        /// <param name="color">Color of circle</param>
        public static void DrawCircleFill(
            SpriteBatch sb,
            int centerX,
            int centerY,
            float radius,
            int divisions,
            Color color
        )
        {
            DrawCircleFill(
                sb,
                new Vector2(centerX, centerY),
                radius,
                divisions,
                color
            );
        }

        #endregion

        #region // Triangle drawing

        /// <summary>
        /// Draws a triangle to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        /// <param name="point1">First triangle vertex</param>
        /// <param name="point2">Second triangle vertex</param>
        /// <param name="point3">Third triangle vertex</param>
        /// <param name="thickness">Thickness of circle outline line</param>
        /// <param name="color">Color of triangle</param>
        public static void DrawTriangleOutline(
            SpriteBatch sb,
            Vector2 point1,
            Vector2 point2,
            Vector2 point3,
            float thickness,
            Color color
        )
        {
            DrawLineCentered(sb, point1, point2, thickness, color);
            DrawLineCentered(sb, point2, point3, thickness, color);
            DrawLineCentered(sb, point3, point1, thickness, color);
        }

        #endregion
    }
}
