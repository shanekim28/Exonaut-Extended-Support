Shader "Shield" {
Properties {
 _Offset ("Time", Range(0,1)) = 0
 _Color ("Tint (RGB)", Color) = (1,1,1,1)
 _SurfaceTex ("Texture (RGB)", 2D) = "white" {}
 _RampTex ("Facing Ratio Ramp (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One One
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 9 [_World2Object]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Float 15 [_Offset]
"!!ARBvp1.0
# 21 ALU
PARAM c[16] = { { 1, 0.5 },
		state.matrix.mvp,
		state.matrix.texture[1],
		program.local[9..15] };
TEMP R0;
TEMP R1;
MOV R1.w, c[0].x;
MOV R1.xyz, c[14];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, R0, R0;
RSQ R0.w, R0.w;
MUL R0.xyz, R0.w, R0;
DP3 R0.x, R0, vertex.normal;
ABS result.texcoord[1].x, R0;
MOV R0.xzw, vertex.texcoord[0];
ADD R0.y, vertex.texcoord[0], c[15].x;
DP4 result.texcoord[0].y, R0, c[6];
DP4 result.texcoord[0].x, R0, c[5];
MOV result.texcoord[2].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
MOV result.texcoord[1].y, c[0];
END
# 21 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture1]
Matrix 8 [_World2Object]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Float 14 [_Offset]
"vs_2_0
; 21 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
def c15, 1.00000000, 0.50000000, 0, 0
mov r1.w, c15.x
mov r1.xyz, c13
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, r0, r0
rsq r0.w, r0.w
mul r0.xyz, r0.w, r0
dp3 r0.x, r0, v1
abs oT1.x, r0
mov r0.xzw, v2
add r0.y, v2, c14.x
dp4 oT0.y, r0, c5
dp4 oT0.x, r0, c4
mov oT2.xyz, v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
mov oT1.y, c15
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_Color]
SetTexture 0 [_RampTex] 2D
SetTexture 1 [_SurfaceTex] 2D
"!!ARBfp1.0
OPTION ARB_fog_exp2;
# 6 ALU, 2 TEX
PARAM c[1] = { program.local[0] };
TEMP R0;
TEMP R1;
TEX R1.xyz, fragment.texcoord[1], texture[0], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[1], 2D;
MUL R1.xyz, R1, c[0].w;
MUL R0.xyz, R0, R1;
MUL result.color.xyz, R0, c[0];
MOV result.color.w, R1.x;
END
# 6 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_Color]
SetTexture 0 [_RampTex] 2D
SetTexture 1 [_SurfaceTex] 2D
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0.xy
dcl t1.xy
texld r0, t0, s1
texld r1, t1, s0
mul r1.xyz, r1, c0.w
mul r0.xyz, r0, r1
mov_pp r0.w, r1.x
mul r0.xyz, r0, c0
mov_pp oC0, r0
"
}
}
  SetTexture [_RampTex] { combine texture }
  SetTexture [_SurfaceTex] { combine texture }
 }
}
Fallback "Transparent/VertexLit"
}