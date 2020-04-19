using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZAxisLevelTool
{
    class SceneObject
    {
        public string m__name;

        public Vec3 m__position;
        public Vec3 m__orientation;
        public Vec3 m__scale;

        public ObjectCollision m__objectCollision;
        public ObjectGeometry m__objectGeometry;
        public List<ObjectMaterial> m__objectMaterials;

        public SceneObject()
        {
            m__name = "";

            m__objectMaterials = new List<ObjectMaterial>();

            m__position = new Vec3();
            m__orientation = new Vec3();
            m__scale = new Vec3();
        }
    }

    class ObjectCollision
    {
        public string m__name;

        public ObjectCollision()
        {
            m__name = "";
        }
    }

    public class Triangle
    {
        public byte unk1;
        public byte unk2;
        public ushort a;
        public ushort b;
        public ushort c;

        public Triangle()
        {
        }

        public Triangle(BinaryReader br)
        {
            unk1 = br.ReadByte();
            unk2 = br.ReadByte();
            a = br.ReadUInt16();
            b = br.ReadUInt16();
            c = br.ReadUInt16();
        }
    }

    public class ObjectGeometryMesh
    {
        public uint m__face_count;
        public List<ushort> m__face_indices;

        public ObjectGeometryMesh()
        {
            m__face_indices = new List<ushort>();
        }
    }

    public class ObjectInstance
    {
        public string m__name;

        public List<ObjectGeometry> m__geometry_object;

        public Vec3 m__object_bbox_min;
        public Vec3 m__object_bbox_max;

        public Vec3 m__object_position;
        public Vec3 m__object_orientation;
        public Vec3 m__object_scale;

        public List<Vec3> m__verts;

        public ObjectInstance()
        {
            m__name = "";

            m__geometry_object = new List<ObjectGeometry>();

            m__object_bbox_min = new Vec3();
            m__object_bbox_max = new Vec3();

            m__object_position = new Vec3();
            m__object_orientation = new Vec3();
            m__object_scale = new Vec3();

            m__verts = new List<Vec3>();
        }
    }

    public class ObjectGeometry
    {
        public string m__name;
        
        public uint m__ref_count;

        public uint m__num_verts;
        public uint m__num_meshes;
        public uint m__num_triangles;

        public List<Vec3> m__positions;
        public List<Vec3> m__normals;
        public List<Vec2> m__texcoords;
        public List<uint> m__colors;
        public List<Triangle> m__triangles;
        public List<ObjectMaterial> m__materials;

        public List<ObjectGeometryMesh> m__meshes;

        public ObjectGeometry()
        {
            m__name = "";

            m__positions = new List<Vec3>();
            m__normals = new List<Vec3>();
            m__texcoords = new List<Vec2>();
            m__colors = new List<uint>();

            m__triangles = new List<Triangle>();
            m__materials = new List<ObjectMaterial>();
            m__meshes = new List<ObjectGeometryMesh>();
        }

        static public bool ObjectNameExists(string name, List<ObjectGeometry> objects)
        {
            int chunkCount = objects.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (objects[i].m__name == name)
                    return true;
            }

            return false;
        }

        static public ObjectGeometry GetObjectByName(string name, List<ObjectGeometry> objects)
        {
            int chunkCount = objects.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (objects[i].m__name == name)
                    return objects[i];
            }

            return null;
        }

        public static string GetUniqueName(string name, List<ObjectGeometry> objects)
        {
            string unique_name = "";
            int _newNameIndex = 0;

            if (!ObjectGeometry.ObjectNameExists(name, objects))
            {
                unique_name = name;
            }
            else
            {
                while (ObjectGeometry.ObjectNameExists(String.Format("{0}_{1}", name, _newNameIndex.ToString("D2")), objects))
                {
                    _newNameIndex++;
                }

                unique_name = String.Format("{0}_{1}", name, _newNameIndex.ToString("D2"));
            }

            return unique_name;
        }
    }

    public class ObjectMaterial
    {
        public string m__name;
        public uint m__checksum;
        public List<Texture> m__textures;

        public ObjectMaterial()
        {
            m__name = "";
            m__textures = new List<Texture>();
        }
    }
}
