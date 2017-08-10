#ifndef __DEPTH_CGINC__
#define __DEPTH_CGINC__



#include "UnityCG.cginc"

float CameraDepthTextureDepthTo01Linear(float z) {
    return (unity_OrthoParams.w < 0.5) ?
        Linear01Depth(z) :
        1.0 - z / _ProjectionParams.y;
}



#endif