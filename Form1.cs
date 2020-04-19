#define OUTPUT_MTL_FILE
//#define DEBUG_AUTOLOAD_DEFAULT
//#define IS_EDITOR_SCENE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ZAxisLevelTool
{
    public partial class Form1 : Form
    {
        public enum Game
        {
            DAVEMIRRA2 = 0,
            AGGRESSIVE_INLINE = 1,
            BMXXX = 2,
        };
        
#if DEBUG_AUTOLOAD_DEFAULT
        static bool DebugAutoLoad = true;
#endif

        NodeTree.NodeChunk gRoot;

        List<ObjectInstance> gInstances;
        List<ObjectGeometry> gGeometry;
        List<Texture> gTextures;
        List<ObjectMaterial> gMaterials;

        Game gGame = Game.AGGRESSIVE_INLINE;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = (int)gGame;
            this.BackColor = Color.FromName("AppWorkspace");
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(WindowHandleDragEnter);
            this.DragLeave += new EventHandler(WindowHandleDragLeave);
            this.DragDrop += new DragEventHandler(WindowHandleDragDrop);
            gInstances = new List<ObjectInstance>();
            gGeometry = new List<ObjectGeometry>();
            gMaterials = new List<ObjectMaterial>();
            gTextures = new List<Texture>();

#if DEBUG_AUTOLOAD_DEFAULT
            DebugAutoLoad = true;
            string DebugSceneName = "FUNHOUSE";
            string DebugAssetsPath = "C:\\projects\\game_assets\\Aggressive Inline\\root\\assets\\";

            string[] files = {
                                 //String.Format("{0}levels\\{1}\\{1}.DAT", DebugAssetsPath, DebugSceneName),
                                 String.Format("{0}editor\\{1}\\{1}.TXR", DebugAssetsPath, DebugSceneName),
                                 String.Format("{0}editor\\{1}\\{1}.GEO", DebugAssetsPath, DebugSceneName), 
                                 String.Format("{0}editor\\{1}\\{1}.INS", DebugAssetsPath, DebugSceneName),
                             };
            ProcessFiles(files);
            DebugAutoLoad = false;
#endif
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                saveFileDialog1.Filter = "Wavefront OBJ (*.obj)|*.obj|All files (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // dump dds files to folder 
                    write_obj_file(saveFileDialog1.FileName, gInstances);
                    write_mtl_file(saveFileDialog1.FileName, gMaterials);

                    String texturePath = saveFileDialog1.FileName.Replace(Path.GetFileName(saveFileDialog1.FileName), "textures\\");

                    if (!Directory.Exists(texturePath))
                        Directory.CreateDirectory(texturePath);

                    Texture.dump_textures(texturePath, gTextures, gGame);
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        void WindowHandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            this.BackColor = Color.FromName("ActiveCaption");
        }

        void WindowHandleDragLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.FromName("AppWorkspace");
        }

        void WindowHandleDragDrop(object sender, DragEventArgs e)
        {
            this.BackColor = Color.FromName("AppWorkspace");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ProcessFiles(files);
        }

        List<Vec3> TransformVertexPositions(List<Vec3> _positions, Vec3 _worldPosition, Vec3 _lookVector, Vec3 _upVector, Vec3 _scale, float _worldScale)
        {

            // TODO:

            // Fix bug: Some objects dont have the correct orientation.
            // Use _upVector ?

            // Setup Transform 4x4 matrix
            Matrix _tempTransforms = new Matrix(4, 4);

            _worldPosition.Multiply(_upVector);

            // Set world pos offset
            _tempTransforms[0, 0] = _worldPosition.x;
            _tempTransforms[0, 1] = _worldPosition.y;
            _tempTransforms[0, 2] = _worldPosition.z;
            _tempTransforms[0, 3] = 1f;

            // Set look vector 
            _tempTransforms[1, 0] = _lookVector.x;
            _tempTransforms[1, 1] = _lookVector.y;
            _tempTransforms[1, 2] = _lookVector.z;
            _tempTransforms[1, 3] = 0f;

            for (int vi = 0; vi < _positions.Count(); vi++)
            {

                // Setup temp 4x4 matrix
                Matrix A = new Matrix(4, 4);
                Matrix B = new Matrix(4, 4);

                // Set vertex position
                A[0, 0] = _positions[vi].x;
                A[0, 1] = _positions[vi].y;
                A[0, 2] = _positions[vi].z;
                A[0, 3] = 1f;

                // Apply rotation transform
                B = _tempTransforms * A;

                // Update the vertex position with new rotated vertex
                _positions[vi].x = (float)(B[0, 0] / B[0, 3]); // v_in.x = v_out.x / v_out.w
                _positions[vi].y = (float)(B[0, 1] / B[0, 3]); // v_in.y = v_out.y / v_out.w
                _positions[vi].z = (float)(B[0, 2] / B[0, 3]); // v_in.z = v_out.z / v_out.w

                // [...]

                // offset vertex position by object world position
                _positions[vi].x += _worldPosition.x;
                _positions[vi].y += _worldPosition.y;
                _positions[vi].z += _worldPosition.z;

                _positions[vi].Multiply(_upVector);

                _positions[vi].x *= _scale[0];
                _positions[vi].y *= _scale[1];
                _positions[vi].z *= _scale[2];

                // set world scale
                _positions[vi].x *= _worldScale;
                _positions[vi].y *= _worldScale;
                _positions[vi].z *= _worldScale;

                _positions[vi].z *= -1;
            }

            return _positions;
        }

        void ProcessFiles(string[] files)
        {
            foreach (string file in files)
            {
                string file_extension = Path.GetExtension(file).ToUpper();
                switch (file_extension)
                {
                    case ".GEO":
                        read_geometry(file);
                        break;
                    case ".INS":
                        read_instances(file);
                        break;
                    case ".DAT":
                        read_data(file);
                        break;
                    case ".TXR":
                        read_textures(file);
                        break;
                }
            }
            Console.WriteLine("done!");
        }

        //-------------------------------------------------------------
        //	Read .DAT file
        //-------------------------------------------------------------
        void read_data(string filename)
        {
            NodeTree.NodeChunk dataChunks;

            using (BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename.ToLower()))))
            {
                dataChunks = NodeTree.ReadRoot(br);
                br.Close();
            }
            int numDataChunks = dataChunks.chunks.Count();
            Console.WriteLine("numDataChunks {0}", numDataChunks);

            List<uint> _tempDataTypes = new List<uint>();

            for (int i = 0; i < numDataChunks; i++)
            {
                if (dataChunks.chunks[i].chunkType != NodeTree.ChunkType.SDPT)
                    continue;

                NodeTree.NodeChunk SDPTData = dataChunks.chunks[i];
                using (BinaryReader br = new BinaryReader(new MemoryStream(SDPTData.data)))
                {
                    Console.Write("SDPT data = [");
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        Console.Write("{0}, ", br.ReadSingle());
                    }
                    Console.WriteLine("]");

                    //br.BaseStream.Seek(4, SeekOrigin.Begin);

                    //uint _tempType = br.ReadUInt32();

                    //if (!_tempDataTypes.Contains(_tempType))
                    //    _tempDataTypes.Add(_tempType);
                }
            }

            for (int i = 0; i < _tempDataTypes.Count(); i++)
            {
            //    Console.WriteLine("0x{0} {1}", _tempDataTypes[i].ToString("x8"), _tempDataTypes[i]);
            }
        }

        //-------------------------------------------------------------
        //	Read .INS file
        //-------------------------------------------------------------
        void read_instances(string filename)
        {
            NodeTree.NodeChunk instanceChunks;

            using (BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename.ToLower()))))
            {
                instanceChunks = NodeTree.ReadRoot(br);
                br.Close();
            }
            int numInstanceChunks = instanceChunks.chunks.Count();
            Console.WriteLine("numInstanceChunks {0}", numInstanceChunks);

            List<string> _tempNames = new List<string>();

            for (int i = 0; i < numInstanceChunks; i++)
            {
                string _tempName;
                ObjectGeometry geomObject;
                NodeTree.NodeChunk GeometryInstance = instanceChunks.chunks[i];
                ObjectInstance tempObjectInstance = new ObjectInstance();
                //gInstances

                if (GeometryInstance.HasChunk(NodeTree.ChunkType.TGIH))
                {
                    NodeTree.NodeChunk GeometryInstanceHeader = GeometryInstance.GetChunk(NodeTree.ChunkType.TGIH);

                    using (BinaryReader br = new BinaryReader(new MemoryStream(GeometryInstanceHeader.data)))
                    {
                        if (gGame == Game.BMXXX)
                        {
                            br.BaseStream.Seek(58, SeekOrigin.Begin);
                        }
                        else if (gGame == Game.DAVEMIRRA2)
                        {
                            br.BaseStream.Seek(76, SeekOrigin.Begin);
                        }
                        else if (gGame == Game.AGGRESSIVE_INLINE)
                        {
                            br.BaseStream.Seek(80, SeekOrigin.Begin);
                        }

                        _tempName = Utils.GetStringFromByteArray(br.ReadBytes(32));

                        if (ObjectGeometry.ObjectNameExists(_tempName, gGeometry))
                        {
                            geomObject = ObjectGeometry.GetObjectByName(_tempName, gGeometry);
                            geomObject.m__ref_count++;

                            if (geomObject.m__ref_count > 1)
                                tempObjectInstance.m__name = String.Format("{0}_Instance{1}", geomObject.m__name, geomObject.m__ref_count.ToString("D2"));
                            else
                                tempObjectInstance.m__name = String.Format("{0}", geomObject.m__name);

                            tempObjectInstance.m__geometry_object.Add(geomObject);

                            br.BaseStream.Seek(0, SeekOrigin.Begin);
                            uint Unknown_index = br.ReadUInt32();

                            tempObjectInstance.m__object_orientation = new Vec3(0.0f, 0.0f, 0.0f);
                            //tempObjectInstance.m__object_orientation = new Vec3(br);

                            br.BaseStream.Seek(16, SeekOrigin.Current);
                            br.ReadSingle();

                            tempObjectInstance.m__object_scale[0] = 1.0f;
                            tempObjectInstance.m__object_scale[1] = 1.0f;
                            tempObjectInstance.m__object_scale[2] = 1.0f;

                            tempObjectInstance.m__object_position = new Vec3(br);
                            tempObjectInstance.m__object_bbox_min = new Vec3(br);
                            tempObjectInstance.m__object_bbox_max = new Vec3(br);

                            for (int vi = 0; vi < geomObject.m__num_verts; vi++)
                            {
                                tempObjectInstance.m__verts.Add(new Vec3(geomObject.m__positions[vi][0],geomObject.m__positions[vi][1], geomObject.m__positions[vi][2]));
                            }

                            tempObjectInstance.m__verts = TransformVertexPositions(
                                    tempObjectInstance.m__verts,
                                    tempObjectInstance.m__object_position,
                                    tempObjectInstance.m__object_orientation,
                                    new Vec3(1.0f, 1.0f, -1.0f),
                                    tempObjectInstance.m__object_scale,
                                    26.2468f);
                            
                            gInstances.Add(tempObjectInstance);

                        }
                        //else
                        //{
                        //    Console.WriteLine("{0} no existo!", _tempName);
                        //}
                    }

                }
            }
        }

        //-------------------------------------------------------------
        //	Read .GEO file
        //-------------------------------------------------------------
        void read_geometry(string filename)
        {
            NodeTree.NodeChunk geometryChunks;

            using (BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename.ToLower()))))
            {
                geometryChunks = NodeTree.ReadRoot(br);
                br.Close();
            }

            int numGeometryChunks = geometryChunks.chunks.Count();
            for (int i = 0; i < numGeometryChunks; i++)
            {
                ObjectGeometry _tempGeometry = new ObjectGeometry();

#if IS_EDITOR_SCENE
                ObjectInstance tempObjectInstance = new ObjectInstance();
#endif

                NodeTree.NodeChunk GeometryObject = geometryChunks.chunks[i];

                //-------------------------------------------------------------
                //	Geometry object header 
                //-------------------------------------------------------------
                if (GeometryObject.HasChunk(NodeTree.ChunkType.TGHD))
                {
                    NodeTree.NodeChunk GeometryObjectHeader = GeometryObject.GetChunk(NodeTree.ChunkType.TGHD);

                    using (BinaryReader br = new BinaryReader(new MemoryStream(GeometryObjectHeader.data)))
                    {
                        br.BaseStream.Seek(28, SeekOrigin.Begin);
                        _tempGeometry.m__name = Utils.GetStringFromByteArray(br.ReadBytes(32));
                    }
                }

                //-------------------------------------------------------------
                //	Geometry object info 
                //-------------------------------------------------------------
                if (GeometryObject.HasChunk(NodeTree.ChunkType.TGPT))
                {
                    NodeTree.NodeChunk VertexData = GeometryObject.GetChunk(NodeTree.ChunkType.TGPT);

                    if (VertexData.HasChunk(NodeTree.ChunkType.TGPH))
                    {
                        NodeTree.NodeChunk VertexDataHeader = VertexData.GetChunk(NodeTree.ChunkType.TGPH);
                        using (BinaryReader br = new BinaryReader(new MemoryStream(VertexDataHeader.data)))
                        {
                            Console.WriteLine("VertexDataHeader {0}", _tempGeometry.m__name);
                            //Console.WriteLine("{0}", br.ReadUInt32());
                            //Console.WriteLine("{0}", _tempGeometry.m__num_verts = br.ReadUInt16());
                            //Console.WriteLine("{0}", _tempGeometry.m__num_triangles = br.ReadUInt16());
                            //Console.WriteLine("{0} //  num materials or meshes?", _tempGeometry.m__num_meshes = br.ReadUInt16());
                            //Console.WriteLine("{0}", br.ReadUInt16());
                            //Console.WriteLine("{0}", br.ReadUInt16());
                            //Console.WriteLine("{0}", br.ReadUInt16());
                            //Console.WriteLine("{0}", br.ReadUInt32());
                            br.ReadUInt32();
                            _tempGeometry.m__num_verts = br.ReadUInt16();
                            _tempGeometry.m__num_triangles = br.ReadUInt16();
                            _tempGeometry.m__num_meshes = br.ReadUInt16();

                            if (_tempGeometry.m__num_meshes > 1)
                            {
                                Console.WriteLine("m__num_meshes = {0}", _tempGeometry.m__num_meshes);
                            }

                            br.ReadByte();
                            br.ReadByte();

                            br.ReadByte();
                            br.ReadByte();

                            ushort unknown_ushort = br.ReadUInt16();
                            //Console.WriteLine("unknown_ushort = {0}", unknown_ushort);

                            uint unknown_uint = br.ReadUInt32();
                            //Console.WriteLine("unknown_uint = {0}", unknown_uint);
                        }
                    }

                    //-------------------------------------------------------------
                    //	Vertex Normals
                    //-------------------------------------------------------------
                    if (VertexData.HasChunk(NodeTree.ChunkType.TGVN))
                    {
                        NodeTree.NodeChunk VertexNormalData = VertexData.GetChunk(NodeTree.ChunkType.TGVN);

                        using (BinaryReader br = new BinaryReader(new MemoryStream(VertexNormalData.data)))
                        {
                            for (int vi = 0; vi < VertexNormalData.chunkSize / 12; vi++)
                                _tempGeometry.m__normals.Add(new Vec3(br));
                            br.Close();
                        }
                    }
                    else if (VertexData.HasChunk(NodeTree.ChunkType.TIVN))
                    {
                        NodeTree.NodeChunk VertexNormalContainer = VertexData.GetChunk(NodeTree.ChunkType.TIVN);

                        List<Vec3> tempVertexNormals = new List<Vec3>();

                        if (VertexNormalContainer.HasChunk(NodeTree.ChunkType.TIDA))
                        {
                            NodeTree.NodeChunk VertexNormalData = VertexNormalContainer.GetChunk(NodeTree.ChunkType.TIDA);

                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexNormalData.data)))
                            {
                                for (int vi = 0; vi < VertexNormalData.chunkSize / 12; vi++)
                                    tempVertexNormals.Add(new Vec3(br));
                                br.Close();
                            }
                        }

                        if (VertexNormalContainer.HasChunk(NodeTree.ChunkType.TII8))
                        {
                            NodeTree.NodeChunk VertexNormalIndices = VertexNormalContainer.GetChunk(NodeTree.ChunkType.TII8);
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexNormalIndices.data)))
                            {
                                for (int vi = 0; vi < VertexNormalIndices.chunkSize; vi++)
                                    _tempGeometry.m__normals.Add(tempVertexNormals[br.ReadByte()]);

                                br.Close();
                            }
                        }
                        else if (VertexNormalContainer.HasChunk(NodeTree.ChunkType.TII6))
                        {
                            NodeTree.NodeChunk VertexNormalIndices = VertexNormalContainer.GetChunk(NodeTree.ChunkType.TII6);
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexNormalIndices.data)))
                            {
                                for (int vi = 0; vi < VertexNormalIndices.chunkSize / 2; vi++)
                                    _tempGeometry.m__normals.Add(tempVertexNormals[br.ReadUInt16()]);

                                br.Close();
                            }
                        }
                    }

                    //-------------------------------------------------------------
                    //	Vertex UVs / Texture Coords
                    //-------------------------------------------------------------
                    if (VertexData.HasChunk(NodeTree.ChunkType.TGVU))
                    {
                        NodeTree.NodeChunk VertexTexCoordData = VertexData.GetChunk(NodeTree.ChunkType.TGVU);

                        if (VertexTexCoordData.HasChunk(NodeTree.ChunkType.TIDA))
                            VertexTexCoordData = VertexTexCoordData.GetChunk(NodeTree.ChunkType.TIDA);

                        if (VertexTexCoordData.chunkSize > 8)
                        {
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexTexCoordData.data)))
                            {
                                for (int vi = 0; vi < VertexTexCoordData.chunkSize / 8; vi++)
                                    _tempGeometry.m__texcoords.Add(new Vec2(br));
                                br.Close();
                            }
                        }
                    }
                    else if (VertexData.HasChunk(NodeTree.ChunkType.TIVU))
                    {
                        NodeTree.NodeChunk VertexTexCoordContainer = VertexData.GetChunk(NodeTree.ChunkType.TIVU);

                        List<Vec2> tempTexCoords = new List<Vec2>();

                        if (VertexTexCoordContainer.HasChunk(NodeTree.ChunkType.TIDA))
                        {
                            NodeTree.NodeChunk VertexTexCoordData = VertexTexCoordContainer.GetChunk(NodeTree.ChunkType.TIDA);

                            if (VertexTexCoordData.chunkSize >= 8)
                            {
                                using (BinaryReader br = new BinaryReader(new MemoryStream(VertexTexCoordData.data)))
                                {
                                    for (int vi = 0; vi < VertexTexCoordData.chunkSize / 8; vi++)
                                        tempTexCoords.Add(new Vec2(br));
                                    br.Close();
                                }
                            }
                        }

                        if (VertexTexCoordContainer.HasChunk(NodeTree.ChunkType.TII8))
                        {
                            NodeTree.NodeChunk VertexTexCoordIndices = VertexTexCoordContainer.GetChunk(NodeTree.ChunkType.TII8);
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexTexCoordIndices.data)))
                            {
                                for (int vi = 0; vi < VertexTexCoordIndices.chunkSize; vi++)
                                    _tempGeometry.m__texcoords.Add(tempTexCoords[br.ReadByte()]);

                                br.Close();
                            }
                        }
                        else if (VertexTexCoordContainer.HasChunk(NodeTree.ChunkType.TII6))
                        {
                            NodeTree.NodeChunk VertexTexCoordIndices = VertexTexCoordContainer.GetChunk(NodeTree.ChunkType.TII6);
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexTexCoordIndices.data)))
                            {
                                for (int vi = 0; vi < VertexTexCoordIndices.chunkSize / 2; vi++)
                                    _tempGeometry.m__texcoords.Add(tempTexCoords[br.ReadUInt16()]);

                                br.Close();
                            }
                        }
                    }

                    //-------------------------------------------------------------
                    //	Vertex Colors
                    //-------------------------------------------------------------
                    if (VertexData.HasChunk(NodeTree.ChunkType.TGVC))
                    {
                        NodeTree.NodeChunk VertexColorData = VertexData.GetChunk(NodeTree.ChunkType.TGVC);

                        if (VertexColorData.chunkSize >= 4)
                        {
                            using (BinaryReader br = new BinaryReader(new MemoryStream(VertexColorData.data)))
                            {
                                for (int vi = 0; vi < VertexColorData.chunkSize / 4; vi++)
                                    _tempGeometry.m__colors.Add(br.ReadUInt32());
                                br.Close();
                            }
                        }
                    }
                    else if (VertexData.HasChunk(NodeTree.ChunkType.TIVC))
                    {
                    }

                    if (VertexData.HasChunk(NodeTree.ChunkType.TGVP))
                    {
                        NodeTree.NodeChunk VertexPointData = VertexData.GetChunk(NodeTree.ChunkType.TGVP);
                        using (BinaryReader br = new BinaryReader(new MemoryStream(VertexPointData.data)))
                        {
                            for (int vi = 0; vi < _tempGeometry.m__num_verts; vi++)
                                _tempGeometry.m__positions.Add(new Vec3(br));
                            br.Close();
                        }
                    }

                    //-------------------------------------------------------------
                    //	Collision faces / triangles ??
                    //-------------------------------------------------------------
                    if (VertexData.HasChunk(NodeTree.ChunkType.TGFP))
                    {
                        NodeTree.NodeChunk TriangleData = VertexData.GetChunk(NodeTree.ChunkType.TGFP);
                        using (BinaryReader br = new BinaryReader(new MemoryStream(TriangleData.data)))
                        {
                            // collison faces???
                            for (int vi = 0; vi < _tempGeometry.m__num_triangles; vi++)
                            {
                                Triangle _tempTriangle = new Triangle(br);
                                _tempGeometry.m__triangles.Add(_tempTriangle);
                                
                            }
                            br.Close();
                        }
                    }

                    //-------------------------------------------------------------
                    //	Mesh triangles / strip ??
                    //-------------------------------------------------------------
                    if (VertexData.HasChunk(NodeTree.ChunkType.TGFM))
                    {
                        NodeTree.NodeChunk FaceData = VertexData.GetChunk(NodeTree.ChunkType.TGFM);
                        using (BinaryReader br = new BinaryReader(new MemoryStream(FaceData.data)))
                        {
                            for (int vi = 0; vi < _tempGeometry.m__num_meshes; vi++)
                            {

                                //Console.WriteLine();
                                //Console.WriteLine("mesh {0}", vi);

                                uint tempFaceCount = 0;
                                ObjectGeometryMesh tempSubMesh = new ObjectGeometryMesh();

                                uint numIndices = 0;

                                ushort indexUnknown = br.ReadUInt16();
                                ushort indexCount = br.ReadUInt16();
                                //br.ReadUInt32();

                                uint faceIndicesChunkSize = (((uint)indexCount) * 2) & 0xFFFFFFFC;
                                
                                //Console.WriteLine("indexUnknown is  {0}", indexUnknown);
                                //Console.WriteLine("indexCount is  {0}", indexCount);

                                /*
                                if (indexUnknown == 256 || indexCount == 16)
                                {
                                    //numIndices = (((uint)indexCount & 0xFFFFFFFC) / 2);
                                    numIndices = ((uint)indexCount);
                                }
                                else if (indexUnknown == 0)
                                {
                                    numIndices = ((uint)indexCount);
                                }
                                else
                                {
                                    //numIndices = ((uint)indexCount & 0xFFFFFFFC);
                                    numIndices = (uint)indexCount;
                                }
                                */

                                // duh...
                                numIndices = ((uint)indexCount);

                                //Console.Write("indices[{0}] = [", numIndices);
                                for (int fi = 0; fi < numIndices; fi++)
                                {
                                    ushort current = br.ReadUInt16();

                                    if (fi == 0 || fi == 1)
                                        current -= 32768;

                                    if (current > _tempGeometry.m__num_verts)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("current > m__num_vert ({0} > {1})", current, _tempGeometry.m__num_verts);
                                        Console.WriteLine();
                                        break;
                                    }


                                    tempSubMesh.m__face_indices.Add(current);
                                    tempFaceCount++;
                                    //Console.Write("0x{0}, ", current.ToString("X4"));
                                }

                                Utils.pad(br, 4);

                                //Console.Write(" ] // {0}", tempFaceCount);
                                //Console.WriteLine();

                                tempSubMesh.m__face_count = (uint)tempSubMesh.m__face_indices.Count();
                                _tempGeometry.m__meshes.Add(tempSubMesh);
                            }

                            Console.WriteLine();
                            br.Close();
                        }
                    }
                }

                //-------------------------------------------------------------
                //	Material ids
                //-------------------------------------------------------------
                if (GeometryObject.HasChunk(NodeTree.ChunkType.TMTB))
                {
                    NodeTree.NodeChunk MaterialContainer = GeometryObject.GetChunk(NodeTree.ChunkType.TMTB);
                    if (MaterialContainer.HasChunk(NodeTree.ChunkType.MATI))
                    {
                        List<NodeTree.NodeChunk> materialChunks = MaterialContainer.GetChunks(NodeTree.ChunkType.MATI);

                        for (int mat = 0; mat < materialChunks.Count(); mat++)
                        {
                            ObjectMaterial _tempMaterial = new ObjectMaterial();
                            _tempMaterial.m__name = String.Format("{0}_Material{1}", _tempGeometry.m__name, mat.ToString("D2"));

                            using (BinaryReader br = new BinaryReader(new MemoryStream(materialChunks[mat].data)))
                            {
                                if (gGame == Game.DAVEMIRRA2)
                                    br.BaseStream.Seek(60, SeekOrigin.Begin);
                                else if (gGame == Game.AGGRESSIVE_INLINE)
                                    br.BaseStream.Seek(64, SeekOrigin.Begin);
                                else
                                    br.BaseStream.Seek(60, SeekOrigin.Begin);

                                uint _tempTextureId = br.ReadUInt32();

                                if (Texture.TextureIdExists(_tempTextureId, gTextures))
                                {
                                    _tempMaterial.m__textures.Add(Texture.GetObjectById(_tempTextureId, gTextures));
                                }

                                br.Close();
                            }

                            if (!gMaterials.Contains(_tempMaterial))
                            {
                                gMaterials.Add(_tempMaterial);
                                _tempGeometry.m__materials.Add(gMaterials[gMaterials.Count() - 1]);
                            }
                        }
                    }
                }

                //if (!gGeometry.Contains(_tempGeometry))
                    gGeometry.Add(_tempGeometry);


