Shader "Hidden/HollywoodFlareStretchShader" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 5 ALU
PARAM c[5] = { program.local[0],
		state.matrix.mvp };
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 5 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mov oT0.xy, v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [offsets]
Float 1 [stretchWidth]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 33 ALU, 11 TEX
PARAM c[4] = { program.local[0..1],
		{ 2, 4, 8, 14 },
		{ 20 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEMP R8;
TEMP R9;
TEMP R10;
MOV R0.xy, c[0];
MUL R4.xy, R0, c[1].x;
MAD R2.xy, R4, c[2].z, fragment.texcoord[0];
MAD R2.zw, -R4.xyxy, c[2].z, fragment.texcoord[0].xyxy;
MAD R3.xy, R4, c[2].w, fragment.texcoord[0];
MAD R3.zw, -R4.xyxy, c[2].w, fragment.texcoord[0].xyxy;
MAD R4.zw, -R4.xyxy, c[3].x, fragment.texcoord[0].xyxy;
MAD R0.xy, R4, c[2].x, fragment.texcoord[0];
MAD R0.zw, -R4.xyxy, c[2].x, fragment.texcoord[0].xyxy;
MAD R1.xy, R4, c[2].y, fragment.texcoord[0];
MAD R1.zw, -R4.xyxy, c[2].y, fragment.texcoord[0].xyxy;
MAD R4.xy, R4, c[3].x, fragment.texcoord[0];
TEX R10, R4.zwzw, texture[0], 2D;
TEX R9, R4, texture[0], 2D;
TEX R8, R3.zwzw, texture[0], 2D;
TEX R7, R3, texture[0], 2D;
TEX R6, R2.zwzw, texture[0], 2D;
TEX R5, R2, texture[0], 2D;
TEX R4, R1.zwzw, texture[0], 2D;
TEX R3, R1, texture[0], 2D;
TEX R2, R0.zwzw, texture[0], 2D;
TEX R1, R0, texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MAX R0, R0, R1;
MAX R0, R0, R2;
MAX R0, R0, R3;
MAX R0, R0, R4;
MAX R0, R0, R5;
MAX R0, R0, R6;
MAX R0, R0, R7;
MAX R0, R0, R8;
MAX R0, R0, R9;
MAX result.color, R0, R10;
END
# 33 instructions, 11 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [offsets]
Float 1 [stretchWidth]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 23 ALU, 11 TEX
dcl_2d s0
def c2, 2.00000000, 4.00000000, 8.00000000, 14.00000000
def c3, 20.00000000, 0, 0, 0
dcl t0.xy
texld r10, t0, s0
mov r0.x, c1
mul r1.xy, c0, r0.x
mad r9.xy, r1, c2.x, t0
mad r8.xy, -r1, c2.x, t0
mad r7.xy, r1, c2.y, t0
mad r6.xy, -r1, c2.y, t0
mad r5.xy, r1, c2.z, t0
mad r4.xy, -r1, c2.z, t0
mad r3.xy, r1, c2.w, t0
mad r2.xy, -r1, c2.w, t0
mad r0.xy, -r1, c3.x, t0
mad r1.xy, r1, c3.x, t0
texld r0, r0, s0
texld r1, r1, s0
texld r2, r2, s0
texld r3, r3, s0
texld r4, r4, s0
texld r5, r5, s0
texld r6, r6, s0
texld r7, r7, s0
texld r8, r8, s0
texld r9, r9, s0
max r9, r10, r9
max r8, r9, r8
max r7, r8, r7
max r6, r7, r6
max r5, r6, r5
max r4, r5, r4
max r3, r4, r3
max r2, r3, r2
max r1, r2, r1
max r0, r1, r0
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}