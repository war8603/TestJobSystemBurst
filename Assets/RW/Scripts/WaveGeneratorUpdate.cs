using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WaveGeneratorUpdate : MonoBehaviour
{
    [Header("Wave Parameters")]
    public float waveScale;
    public float waveOffsetSpeed;
    public float waveHieght;

    [Header("References and Prefabs")]
    public MeshFilter waterMeshFilter;
    private Mesh waterMesh;

    private List<Vector3> waterVertices;
    private List<Vector3> waterNormals;

    private List<Vector3> baseWaterVertices;

    bool _isStart = false;
    void Start()
    {
        waterMesh = waterMeshFilter.mesh;

        //https://docs.unity3d.com/kr/current/ScriptReference/Mesh.MarkDynamic.html
        //빈번한 업데이트를 위해 메시를 최적화합니다.

        //메시를 지속적으로 업데이트할 때 더 나은 성능을 얻기 위해 꼭짓점을 할당하기 전에 이것을 호출합니다.
        //내부적으로 이렇게 하면 메시가 기본 그래픽스 API에서 "동적 버퍼"를 사용하게 됩니다.메시 데이터가 자주 변경되는 경우 더 효율적입니다.
        waterMesh.MarkDynamic();

        waterVertices = new List<Vector3>(waterMesh.vertices);
        waterNormals = new List<Vector3>(waterMesh.normals);

        baseWaterVertices = new List<Vector3>(waterMesh.vertices);
    }

    private void Init()
    {
        for(int i = 0; i < waterVertices.Count; i++)
        {
            waterVertices[i] = baseWaterVertices[i];
        }
        waterMesh.SetVertices(waterVertices);
        waterMesh.RecalculateNormals();
    }

    void Update()
    {
        if (_isStart == false)
        {
            return;
        }

        MakeNoise();

        //새 꼭짓점 위치 배열을 할당합니다.
        waterMesh.SetVertices(waterVertices);

        //삼각형과 꼭짓점에서 메시의 법선을 다시 계산
        waterMesh.RecalculateNormals();
    }

    private void MakeNoise()
    {
        for(int i = 0; i < waterVertices.Count; i++)
        {
            if (waterNormals[i].z <= 0f)
                continue;

            var vertex = waterVertices[i];

            var x = vertex.x * waveScale + waveOffsetSpeed * Time.time;
            var y = vertex.y * waveScale + waveOffsetSpeed * Time.time;
            float2 pos = math.float2(x, y);
            float noiceValue = noise.snoise(pos);
            
            waterVertices[i] = new Vector3(vertex.x, vertex.y, noiceValue * waveHieght + 0.3f);
        }
    }

    public void OnStart(bool isStart)
    {
        _isStart = isStart;
        if (_isStart == false)
            Init();
    }
}
