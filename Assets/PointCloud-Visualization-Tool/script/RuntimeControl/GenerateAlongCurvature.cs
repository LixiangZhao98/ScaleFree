
using UnityEngine;

public class GenerateAlongCurvature : MonoBehaviour{

    ParticleGroup pG;
    public GameObject cylinder;

    public void Generate()
    {        
        pG = this.transform.parent.GetComponentInChildren<DataLoader>().particles;
        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            Vector4 v=pG.GetParticleWorldPos(i,this.transform.parent);
            GameObject go=Instantiate(cylinder, new Vector3(v.x, v.y, v.z),Quaternion.identity);
            go.transform.up = pG.GetParticleGradient(i).normalized;
            // go.transform.up = pG.GetParticlePrimaryCurvature(i).normalized;
            
            var mat = new Material(Shader.Find("Standard"));
            Vector3 rgb = (go.transform.up * 0.5f + Vector3.one * 0.5f);
            mat.color =new Color(rgb.x, rgb.y, rgb.z, 1f);
            go.GetComponent<Renderer>().material = mat;
            
            
            
            
        }
        
    }
    

}
