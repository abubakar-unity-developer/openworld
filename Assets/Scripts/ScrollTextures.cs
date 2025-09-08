using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextures : MonoBehaviour
{
    public float scrollSpeed, offset;
    MeshRenderer mesh;
    void Start()
    {
        mesh = this.transform.GetComponent<MeshRenderer>();
    }
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10.0f;
        mesh.material.SetTextureOffset("_MainTex", new Vector3(offset, 0));
    }
}
