using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class testcshader : MonoBehaviour
{
    public struct Particle
    {
        public Vector2 position;
        public Vector2 velocity;
    }
    public ComputeShader computeshader;
    public Material material;
    ComputeBuffer particles;
    const int WARP_SIZE = 1024;
    int size = 1024000;
    int stride;
    int warpCount;
    int kernelIndex;
    Particle[] initBuffer;
    Particle[] getBuffer;
    public float dt;
    public Vector2 origin_pos;
    // Start is called before the first frame update
    void Start()
    {
        //运算次数
        warpCount = Mathf.CeilToInt((float)size / WARP_SIZE);
        //结构体内存大小
        stride = Marshal.SizeOf(typeof(Particle));
        //声明申请要传递的结构体数组的内存位置
        particles = new ComputeBuffer(size, stride);
        //初始化该结构体内存位置的数值
        initBuffer = new Particle[size];
        for (int i = 0; i < size; i++)
        {
            // initBuffer[i] = new Particle();
            initBuffer[i].position = Random.insideUnitCircle * 10f;
            initBuffer[i].velocity = Vector2.zero;
        }
        particles.SetData(initBuffer);

        //寻到computeshader里边的核函数
        kernelIndex = computeshader.FindKernel("CSMain");
        //从C#变量内存传送去computeshader变量内存位置
        /*
        注意：computeshader.SetXXX()
        如果带有kernelIndex变量的，
        就说明这个api设置的变量是仅用于单个kernel的，
        并且是一种绑定的关系，不用每次update()都set一次。
        如果不带有，则该变量是通用于所有kernel的。
        */
        computeshader.SetBuffer(kernelIndex, "particles", particles);
        //绑定在material里
        material.SetBuffer("Particles", particles);
        //初始化一些额外东西
        origin_pos = GetMousePosition();
        //初始化该获取缓冲
        getBuffer = new Particle[size];
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            //重置buffer与鼠标位置
            particles.SetData(initBuffer);
            origin_pos = GetMousePosition();
        }
        // //传递 int float Vector3 等数据 
        // //Vector2 和 Vector3 要转成 float[2] float[3] 才可以用
        computeshader.SetFloats("mousePosition", new float[] { origin_pos.x, origin_pos.y });
        computeshader.SetFloat("dt", Time.deltaTime);
        computeshader.Dispatch(kernelIndex, warpCount, 1, 1);


        particles.GetData(getBuffer);
        Debug.Log(getBuffer[0].velocity.x.ToString() + " " + getBuffer[0].velocity.y.ToString() + " " + getBuffer[0].position.x.ToString() + " " + getBuffer[0].position.y.ToString() + "origin_pos: " + origin_pos[0].ToString() + " " + origin_pos[1].ToString());

    }
    Vector2 GetMousePosition()
    {
        var mp = Input.mousePosition + new Vector3(0, 0, 0.3f);
        mp = Camera.main.ScreenToWorldPoint(mp);
        return new Vector2(mp.x * 100, mp.y * 100);
    }
    //渲染效果
    void OnRenderObject()
    {
        //写入材质球 pass
        material.SetPass(0);
        //渲染粒子
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, size);
    }
    void OnDestroy()
    {
        if (particles != null)
            particles.Release();
    }
}
