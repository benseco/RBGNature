﻿using Microsoft.Xna.Framework;

namespace RBGNature
{
    class Camera
    {
        public Camera()
        {
            Zoom = 1;
            Position = Vector2.Zero;
            Rotation = 0;
            Origin = Vector2.Zero;
            Position = Vector2.Zero;
        }

        public float Zoom { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }

        public void Move(Vector2 direction)
        {
            Position += direction;
        }

        public Matrix GetTransform()
        {
            var translationMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
            //var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Origin.X, Origin.Y, 0));

            return translationMatrix * scaleMatrix * originMatrix;
        }

        //screen center, hardcode to 4x right now as the screen resolution is hardcoded to 2560x1440
        public Vector2 FocalPoint
        {
            get { return Origin * 4; }
        }
    }
}
