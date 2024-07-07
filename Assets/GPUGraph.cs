using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GPUGraph : MonoBehaviour
{   
	[SerializeField]
	ComputeShader computeShader;

    [SerializeField]
    Material material;
    [SerializeField]
    Mesh mesh;

    static readonly int positionsId = Shader.PropertyToID("_Positions"),
		resolutionId = Shader.PropertyToID("_Resolution"),
		stepId = Shader.PropertyToID("_Step"),
		timeId = Shader.PropertyToID("_Time");

	[SerializeField, Range(10, 200)]
	int resolution = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function;

	public enum TransitionMode { Cycle, Random }

	[SerializeField]
	TransitionMode transitionMode = TransitionMode.Cycle;

	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;

	float duration;

	bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;

    void Awake(){
        OnEnable();
    }

    void OnEnable(){
        // 3 floats, so 3 * 4bytes per elt
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3*4);
    }

	void OnDisable () {
		positionsBuffer.Release();
        positionsBuffer = null;
	}

	void UpdateFunctionOnGPU () {
		float step = 2f / resolution;
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(computeShader.FindKernel("FunctionKernel"), positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(computeShader.FindKernel("FunctionKernel"), groups, groups,1);
        material.SetBuffer(positionsId, positionsBuffer);
		material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
	}

	void Update () {
        UpdateFunctionOnGPU();
    }


}
