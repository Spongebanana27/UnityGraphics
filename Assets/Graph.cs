using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;

public class Graph : MonoBehaviour
{   
    [SerializeField]
    Transform pointPrefab;

	[SerializeField]
	FunctionLibrary.FunctionName function;

	[SerializeField, Range(10, 100)]
	int resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    Transform[] points;

    float duration;	

    bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;

    void Start()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
		for (int i = 0; i < points.Length; i++) {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
		}
    }

    void Update(){

       	duration += Time.deltaTime;
		if (transitioning) {
			UpdateFunctionTransition();
            if (duration >= transitionDuration) {
				duration -= transitionDuration;
				transitioning = false;
			}
        }
		else if (duration >= functionDuration) {
			duration -= functionDuration;
			transitioning = true;
			transitionFunction = function;
			PickNextFunction();
		}else{
            UpdateFunction();
        }
    }	

    void PickNextFunction () {
		function = GetNextFunctionName(function);
    }
    
    void UpdateFunction()
    {
        Function f = GetFunction(function);
		float step = 2f / resolution;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
			}
            // Step draws them together, -1f brings range to (-1,1)
            // + .5f makes it actually fill -1,1 range
			float u = (x + 0.5f) * step - 1f;
			float v = (z + 0.5f) * step - 1f;
			points[i].localPosition = f(u, v, Time.time);
		}
    }

    void UpdateFunctionTransition () {
		Function
			from = GetFunction(transitionFunction),
			to = GetFunction(function);
		float step = 2f / resolution;
		float progress = duration / transitionDuration;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
			}
            // Step draws them together, -1f brings range to (-1,1)
            // + .5f makes it actually fill -1,1 range
			float u = (x + 0.5f) * step - 1f;
			float v = (z + 0.5f) * step - 1f;
			points[i].localPosition = Morph(u, v, Time.time, from, to, progress);
		}
	}
}
