using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Godot;

namespace Arphros.Serializer
{
    public static class ObjectImporter
    {
        private struct MeshStruct
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector2[] uv;
            public Vector2[] uv1;
            public Vector2[] uv2;
            public int[] triangles;
            public Vector3[] faceData;
        }

        public static ArrayMesh ImportFile(string filePath)
        {
            string content = File.ReadAllText(filePath);

            MeshStruct newMesh = CreateMeshStruct(filePath, ref content);
            PopulateMeshStruct(ref newMesh, ref content);

            Vector3[] newVerts = new Vector3[newMesh.faceData.Length];
            Vector2[] newUVs = new Vector2[newMesh.faceData.Length];
            Vector3[] newNormals = new Vector3[newMesh.faceData.Length];
            int i = 0;

            foreach (Vector3 v in newMesh.faceData)
            {
                newVerts[i] = newMesh.vertices[(int)v.X - 1];
                if (v.Y >= 1)
                    newUVs[i] = newMesh.uv[(int)v.Y - 1];

                if (v.Z >= 1)
                    newNormals[i] = newMesh.normals[(int)v.Z - 1];
                i++;
            }

            ArrayMesh mesh = new();
            mesh.ClearSurfaces();
            mesh.ClearBlendShapes();

            // Add vertex array to the mesh
            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = newVerts;
            arrays[(int)Mesh.ArrayType.TexUV] = newUVs;
            arrays[(int)Mesh.ArrayType.Normal] = newNormals;
            arrays[(int)Mesh.ArrayType.Index] = newMesh.triangles;

            // Add surface to the mesh
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

            return mesh;
        }

