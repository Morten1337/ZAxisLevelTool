using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ZAxisLevelTool
{
    class BBox
    {

        class bounding_sphere
        {
            public float x, y, z, radius;

            public bounding_sphere()
            {
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
                radius = 0.0f;
            }
        }

        /*
        public static bool GetBoundingBoxData(SceneObject sceneObject, ref float min_x, ref float min_y, ref float min_z, ref float max_x, ref float max_y, ref float max_z)
        {
            min_x = sceneObject.m__meshes[0].m__positions[0].x;
            min_y = sceneObject.m__meshes[0].m__positions[0].y;
            min_z = sceneObject.m__meshes[0].m__positions[0].z;

            max_x = sceneObject.m__meshes[0].m__positions[0].x;
            max_y = sceneObject.m__meshes[0].m__positions[0].y;
            max_z = sceneObject.m__meshes[0].m__positions[0].z;

            for (int i = 0; i < sceneObject.m__meshes.Count(); i++)
            {
                for (int vi = 0; vi < sceneObject.m__meshes[i].m__positions.Count(); vi++)
                {
                    if (sceneObject.m__meshes[i].m__positions[vi].x < min_x) min_x = sceneObject.m__meshes[i].m__positions[vi].x;
                    if (sceneObject.m__meshes[i].m__positions[vi].y < min_y) min_y = sceneObject.m__meshes[i].m__positions[vi].y;
                    if (sceneObject.m__meshes[i].m__positions[vi].z < min_z) min_z = sceneObject.m__meshes[i].m__positions[vi].z;

                    if (sceneObject.m__meshes[i].m__positions[vi].x > max_x) max_x = sceneObject.m__meshes[i].m__positions[vi].x;
                    if (sceneObject.m__meshes[i].m__positions[vi].y > max_y) max_y = sceneObject.m__meshes[i].m__positions[vi].y;
                    if (sceneObject.m__meshes[i].m__positions[vi].z > max_z) max_z = sceneObject.m__meshes[i].m__positions[vi].z;
                }
            }
            return true;
        }
        */

        public static bool GetBoundingBoxData(List<Vec3> positions, ref float min_x, ref float min_y, ref float min_z, ref float max_x, ref float max_y, ref float max_z)
        {
            min_x = positions[0].x;
            min_y = positions[0].y;
            min_z = positions[0].z;

            max_x = positions[0].x;
            max_y = positions[0].y;
            max_z = positions[0].z;

            for (int vi = 0; vi < positions.Count(); vi++)
            {
                if (positions[vi].x < min_x) min_x = positions[vi].x;
                if (positions[vi].y < min_y) min_y = positions[vi].y;
                if (positions[vi].z < min_z) min_z = positions[vi].z;

                if (positions[vi].x > max_x) max_x = positions[vi].x;
                if (positions[vi].y > max_y) max_y = positions[vi].y;
                if (positions[vi].z > max_z) max_z = positions[vi].z;
            }
            return true;
        }

        public static bool GetBoundingSphereData(ref float[] center, ref float[] diag, ref float diag_length, ref float radius, ref float min_x, ref float min_y, ref float min_z, ref float max_x, ref float max_y, ref float max_z)
        {
            center[0] = (max_x + min_x) / 2;
            diag[0] = (max_x - min_x);
            diag_length += (diag[0] * diag[0]);

            center[1] = (max_y + min_y) / 2;
            diag[1] = (max_y - min_y);
            diag_length += (diag[1] * diag[1]);

            center[2] = (max_z + min_z) / 2;
            diag[2] = (max_z - min_z);
            diag_length += (diag[2] * diag[2]);

            diag_length = (float)Math.Sqrt(diag_length);
            radius = diag_length / 2.0f;
            return true;
        }
    }
}
