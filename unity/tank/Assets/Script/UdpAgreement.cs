using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

/*UPD通信协议定义*/
public class UdpAgreement
{
    //结构体序列化  
    [System.Serializable]
    //1字节对齐   
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SocketPackage
    {
        public double x ;
        public double y;
        public double z;
        public double phi;
        public double theta;
        public double psi;
    };
}
