using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class testtext : MonoBehaviour
{
    public GameObject cubetext;
    TextMesh tm;
    public ComputeShader computeshader;
    ComputeBuffer Q;
    ComputeBuffer E;
    int size = 1024;
    int E2QRIndex;
    // Start is called before the first frame update
    void Start()
    {
        tm = cubetext.GetComponent<TextMesh>();
        //结构体内存大小
        int stride = Marshal.SizeOf(typeof(Quaternion));
        //声明申请要传递的结构体数组的内存位置
        Q = new ComputeBuffer(size, stride);
        E2QRIndex = computeshader.FindKernel("Eular2QRotate");
        computeshader.SetBuffer(E2QRIndex, "Quant", Q);
        computeshader.SetBuffer(E2QRIndex, "Eular", E);
    }

    // Update is called once per frame
    void Update()
    {
        tm.text = "heppp" + Time.deltaTime.ToString();
    }
}
