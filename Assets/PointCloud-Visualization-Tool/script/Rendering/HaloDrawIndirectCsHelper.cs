
    using UnityEngine;
    public class HaloDrawIndirectCsHelper : MonoBehaviour {
        int instanceCount = 100000;
        public Mesh instanceMesh;
        public Material instanceMaterial;
        private int subMeshIndex = 0;
        private Camera cam;
        private ComputeBuffer positionBuffer;
        private ComputeBuffer argsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
        private ParticleGroup pG;
        public void HaloDrawIndirectCsHelperInit()
        {
            cam=Camera.main;
            pG = transform.parent.GetComponentInChildren<DataLoader>().particles;
            instanceMaterial = new Material(Shader.Find("Instanced/Halo"));
            Init(pG.GetParticlenum());
        }
        
       public void Init(int pointcount) {
            instanceCount=pointcount;
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            if (instanceMesh != null)
                subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);
            if (positionBuffer != null)
                positionBuffer.Release();
            positionBuffer = new ComputeBuffer(instanceCount, 16);
            Vector4[] positions = new Vector4[instanceCount];
            for (int i = 0; i < instanceCount; i++) {
                float lp=(float)(pG.GetParticleDensity(i)-pG.MINPARDEN)/(pG.MAXPARDEN-pG.MINPARDEN);
                Vector3 worldPos = pG.GetParticleWorldPos(i, this.transform.parent);
                positions[i] = new Vector4(worldPos.x,worldPos.y,worldPos.z,lp);
            }
            positionBuffer.SetData(positions);
            instanceMaterial.SetBuffer("positionBuffer", positionBuffer);
            
    
            if (instanceMesh != null) {
                args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
                args[1] = (uint)instanceCount;
                args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
                args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
            }
            else
            {
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            argsBuffer.SetData(args);
    
    
        }
    
        void Update() {
    
            Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, new Bounds(Vector3.zero, new Vector3(1000.0f, 1000.0f, 1000.0f)), argsBuffer);
            instanceMaterial.SetMatrix("_LocalToWorld", Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, new Vector3(1f,1f,1f)));
    
        }
    
    
    
       
    
        private void LateUpdate()
        {
            instanceMaterial.SetVector("_CamPos",new Vector4(cam.transform.position.x,cam.transform.position.y,cam.transform.position.z,1f));
        }
    
        void OnDisable() {
            if (positionBuffer != null)
                positionBuffer.Release();
            positionBuffer = null;
    
            if (argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
    }

