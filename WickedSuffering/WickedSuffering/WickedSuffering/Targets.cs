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
    class Targets
    {
        public float[,] heightdata;
        Model target;
        Camera c;
        ContentManager content;
        List<Vector3> positions;
        int terrainHeight;
        int terrainWidth;

        public Targets(Camera c, ContentManager content)
        {
            this.c = c;
            this.content = content;
            positions = new List<Vector3>();
           

        }

        private void generatePositions(){
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int x = random.Next((-terrainWidth / 2) + 20, (terrainWidth / 2) -20);
                int z = random.Next((-terrainHeight / 2) + 20, (terrainHeight / 2) -20);
                Vector3 pos = new Vector3(x, heightdata[(terrainHeight / 2) + x, (terrainWidth / 2) - z], z);
                positions.Add(pos);
            }
        }

        public void loadContent(int width, int height, float[,] heightdata)
        {
            
            target = content.Load<Model>("Models/nanosuit/nanosuit");
            this.terrainWidth = width;
            this.terrainHeight = height;
            this.heightdata = heightdata;
            generatePositions();

            //effect = content.Load<Effect>("Heightmap/effects");
            //AK47.Meshes[0].MeshParts[0].Effect = effect;

        }

        public void DrawTarget(GameTime gametime)
        {
            for (int i = 0; i < positions.Count; i++)
            {


                // draw model infront of screen .. with no shaders needed
                Matrix[] transforms = new Matrix[target.Bones.Count];
                target.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in target.Meshes)
                {

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.LightingEnabled = true;
                      



                        effect.World = Matrix.CreateScale(0.8f, 0.8f, 0.8f) * transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(positions[i]);
                        effect.View = c.View;
                        effect.Projection = c.Projection;
                    }

                    mesh.Draw();

                    /*
                    Matrix worldMatrix = Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(modelPosition);
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
                            currentEffect.Parameters["xAmbient"].SetValue(0.5f);


                        }

                        mesh.Draw();
                    }

                    */

                }

            }

        }






    }
}
