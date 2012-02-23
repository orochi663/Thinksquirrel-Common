// Editor Obj Exporter
// EditorObjExporter.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//		 KeliHlodversson <http://www.unifycommunity.com/wiki/index.php?title=ObjExporter>
// 
// Copyright (c) 2011-2012, Thinksquirrel Software, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
// SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// This file is available at https://github.com/Thinksquirrel-Software/Thinksquirrel-Common
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace ThinksquirrelSoftware.Common.Editor
{

	struct ObjMaterial
	{
	    public string name;
	    public string textureName;
	}
	
	public class EditorObjExporter : ScriptableObject
	{
	    private static int vertexOffset = 0;
	    private static int normalOffset = 0;
	    private static int uvOffset = 0;
	    
	    
	    //User should probably be able to change this. It is currently left as an excercise for
	    //the reader.
	    private static string targetFolder = "ExportedObj";
	    
	
	    private static string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList) 
	    {
	        Mesh m = mf.sharedMesh;
	        Material[] mats = mf.renderer.sharedMaterials;
	        
	        StringBuilder sb = new StringBuilder();
	        
	        sb.Append("g ").Append(mf.name).Append("\n");
	        foreach(Vector3 lv in m.vertices) 
	        {
	            Vector3 wv = mf.transform.TransformPoint(lv);
	        
	            //This is sort of ugly - inverting x-component since we're in
	            //a different coordinate system than "everyone" is "used to".
	            sb.Append(string.Format("v {0} {1} {2}\n",-wv.x,wv.y,wv.z));
	        }
	        sb.Append("\n");
	        
	        foreach(Vector3 lv in m.normals) 
	        {
	            Vector3 wv = mf.transform.TransformDirection(lv);
	        
	            sb.Append(string.Format("vn {0} {1} {2}\n",-wv.x,wv.y,wv.z));
	        }
	        sb.Append("\n");
	        
	        foreach(Vector3 v in m.uv) 
	        {
	            sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
	        }
	        
	        for (int material=0; material < m.subMeshCount; material ++) {
	            sb.Append("\n");
	            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
	            sb.Append("usemap ").Append(mats[material].name).Append("\n");
	                
	            //See if this material is already in the materiallist.
	            try
	         {
	              ObjMaterial objMaterial = new ObjMaterial();
	              
	              objMaterial.name = mats[material].name;
	              
	              if (mats[material].mainTexture)
	                objMaterial.textureName = AssetDatabase.GetAssetPath(mats[material].mainTexture);
	              else 
	                objMaterial.textureName = null;
	              
	              materialList.Add(objMaterial.name, objMaterial);
	            }
	            catch (ArgumentException)
	            {
	                //Already in the dictionary
	            }
	
	                
	            int[] triangles = m.GetTriangles(material);
	            for (int i=0;i<triangles.Length;i+=3) 
	            {
	                //Because we inverted the x-component, we also needed to alter the triangle winding.
	                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", 
	                    triangles[i]+1 + vertexOffset, triangles[i+1]+1 + normalOffset, triangles[i+2]+1 + uvOffset));
	            }
	        }
	        
	        vertexOffset += m.vertices.Length;
	        normalOffset += m.normals.Length;
	        uvOffset += m.uv.Length;
	        
	        return sb.ToString();
	    }
	    
	    private static void Clear()
	    {
	        vertexOffset = 0;
	        normalOffset = 0;
	        uvOffset = 0;
	    }
	    
	    private static Dictionary<string, ObjMaterial> PrepareFileWrite()
	    {
	     Clear();
	        
	        return new Dictionary<string, ObjMaterial>();
	    }
	    
	    private static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
	    {
	     using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl")) 
	        {
	            foreach( KeyValuePair<string, ObjMaterial> kvp in materialList )
	            {
	                sw.Write("\n");
	                sw.Write("newmtl {0}\n", kvp.Key);
	                sw.Write("Ka  0.6 0.6 0.6\n");
	                sw.Write("Kd  0.6 0.6 0.6\n");
	                sw.Write("Ks  0.9 0.9 0.9\n");
	                sw.Write("d  1.0\n");
	                sw.Write("Ns  0.0\n");
	                sw.Write("illum 2\n");
	                
	                if (kvp.Value.textureName != null)
	                {
	                    string destinationFile = kvp.Value.textureName;
	                
	                
	                    int stripIndex = destinationFile.LastIndexOf(Path.PathSeparator);
	         
	           if (stripIndex >= 0)
	                        destinationFile = destinationFile.Substring(stripIndex + 1).Trim();
	                    
	                    
	                    string relativeFile = destinationFile;
	                    
	                    destinationFile = folder + "/" + destinationFile;
	                
	                    Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);
	                
	                    try
	                    {
	                        //Copy the source file
	                        File.Copy(kvp.Value.textureName, destinationFile);
	                    }
	                    catch
	                    {
	                    
	                    }   
	                
	                
	                    sw.Write("map_Kd {0}", relativeFile);
	                }
	                    
	                sw.Write("\n\n\n");
	            }
	        }
	    }
	    
	    private static void MeshToFile(MeshFilter mf, string folder, string filename) 
	    {
	        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
	    
	        using (StreamWriter sw = new StreamWriter(folder +"/" + filename + ".obj")) 
	        {
	            sw.Write("mtllib ./" + filename + ".mtl\n");
	        
	            sw.Write(MeshToString(mf, materialList));
	        }
	        
	        MaterialsToFile(materialList, folder, filename);
	    }
	    
	    private static void MeshesToFile(MeshFilter[] mf, string folder, string filename) 
	    {
	        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
	    
	        using (StreamWriter sw = new StreamWriter(folder +"/" + filename + ".obj")) 
	        {
	            sw.Write("mtllib ./" + filename + ".mtl\n");
	        
	            for (int i = 0; i < mf.Length; i++)
	            {
	                sw.Write(MeshToString(mf[i], materialList));
	            }
	        }
	        
	        MaterialsToFile(materialList, folder, filename);
	    }
	    
	    private static bool CreateTargetFolder()
	    {
	        try
	        {
	            System.IO.Directory.CreateDirectory(targetFolder);
	        }
	        catch
	        {
	            EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
	            return false;
	        }
	        
	        return true;
	    }
	    
	    //[MenuItem ("Assets/Export Mesh/Export all MeshFilters in selection to separate OBJs")]
	    static void ExportSelectionToSeparate()
	    {
	        if (!CreateTargetFolder())
	            return;
	    
	        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
	        
	        if (selection.Length == 0)
	        {
	            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
	            return;
	        }
	        
	        int exportedObjects = 0;
	        
	        for (int i = 0; i < selection.Length; i++)
	        {
	         Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));
	         
	         for (int m = 0; m < meshfilter.Length; m++)
	         {
	          exportedObjects++;
	          MeshToFile((MeshFilter)meshfilter[m], targetFolder, selection[i].name + "_" + i + "_" + m);
	         }
	        }
	        
	        if (exportedObjects > 0)
	         EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
	        else
	         EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
	    }
	    
	    //[MenuItem ("Assets/Export Mesh/Export whole selection to single OBJ")]
	    static void ExportWholeSelectionToSingle()
	    {
	        if (!CreateTargetFolder())
	            return;
	            
	            
	        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
	        
	        if (selection.Length == 0)
	        {
	            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
	            return;
	        }
	        
	        int exportedObjects = 0;
	        
	        ArrayList mfList = new ArrayList();
	        
	        for (int i = 0; i < selection.Length; i++)
	        {
	         Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));
	         
	         for (int m = 0; m < meshfilter.Length; m++)
	         {
	          exportedObjects++;
	          mfList.Add(meshfilter[m]);
	         }
	        }
	        
	        if (exportedObjects > 0)
	        {
	         MeshFilter[] mf = new MeshFilter[mfList.Count];
	        
	         for (int i = 0; i < mfList.Count; i++)
	         {
	          mf[i] = (MeshFilter)mfList[i];
	         }
	         
	         string filename = EditorApplication.currentScene + "_" + exportedObjects;
	        
	         int stripIndex = filename.LastIndexOf(Path.PathSeparator);
	         
	         if (stripIndex >= 0)
	                filename = filename.Substring(stripIndex + 1).Trim();
	        
	         MeshesToFile(mf, targetFolder, filename);
	        
	        
	         EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects to " + filename, "");
	        }
	        else
	         EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
	    }
	    
	    
	    
	    //[MenuItem ("Assets/Export Mesh/Export each selected to single OBJ")]
	    static void ExportEachSelectionToSingle()
	    {
	        if (!CreateTargetFolder())
	            return;
	    
	        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
	        
	        if (selection.Length == 0)
	        {
	            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
	            return;
	        }
	        
	        int exportedObjects = 0;
	        
	        
	        for (int i = 0; i < selection.Length; i++)
	        {
	         Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));
	         
	         MeshFilter[] mf = new MeshFilter[meshfilter.Length];
	        
	         for (int m = 0; m < meshfilter.Length; m++)
	         {
	          exportedObjects++;
	          mf[m] = (MeshFilter)meshfilter[m];
	         }
	        
	         MeshesToFile(mf, targetFolder, selection[i].name + "_" + i);
	        }
	        
	        if (exportedObjects > 0)
	        {
	         EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
	        }
	        else
	         EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
	    }
	    
	}
}
