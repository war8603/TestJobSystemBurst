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
        //����� ������Ʈ�� ���� �޽ø� ����ȭ�մϴ�.

        //�޽ø� ���������� ������Ʈ�� �� �� ���� ������ ��� ���� �������� �Ҵ��ϱ� ���� �̰��� ȣ���մϴ�.
        //���������� �̷��� �ϸ� �޽ð� �⺻ �׷��Ƚ� API���� "���� ����"�� ����ϰ� �˴ϴ�.�޽� �����Ͱ� ���� ����Ǵ� ��� �� ȿ�����Դϴ�.
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

        //�� ������ ��ġ �迭�� �Ҵ��մϴ�.
        waterMesh.SetVertices(waterVertices);

        //�ﰢ���� ���������� �޽��� ������ �ٽ� ���
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
