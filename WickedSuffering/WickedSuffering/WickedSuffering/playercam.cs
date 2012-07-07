﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WickedSuffering
{
    class playercam
    {
        public float[,] heightdata; 
        int terrainwidth;
        int terrainheight;

        GraphicsDevice device;

        Camera c;

        MouseState OrigMouseState;

        float movspeed = 30.0f;

        float rotspeed = 0.3f;

        float HorizonRot = MathHelper.PiOver2;

        float VerticalRot = -MathHelper.Pi / 10.0f;

        public playercam(GraphicsDevice device, Camera c)
        {
            this.c = c;
            this.device = device;
            
        }


        public void loadcontent(float[,] heightdata, int terrainlength, int terrainheight)
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            this.heightdata = heightdata;
            this.terrainheight = terrainheight;
            this.terrainwidth = terrainlength;
            OrigMouseState = Mouse.GetState();  
        }

        public void update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            MouseState mousestate = Mouse.GetState();

            if (mousestate != OrigMouseState)
            {
                float xDifference = mousestate.X - OrigMouseState.X;
                float yDifference = mousestate.Y - OrigMouseState.Y;

                HorizonRot -= rotspeed * xDifference * timeDifference;
                VerticalRot -= rotspeed * yDifference * timeDifference;

                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);

                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -2);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 2);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(2, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-2, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 2, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -2, 0);

            AddToCameraPosition(moveVector * timeDifference);
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = c.Position + cameraRotatedTarget;
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

        
          c.View = Matrix.CreateLookAt(c.Position, cameraFinalTarget, cameraRotatedUpVector);
        }





        
        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(VerticalRot) * Matrix.CreateRotationY(HorizonRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);

            c.Position += movspeed * rotatedVector;

           // Y coordinates is set to 10 above the heightdata altitude, remove this line to wonder in space again.
           c.Position = new Vector3(c.Position.X, heightdata[(terrainheight/ 2) + (int)c.Position.X,(terrainwidth/2) - (int)c.Position.Z] + 10, c.Position.Z);


            UpdateViewMatrix();
        }



    }
}
