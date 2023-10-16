using System;
using System.Collections;
using System.Collections.Generic;
using UniTools;
using UnityEngine;
public class LineFader : MonoBehaviour
{
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private Material cullingMaterial;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private RayCaster rayCaster = new();
    private void Start()
    {
        Material[] lastCullingMaterials = null;
        List<Material> cullingList = new();
        rayCaster.OnRayEnter += hit =>
        {
            var meshRenderer = hit.transform?.GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;
            cullingList.Resize(meshRenderer.materials.Length, cullingMaterial);
            lastCullingMaterials = meshRenderer.materials;
            meshRenderer.materials = cullingList.ToArray();
        };
        rayCaster.OnRayExit += hit =>
        {
            var meshRenderer = hit.transform.GetComponent<MeshRenderer>();
            if (meshRenderer == null || lastCullingMaterials == null) return;
            meshRenderer.materials = lastCullingMaterials;
        };
    }
    private void Update()
    {
        if (rayCaster.ColliderExist(startPoint.position, endPoint.position, cullingMask)) rayCaster.Cast(startPoint.position, endPoint.position, cullingMask);
        else rayCaster.Cast(endPoint.position, startPoint.position, cullingMask);
        // rayCaster.Cast(startPos +, endPos, cullingMask);
    }
}
