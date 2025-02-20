using UnityEngine;

public class Fractal : MonoBehaviour
{

    struct FractalPart {
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    FractalPart[][] parts;

    [SerializeField, Range(1, 8)]
    int depth = 4;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

	static Vector3[] directions = {
		Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
	};

	static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
	};

    FractalPart CreatePart(int levelIndex, int childIndex, float scale) {
        var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
        go.transform.SetParent(transform, false);
        go.transform.localScale = scale * Vector3.one;
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;
        return new FractalPart(){
            direction = directions[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform
        };
    }

    void Awake(){
        parts = new FractalPart[depth][];
        parts[0] = new FractalPart[1];

        for(int i = 0, length = 1;i < parts.Length; i++, length *= 5){
            parts[i] = new FractalPart[length];
        }

        parts[0][0] = CreatePart(0,0,1f);
        float scale = 1f;
        for(int i = 1; i < parts.Length; i++){
            scale *= .5f;
            FractalPart[] levelParts = parts[i];
            for(int j = 0; j < levelParts.Length; j+=5){
                for(int k = 0; k < 5; k++){
                    levelParts[j+k] = CreatePart(i, k, scale);
                }
            }
        }
    }

    void Update () {
		for (int li = 1; li < parts.Length; li++) {
			FractalPart[] parentParts = parts[li - 1];
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				Transform parentTransform = parentParts[fpi / 5].transform;
				FractalPart part = levelParts[fpi];
                part.transform.localRotation = part.rotation * parentTransform.localRotation;
                part.transform.localPosition = parentTransform.localPosition + parentTransform.localRotation * (part.transform.localScale.x * part.direction * 1.5f);
			}
		}
	}

    /* Start is called before the first frame update
    void Start()
    {
        name = "Fractal " + depth;
        if(depth <= 1){
            return;
        }

		Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
		Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0, 0, -90f));
		Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0, 0, 90f));
		Fractal childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0, 0));
		Fractal childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0, 0));
		
		childA.transform.SetParent(transform, false);
		childB.transform.SetParent(transform, false);
		childC.transform.SetParent(transform, false);
		childD.transform.SetParent(transform, false);
		childE.transform.SetParent(transform, false);
    }

    Fractal CreateChild (Vector3 direction, Quaternion rotation) {
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = rotation;
		child.transform.localScale = 0.5f * Vector3.one;
		return child;
	}

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
    }*/
}
