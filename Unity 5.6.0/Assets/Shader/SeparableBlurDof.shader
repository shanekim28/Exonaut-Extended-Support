Shader "Hidden/SeparableBlurDof" {
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
Vector 5 [offsets]
"!!ARBvp1.0
# 10 ALU
PARAM c[7] = { { 2, -2, 3, -3 },
		state.matrix.mvp,
		program.local[5],
		{ 1, -1 } };
TEMP R0;
TEMP R1;
MOV R1, c[0];
MOV R0.xy, c[6];
MAD result.texcoord[1], R0.xxyy, c[5].xyxy, vertex.texcoord[0].xyxy;
MAD result.texcoord[2], R1.xxyy, c[5].xyxy, vertex.texcoord[0].xyxy;
MAD result.texcoord[3], R1.zzww, c[5].xyxy, vertex.texcoord[0].xyxy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [offsets]
"vs_2_0
; 11 ALU
def c5, 1.00000000, -1.00000000, 2.00000000, -2.00000000
def c6, 3.00000000, -3.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mad oT1, c5.xxyy, r0.xyxy, v1.xyxy
mov r0.xy, c4
mov r0.zw, c4.xyxy
mad oT2, c5.zzww, r0.xyxy, v1.xyxy
mad oT3, c6.xxyy, r0.zwzw, v1.xyxy
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
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 14 ALU, 7 TEX
PARAM c[1] = { { 0.14285715 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEX R6, fragment.texcoord[3].zwzw, texture[0], 2D;
TEX R5, fragment.texcoord[3], texture[0], 2D;
TEX R4, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R3, fragment.texcoord[2], texture[0], 2D;
TEX R2, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R0, R0, R1;
ADD R0, R0, R2;
ADD R0, R0, R3;
ADD R0, R0, R4;
ADD R0, R0, R5;
ADD R0, R0, R6;
MUL result.color, R0, c[0].x;
END
# 14 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 16 ALU, 7 TEX
dcl_2d s0
def c0, 0.14285715, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2
dcl t3
texld r3, t2, s0
texld r6, t0, s0
texld r5, t1, s0
mov r0.y, t1.w
mov r0.x, t1.z
mov r4.xy, r0
mov r1.y, t2.w
mov r1.x, t2.z
mov r2.xy, r1
mov r0.y, t3.w
mov r0.x, t3.z
add_pp r5, r6, r5
texld r0, r0, s0
texld r1, t3, s0
texld r2, r2, s0
texld r4, r4, s0
add_pp r4, r5, r4
add_pp r3, r4, r3
add_pp r2, r3, r2
add_pp r1, r2, r1
add_pp r0, r1, r0
mul_pp r0, r0, c0.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}