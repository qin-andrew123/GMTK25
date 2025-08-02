using UnityEngine;

public class CorruptedMaterialSwap : MonoBehaviour
{
    [SerializeField]
    public GameObject Object;
    public Material baseMaterial;
    public Material corruptedMaterial;
    public bool isCorrupted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isCorrupted)
        {
            Object.GetComponent<MeshRenderer>().material = corruptedMaterial;
        }
        else
        {
            Object.GetComponent<MeshRenderer>().material = baseMaterial;
        }
    }

}
