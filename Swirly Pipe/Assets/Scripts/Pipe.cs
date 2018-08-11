using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private float _pipeRadius;    
    [SerializeField] private int _pipeSegmentCount;

    [SerializeField] private float _ringDistance;

    [SerializeField] private float _minCurveRadius, _maxCurveRadius;
    [SerializeField] private int _minCurveSegmentCount, _maxCurveSegmentCount;

    private float _relativeRotation;

    public float RelativeRotation {
        get {
            return _relativeRotation;
        }
    }
    
    public float CurveRadius
    {
        get { return _curveRadius; }
    }
    
    public float CurveAngle 
    {
        get { return _curveAngle; }
    }

    private float _curveRadius;
    private int _curveSegmentCount;
    
    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;    
    private float _curveAngle;
    private Vector2[] _uv;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Pipe";
     
    }

    public void Generate()
    {
        _curveRadius = Random.Range(_minCurveRadius, _maxCurveRadius);
        _curveSegmentCount = Random.Range(_minCurveSegmentCount, _maxCurveSegmentCount + 1);
        _mesh.Clear();
        SetVertices();
        SetUV();
        SetTriangles();
        _mesh.RecalculateNormals();
    }

    public void AlignWith (Pipe pipe) 
    {
        _relativeRotation = Random.Range(0, _curveSegmentCount) * 360f / _pipeSegmentCount;

        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe._curveAngle);
        transform.Translate(0f, pipe._curveRadius, 0f);
        transform.Rotate(_relativeRotation, 0f, 0f);
        transform.Translate(0f, -_curveRadius, 0f);

        transform.SetParent(pipe.transform.parent);
        transform.localScale = Vector3.one;
    }
    
    private void SetUV()
    {
        _uv = new Vector2[_vertices.Length];

        for (int i = 0; i < _vertices.Length; i+= 4) 
        {
            _uv[i] = Vector2.zero;
            _uv[i + 1] = Vector2.right;
            _uv[i + 2] = Vector2.up;
            _uv[i + 3] = Vector2.one;
        }
        
        _mesh.uv = _uv;    
    }
   
    private void SetTriangles()
    {
        _triangles = new int[_pipeSegmentCount * _curveSegmentCount * 6];
        
        for (int t = 0, i = 0; t < _triangles.Length; t += 6, i += 4) 
        {
            _triangles[t] = i;
            _triangles[t + 1] = _triangles[t + 4] = i + 2;
            _triangles[t + 2] = _triangles[t + 3] = i + 1;
            _triangles[t + 5] = i + 3;
        }
        
        _mesh.triangles = _triangles;
    }

    private void SetVertices()
    {
        _vertices = new Vector3[_pipeSegmentCount * _curveSegmentCount * 4];
        var uStep = _ringDistance / _curveRadius;
        _curveAngle = uStep * _curveSegmentCount * (360f / (2f * Mathf.PI));

        CreateFirstQuadRing(uStep);
        int iDelta = _pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= _curveSegmentCount; u++, i += iDelta) {
            CreateQuadRing(u * uStep, i);
        }
        _mesh.vertices = _vertices;
    }

    private void CreateFirstQuadRing(float u)
    {
        var vStep = (2f * Mathf.PI) / _pipeSegmentCount;

        var vertexA = GetPointOnTorus(0f, 0f);
        var vertexB = GetPointOnTorus(u, 0f);

        for (int v = 1, i = 0; v <= _pipeSegmentCount; v++, i += 4)
        {
            _vertices[i] = vertexA;
            _vertices[i+1] = vertexA = GetPointOnTorus(0f, v * vStep);
            _vertices[i + 2] = vertexB;
            _vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }
    
    private void CreateQuadRing (float u, int i) {
        float vStep = (2f * Mathf.PI) / _pipeSegmentCount;
        int ringOffset = _pipeSegmentCount * 4;
		
        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= _pipeSegmentCount; v++, i += 4)
        {
            _vertices[i] = _vertices[i - ringOffset + 2];
            _vertices[i + 1] = _vertices[i - ringOffset + 3];
            _vertices[i + 2] = vertex;
            _vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p;
        var r = (_curveRadius + _pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = _pipeRadius * Mathf.Sin(v);

        return p;
    }

//    private void OnDrawGizmos()
//    {
//        var uStep = (2f * Mathf.PI) / _curveSegmentCount; 
//        var vStep = (2f * Mathf.PI) / _pipeSegmentCount;
//
//        for (var u = 0; u < _curveSegmentCount; u++)
//        {
//            for (var v = 0; v < _pipeSegmentCount; v++)
//            {
//                var point = GetPointOnTorus(u * uStep, v * vStep);
//                Gizmos.color = new Color(
//                    1f,
//                    (float)v / _pipeSegmentCount,
//                    (float)u / _curveSegmentCount);
//                Gizmos.DrawSphere(point, 0.1f);
//            }
//        }
//    }
}