#if IS_EDITOR_SCENE
                    tempObjectInstance.m__name = _tempGeometry.m__name;

                    for (int vi = 0; vi < _tempGeometry.m__num_verts; vi++)
                    {
                        tempObjectInstance.m__verts.Add(new Vec3(_tempGeometry.m__positions[vi][0], _tempGeometry.m__positions[vi][1], _tempGeometry.m__positions[vi][2]));
                    }

                    tempObjectInstance.m__verts = TransformVertexPositions(
                            tempObjectInstance.m__verts,
                            new Vec3(0.0f, 0.0f, 0.0f),
                            new Vec3(0.0f, 0.0f, 0.0f),
                            new Vec3(1.0f, 1.0f, -1.0f),
                            new Vec3(1.0f, 1.0f, 1.0f),
                            26.2468f);

                    tempObjectInstance.m__geometry_object.Add(_tempGeometry);
                    gInstances.Add(tempObjectInstance);
#endif
            }
            
            Console.WriteLine("numGeometryChunks {0}", numGeometryChunks);

        }

        void write_mtl_file(string filename, List<ObjectMaterial> materials)
        {
            StreamWriter dump = new StreamWriter(filename.ToLower().Replace(".obj", ".mtl"));
            dump.WriteLine(String.Format("# material file '{0}'", Path.GetFileName(filename).ToLower()));
            dump.WriteLine();
            foreach (ObjectMaterial material in materials)
            {
                dump.WriteLine(String.Format("newmtl {0}", material.m__name));
                if (material.m__textures.Count() > 0)
                {
					dump.WriteLine(String.Format("map_Kd {0}.{1}", material.m__textures[0].m__name, material.m__textures[0].m__extenstion));
                }
                dump.WriteLine();
            }
            dump.Close();
        }

        void write_collsion_obj_file(string filename, List<ObjectGeometry> meshes)
        {
            StreamWriter dump = new StreamWriter(filename);

            dump.WriteLine(String.Format("# dump file '{0}_collsion'", Path.GetFileName(filename).ToLower()));
            dump.WriteLine();

            uint _FACE_INDEX_OFFSET = 1;
            //uint _FACE_INDEX_OFFSET_NORMAL = 1;
            //uint _FACE_INDEX_OFFSET_UV = 1;

            foreach (ObjectGeometry _object in meshes)
            {
                if (_object.m__normals.Count() <= 0)
                    continue;
                if (_object.m__texcoords.Count() <= 0)
                    continue;
                if (_object.m__positions.Count() <= 0)
                    continue;
                if (_object.m__name == String.Empty)
                    continue;
                if (_object.m__name.ToLower().Contains("animobj_"))
                    continue;

                dump.WriteLine(String.Format("# object {0}", _object.m__name));
                dump.WriteLine(String.Format("g {0}", _object.m__name));

                dump.WriteLine();
                dump.WriteLine("# verts");
                for (int vi = 0; vi < _object.m__positions.Count(); vi++)
                {
                    string str_vert = string.Format("v {0} {1} {2}",
                        (_object.m__positions[vi][0]).ToString("f6"),
                        (_object.m__positions[vi][1]).ToString("f6"),
                        (_object.m__positions[vi][2]).ToString("f6"));

                    dump.WriteLine(str_vert.Replace(",", "."));
                }

                dump.WriteLine("# tris");
                for (int fi = 0; fi < _object.m__triangles.Count(); fi++)
                {
                    dump.WriteLine("f {0} {1} {2}", _object.m__triangles[fi].a + _FACE_INDEX_OFFSET, _object.m__triangles[fi].b + _FACE_INDEX_OFFSET, _object.m__triangles[fi].c + _FACE_INDEX_OFFSET);
                }
                dump.WriteLine();
                dump.WriteLine();

                _FACE_INDEX_OFFSET += (uint)_object.m__positions.Count();

            }
            dump.Close();
        }

        void write_obj_file(string filename, List<ObjectInstance> instanceObjects/*, List<ObjectGeometry> geomObjects*/)
        {
            StreamWriter dump = new StreamWriter(filename);

            dump.WriteLine(String.Format("# dump file '{0}'", Path.GetFileName(filename).ToLower()));
            dump.WriteLine();

#if OUTPUT_MTL_FILE
            dump.WriteLine(String.Format("mtllib {0}", Path.GetFileName(filename).ToLower()).Replace(".obj", ".mtl"));
            dump.WriteLine();
#endif
            uint _FACE_INDEX_OFFSET = 1;
            uint _FACE_INDEX_OFFSET_NORMAL = 1;
            uint _FACE_INDEX_OFFSET_UV = 1;

            foreach (ObjectInstance instaceObject in instanceObjects)
            {
                ObjectGeometry _object = instaceObject.m__geometry_object[0];

                if (_object.m__texcoords.Count() <= 0)
                    continue;

                if (_object.m__positions.Count() <= 0)
                    continue;

                if (_object.m__name == String.Empty)
                    continue;

                if (_object.m__name.ToLower().Contains("animobj_"))
                    continue;

                dump.WriteLine(String.Format("# object {0}", instaceObject.m__name));
                dump.WriteLine(String.Format("o {0}", instaceObject.m__name));

                dump.WriteLine();
                dump.WriteLine("# verts");
                for (int vi = 0; vi < instaceObject.m__verts.Count(); vi++)
                {
                    string str_vert = string.Format("v {0} {1} {2}",
                        (instaceObject.m__verts[vi][0]).ToString("f6"),
                        (instaceObject.m__verts[vi][1]).ToString("f6"),
                        (instaceObject.m__verts[vi][2]).ToString("f6"));

                    dump.WriteLine(str_vert.Replace(",", "."));
                }

                dump.WriteLine("# normals");
                for (int vi = 0; vi < instaceObject.m__verts.Count(); vi++)
                {
                    string str_norm = string.Format("vn 0.0 0.0 0.0");
                    if (vi < _object.m__normals.Count())
                    {
                        str_norm = string.Format("vn {0} {1} {2}",
                            (_object.m__normals[vi][0] * -1.0f).ToString("f6"),
                            (_object.m__normals[vi][1] * -1.0f).ToString("f6"),
                            _object.m__normals[vi][2].ToString("f6"));
                    }

                    dump.WriteLine(str_norm.Replace(",", "."));
                }

                dump.WriteLine("# uvs");
                for (int vi = 0; vi < instaceObject.m__verts.Count(); vi++)
                {
                    string str_tex = string.Format("vt {0} {1}",
                        _object.m__texcoords[vi][0].ToString("f6"),
                        (_object.m__texcoords[vi][1] * -1.0f).ToString("f6"));

                    dump.WriteLine(str_tex.Replace(",", "."));
                }

                for (int fi = 0; fi < _object.m__meshes.Count(); fi++)
                {
                    //dump.WriteLine(String.Format("g {0}_Mesh_{1}", instaceObject.m__name, fi));
#if OUTPUT_MTL_FILE
                    //dump.WriteLine(String.Format("usemtl {0}", _object.m__materials[0].m__name));
					int materialIndex = fi;

					if (fi > _object.m__materials.Count - 1)
					{
						materialIndex = _object.m__materials.Count - 1;
					}
					dump.WriteLine(String.Format("usemtl {0}", _object.m__materials[materialIndex].m__name));
#endif
                    dump.WriteLine("# tris");

                    ushort v1, v2, v3;
                    int tmp = 1;

                    if (_object.m__meshes[fi].m__face_count == 0)
                        continue;

                    if (_object.m__meshes[fi].m__face_count < 3)
                    {
                        //Console.WriteLine("DAFUQ? (tris < 3 ) {0}[{1}] (count = {2})", _object.m__name, fi, _object.m__meshes[fi].m__face_count);
                        continue;
                    }

                    for (int lp3 = 0; lp3 < _object.m__meshes[fi].m__face_count - 2; lp3++)
                    {
                        
                        if (tmp == 0)
                        {
                            v1 = _object.m__meshes[fi].m__face_indices[lp3];
                            v2 = _object.m__meshes[fi].m__face_indices[lp3 + 2];
                            v3 = _object.m__meshes[fi].m__face_indices[lp3 + 1];
                            tmp = 1;
                        }
                        else
                        {
                            v1 = _object.m__meshes[fi].m__face_indices[lp3];
                            v2 = _object.m__meshes[fi].m__face_indices[lp3 + 1];
                            v3 = _object.m__meshes[fi].m__face_indices[lp3 + 2];
                            tmp = 0;
                        }
                        
                        //v1 = _object.m__meshes[fi].m__face_indices[lp3];
                        //v2 = _object.m__meshes[fi].m__face_indices[lp3 + 1];
                        //v3 = _object.m__meshes[fi].m__face_indices[lp3 + 2];

                        //if (v1 != v2 && v1 != v3 && v2 != v3)
                        {
                            dump.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", v1 + _FACE_INDEX_OFFSET, v2 + _FACE_INDEX_OFFSET, v3 + _FACE_INDEX_OFFSET);
                            //dump.WriteLine("f {0} {1} {2}", v1 + _FACE_INDEX_OFFSET, v2 + _FACE_INDEX_OFFSET, v3 + _FACE_INDEX_OFFSET);
                        }
                    }

                    dump.WriteLine();
                    dump.WriteLine();

                }

                _FACE_INDEX_OFFSET += (uint)_object.m__positions.Count();


                if (_object.m__normals.Count() > 0)
                    _FACE_INDEX_OFFSET_NORMAL += (uint)_object.m__normals.Count();
                else
                    _FACE_INDEX_OFFSET_NORMAL += (uint)_object.m__positions.Count();

                if (_object.m__texcoords.Count() > 0)
                    _FACE_INDEX_OFFSET_UV += (uint)_object.m__texcoords.Count();
                else
                    _FACE_INDEX_OFFSET_UV += (uint)_object.m__positions.Count();
            }
            dump.Close();
            Console.WriteLine("{0} saved!", Path.GetFileName(filename).ToLower());
        }

        void read_textures(string filename)
        {
            NodeTree.NodeChunk textureChunks;

            using (BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename.ToLower()))))
            {
                textureChunks = NodeTree.ReadRoot(br);
                br.Close();
            }

            int numTextureChunks = textureChunks.chunks.Count();
            for (int i = 0; i < numTextureChunks; i++)
            {

                if (!textureChunks.chunks[i].isNull())
                {
                    Texture _tempTexture = new Texture();

                    if (textureChunks.chunks[i].HasChunk(NodeTree.ChunkType.TXRH))
                    {
                        NodeTree.NodeChunk TextureHeader = textureChunks.chunks[i].GetChunk(NodeTree.ChunkType.TXRH);
                        using (BinaryReader br = new BinaryReader(new MemoryStream(TextureHeader.data)))
                        {
                            _tempTexture.m__texture_id = br.ReadUInt32();
                            _tempTexture.m__width = br.ReadUInt16();
                            _tempTexture.m__height = br.ReadUInt16();

                            // 12
                            br.BaseStream.Seek(20, SeekOrigin.Begin);

                            _tempTexture.m__name = (Utils.GetStringFromByteArray(br.ReadBytes(20)));

                            br.Close();
                        }
                    }
                    switch (gGame)
                    {
                        case Game.BMXXX:
                            if (textureChunks.chunks[i].HasChunk(NodeTree.ChunkType.TXPR))
                            {
                                NodeTree.NodeChunk TextureData = textureChunks.chunks[i].GetChunk(NodeTree.ChunkType.TXPR);
                                using (BinaryReader br = new BinaryReader(new MemoryStream(TextureData.data)))
                                {
                                    _tempTexture.__xpr = new XPRTexture(br);
                                    _tempTexture.m__dxt = br.ReadUInt32();
                                    br.BaseStream.Seek(2052, SeekOrigin.Begin);
                                    _tempTexture.m__data = br.ReadBytes((int)TextureData.chunkSize - 2052);
									br.Close();
									_tempTexture.m__extenstion = "xpr";
                                }
                                if (!gTextures.Contains(_tempTexture))
                                    gTextures.Add(_tempTexture);
                            }
                            break;
                        case Game.AGGRESSIVE_INLINE:
                        case Game.DAVEMIRRA2:
                            if (textureChunks.chunks[i].HasChunk(NodeTree.ChunkType.TDDS))
                            {
                                NodeTree.NodeChunk TextureData = textureChunks.chunks[i].GetChunk(NodeTree.ChunkType.TDDS);
                                using (BinaryReader br = new BinaryReader(new MemoryStream(TextureData.data)))
                                {
                                    br.BaseStream.Seek(4, SeekOrigin.Begin);
                                    _tempTexture.m__data = br.ReadBytes((int)TextureData.chunkSize - 4);
									_tempTexture.m__extenstion = "dds";
                                    br.Close();
                                }
                                if (!gTextures.Contains(_tempTexture))
                                    gTextures.Add(_tempTexture);
							}
							else if (textureChunks.chunks[i].HasChunk(NodeTree.ChunkType.TBMP))
							{
								NodeTree.NodeChunk TextureData = textureChunks.chunks[i].GetChunk(NodeTree.ChunkType.TBMP);
								using (BinaryReader br = new BinaryReader(new MemoryStream(TextureData.data)))
								{
									br.BaseStream.Seek(4, SeekOrigin.Begin);
									_tempTexture.m__data = br.ReadBytes((int)TextureData.chunkSize - 4);
									_tempTexture.m__extenstion = "bmp";
									br.Close();
								}
								if (!gTextures.Contains(_tempTexture))
									gTextures.Add(_tempTexture);
							}
							else if (textureChunks.chunks[i].HasChunk(NodeTree.ChunkType.TTGA))
							{
								NodeTree.NodeChunk TextureData = textureChunks.chunks[i].GetChunk(NodeTree.ChunkType.TTGA);
								using (BinaryReader br = new BinaryReader(new MemoryStream(TextureData.data)))
								{
									br.BaseStream.Seek(4, SeekOrigin.Begin);
									_tempTexture.m__data = br.ReadBytes((int)TextureData.chunkSize - 4);
									_tempTexture.m__extenstion = "tga";
									br.Close();
								}
								if (!gTextures.Contains(_tempTexture))
									gTextures.Add(_tempTexture);
							}
                            break;
                    }

                }
            }
            
#if DEBUG_AUTOLOAD_DEFAULT
            if (DebugAutoLoad)
                goto exit; // ugh
#endif
            if (gTextures.Count > 0)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    // dump dds files to folder 
                    Texture.dump_textures(folderBrowserDialog1.SelectedPath, gTextures, gGame);
                }
            }
            
#if DEBUG_AUTOLOAD_DEFAULT
            exit:
#endif
            Console.WriteLine("numTextureChunks {0}", numTextureChunks);
        }

        void read_tree(string filename)
        {

            using (BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(filename.ToLower()))))
            {
                gRoot = NodeTree.ReadRoot(br);
                br.Close();
            }

            Console.WriteLine("gRoot chunk count {0}", gRoot.chunks.Count());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ((ComboBox)sender).SelectedIndex;
            gGame = (Game)index;

        }
    }
}
