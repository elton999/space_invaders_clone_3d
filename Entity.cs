using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space_inveders_clone_3d
{
    public class Entity
    {
        public Model Model;
        public Effect Effect;
        public Vector3 Position = new Vector3(0, 0, 0);
        public Matrix World;
        public Vector4 Color;
        public virtual void Start()
        {
        }
        public virtual void Update(GameTime gameTime)
        {
            if (this.Effect == null)
                this.Effect = Game1.Instance.Content.Load<Effect>("effect");
            this.World = Matrix.CreateTranslation(this.Position);
        }

        public void DrawModel(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = this.Effect;

                    this.Effect.Parameters["World"].SetValue(this.World);
                    this.Effect.Parameters["View"].SetValue(view);
                    this.Effect.Parameters["Projection"].SetValue(projection);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * this.World));
                    this.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    this.Effect.Parameters["AmbientColor"].SetValue(this.Color);
                    this.Effect.Parameters["AmbientIntensity"].SetValue(0.6f);
                    this.Effect.Parameters["DiffuseIntensity"].SetValue(0.8f);
                    this.Effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(82.0f, 12.0f, -100.0f));
                }
                mesh.Draw();
            }
        }

        // code from http://rbwhitaker.wikidot.com/collision-detection
        public bool IsCollision(Model model2, Matrix world2)
        {
            for (int meshIndex1 = 0; meshIndex1 < this.Model.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = this.Model.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                for (int meshIndex2 = 0; meshIndex2 < model2.Meshes.Count; meshIndex2++)
                {
                    BoundingSphere sphere2 = model2.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(world2);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            return false;
        }
    }
}