Shader "VertexLit Funky" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "RenderType"="Funky" }
 Pass {
  Tags { "RenderType"="Funky" }
  Lighting On
  Material {
   Ambient [_Color]
   Diffuse [_Color]
  }
  SetTexture [_MainTex] { combine texture * primary double, texture alpha * primary alpha }
 }
}
}