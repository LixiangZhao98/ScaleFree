using UnityEngine;

public enum FieldType {
    Electric,
    Gravity,
    SimpleAttractor,
    Vortex,
    Airflow,
    CurvatureField
}
public class ParticleFlow : MonoBehaviour
{
    private ParticleGroup pG;
    private DensityField dF;
    public FieldType FieldType;
    public Gradient ParticleColourGradient;
    public float ForceMultiplier = 1.0f;
    public int NumAttractors = 5;
    public GameObject AttractorObj;
    public GameObject cylinder;
    private GameObject[] attractors;
    private float g = 1f;
    private float mass = 2f;
    private ParticleSystem ps;

    // Use this for initialization
    void Start()
    {
        // initAttractors();
        ps = GetComponent<ParticleSystem>();
        pG = this.transform.parent.GetComponentInChildren<DataLoader>().particles;
            ;dF = this.transform.parent.GetComponentInChildren<GPUKDECsHelper>().densityField;
    }

    void Update()
    {
        // add variation to particle colour
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = ParticleColourGradient.Evaluate(Random.Range(0f, 1f));
        
        main.loop = false; // 关闭循环播放，确保粒子不会自动生成
        ps.Stop();
        
        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            EmitParticle(pG.GetParticleWorldPos(i,transform.parent));
        }
    }

    void LateUpdate()
    {
        //put particles of the system into array & update them to gravity algorithm
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);


        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.Particle p = particles[i];
            Vector3 particleWorldPosition;
            if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                particleWorldPosition = transform.TransformPoint(p.position);
            }
            else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            {
                particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
            }
            else
            {
                particleWorldPosition = p.position;
            }

            Vector3 totalForce;

            switch(FieldType)
            {
                case FieldType.SimpleAttractor:
                    totalForce = applySimple(particleWorldPosition);
                    break;
                case FieldType.Gravity:
                    totalForce = applyGravity(particleWorldPosition);
                    break;
                case FieldType.Electric:
                    totalForce = applyElectric(p);
                    break;
                case FieldType.Vortex:
                    totalForce = applyVortex(p);
                    break;
                case FieldType.Airflow:
                    totalForce = applyAirFlow(particleWorldPosition);
                    break;
                case FieldType.CurvatureField:
                    totalForce = applyCurvature(particleWorldPosition);
                    break;
                default:
                    totalForce = applySimple(particleWorldPosition);
                    break;
            }

            if (FieldType == FieldType.Gravity)
            {
                p.velocity += totalForce;   //visualise  acceleration
            }
            else
            {
                p.position+= totalForce;    //visualise velocity
            }

            particles[i] = p;
        }
        ps.SetParticles(particles, particles.Length); //set updated particles into the system
    }

    // potential flow  https://github.com/arkaragian/Fluid-Field/blob/master/field.js
    Vector3 applyAirFlow(Vector3 particleWorldPosition)
    {
        Vector3 direction =  (Vector3.back * 10);
        float distance = float.MaxValue; // used to find closest attractor
        float maxDistance = 20f;
        float fieldStrength = 10f;

        foreach(GameObject a in attractors)
        {
            distance = Vector3.Distance(particleWorldPosition, a.transform.position);
            if (distance < maxDistance)
            {
                float dx = particleWorldPosition.x - a.transform.position.x;
                float dz = particleWorldPosition.z - a.transform.position.z;
                
                float angle = Mathf.Atan2(dz, dx);
                float ux = (fieldStrength / distance) * Mathf.Cos(angle);
                float uz = (fieldStrength / distance) * Mathf.Sin(angle);

                float falloff = ((maxDistance - distance) / distance);
                direction = direction + new Vector3(ux, 0, uz) * falloff ;
            }

        }
        Vector3 totalForce = direction * ForceMultiplier * Time.deltaTime;
        return totalForce;
    }
    Vector3 applySimple(Vector3 particleWorldPosition)
    {
        Vector3 direction = Vector3.zero;
        float distance = float.MaxValue; // used to find closest attractor

        foreach(GameObject a in attractors)
        {
            if (Vector3.Distance(particleWorldPosition, a.transform.position) < distance)
            {
                distance = Vector3.Distance(particleWorldPosition, a.transform.position);
                direction = (a.transform.position - particleWorldPosition).normalized;
            }

        }
        Vector3 totalForce = ((direction) * ForceMultiplier) * Time.deltaTime;
        return totalForce;
    }
    /*
     * algo from: https://gamedevelopment.tutsplus.com/tutorials/adding-turbulence-to-a-particle-system--gamedev-13332
     */
    Vector3 applyVortex(ParticleSystem.Particle p)
    {
        float distanceX = float.MaxValue;
        float distanceY = float.MaxValue;
        float distanceZ = float.MaxValue;
        float distance = float.MaxValue;
    
        Vector3 direction = Vector3.zero;
        foreach(GameObject a in attractors)
        {
            if(Vector3.Distance(p.position,a.transform.localPosition) < distance)
            {
                distanceX = (p.position.x - a.transform.localPosition.x);
                distanceY = (p.position.y - a.transform.localPosition.y);
                distanceZ = (p.position.z - a.transform.localPosition.z);
                distance = Vector3.Distance(p.position, a.transform.localPosition);
            }

            direction += (a.transform.localPosition - p.position).normalized;
        }

        float vortexScale = 10.0f;
        float vortexSpeed = 10.0f;
        float factor = 1 / (1 + (distanceX * distanceX + distanceZ * distanceZ)/ vortexScale);

        float vx = distanceX  * vortexSpeed * factor;
        float vy = distanceY * vortexSpeed * factor;
        float vz = distanceZ * vortexSpeed * factor;

        Vector3 totalForce = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(vx, 0, vz) * ForceMultiplier + (direction);
        return totalForce;
    }
    Vector3 applyGravity(Vector3 particleWorldPosition)
    {
        Vector3 direction = Vector3.zero;
        Vector3 totalForce = Vector3.zero;
        foreach(GameObject a in attractors)
        {
            direction = (a.transform.position - particleWorldPosition).normalized;
            float magnitude = direction.magnitude;
            Mathf.Clamp(magnitude, 5.0f, 10.0f); //eliminate extreme result for very close or very far objects

            float gforce = (g * mass * mass) / direction.magnitude * direction.magnitude;
            totalForce += ((direction) * gforce) * Time.deltaTime;
        }

        totalForce = totalForce * ForceMultiplier;
        return totalForce;
    }

    Vector3 applyElectric(ParticleSystem.Particle p)
    {
        Vector3 totalForce = Vector3.zero;
        Vector3 force = Vector3.zero;
        int i = 0;
        foreach(GameObject a in attractors)
        {
            float dist = Vector3.Distance(p.position, a.transform.position) * 100000;
            float fieldMag = 99999 / dist * dist;
            Mathf.Clamp(fieldMag, 0.0f, 5.0f);

            //alternate postive and negative charges
            if (i % 2 == 0)
            {
                force.x -= fieldMag * (p.position.x - a.transform.position.x) / dist;
                force.y -= fieldMag * (p.position.y - a.transform.position.y) / dist;
                force.z -= fieldMag * (p.position.z - a.transform.position.z) / dist;
            }
            else
            {
                force.x += fieldMag * (p.position.x - a.transform.position.x) / dist;
                force.y += fieldMag * (p.position.y - a.transform.position.y) / dist;
                force.z += fieldMag * (p.position.z - a.transform.position.z) / dist;
            }
            i++;
        }
        totalForce = force * ForceMultiplier;
        return totalForce;
    }
    Vector3 applyCurvature(Vector3 particleWorldPosition)
    {
        float ratio = 0.1f;
        Vector3 totalForce =
            dF.InterpolatePrimaryCurvature(pG.GetObjPosOfVec3(this.transform.parent, particleWorldPosition))
                .normalized * dF.XSTEP * pG.GetXScale() * ratio;
        return totalForce;
    }
    
    void initAttractors()
    {
        attractors = new GameObject[NumAttractors];
        for (int i = 0; i < NumAttractors; i++)
        {
            GameObject newAttractor;
            if(AttractorObj == null)
            {
                newAttractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newAttractor.GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                newAttractor = Instantiate(AttractorObj);
            }
            newAttractor.transform.position = new Vector3(
                Random.Range(-4f,4f), 
                0, 
                Random.Range(-4f,4f));
            attractors[i] = newAttractor;
           // newAttractor.transform.parent = GameObject.Find("OBJ").transform;
        }
    }    
    void EmitParticle(Vector3 position)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = position; // 设置粒子起始位置
        emitParams.startSize = 0.002f; // 设置粒子大小（可选）
        emitParams.startColor = Color.red; // 设置粒子颜色（可选）
        ps.Emit(emitParams, 1); // 只发射一个粒子
    }
}

