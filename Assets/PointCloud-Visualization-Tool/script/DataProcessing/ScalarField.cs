
    using System.Collections.Generic;
    using UnityEngine;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System;
    using System.IO;
using System.Threading;
using Debug = UnityEngine.Debug;

[System.Serializable]
        public class FieldNode
        {
            [SerializeField]
            private Vector3 nodePosition;
            [SerializeField]
            private double nodeDensity;

            [SerializeField]
            private Vector3 nodeGradient;
            [SerializeField]
            private Vector4 nodePrincipalCurvature;
            [SerializeField]
            private Vector4 nodeSecondaryCurvature;
            [SerializeField]
            private Vector3 nodeGridPos;
            public FieldNode(Vector3 pos,Vector3 gridPos)
            {
                nodePosition = pos;
                nodeGridPos = gridPos;
            }

            public double GetNodeDensity()
            {
                return nodeDensity;
            }
            public Vector3 GetNodePosition()
            {
                return nodePosition;
            }
            public Vector3 GetNodeGradient()
            {
                return nodeGradient;
            }
            public Vector4 GetNodePrimaryCurvature()
            {
                return nodePrincipalCurvature;
            }
            public Vector4 GetNodeSecondaryCurvature()
            {
                return nodeSecondaryCurvature;
            }
            public Vector3 GetNodeGridPos()
            {
                return nodeGridPos;
            }
            public void SetNodeDensity(double density)
            {
                nodeDensity = density;
            }
            public void SetNodeCurvature(Vector4 c1, Vector4 c2)
            {
                nodePrincipalCurvature = c1;
                nodeSecondaryCurvature = c2;
            }
            public void SetNodeGradient(Vector3 g)
            {
                nodeGradient = g;
            }
            public void AddNodeDensity(float d)
            {
                nodeDensity+= d;
            }

        }
        [System.Serializable]
        public class DensityField
        {
            #region  Variables
    
            [SerializeField]
            public string name;
            [SerializeField]
            private List<FieldNode> fieldNode;
            [SerializeField]
            private List<LUT> LUT_;
            [SerializeField]
            private int xNum;  //total nodes number on x axis
            [SerializeField]
            private int yNum;
            [SerializeField]
            private int zNum;
            [SerializeField]
            private float xStep;  //distance between two nodes along x axis
            [SerializeField]
            private float yStep;
            [SerializeField]
            private float zStep;
            private float maxNodeDen;
            private float minNodeDen;
            private float aveNodeDensity;
            public float XSTEP { get { return xStep; } }
            public float YSTEP { get { return yStep; } }
            public float ZSTEP { get { return zStep; } }
            public int XNUM { get { return xNum; } }
            public int YNUM { get { return yNum; } }
            public int ZNUM { get { return zNum; } }
            public float MAXNODEDEN { get { return maxNodeDen; } set { maxNodeDen = value; } }
            public float MINNODEDEN { get { return minNodeDen; } set { minNodeDen = value; } }
            public float AVENODEDENSITY { get { return aveNodeDensity; } set { aveNodeDensity = value; }}
    
    
            #endregion
    
            #region Get
            public Vector3 GetNodeGradient(int i)
            {
                return fieldNode[i].GetNodeGradient();
            }
            public Vector4 GetNodePrimaryCurvature(int i)
            {
                return fieldNode[i].GetNodePrimaryCurvature();
            }
            public Vector4 GetNodeSecondaryCurvature(int i)
            {
                return fieldNode[i].GetNodeSecondaryCurvature();
            }
            public Vector3 GetNodedPos(int i)
            {
                return fieldNode[i].GetNodePosition();
            }
            public Vector3 GetNodeGridPos(int i)
            {
                return fieldNode[i].GetNodeGridPos();
            }
            public double GetNodeDensity(int i)
            {
                return fieldNode[i].GetNodeDensity();
            }

            public int GetNodeNum()
            {
                return fieldNode.Count;
            }
            public int GetLUTUnitNum(int index)
            {
                return LUT_[index].GetLTUnitNum();
            }
            public List<int> GetLUTUnit(int index)
            {
                return LUT_[index].GetLTUnit();
            }
            
            #endregion
    
            #region Set
            public void SetNodeDensity(int i, double density)
            {
                fieldNode[i].SetNodeDensity(density);
            }
            public void SetNodeGradient(int i, Vector3 g)
            {
                fieldNode[i].SetNodeGradient(g);
            }
            public void SetNodeCurvature(int i, Vector4 c1, Vector4 c2)
            {
                fieldNode[i].SetNodeCurvature(c1,c2);
            }
            public void AddNodeDensity(int i,float d)
            {
                fieldNode[i].AddNodeDensity(d);
            }
            #endregion

            #region  KDEGpuSharedGroupMemory
        //     ComputeBuffer slModified;
        //     ComputeBuffer parPos;
        //     ComputeBuffer nodePos;
        //     ComputeBuffer nodeDen;
        //     ComputeBuffer parDen;
        //     Vector3[] parPos_;
        //     Vector3[] parSL_;
        //     Vector3[]  gridPos_;
        //     float [] dens;
        //     int GROUPSIZE=8;
        //     const int PARGROUPSIZE=512;
        //      public void KDEGpuSharedGroupMem(ParticleGroup pG,ComputeShader kde_Cs)
        // {
        //     parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);
        //     nodePos = new ComputeBuffer(GetNodeNum(), 3 * sizeof(float), ComputeBufferType.Default);
        //     nodeDen = new ComputeBuffer(GetNodeNum(), sizeof(float));
        //     parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
        //     slModified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
        //     
        //     int kernel_Pilot = kde_Cs.FindKernel("Pilot");
        //     int kernel_ParDen = kde_Cs.FindKernel("ParDen");
        //     int kernel_SL_Modified = kde_Cs.FindKernel("SL_Modified");
        //     int kernel_FinalDensity = kde_Cs.FindKernel("FinalDensity");
        //
        //     kde_Cs.SetVector("parMinPos_", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
        //     kde_Cs.SetVector("nodeStep_", new Vector4(xStep, yStep, zStep, 0f));
        //     kde_Cs.SetVector("nodeNum_", new Vector4(xNum, yNum, zNum, 0f));
        //     kde_Cs.SetFloat("parCount_",pG.GetParticlenum() );
        //     kde_Cs.SetVector("SL_", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));
        //     
        //     parPos_ = new Vector3[pG.GetParticlenum()];
        //     parSL_ = new Vector3[pG.GetParticlenum()];
        //     gridPos_ = new Vector3[GetNodeNum()]; 
        //     dens = new float[GetNodeNum()];
        //
        //     for (int i = 0; i < pG.GetParticlenum(); i++)
        //     {
        //         parPos_[i] = pG.GetParticleObjectPos(i);
        //     }
        //     for (int i = 0; i < GetNodeNum(); i++)
        //     {
        //         gridPos_[i] = GetNodedPos(i);
        //     }
        //     parPos.SetData(parPos_);
        //     nodePos.SetData(gridPos_);
        //     
        //     kde_Cs.SetBuffer(kernel_Pilot, "partiPos_", parPos);
        //     kde_Cs.SetBuffer(kernel_Pilot, "nodePos_", nodePos);
        //     kde_Cs.SetBuffer(kernel_Pilot, "nodeDen_", nodeDen);
        //     
        //     kde_Cs.SetBuffer(kernel_ParDen, "partiPos_", parPos);
        //     kde_Cs.SetBuffer(kernel_ParDen, "nodeDen_", nodeDen);
        //     kde_Cs.SetBuffer(kernel_ParDen, "parDen_", parDen);
        //
        //     kde_Cs.SetBuffer(kernel_SL_Modified, "partiPos_", parPos);
        //     kde_Cs.SetBuffer(kernel_SL_Modified, "nodeDen_", nodeDen);
        //     kde_Cs.SetBuffer(kernel_SL_Modified, "parDen_", parDen);
        //     kde_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW_", slModified);
        //     
        //     kde_Cs.SetBuffer(kernel_FinalDensity, "partiPos_", parPos);
        //     kde_Cs.SetBuffer(kernel_FinalDensity, "nodePos_", nodePos);
        //     kde_Cs.SetBuffer(kernel_FinalDensity, "nodeDen_", nodeDen);
        //     kde_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW_", slModified);
        //     
        //     System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //     sw.Start();
        //         kde_Cs.Dispatch(kernel_Pilot, XNUM / GROUPSIZE, YNUM / GROUPSIZE, ZNUM / GROUPSIZE);  
        //         kde_Cs.Dispatch(kernel_ParDen, pG.GetParticlenum() / PARGROUPSIZE, 1, 1);
        //         kde_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum()  / PARGROUPSIZE, 1, 1);
        //         kde_Cs.Dispatch(kernel_FinalDensity, XNUM / GROUPSIZE, YNUM / GROUPSIZE, ZNUM / GROUPSIZE);
        //         nodeDen.GetData(dens);
        //         slModified.GetData(parSL_);
        //     sw.Stop();
        //     
        //     UnityEngine.Debug.Log("Kernel density estimation on GPU in " + sw.ElapsedMilliseconds+" ms");
        //     
        //     // //density normalization
        //     //
        //     //  //z-score method
        //     //  float sum = 0f;
        //     //  for (int i = 0; i < dens.Length; i++)
        //     //  {
        //     //      sum += dens[i];
        //     //  }
        //     //  float mean = sum / dens.Length;
        //     //
        //     //  sum = 0;
        //     //  for (int i = 0; i < dens.Length; i++)
        //     //  {
        //     //      sum += Mathf.Pow(dens[i]-mean,2);
        //     //  }
        //     //  float std =Mathf.Sqrt(sum / dens.Length) ;
        //     //  
        //     //  float min=float.MaxValue;float max=float.MinValue;
        //     //  for (int i = 0; i < dens.Length; i++)
        //     //  {
        //     //      dens[i] = (dens[i]-mean)/std;
        //     //      if(dens[i]>max)
        //     //          max=dens[i];
        //     //      if(dens[i]<min)
        //     //          min=dens[i];
        //     //  }
        //     //  
        //     
        //     //        min-max method
        //     // float min=float.MaxValue;float max=float.MinValue;
        //     //   for (int i = 0; i < dens.Length; i++)
        //     //   {
        //     //       if(dens[i]>max)
        //     //           max=dens[i];
        //     //       if(dens[i]<min)
        //     //           min=dens[i];
        //     //   }
        //     // for (int i = 0; i < dens.Length; i++)
        //     // {
        //     //     dens[i] = (dens[i]-min)/ (max-min);
        //     //     // dens[i] *= 255f;UnityEngine.Debug.Log(dens[i]);
        //     // }
        //     
        //     
        //     
        //     
        //     //(1) set node density and (2) calculate the max, min, average node density
        //     float minNodeDen_=float.MaxValue;float maxNodeDen_=float.MinValue;
        //     float sumNodeDen_ = 0f;
        //     for (int i = 0; i < GetNodeNum(); i++)
        //     {
        //         sumNodeDen_ += dens[i];
        //         if(dens[i]>minNodeDen_)
        //             maxNodeDen_=dens[i];
        //         if(dens[i]<minNodeDen_)
        //             maxNodeDen_=dens[i];
        //         SetNodeDensity(i, dens[i]);
        //     }
        //
        //     maxNodeDen = maxNodeDen_;
        //     minNodeDen = minNodeDen_;
        //     aveNodeDensity=sumNodeDen_ / GetNodeNum();
        //     
        //     
        //     //if the node is on the boundary, set density and gradient to 0/Vector.Zero. if not, calculate the gradient
        //     float delta = 0.1f * (xStep + yStep + zStep) / 3;
        //     for (int i = 0; i < GetNodeNum(); i++)
        //     {
        //         if (!NodeOnBoundary(i))
        //         {
        //             Vector3 nodePos = GetNodedPos(i);
        //             float gradx = ((float)Utility.InterpolateVector(new Vector3(nodePos.x + delta, nodePos.y, nodePos.z), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x - delta, nodePos.y, nodePos.z), pG, this)) / (2 * delta);
        //             float grady = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y + delta, nodePos.z), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y - delta, nodePos.z), pG, this)) / (2 * delta);
        //             float gradz = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z + delta), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z - delta), pG, this)) / (2 * delta);
        //
        //             SetNodeGradient(i, new Vector3(gradx, grady, gradz));
        //         }
        //         else
        //         {
        //             SetNodeDensity(i, 0f);
        //             SetNodeGradient(i, Vector3.zero);
        //         }
        //     }
        //
        //     //(1) calculate the particle density, (2) assign smooth length and (3) the max, min and average particle density
        //     float minParDen=float.MaxValue;float maxParDen=float.MinValue;
        //     float sumParDen = 0f;
        //     for (int i=0;i<pG.GetParticlenum(); i++)
        //     {
        //         float density=(float)Utility.InterpolateVector(pG.GetParticleObjectPos(i), pG, this);
        //         sumParDen += density;
        //         pG.SetParticleDensity(i,density);//set particle density
        //         if(density>maxParDen)
        //             maxParDen=density;
        //         if(density<minParDen)
        //             minParDen=density;
        //         pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles
        //
        //         // List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
        //         // pG.SetFlowEnd(i, (v[v.Count - 1]));
        //     };
        //     pG.MAXPARDEN=maxParDen; pG.MINPARDEN=minParDen;
        //     pG.AVEPARDEN=sumParDen / pG.GetParticlenum();
        //     
        //     
        //     // Parallel.For(0, pG.GetParticlenum(), i =>
        //     // {
        //     //     pG.SetParticleDensity(i, Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF));//set particle density
        //     //     List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
        //     //     pG.SetFlowEnd(i, (v[v.Count - 1]));
        //     // });
        //
        //
        //     UpdateLUT(pG);
        //     
        //     slModified.Release();
        //     parPos.Release();
        //     nodePos.Release();
        //     nodeDen.Release();
        //     parDen.Release();
        // }
         #endregion
    
             #region KDE
          ComputeBuffer SL_modified;
          ComputeBuffer parPos;
          ComputeBuffer gridPos;
          ComputeBuffer nodeDen;
          ComputeBuffer parDen;
          ComputeBuffer groupAveDen;
          Vector3[] parPos_;
          Vector3[]  gridPos_;
          Vector3[] parSL_;
          float [] dens;
          float [] parDens;

          public void KDE(ParticleGroup pG, ComputeShader KDE_Cs, bool loadFromLocal_,GPUKDECsHelper.ComputeMode computeMode,int core=1)
          {
              string fieldTexturePath = "Assets/PointCloud-Visualization-Tool/data/field/Texture3D/" + pG.name + "_" + XNUM + "_" + YNUM + "_" + ZNUM + ".asset";
              string fieldBinPath = "Assets/PointCloud-Visualization-Tool/data/field/Bin/" + pG.name + "_" + XNUM + "_" + YNUM + "_" + ZNUM + ".bin";
              if (loadFromLocal_&& (System.IO.File.Exists(fieldTexturePath)||System.IO.File.Exists(fieldBinPath)))
              {
                  float[] fs=new float[0];
                  if(System.IO.File.Exists(fieldTexturePath))
                      fs =LoadField.LoadTexture3D(fieldTexturePath);
                  if(System.IO.File.Exists(fieldBinPath))
                      fs = LoadField.LoadBin(fieldBinPath);

                  for (int i = 0; i < fs.Length; i++)
                  {
                      SetNodeDensity(i, fs[i]); //set node density
                  }
              }
              else
              {
                  if (computeMode==GPUKDECsHelper.ComputeMode.GPU)
                  {
                      // if (XNUM <= 128)
                          KDE_GPU_SmallGrid(pG, KDE_Cs);
                      // else
                          // KDE_GPU_LargeGrid(pG,KDE_Cs,batchSize:128);
                  }
                  else if(computeMode==GPUKDECsHelper.ComputeMode.CPU)
                  {
                      if (core == 1)
                      {
                          KDE_CPU_SingleCore(pG);
                      }
                      else if (core > 1)
                      {
                          KDE_CPU_MultiCore(pG, core);
                      }
                  }
              }

              PostProcess(pG,fieldTexturePath);


          }

          public void KDE_CPU_SingleCore(ParticleGroup pG)
          {
              for (int ii = 0; ii < 3; ii++)
              {
                  
            
              Stopwatch sw = new Stopwatch();
              sw.Start();

              float contribution = 0;
              for (int j = 0; j < pG.GetParticlenum(); j++)
              {
                  contribution = 0;
                  float maxSL=Mathf.Max(pG.GetSmoothLength().x,pG.GetSmoothLength().y,pG.GetSmoothLength().z);
                  Vector3 minPos = new Vector3(
                      Mathf.Clamp(pG.GetParticleObjectPos(j).x - maxSL,pG.XMIN, pG.XMAX),
                      Mathf.Clamp(pG.GetParticleObjectPos(j).y - maxSL,pG.YMIN,pG.YMAX), 
                      Mathf.Clamp(pG.GetParticleObjectPos(j).z - maxSL,pG.ZMIN,pG.ZMAX));
                  Vector3 maxPos = new Vector3(
                      Mathf.Clamp(pG.GetParticleObjectPos(j).x + maxSL,pG.XMIN, pG.XMAX),
                      Mathf.Clamp(pG.GetParticleObjectPos(j).y + maxSL,pG.YMIN,pG.YMAX), 
                      Mathf.Clamp(pG.GetParticleObjectPos(j).z + maxSL,pG.ZMIN,pG.ZMAX));
                  
                 List<int> nodeIndex = new List<int>();
                 for(float x=minPos.x; x<maxPos.x; x+=XSTEP)
                 {for(float y=minPos.y; y<maxPos.y; y+=YSTEP)
                 {for(float z=minPos.z; z<maxPos.z; z+=ZSTEP)
                             nodeIndex.Add(VectorToIndex(new Vector3(x,y,z),pG));}}
                 
                 for (int i = 0; i < nodeIndex.Count; i++)
                  {
                      if(nodeIndex[i]>GetNodeNum()-1||nodeIndex[i]<0) 
                          continue;
                      Vector3 rk = new Vector3((pG.GetParticleObjectPos(j).x - GetNodedPos(nodeIndex[i]).x) / pG.GetSmoothLength().x, (pG.GetParticleObjectPos(j).y - GetNodedPos(nodeIndex[i]).y) / pG.GetSmoothLength().y, (pG.GetParticleObjectPos(j).z- GetNodedPos(nodeIndex[i]).z) / pG.GetSmoothLength().z);
                      contribution= Mathf.Max(0,1 - Vector3.Dot(rk, rk));
                      AddNodeDensity(nodeIndex[i],
                          (float)0.5968310365946 /
                          (pG.GetSmoothLength().x * pG.GetSmoothLength().y * pG.GetSmoothLength().z) * contribution);
                  }
              }
              
              
              for (int j = 0; j < pG.GetParticlenum(); j++)
              { 
                  pG.SetParticleDensity(j,InterpolateDensity(pG.GetParticleObjectPos(j)));
              }
              
              float aveParDen = 0;
              for (int j = 0; j < pG.GetParticlenum(); j++)
              {
                  aveParDen +=pG.GetParticleDensity(j);
              }
              aveParDen = aveParDen / pG.GetParticlenum();
              
              for (int j = 0; j < pG.GetParticlenum(); j++)
              {
                  float interp=pG.GetParticleDensity(j);
                  Vector3 SL_ModifiedRW=new Vector3(Mathf.Min(pG.GetSmoothLength().x * Mathf.Pow(Mathf.Abs(( aveParDen/ interp)), (1.0f / 3.0f)), 5 * xStep),
                                Mathf.Min(pG.GetSmoothLength().y * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)), 5 * yStep),
                                Mathf.Min(pG.GetSmoothLength().z * Mathf.Pow(Mathf.Abs(( aveParDen / interp)), (1.0f / 3.0f)), 5 * zStep));
                  pG.SetMySmoothLength(j,SL_ModifiedRW);
              }


              for (int i = 0; i < GetNodeNum(); i++)
              {
                  SetNodeDensity(i,0f);
              }
              
              for (int j = 0; j < pG.GetParticlenum(); j++)
              {
                  contribution = 0;
                  float maxSL=Mathf.Max(pG.GetMySmoothLength(j).x,pG.GetMySmoothLength(j).y,pG.GetMySmoothLength(j).z);
                  Vector3 minPos = new Vector3(
                      Mathf.Clamp(pG.GetParticleObjectPos(j).x - maxSL,pG.XMIN, pG.XMAX),
                      Mathf.Clamp(pG.GetParticleObjectPos(j).y - maxSL,pG.YMIN,pG.YMAX), 
                      Mathf.Clamp(pG.GetParticleObjectPos(j).z - maxSL,pG.ZMIN,pG.ZMAX));
                  Vector3 maxPos = new Vector3(
                      Mathf.Clamp(pG.GetParticleObjectPos(j).x + maxSL,pG.XMIN, pG.XMAX),
                      Mathf.Clamp(pG.GetParticleObjectPos(j).y + maxSL,pG.YMIN,pG.YMAX), 
                      Mathf.Clamp(pG.GetParticleObjectPos(j).z + maxSL,pG.ZMIN,pG.ZMAX));
                  
                  List<int> nodeIndex = new List<int>();
                  for(float x=minPos.x; x<maxPos.x; x+=XSTEP)
                  {for(float y=minPos.y; y<maxPos.y; y+=YSTEP)
                  {for(float z=minPos.z; z<maxPos.z; z+=ZSTEP)
                      nodeIndex.Add(VectorToIndex(new Vector3(x,y,z),pG));}}
                 
                  for (int i = 0; i < nodeIndex.Count; i++)
                  {
                      if(nodeIndex[i]>GetNodeNum()-1||nodeIndex[i]<0) 
                          continue;
                      Vector3 rk = new Vector3((pG.GetParticleObjectPos(j).x - GetNodedPos(nodeIndex[i]).x) / pG.GetMySmoothLength(j).x, (pG.GetParticleObjectPos(j).y - GetNodedPos(nodeIndex[i]).y) / pG.GetMySmoothLength(j).y, (pG.GetParticleObjectPos(j).z- GetNodedPos(nodeIndex[i]).z) / pG.GetMySmoothLength(j).z);
                      contribution= Mathf.Max(0,1 - Vector3.Dot(rk, rk));
                      AddNodeDensity(nodeIndex[i],
                          (float)0.5968310365946 /
                          (pG.GetSmoothLength().x * pG.GetSmoothLength().y * pG.GetSmoothLength().z) * contribution);
                  }
              }
              sw.Stop();
              UnityEngine.Debug.Log("Density estimation finish in " + sw.ElapsedMilliseconds);
              }
          }
          public void KDE_CPU_MultiCore(ParticleGroup pG, int core)
          {
              for (int ii = 0; ii < 1; ii++)
              {
                  var options = new ParallelOptions()
                  {
                      MaxDegreeOfParallelism = core
                  };
                  Stopwatch sw = new Stopwatch();
                  sw.Start();
                  Parallel.For(0, pG.GetParticlenum(), options, j =>
                  {
              
                      float contribution = 0;
              
                      float maxSL = Mathf.Max(pG.GetSmoothLength().x, pG.GetSmoothLength().y, pG.GetSmoothLength().z);
                      Vector3 minPos = new Vector3(
                          Mathf.Clamp(pG.GetParticleObjectPos(j).x - maxSL, pG.XMIN, pG.XMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).y - maxSL, pG.YMIN, pG.YMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).z - maxSL, pG.ZMIN, pG.ZMAX));
                      Vector3 maxPos = new Vector3(
                          Mathf.Clamp(pG.GetParticleObjectPos(j).x + maxSL, pG.XMIN, pG.XMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).y + maxSL, pG.YMIN, pG.YMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).z + maxSL, pG.ZMIN, pG.ZMAX));
              
                      List<int> nodeIndex = new List<int>();
                      for (float x = minPos.x; x < maxPos.x; x += XSTEP)
                      {
                          for (float y = minPos.y; y < maxPos.y; y += YSTEP)
                          {
                              for (float z = minPos.z; z < maxPos.z; z += ZSTEP)
                                  nodeIndex.Add(VectorToIndex(new Vector3(x, y, z), pG));
                          }
                      }
              
                      for (int i = 0; i < nodeIndex.Count; i++)
                      {
                          if (nodeIndex[i] > GetNodeNum() - 1 || nodeIndex[i] < 0)
                              continue;
                          Vector3 rk = new Vector3(
                              (pG.GetParticleObjectPos(j).x - GetNodedPos(nodeIndex[i]).x) / pG.GetSmoothLength().x,
                              (pG.GetParticleObjectPos(j).y - GetNodedPos(nodeIndex[i]).y) / pG.GetSmoothLength().y,
                              (pG.GetParticleObjectPos(j).z - GetNodedPos(nodeIndex[i]).z) / pG.GetSmoothLength().z);
                          contribution = Mathf.Max(0, 1 - Vector3.Dot(rk, rk));
                          AddNodeDensity(nodeIndex[i],
                              (float)0.5968310365946 /
                              (pG.GetSmoothLength().x * pG.GetSmoothLength().y * pG.GetSmoothLength().z) *
                              contribution);
                      }
                  });
              
              
                  Parallel.For(0, pG.GetParticlenum(), options,
                      j => { pG.SetParticleDensity(j, InterpolateDensity(pG.GetParticleObjectPos(j))); });
              
                  float aveParDen = 0;
                  for (int j = 0; j < pG.GetParticlenum(); j++)
                  {
                      aveParDen += pG.GetParticleDensity(j);
                  }
              
                  aveParDen = aveParDen / pG.GetParticlenum();
              
                  Parallel.For(0, pG.GetParticlenum(), options, j =>
                  {
                      float interp = pG.GetParticleDensity(j);
                      Vector3 SL_ModifiedRW = new Vector3(
                          Mathf.Min(pG.GetSmoothLength().x * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
                              5 * xStep),
                          Mathf.Min(pG.GetSmoothLength().y * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
                              5 * yStep),
                          Mathf.Min(pG.GetSmoothLength().z * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
                              5 * zStep));
                      pG.SetMySmoothLength(j, SL_ModifiedRW);
                  });
              
              
                  for (int i = 0; i < GetNodeNum(); i++)
                  {
                      SetNodeDensity(i, 0f);
                  }
              
                  Parallel.For(0, pG.GetParticlenum(), options, j =>
                  {
                      float contribution = 0;
                      float maxSL = Mathf.Max(pG.GetMySmoothLength(j).x, pG.GetMySmoothLength(j).y,
                          pG.GetMySmoothLength(j).z);
                      Vector3 minPos = new Vector3(
                          Mathf.Clamp(pG.GetParticleObjectPos(j).x - maxSL, pG.XMIN, pG.XMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).y - maxSL, pG.YMIN, pG.YMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).z - maxSL, pG.ZMIN, pG.ZMAX));
                      Vector3 maxPos = new Vector3(
                          Mathf.Clamp(pG.GetParticleObjectPos(j).x + maxSL, pG.XMIN, pG.XMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).y + maxSL, pG.YMIN, pG.YMAX),
                          Mathf.Clamp(pG.GetParticleObjectPos(j).z + maxSL, pG.ZMIN, pG.ZMAX));
              
                      List<int> nodeIndex = new List<int>();
                      for (float x = minPos.x; x < maxPos.x; x += XSTEP)
                      {
                          for (float y = minPos.y; y < maxPos.y; y += YSTEP)
                          {
                              for (float z = minPos.z; z < maxPos.z; z += ZSTEP)
                                  nodeIndex.Add(VectorToIndex(new Vector3(x, y, z), pG));
                          }
                      }
              
                      for (int i = 0; i < nodeIndex.Count; i++)
                      {
                          if (nodeIndex[i] > GetNodeNum() - 1 || nodeIndex[i] < 0)
                              continue;
                          Vector3 rk = new Vector3(
                              (pG.GetParticleObjectPos(j).x - GetNodedPos(nodeIndex[i]).x) / pG.GetMySmoothLength(j).x,
                              (pG.GetParticleObjectPos(j).y - GetNodedPos(nodeIndex[i]).y) / pG.GetMySmoothLength(j).y,
                              (pG.GetParticleObjectPos(j).z - GetNodedPos(nodeIndex[i]).z) / pG.GetMySmoothLength(j).z);
                          contribution = Mathf.Max(0, 1 - Vector3.Dot(rk, rk));
                          AddNodeDensity(nodeIndex[i],
                              (float)0.5968310365946 /
                              (pG.GetSmoothLength().x * pG.GetSmoothLength().y * pG.GetSmoothLength().z) *
                              contribution);
                      }
                  });
                  sw.Stop();
                  UnityEngine.Debug.Log("Density estimation finish in " + sw.ElapsedMilliseconds);
              }
              
              
              
              
              
              
              // for (int ii = 0; ii < 1; ii++)
              // {
              //     var options = new ParallelOptions()
              //     {
              //         MaxDegreeOfParallelism = core
              //     };
              //     Stopwatch sw = new Stopwatch();
              //     sw.Start();
              //     Parallel.For(0, GetNodeNum(), options, i =>
              //     {
              //         float sum = 0f;
              //         for (int j = 0; j < pG.GetParticlenum(); j++)
              //         {
              //             Vector3 rk = new Vector3(
              //                 (pG.GetParticleObjectPos(j).x - GetNodedPos(i).x) / pG.GetSmoothLength().x,
              //                 (pG.GetParticleObjectPos(j).y - GetNodedPos(i).y) / pG.GetSmoothLength().y,
              //                 (pG.GetParticleObjectPos(j).z - GetNodedPos(i).z) / pG.GetSmoothLength().z);
              //             sum+= Mathf.Max(0, 1 - Vector3.Dot(rk, rk));
              //
              //         }
              //         SetNodeDensity(i,
              //             (float)0.5968310365946 /
              //             (pG.GetSmoothLength().x * pG.GetSmoothLength().y * pG.GetSmoothLength().z) *
              //             sum);
              //     });
              //
              //
              //     Parallel.For(0, pG.GetParticlenum(), options,
              //         j => { pG.SetParticleDensity(j, InterpolateDensity(pG.GetParticleObjectPos(j))); });
              //
              //     float aveParDen = 0;
              //     for (int j = 0; j < pG.GetParticlenum(); j++)
              //     {
              //         aveParDen += pG.GetParticleDensity(j);
              //     }
              //
              //     aveParDen = aveParDen / pG.GetParticlenum();
              //
              //     Parallel.For(0, pG.GetParticlenum(), options, j =>
              //     {
              //         float interp = pG.GetParticleDensity(j);
              //         Vector3 SL_ModifiedRW = new Vector3(
              //             Mathf.Min(pG.GetSmoothLength().x * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
              //                 5 * xStep),
              //             Mathf.Min(pG.GetSmoothLength().y * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
              //                 5 * yStep),
              //             Mathf.Min(pG.GetSmoothLength().z * Mathf.Pow(Mathf.Abs((aveParDen / interp)), (1.0f / 3.0f)),
              //                 5 * zStep));
              //         pG.SetMySmoothLength(j, SL_ModifiedRW);
              //     });
              //
              //
              //
              //
              //     Parallel.For(0, GetNodeNum(), options, i =>
              //     {
              //         float sum = 0f;
              //         for (int j = 0; j < pG.GetParticlenum(); j++)
              //         {
              //             Vector3 rk = new Vector3(
              //                 (pG.GetParticleObjectPos(j).x - GetNodedPos(i).x) / pG.GetSmoothLength().x,
              //                 (pG.GetParticleObjectPos(j).y - GetNodedPos(i).y) / pG.GetSmoothLength().y,
              //                 (pG.GetParticleObjectPos(j).z - GetNodedPos(i).z) / pG.GetSmoothLength().z);
              //             sum+= Mathf.Max(0, 1 - Vector3.Dot(rk, rk))/(pG.GetMySmoothLength(i).x * pG.GetMySmoothLength(i).y * pG.GetMySmoothLength(i).z);
              //         }
              //         SetNodeDensity(i,
              //             (float)0.5968310365946 *sum);
              //     });
              //     sw.Stop();
              //     UnityEngine.Debug.Log("Density estimation finish in " + sw.ElapsedMilliseconds);
              // }
          }

          public void KDE_GPU_SmallGrid(ParticleGroup pG,ComputeShader KDE_Cs)
            {
        
            parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);
            gridPos = new ComputeBuffer(GetNodeNum(), 3 * sizeof(float), ComputeBufferType.Default);
            nodeDen = new ComputeBuffer(GetNodeNum(), sizeof(float));
            parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
            SL_modified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
            groupAveDen = new ComputeBuffer(pG.GetParticlenum()/1024, sizeof(float));

            int kernel_Pilot = KDE_Cs.FindKernel("Pilot");
            int kernel_SL_Modified = KDE_Cs.FindKernel("SL_Modified");
            int kernel_FinalDensity = KDE_Cs.FindKernel("FinalDensity");
            KDE_Cs.SetVector("parMinPos", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
            KDE_Cs.SetVector("gridStep", new Vector4(XSTEP, YSTEP, ZSTEP, 0f));
            KDE_Cs.SetVector("gridNum", new Vector4(XNUM, YNUM, ZNUM, 0f));
            KDE_Cs.SetFloat("parCount", pG.GetParticlenum());
            KDE_Cs.SetVector("SL", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));

            parPos_ = new Vector3[pG.GetParticlenum()];
            gridPos_ = new Vector3[GetNodeNum()];
            parSL_ = new Vector3[pG.GetParticlenum()];
            dens = new float[GetNodeNum()];
            parDens = new float[pG.GetParticlenum()];

            for (int i = 0; i < pG.GetParticlenum(); i++)
            {
                parPos_[i] = pG.GetParticleObjectPos(i);
            }
            for (int i = 0; i < GetNodeNum(); i++)
            {
                gridPos_[i] = GetNodedPos(i);
            }
            KDE_Cs.SetBuffer(kernel_Pilot, "partiPos", parPos);
            KDE_Cs.SetBuffer(kernel_Pilot, "gridPos", gridPos);
            KDE_Cs.SetBuffer(kernel_Pilot, "Den", nodeDen);

            KDE_Cs.SetBuffer(kernel_SL_Modified, "partiPos", parPos);
            KDE_Cs.SetBuffer(kernel_SL_Modified, "gridPos", gridPos);
            KDE_Cs.SetBuffer(kernel_SL_Modified, "parDen", parDen);
            KDE_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW", SL_modified);
            KDE_Cs.SetBuffer(kernel_SL_Modified, "Den", nodeDen);
            KDE_Cs.SetBuffer(kernel_SL_Modified, "groupAveDen", groupAveDen);

            KDE_Cs.SetBuffer(kernel_FinalDensity, "partiPos", parPos);
            KDE_Cs.SetBuffer(kernel_FinalDensity, "gridPos", gridPos);
            KDE_Cs.SetBuffer(kernel_FinalDensity, "Den", nodeDen);
            KDE_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW", SL_modified);

            parPos.SetData(parPos_);
            gridPos.SetData(gridPos_);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1; i++)
            {
                
            
            //-------------------------------------------------------------------------------------------------kernel 0
            KDE_Cs.Dispatch(kernel_Pilot, XNUM / 8, YNUM / 8, ZNUM / 8);  //pilot density
            //-------------------------------------------------------------------------------------------------kernel 1
            KDE_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum() / 1024, 1, 1);//new SL
            //-------------------------------------------------------------------------------------------------kernel 2
            KDE_Cs.Dispatch(kernel_FinalDensity, XNUM / 8, YNUM / 8, ZNUM / 8);  // Final density
            //-------------------------------------------------------------------------------------------------end
            } 

            nodeDen.GetData(dens);
            sw.Stop();
            UnityEngine.Debug.Log("Density estimation+ transfer from GPU to CPU in total of " + sw.ElapsedMilliseconds+"ms");
            // SL_modified.GetData(parSL_);


            for (int i = 0; i < GetNodeNum(); i++)
            {
                SetNodeDensity(i, dens[i]); //set node density
            }
            Debug.Log(GetNodeDensity(100000));
            SL_modified.Release();
            parPos.Release();
            gridPos.Release();
            nodeDen.Release();
            parDen.Release();

            }


    public void KDE_GPU_LargeGrid(ParticleGroup pG, ComputeShader KDE_Cs, int batchSize)//batchSize 64/128
    {
        
                int xBatchNum = XNUM / batchSize; int yBatchNum = YNUM / batchSize; int zBatchNum = ZNUM / batchSize;

                parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);


                int kernel_Pilot = KDE_Cs.FindKernel("Pilot");
                int kernel_SL_Modified = KDE_Cs.FindKernel("SL_Modified");
                int kernel_FinalDensity = KDE_Cs.FindKernel("FinalDensity");

                KDE_Cs.SetVector("parMinPos", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
                KDE_Cs.SetVector("gridStep", new Vector4(XSTEP, YSTEP, ZSTEP, 0f));
                KDE_Cs.SetVector("gridNum", new Vector4(batchSize, batchSize, batchSize, 0f));
                KDE_Cs.SetFloat("parCount", pG.GetParticlenum());
                KDE_Cs.SetVector("SL", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));

                parPos_ = new Vector3[pG.GetParticlenum()];



                for (int i = 0; i < pG.GetParticlenum(); i++)
                {
                    parPos_[i] = pG.GetParticleObjectPos(i);
                }
                parPos.SetData(parPos_);

                KDE_Cs.SetBuffer(kernel_Pilot, "partiPos", parPos);
                KDE_Cs.SetBuffer(kernel_SL_Modified, "partiPos", parPos);
                KDE_Cs.SetBuffer(kernel_FinalDensity, "partiPos", parPos);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int groupZ = 0; groupZ < zBatchNum; groupZ++)
                {
                    for (int groupY = 0; groupY < yBatchNum; groupY++)
                    {
                        for (int groupX = 0; groupX < xBatchNum; groupX++)
                        {
                            parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
                            SL_modified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
                            gridPos = new ComputeBuffer(batchSize * batchSize * batchSize, 3 * sizeof(float), ComputeBufferType.Default);
                            nodeDen = new ComputeBuffer(batchSize * batchSize * batchSize, sizeof(float));
                            KDE_Cs.SetBuffer(kernel_Pilot, "gridPos", gridPos);
                            KDE_Cs.SetBuffer(kernel_Pilot, "Den", nodeDen);

                            KDE_Cs.SetBuffer(kernel_SL_Modified, "gridPos", gridPos);
                            KDE_Cs.SetBuffer(kernel_SL_Modified, "parDen", parDen);
                            KDE_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW", SL_modified);
                            KDE_Cs.SetBuffer(kernel_SL_Modified, "Den", nodeDen);

                            KDE_Cs.SetBuffer(kernel_FinalDensity, "gridPos", gridPos);
                            KDE_Cs.SetBuffer(kernel_FinalDensity, "Den", nodeDen);
                            KDE_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW", SL_modified);

                            gridPos_ = new Vector3[batchSize * batchSize * batchSize];
                            parSL_ = new Vector3[pG.GetParticlenum()];
                            dens = new float[batchSize * batchSize * batchSize];
                            parDens = new float[pG.GetParticlenum()];




                            int idx = 0;
                            for (int zInGroup = 0; zInGroup < batchSize; zInGroup++)
                            {
                                for (int yInGroup = 0; yInGroup < batchSize; yInGroup++)
                                {
                                    for (int xInGroup = 0; xInGroup < batchSize; xInGroup++, idx++)
                                    {
                                        gridPos_[idx] = GetNodedPos(Grid3DIndexToIndex(groupZ * batchSize + zInGroup, groupY * batchSize + yInGroup, groupX * batchSize + xInGroup));
                                    }
                                }
                            }
                            gridPos.SetData(gridPos_);
                            Thread.Sleep(50);
                            //-------------------------------------------------------------------------------------------------kernel 0
                            KDE_Cs.Dispatch(kernel_Pilot, XNUM / 8, YNUM / 8, ZNUM / 8);  //pilot density
                            Thread.Sleep(50);                                                              //-------------------------------------------------------------------------------------------------kernel 1
                            KDE_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum() / 1024, 1, 1);//new SL
                            Thread.Sleep(50);                                                                      //-------------------------------------------------------------------------------------------------kernel 2
                            KDE_Cs.Dispatch(kernel_FinalDensity, XNUM / 8, YNUM / 8, ZNUM / 8);  // Final density
                            Thread.Sleep(50);                                                                     //-------------------------------------------------------------------------------------------------end

                            nodeDen.GetData(dens);

                            idx = 0;
                            for (int zInGroup = 0; zInGroup < batchSize; zInGroup++)
                            {
                                for (int yInGroup = 0; yInGroup < batchSize; yInGroup++)
                                {
                                    for (int xInGroup = 0; xInGroup < batchSize; xInGroup++, idx++)
                                    {
                                        SetNodeDensity(Grid3DIndexToIndex(groupZ * batchSize + zInGroup, groupY * batchSize + yInGroup, groupX * batchSize + xInGroup), dens[idx]);
                                    }
                                }
                            }

                            SL_modified.Release();
                            gridPos.Release();
                            nodeDen.Release();
                            parDen.Release();

                            Thread.Sleep(2000);
                        }
                    }
                
                parPos.Release();

                sw.Stop();
                UnityEngine.Debug.Log("Density estimation+ transfer from GPU to CPU in total of " + sw.ElapsedMilliseconds+"ms");
                

        }
        
    }

    public void PostProcess(ParticleGroup pG,string fieldTexturePath)
    {
        Parallel.For(0, GetNodeNum(), i =>
        {
            if (NodeOnBoundary(i))
            {
                SetNodeDensity(i, 0f);

            }

        });
    
        float minDen=float.MaxValue;float maxDen=float.MinValue;
        float sum = 0f;
        for (int i=0;i<pG.GetParticlenum(); i++)
        {
            float density=(float)InterpolateDensity(pG.GetParticleObjectPos(i));
            sum += density;
            pG.SetParticleDensity(i,density);//set particle density
            if(density>maxDen)
                maxDen=density;
            if(density<minDen)
                minDen=density;
            // pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles
    
            // List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
            // pG.SetFlowEnd(i, (v[v.Count - 1]));
        };
    
        pG.MAXPARDEN=maxDen; pG.MINPARDEN=minDen;
        AVENODEDENSITY=(sum / pG.GetParticlenum());
             
        // Parallel.For(0, pG.GetParticlenum(), i =>
        // {
        //     pG.SetParticleDensity(i, Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF));//set particle density
        //     pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles
    
        //     List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
        //     pG.SetFlowEnd(i, (v[v.Count - 1]));
        // });
    
    
        UpdateLUT(pG);
        
        
        if(!System.IO.File.Exists(fieldTexturePath))
            SaveAsTexture3D(pG.name);            

        // if(!System.IO.File.Exists(fieldBinPath)) 
        //     SaveAsBin(pG.name);   
            
        //SaveAsRaw(pG.name);
    }
        public float InterpolateDensity(UnityEngine.Vector3 pos)
        {
            double x = pos.x, y = pos.y, z = pos.z;
            if (x < GetNodedPos(0).x || y < GetNodedPos(0).y || z < GetNodedPos(0).z)
                return 0;
        
            double x_scaled = (x - GetNodedPos(0).x) / XSTEP;
            int xbin = (int)x_scaled;
            double xratio = x_scaled - xbin;
        
            double y_scaled = (y - GetNodedPos(0).y) / YSTEP;
            int ybin = (int)y_scaled;
            double yratio = y_scaled - ybin;
        
            double z_scaled = (z - GetNodedPos(0).z) / ZSTEP;
            int zbin = (int)z_scaled;
            double zratio = z_scaled - zbin;
        
            double dens1, dens2, dens3, dens4;
        
            if (zbin >= ZNUM - 1 || ybin >= YNUM - 1 || xbin >= XNUM - 1)
                return 0f;
            else
            {
                dens1 = GetNodeDensity(Grid3DIndexToIndex(zbin, ybin, xbin)) + (GetNodeDensity(Grid3DIndexToIndex(zbin, ybin, xbin + 1)) - GetNodeDensity(Grid3DIndexToIndex(zbin, ybin, xbin))) * xratio;
                dens2 = GetNodeDensity(Grid3DIndexToIndex(zbin, ybin + 1, xbin)) + (GetNodeDensity(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)) - GetNodeDensity(Grid3DIndexToIndex(zbin, ybin + 1, xbin))) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
        
                dens1 = GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin, xbin)) + (GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)) - GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin, xbin))) * xratio;
                dens2 = GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)) + (GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)) - GetNodeDensity(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin))) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
        
                return (float)(dens3 + (dens4 - dens3) * zratio) ;
            }
        }
    #endregion
    #region Gradient_Curvature

    public void CalGradient(ParticleGroup pG)
    {
        float delta = (XSTEP + YSTEP + ZSTEP) / 3;
        Parallel.For(0, GetNodeNum(), i =>
        {
            if (!NodeOnBoundary(i))
            {
                Vector3 nodePos = GetNodedPos(i);
                float gradx = ((float)InterpolateDensity(new Vector3(nodePos.x + delta, nodePos.y, nodePos.z)) - (float)InterpolateDensity(new Vector3(nodePos.x - delta, nodePos.y, nodePos.z))) / (2 * delta);
                float grady = ((float)InterpolateDensity(new Vector3(nodePos.x, nodePos.y + delta, nodePos.z)) - (float)InterpolateDensity(new Vector3(nodePos.x, nodePos.y - delta, nodePos.z))) / (2 * delta);
                float gradz = ((float)InterpolateDensity(new Vector3(nodePos.x, nodePos.y, nodePos.z + delta)) - (float)InterpolateDensity(new Vector3(nodePos.x, nodePos.y, nodePos.z - delta))) / (2 * delta);
                SetNodeGradient(i, new Vector3(gradx, grady, gradz));
            }
            else{ SetNodeGradient(i, Vector3.zero);}
        });
        UnityEngine.Debug.Log(name+"'s gradient calculation finishes.");
        
        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            pG.SetParticleGradient(i,InterpolateGradient(pG.GetParticleObjectPos(i)));
        }
        
          // SaveGradientAsTexture3D(pG.name);
    }

    public void CalCurvature(ParticleGroup pG)
    {
        float _CurvatureScale =1f;
        Parallel.For(0, GetNodeNum(), i =>
        {
            if (!NodeOnBoundary(i))
            {
                Vector3 normal = GetNodeGradient(i).normalized;
                Vector3 u1 = Vector3.Cross(normal, new Vector3(1,0,0));
                Vector3 u2 = Vector3.Cross(normal, new Vector3(0,1,0));
                Vector3 u = Vector3.Dot(u1, u1) > 0.1 ? u1 : u2;  // Safeguard in case normal is close to (1,0,0)
                Vector3 v = Vector3.Cross(normal, u).normalized;

                // Compute the 2x2 hessian along the tangent plane
                float e = XSTEP * _CurvatureScale;
                Vector3 p = GetNodedPos(i);
                Vector3 gu_p = InterpolateGradient(p+e*u);
                Vector3 gu_n = InterpolateGradient(p-e*u);
                Vector3 gv_p = InterpolateGradient(p+e*v);
                Vector3 gv_n = InterpolateGradient(p-e*v);
    
                Vector3 hu = (gu_p - gu_n)/(e);
                Vector3 hv = (gv_p - gv_n)/(e);
    
                float fxx = Vector3.Dot(hu, u);
                float fyy = Vector3.Dot(hv, v);
                float fxy = (Vector3.Dot(hu, v)+Vector3.Dot(hv, u))/2;
    
                // Derive eigenvectors and eigenvalues by diagonalizing
                float D = Mathf.Sqrt((fxx - fyy)*(fxx - fyy)+4*fxy*fxy);
    
                float l1 = (fxx + fyy + D)/2;
                float l2 = (fxx + fyy - D)/2;
    
                Vector3 ev1 = (2*fxy)*u + (fyy - fxx + D)*v;
                Vector3 ev2 = (2*fxy)*u + (fyy - fxx - D)*v;

                // Save first principal direction and curvature
                Vector4 principalCurvature = new Vector4(ev1.x,ev1.y,ev1.z, Mathf.Abs(l1));
                Vector4 secondaryCurvature = new Vector4(ev2.x,ev2.y,ev2.z, Mathf.Abs(l2));
                SetNodeCurvature(i, principalCurvature,secondaryCurvature);
            }
        });
        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            pG.SetParticlePrimaryCurvature(i,InterpolatePrimaryCurvature(pG.GetParticleObjectPos(i)));
            pG.SetParticleSecondaryCurvature(i,InterpolateSecondaryCurvature(pG.GetParticleObjectPos(i)));
        }

        UnityEngine.Debug.Log(name+"'s curvature calculation finishes.");

        
    }

    public void CalNodeTangent(ParticleGroup pG, Vector3 referenceDirection)
    {
        Texture3D texture3D = new Texture3D(XNUM,YNUM, ZNUM, TextureFormat.RGBA32, false);
        texture3D.wrapMode = TextureWrapMode.Clamp;
        Color [] colors_tan = new Color[XNUM * XNUM * ZNUM];
        for (int i = 0; i < colors_tan.Length; i++) colors_tan[i] = Color.black;
        
        
        var idx = 0;
        for (var z = 0; z < ZNUM; z++)
        {
            for (var y = 0; y < YNUM; y++)
            {
                for (var x = 0; x < XNUM; x++, idx++)
                {
                    // if (NodeOnBoundary(idx))
                    //     continue;

                    Vector3 normal = GetNodeGradient(idx).normalized;

                    Vector3 crossDir = Vector3.Cross(normal, referenceDirection);

                    float sqrMag = crossDir.sqrMagnitude;
                    if (sqrMag < 1e-8f)
                    {
                        // 在此处可自定义一个回退策略，比如：
                        // 1. 直接投影参考方向到切平面，
                        // 2. 或选一条与 normal 正交的固定向量。
                        Vector3 fallback = referenceDirection - Vector3.Dot(normal, referenceDirection) * normal;
                        crossDir = fallback.sqrMagnitude > 1e-8f ? fallback : Vector3.right;
                    }

                    crossDir.Normalize();

                    
                    colors_tan[idx] = new Color(crossDir.x,crossDir.y,crossDir.z);
                }
            }
        }

        texture3D.SetPixels(colors_tan);
        texture3D.Apply();
            
        string directoryPath = "Assets/PointCloud-Visualization-Tool/data/field/Texture3D/";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    
        SaveField.SaveAsTexture3D(directoryPath + pG.name + "_tangent_" + XNUM + "_" + YNUM + "_" + ZNUM + ".asset", texture3D);

    }
             public Vector3 InterpolateGradient(UnityEngine.Vector3 v) //for gradient interpolation
        {
            double x_ = v.x, y_ = v.y, z_ = v.z;
            if (x_ < GetNodedPos(0).x || y_ < GetNodedPos(0).y || z_ < GetNodedPos(0).z)
                return Vector3.zero;
    
            double x_scaled = (x_ - GetNodedPos(0).x) / XSTEP;
            int xbin = (int)x_scaled;
            double xratio = x_scaled - xbin;
    
            double y_scaled = (y_ - GetNodedPos(0).y) / YSTEP;
            int ybin = (int)y_scaled;
            double yratio = y_scaled - ybin;
    
            double z_scaled = (z_ - GetNodedPos(0).z) / ZSTEP;
            int zbin = (int)z_scaled;
            double zratio = z_scaled - zbin;
            double dens1, dens2, dens3, dens4;
            if (zbin >= ZNUM - 1 || ybin >= YNUM - 1 || xbin >= XNUM - 1 || zbin <= 0 || xbin <= 0 || ybin <= 0)
            { return Vector3.zero; }
            else
            {
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).x + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).x - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).x) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).x - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).x - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).x - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double x = dens3 + (dens4 - dens3) * zratio;
                //--------------------------------------------------------
    
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).y + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).y - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).y) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).y - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).y - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).y - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double y = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
    
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).z + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).z - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin, xbin)).z) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z + (GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).z - GetNodeGradient(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).z - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z) * xratio;
                dens2 = GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z + (GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).z - GetNodeGradient(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double z = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
                return new Vector3((float)x, (float)y, (float)z);
            }
        }
        
            public Vector3 InterpolatePrimaryCurvature(UnityEngine.Vector3 v) //for gradient interpolation
        {
            double x_ = v.x, y_ = v.y, z_ = v.z;
            if (x_ < GetNodedPos(0).x || y_ < GetNodedPos(0).y || z_ < GetNodedPos(0).z)
                return Vector3.zero;
    
            double x_scaled = (x_ - GetNodedPos(0).x) / XSTEP;
            int xbin = (int)x_scaled;
            double xratio = x_scaled - xbin;
    
            double y_scaled = (y_ - GetNodedPos(0).y) / YSTEP;
            int ybin = (int)y_scaled;
            double yratio = y_scaled - ybin;
    
            double z_scaled = (z_ - GetNodedPos(0).z) / ZSTEP;
            int zbin = (int)z_scaled;
            double zratio = z_scaled - zbin;
            double dens1, dens2, dens3, dens4;
            if (zbin >= ZNUM - 1 || ybin >= YNUM - 1 || xbin >= XNUM - 1 || zbin <= 0 || xbin <= 0 || ybin <= 0)
            { return Vector3.zero; }
            else
            {
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).x + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).x - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).x) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).x - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).x - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).x - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double x = dens3 + (dens4 - dens3) * zratio;
                //--------------------------------------------------------
    
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).y + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).y - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).y) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).y - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).y - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).y - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double y = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
    
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).z + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).z - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).z) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).z - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).z - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z) * xratio;
                dens2 = GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z + (GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).z - GetNodePrimaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double z = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
                return new Vector3((float)x, (float)y, (float)z);
            }
        }
            
            public Vector3 InterpolateSecondaryCurvature(UnityEngine.Vector3 v) //for gradient interpolation
        {
            double x_ = v.x, y_ = v.y, z_ = v.z;
            if (x_ < GetNodedPos(0).x || y_ < GetNodedPos(0).y || z_ < GetNodedPos(0).z)
                return Vector3.zero;
    
            double x_scaled = (x_ - GetNodedPos(0).x) / XSTEP;
            int xbin = (int)x_scaled;
            double xratio = x_scaled - xbin;
    
            double y_scaled = (y_ - GetNodedPos(0).y) / YSTEP;
            int ybin = (int)y_scaled;
            double yratio = y_scaled - ybin;
    
            double z_scaled = (z_ - GetNodedPos(0).z) / ZSTEP;
            int zbin = (int)z_scaled;
            double zratio = z_scaled - zbin;
            double dens1, dens2, dens3, dens4;
            if (zbin >= ZNUM - 1 || ybin >= YNUM - 1 || xbin >= XNUM - 1 || zbin <= 0 || xbin <= 0 || ybin <= 0)
            { return Vector3.zero; }
            else
            {
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).x + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).x - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).x) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).x - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).x) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).x - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).x) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).x - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).x) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double x = dens3 + (dens4 - dens3) * zratio;
                //--------------------------------------------------------
    
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).y + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).y - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).y) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).y - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).y) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).y - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).y) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).y - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).y) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double y = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
    
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).z + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin + 1)).z - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin, xbin)).z) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin + 1)).z - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin, ybin + 1, xbin)).z) * xratio;
                dens3 = dens1 + (dens2 - dens1) * yratio;
    
                dens1 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin + 1)).z - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin, xbin)).z) * xratio;
                dens2 = GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z + (GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin + 1)).z - GetNodeSecondaryCurvature(Grid3DIndexToIndex(zbin + 1, ybin + 1, xbin)).z) * xratio;
                dens4 = dens1 + (dens2 - dens1) * yratio;
                double z = dens3 + (dens4 - dens3) * zratio;
                //----------------------------------------------------------
                return new Vector3((float)x, (float)y, (float)z);
            }
        }
    #endregion




    public  Vector3 IndexToVector(int index)  
            {
                int z = index / (xNum * yNum);
                int left = index - z * xNum * yNum;
                int y = left / xNum;
                int x = left - y * xNum;
                return new Vector3(x, y, z);
            }
            public  int VectorToIndex(Vector3 v, ParticleGroup pG)  //return the box index by inputting a random pos  //can judge the outside pointing,
            {
                int a = (int)((v.x - pG.XMIN) / XSTEP) + (int)((v.y - pG.YMIN) / YSTEP) * XNUM + (int)((v.z - pG.ZMIN) / ZSTEP) * XNUM * YNUM;
                if (a >= GetNodeNum() || a <= 0 || v.x > pG.XMAX || v.x < pG.XMIN || v.y > pG.YMAX || v.y < pG.YMIN || v.z > pG.ZMAX || v.z < pG.ZMIN)
                { return -1; }
                else
                    return a;
            }
    
            public void CreateField(ParticleGroup pG, int xAxisNum, int yAxisNum, int zAxisNum)
            {
                float scalingFactor = 0f;
                float xBias = (pG.XMAX - pG.XMIN) / xAxisNum;
                float yBias = (pG.XMAX - pG.XMIN) / yAxisNum;
                float zBias = (pG.XMAX - pG.XMIN) / zAxisNum;
                this.InitializeField(pG.name, pG.XMIN-scalingFactor*xBias, pG.XMAX+scalingFactor*xBias, xAxisNum, pG.YMIN-scalingFactor*yBias, pG.YMAX+scalingFactor*yBias, yAxisNum, pG.ZMIN-scalingFactor*zBias, pG.ZMAX+scalingFactor*zBias, zAxisNum);
            }
    
            public void InitializeField(string pgName,float xmin, float xmax, int xAxisNum, float ymin, float ymax, int yAxisNum, float zmin, float zmax, int zAxisNum)
            {   
                name = pgName+"_DF";
                fieldNode = new List<FieldNode>();
    
                xStep = (xmax - xmin) / (xAxisNum-1);
                yStep = (ymax - ymin) / (yAxisNum-1);
                zStep = (zmax - zmin) / (zAxisNum-1);
    
                for (int k = 0; k < zAxisNum; k ++)
                {
                    for (int j = 0; j < yAxisNum; j ++)
                    {
                        for (int i = 0; i <xAxisNum; i ++)
                        {
                            FieldNode fd = new FieldNode(new Vector3(xmin+i*xStep,ymin+j*yStep,zmin+k*zStep), new Vector3(i, j, k));
                          
                            fieldNode.Add(fd);
                        }
                    }
                }
    
    
                xNum = xAxisNum;
                yNum = yAxisNum;
                zNum = zAxisNum;
                UnityEngine.Debug.Log("Create density field "+name+" success. xNodeNum: "+xNum+" yNodeNum: "+yNum+" zNodeNum: "+zNum+ " xStep: "+xStep+" yStep: "+yStep+" zStep: "+zStep);
            }
            public int Grid3DIndexToIndex(int z, int y, int x)
            {
                return (z) * xNum * yNum + (y) * xNum + x;
    
            }
    
            public void LUTInit()
            {
                LUT_ = new List<LUT>();
                for (int i = 0; i < xNum*yNum*zNum; i++)
                    LUT_.Add(new LUT());
            }
    
            private void AddToLUT(int index, int targetint)
            {
                LUT_[index].AddToLUT(targetint);
            }
            
            public void UpdateLUT(ParticleGroup pG)
            {
                LUTInit();
    
                for (int i = 0; i < pG.GetParticlenum(); i++)
                {
                    int x = Mathf.FloorToInt((pG.GetParticleObjectPos(i).x - pG.XMIN) / xStep);
                    int y = Mathf.FloorToInt((pG.GetParticleObjectPos(i).y - pG.YMIN) / yStep);
                    int z = Mathf.FloorToInt((pG.GetParticleObjectPos(i).z - pG.ZMIN) / zStep);
                    int index = z * xNum * yNum + y * xNum + x;
                    AddToLUT(index, i);
                }
    
            }
            
            public bool NodeOnBoundary(int index)
            {
                float _xNum, _yNum, _zNum = 0;
                _xNum = GetNodeGridPos(index).x;
                _yNum = GetNodeGridPos(index).y;
                _zNum = GetNodeGridPos(index).z;
                if (_xNum == 0 || _xNum == xNum - 1 || _xNum == xNum || _yNum == 0 || _yNum == yNum - 1 || _yNum == yNum || _zNum == 0 || _zNum == zNum - 1 || _zNum == zNum)
                { return true; }
                else
                {
                    return false;
                }
            }
            
  

    #region save
    public void SaveAsBin(string datasetName)
            {
                float[] fs = new float[GetNodeNum()];
                for (int i = 0; i < GetNodeNum(); i++)
                {
                    fs[i] = (float)GetNodeDensity(i);
                }
                SaveField.FloatsToBytes(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/field/Bin/"+datasetName+"_"+XNUM+"_"+YNUM+"_"+ZNUM+".bin",fs);
                
            }

            public void SaveAsTexture3D(string datasetName)
            { 
            if (this.GetNodeNum()==0)
            {
                UnityEngine.Debug.LogError("Density is not estimated.");
                return;
            }
            Texture3D texture3D = new Texture3D(XNUM,YNUM, ZNUM, TextureFormat.RFloat, false);
            texture3D.wrapMode = TextureWrapMode.Clamp;
            Color [] colors_den = new Color[XNUM * XNUM * ZNUM];
            for (int i = 0; i < colors_den.Length; i++) colors_den[i] = Color.black;
    
            var idx = 0;
            for (var z = 0; z < ZNUM; z++)
            {
                for (var y = 0; y < YNUM; y++)
                {
                    for (var x = 0; x < XNUM; x++, idx++)
                    {
                        colors_den[idx].r = (float)GetNodeDensity(idx);
                    }
                }
            }
            texture3D.SetPixels(colors_den);
            texture3D.Apply();
            
            string directoryPath = "Assets/PointCloud-Visualization-Tool/data/field/Texture3D/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
    
        SaveField.SaveAsTexture3D(directoryPath + datasetName + "_" + XNUM + "_" + YNUM + "_" + ZNUM + ".asset", texture3D);
            }
            
            public void SaveGradientAsTexture3D(string datasetName)
            { 
                if (this.GetNodeNum()==0)
                {
                    UnityEngine.Debug.LogError("Density is not estimated.");
                    return;
                }
                Texture3D texture3D = new Texture3D(XNUM,YNUM, ZNUM, TextureFormat.RGBA32, false);
                texture3D.wrapMode = TextureWrapMode.Clamp;
                Color [] colors_gra = new Color[XNUM * XNUM * ZNUM];
                for (int i = 0; i < colors_gra.Length; i++) colors_gra[i] = Color.black;
    
                var idx = 0;
                for (var z = 0; z < ZNUM; z++)
                {
                    for (var y = 0; y < YNUM; y++)
                    {
                        for (var x = 0; x < XNUM; x++, idx++)
                        {
                            colors_gra[idx] = new Color(GetNodeGradient(idx).x,GetNodeGradient(idx).y,GetNodeGradient(idx).z);
                        }
                    }
                }
                texture3D.SetPixels(colors_gra);
                texture3D.Apply();
            
                string directoryPath = "Assets/PointCloud-Visualization-Tool/data/field/Texture3D/";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
    
                SaveField.SaveAsTexture3D(directoryPath + datasetName + "_gradient_" + XNUM + "_" + YNUM + "_" + ZNUM + ".asset", texture3D);
            }
            
            
            public void SaveAsRaw(string datasetName)
            { 
            if (this.GetNodeNum()==0)
            {
                UnityEngine.Debug.LogError("Density is not estimated.");
                return;
            }
            
            string directoryPath = "Assets/PointCloud-Visualization-Tool/data/field/Raw/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

        UInt32[] uis = new UInt32[GetNodeNum()];
        for (int i = 0; i < GetNodeNum(); i++)
        {
            uis[i] = (UInt32)GetNodeDensity(i);
        }
        SaveField.SaveAsRaw(directoryPath + datasetName + "_" + XNUM + "_" + YNUM + "_" + ZNUM + ".bin", uis);
            }

    #endregion
    

}


    

