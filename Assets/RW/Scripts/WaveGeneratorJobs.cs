using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class WaveGeneratorJobs : MonoBehaviour
{
    struct UpdateMeshJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;

        [ReadOnly]
        public NativeArray<Vector3> normals;

        public float offsetSpeed;
        public float scale;
        public float height;

        public float time;

        private float Noise(float x, float y)
        {
            float2 pos = math.float2(x, y);
            return noise.snoise(pos);
        }

        public void Execute(int i)
        {
            if (normals[i].z > 0f)
            {
                var vertex = vertices[i];
                float noiseValue = Noise(vertex.x * scale + offsetSpeed * time, vertex.y * scale + offsetSpeed * time);
                vertices[i] = new Vector3(vertex.x, vertex.y, noiseValue * height + 0.3f);
            }
        }
    }

    [Header("Wave Parameters")]
    public float waveScale;
    public float waveOffsetSpeed;
    public float waveHieght;

    [Header("References and Prefabs")]
    public MeshFilter waterMeshFilter;
    private Mesh waterMesh;

    private NativeArray<Vector3> waterVertices;
    private NativeArray<Vector3> waterNormals;

    private List<Vector3> baseWaterVertices;

    private JobHandle meshModificationJobHandle;
    private UpdateMeshJob meshModificationJob;

    bool _isStart = false;
    void Start()
    {
        waterMesh = waterMeshFilter.mesh;

        waterMesh.MarkDynamic();

        // Allocator.Temp : 1프레임 이하 수명. 빠른 할당
        // Allocator.TempJob : 4프레임 이하 수명. Temp보다 느린 할당
        // Allocator.Persistent : 어플리케이션 주기의 수명. 가장 느린 할당.

        waterVertices = new NativeArray<Vector3>(waterMesh.vertices, Allocator.Persistent);
        waterNormals = new NativeArray<Vector3>(waterMesh.normals, Allocator.Persistent);

        baseWaterVertices = new List<Vector3>(waterMesh.vertices);
    }

    private void Init()
    {
        for (int i = 0; i < waterVertices.Length; i++)
        {
            waterVertices[i] = baseWaterVertices[i];
        }
        waterMesh.SetVertices(waterVertices);
        waterMesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStart == false)
        {
            return;
        }

        meshModificationJob = new UpdateMeshJob()
        {
            vertices = waterVertices,
            normals = waterNormals,
            offsetSpeed = waveOffsetSpeed,
            time = Time.time,
            scale = waveScale,
            height = waveHieght,
        };

        meshModificationJobHandle = meshModificationJob.Schedule(waterVertices.Length, 64);
    }

    private void LateUpdate()
    {
        if (_isStart == false)
        {
            return;
        }
        meshModificationJobHandle.Complete();
        waterMesh.SetVertices(meshModificationJob.vertices);
        waterMesh.RecalculateNormals();
    }

    private void OnDestroy()
    {
        waterVertices.Dispose();
        waterNormals.Dispose();
    }

    public void OnStart(bool isStart)
    {
        _isStart = isStart;
        if (_isStart == false)
            Init();
    }
}
