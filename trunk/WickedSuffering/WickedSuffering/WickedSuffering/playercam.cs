﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WickedSuffering
{
    class playercam
    {
        Model AK47;
      float modelRotation = 0.0f;
        Vector3 modelPosition = Vector3.Zero;
        
        public float[,] heightdata; 

        int terrainwidth;
        
        int terrainheight;

        GraphicsDevice device;

        Camera c;

        MouseState OrigMouseState;

        ContentManager content;

        float movspeed = 30.0f;

        float rotspeed = 0.3f;

        float HorizonRot = MathHelper.PiOver2;

        float VerticalRot = -MathHelper.Pi / 10.0f;

        public playercam(GraphicsDevice device, Camera c, ContentManager content)
        {
            this.c = c;
            this.device = device;
            this.content = content;
            
        }


        public void loadcontent(float[,] heightdata, int terrainlength, int terrainheight)
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            this.heightdata = heightdata;
            this.terrainheight = terrainheight;
            this.terrainwidth = terrainlength;
            OrigMouseState = Mouse.GetState();  
            AK47 = content.Load<Model>("Models/AK");
            
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
            Vector3 pos = c.Position;
            pos += movspeed * rotatedVector;
            if (pos.X > -349 && pos.Z > -349 && pos.X < 349 && pos.Z < 349)
            {
                c.Position = pos;
            }

                // Y coordinates is set to 10 above the heightdata altitude, remove this line to wonder in space again.
                c.Position = new Vector3(c.Position.X, heightdata[(terrainheight / 2) + (int)c.Position.X, (terrainwidth / 2) - (int)c.Position.Z] + 10, c.Position.Z);




            UpdateViewMatrix();
        }

        public void DrawAK47(GameTime gametime)
        {
            // draw model infront of screen .. with no shaders needed
            Matrix[] transforms = new Matrix[AK47.Bones.Count];
            AK47.CopyAbsoluteBoneTransformsTo(transforms);

          
            foreach (ModelMesh mesh in AK47.Meshes)
            {
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(new Vector3(100,100,-100),
                        Vector3.Zero, Vector3.Up);
                  effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), device.Viewport.AspectRatio,
                        1.0f, 10000.0f);
                }
                
                mesh.Draw();

           /* Matrix worldMatrix = Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(modelPosition);    
            Matrix[] Transforms = new Matrix[AK47.Bones.Count];
            AK47.CopyAbsoluteBoneTransformsTo(Transforms);
            foreach (ModelMesh mesh in AK47.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["ColoredNoShading"];
                    currentEffect.Parameters["xWorld"].SetValue(Transforms[mesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(c.View);
                    currentEffect.Parameters["xProjection"].SetValue(c.Projection);
                  
                    /*currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
                    lightDirection.Normalize();
                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xAmbient"].SetValue(0.5f);*/


                }

               // mesh.Draw();
            }

            

        }

    }