        private static MeshStruct CreateMeshStruct(string filename, ref string content)
        {
            int triangles = 0;
            int vertices = 0;
            int vt = 0;
            int vn = 0;
            int face = 0;

            MeshStruct mesh = new MeshStruct();
            using (StringReader reader = new StringReader(content))
            {
                string currentText = reader.ReadLine();
                char[] splitIdentifier = { ' ' };
                string[] brokenString;
                while (currentText != null)
                {
                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ")
                        && !currentText.StartsWith("vn "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();                           //Trim the current line
                        brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                        switch (brokenString[0])
                        {
                            case "v":
                                vertices++;
                                break;
                            case "vt":
                                vt++;
                                break;
                            case "vn":
                                vn++;
                                break;
                            case "f":
                                face = face + brokenString.Length - 1;
                                triangles = triangles + 3 * (brokenString.Length - 2); /*brokenString.Length is 3 or greater since a face must have at least
                                                                                     3 vertices.  For each additional vertice, there is an additional
                                                                                     triangle in the mesh (hence this formula).*/
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                }
            }
            mesh.triangles = new int[triangles];
            mesh.vertices = new Vector3[vertices];
            mesh.uv = new Vector2[vt];
            mesh.normals = new Vector3[vn];
            mesh.faceData = new Vector3[face];
            return mesh;
        }

        private static void PopulateMeshStruct(ref MeshStruct mesh, ref string content)
        {
            using (StringReader reader = new StringReader(content))
            {
                string currentText = reader.ReadLine();

                string[] brokenString;
                string[] brokenBrokenString;

                int f = 0;
                int f2 = 0;
                int v = 0;
                int vn = 0;
                int vt = 0;
                int vt1 = 0;
                int vt2 = 0;
                while (currentText != null)
                {
                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ") &&
                        !currentText.StartsWith("vn ") && !currentText.StartsWith("g ") && !currentText.StartsWith("usemtl ") &&
                        !currentText.StartsWith("mtllib ") && !currentText.StartsWith("vt1 ") && !currentText.StartsWith("vt2 ") &&
                        !currentText.StartsWith("vc ") && !currentText.StartsWith("usemap "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();
                        brokenString = currentText.Split(splitIdentifier, 50);
                        switch (brokenString[0])
                        {
                            case "g":
                                break;
                            case "usemtl":
                                break;
                            case "usemap":
                                break;
                            case "mtllib":
                                break;
                            case "v":
                                mesh.vertices[v] = GetVector3(brokenString, 1);
                                v++;
                                break;
                            case "vt":
                                mesh.uv[vt] = GetVector2(brokenString, 1);
                                vt++;
                                break;
                            case "vt1":
                                mesh.uv[vt1] = GetVector2(brokenString, 1);
                                vt1++;
                                break;
                            case "vt2":
                                mesh.uv[vt2] = GetVector2(brokenString, 1);
                                vt2++;
                                break;
                            case "vn":
                                mesh.normals[vn] = GetVector3(brokenString, 1);
                                vn++;
                                break;
                            case "vc":
                                break;
                            case "f":

                                int j = 1;
                                List<int> intArray = new List<int>();
                                while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
                                {
                                    Vector3 temp = new Vector3();
                                    brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    //Separate the face into individual components (vert, uv, normal)
                                    temp.X = ToInt32(brokenBrokenString[0]);
                                    if (brokenBrokenString.Length > 1)                                  //Some .obj files skip UV and normal
                                    {
                                        if (brokenBrokenString[1] != "")                                    //Some .obj files skip the uv and not the normal
                                        {
                                            temp.Y = ToInt32(brokenBrokenString[1]);
                                        }
                                        temp.Z = ToInt32(brokenBrokenString[2]);
                                    }
                                    j++;

                                    mesh.faceData[f2] = temp;
                                    intArray.Add(f2);
                                    f2++;
                                }
                                j = 1;
                                while (j + 2 < brokenString.Length)     //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                                {
                                    mesh.triangles[f] = intArray[0];
                                    f++;
                                    mesh.triangles[f] = intArray[j];
                                    f++;
                                    mesh.triangles[f] = intArray[j + 1];
                                    f++;

                                    j++;
                                }
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");       //Some .obj files insert double spaces, this removes them.
                        }
                    }
                }
            }
        }

        static char[] splitIdentifier = { ' ' };
        static char[] splitIdentifier2 = { '/' };

        public static Vector3 GetRoundVector3(string[] split, int offset) =>
            new Vector3(
                ToInt32(split[0 + offset]),
                IsBelowLengthAndNotEmpty(split, 1 + offset) ? ToInt32(split[1 + offset]) : 0,
                IsBelowLengthAndNotEmpty(split, 2 + offset) ? ToInt32(split[2 + offset]) : 0);

        public static Vector3 GetRawVector3(string[] split, int offset) =>
            new Vector3(
                ToSingle(split[0 + offset]),
                ToSingle(split[1 + offset]),
                ToSingle(split[2 + offset]));

        public static Vector3 GetVector3(string[] split, int offset) =>
            new Vector3(
                ToSingle(split[0 + offset]),
                IsBelowLengthAndNotEmpty(split, 1 + offset) ? ToSingle(split[1 + offset]) : 0,
                IsBelowLengthAndNotEmpty(split, 2 + offset) ? ToSingle(split[2 + offset]) : 0);

        public static Vector3 GetVector3(string x, string y, string z) =>
            new Vector3(ToSingle(x), ToSingle(y), ToSingle(z));

        public static Vector2 GetRoundVector2(string[] split, int offset) =>
            new Vector2(
                ToInt32(split[0 + offset]),
                IsBelowLengthAndNotEmpty(split, 1 + offset) ? ToInt32(split[1 + offset]) : 0);

        public static Vector2 GetRoundVector2(string x, string y) =>
            new Vector2(ToInt32(x), ToInt32(y));

        public static Vector2 GetRawVector2(string[] split, int offset) =>
            new Vector2(ToSingle(split[0 + offset]), ToSingle(split[1 + offset]));

        public static Vector2 GetVector2(string[] split, int offset) =>
            new Vector2(
                ToSingle(split[0 + offset]),
                IsBelowLengthAndNotEmpty(split, 1 + offset) ? ToSingle(split[1 + offset]) : 0);

        public static Vector2 GetVector2(string x, string y) =>
            new Vector2(ToSingle(x), ToSingle(y));

        public static bool IsBelowLengthAndNotEmpty(string[] split, int index) =>
            index < split.Length && !string.IsNullOrWhiteSpace(split[index]);

        public static int ToInt32(string str) => Convert.ToInt32(str, CultureInfo.InvariantCulture);
        public static float ToSingle(string str)
        {
            try
            {
                return Convert.ToSingle(str, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to parse: {str}\nError:{e.Message}");
                return float.Parse(str, CultureInfo.InvariantCulture);
            }
        }
    }
}
