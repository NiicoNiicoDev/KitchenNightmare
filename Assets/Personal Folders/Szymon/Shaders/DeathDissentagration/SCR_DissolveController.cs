using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SCR_DissolveController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect vfxGraph;
    private Material[] skinnedMaterials;

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;
    void Start()
    {
        if(skinnedMesh != null)
        {
            skinnedMaterials = skinnedMesh.materials;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Dissolve()
    {
        if (vfxGraph != null)
        {
            vfxGraph.gameObject.SetActive(true);
            vfxGraph.Play();
        }
        if(skinnedMaterials.Length > 0)
        {
            float counter = 0;
            while(skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;

                for(int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (gameObject.CompareTag("Boss"))
        {
            GameManager.gameManager.LevelEnded();
        }
        Destroy(gameObject, 1f);
    }
}
