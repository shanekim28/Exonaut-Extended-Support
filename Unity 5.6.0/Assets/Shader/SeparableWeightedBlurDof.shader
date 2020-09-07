Shader "Hidden/SeparableWeightedBlurDof" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _TapMedium ("TapMedium (RGB)", 2D) = "" {}
 _TapLow ("TapLow (RGB)", 2D) = "" {}
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
Vector 0 [_Threshhold]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 38 ALU, 5 TEX
PARAM c[2] = { program.local[0],
		{ 1, 0.2199707, 0.70703125, 0.070983887 } };
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
TEX R0, fragment.texcoord[1], texture[0], 2D;
TEX R1, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R10, fragment.texcoord[2], texture[0], 2D;
TEX R9, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R2, fragment.texcoord[0], texture[0], 2D;
MUL R6.xyz, R0, R0.w;
MUL R7.xyz, R1, R1.w;
MOV R0.x, R0.w;
MOV R6.w, R0;
MOV R7.w, R1;
MAX R8, R6, R7;
MUL R4.xyz, R10, R10.w;
MUL R3.xyz, R9, R9.w;
MOV R0.y, R1.w;
MOV R0.z, R10.w;
MOV R0.w, R9;
DP4 R0.w, R0, c[1].x;
ADD R0.w, R2, R0;
RCP R0.w, R0.w;
MOV R4.w, R10;
MOV R3.w, R9;
MAX R5, R4, R3;
MAX R5, R8, R5;
MUL R8.xyz, R2, R2.w;
ADD R1.xyz, R6, R8;
ADD R0.xyz, R7, R1;
MOV R8.w, R2;
MAX R5, R5, R8;
ADD R0.xyz, R4, R0;
ADD R0.xyz, R3, R0;
MUL R0.xyz, R0, R0.w;
DP3 R2.x, R5, c[1].yzww;
ADD_SAT R0.w, R2.x, -c[0].x;
ADD R1.xyz, R5, -R0;
MUL R0.w, R0, c[0].y;
MAD R1.xyz, R0.w, R1, R0;
MAX result.color.xyz, R1, R0;
MOV result.color.w, R5;
END
# 38 instructions, 11 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_Threshhold]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 42 ALU, 5 TEX
dcl_2d s0
def c1, 1.00000000, 0.21997070, 0.70703125, 0.07098389
dcl t0.xy
dcl t1
dcl t2
texld r4, t0, s0
texld r10, t1, s0
texld r3, t2, s0
mul_pp r2.xyz, r10, r10.w
mul_pp r6.xyz, r3, r3.w
mov r0.y, t1.w
mov r0.x, t1.z
mov r1.xy, r0
mov_pp r2.w, r10
mov r0.y, t2.w
mov r0.x, t2.z
mov_pp r6.w, r3
texld r9, r1, s0
texld r0, r0, s0
mul_pp r7.xyz, r0, r0.w
mov_pp r7.w, r0
mul_pp r1.xyz, r9, r9.w
mov_pp r1.w, r9
max_pp r5, r2, r1
max_pp r8, r6, r7
max_pp r5, r5, r8
mul_pp r8.xyz, r4, r4.w
add_pp r3.xyz, r2, r8
mov_pp r8.w, r4
max_pp r5, r5, r8
mov_pp r2.w, r0
add_pp r3.xyz, r1, r3
mov r0.z, c1.w
mov r0.y, c1.z
mov r0.x, c1.y
dp3_pp r0.x, r5, r0
add_sat r0.x, r0, -c0
mov_pp r2.y, r9.w
mov_pp r2.z, r3.w
mov_pp r2.x, r10.w
dp4_pp r2.x, r2, c1.x
add_pp r1.x, r4.w, r2
add_pp r2.xyz, r6, r3
add_pp r2.xyz, r7, r2
rcp_pp r1.x, r1.x
mul_pp r1.xyz, r2, r1.x
add_pp r2.xyz, r5, -r1
mul r0.x, r0, c0.y
mad_pp r0.xyz, r0.x, r2, r1
max_pp r0.xyz, r0, r1
mov_pp r0.w, r5
mov_pp oC0, r0
"
}
}
 }
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
# 19 ALU, 5 TEX
PARAM c[1] = { { 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R1, fragment.texcoord[1], texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R3, fragment.texcoord[2], texture[0], 2D;
TEX R4, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R2, fragment.texcoord[1].zwzw, texture[0], 2D;
MUL R1.xyz, R1, R1.w;
MAD R0.xyz, R0, R0.w, R1;
MOV R1.x, R1.w;
MAD R0.xyz, R2, R2.w, R0;
MAD R0.xyz, R3, R3.w, R0;
MOV R1.y, R2.w;
MOV R1.w, R4;
MOV R1.z, R3.w;
DP4 R1.x, R1, c[0].x;
ADD R1.x, R0.w, R1;
RCP R1.x, R1.x;
MAD R0.xyz, R4, R4.w, R0;
MUL result.color.xyz, R0, R1.x;
MOV result.color.w, R0;
END
# 19 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 18 ALU, 5 TEX
dcl_2d s0
def c0, 1.00000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2
texld r2, t2, s0
texld r4, t1, s0
mul_pp r4.xyz, r4, r4.w
mov r0.y, t1.w
mov r0.x, t1.z
mov r1.y, t2.w
mov r1.x, t2.z
texld r3, r0, s0
texld r1, r1, s0
texld r0, t0, s0
mad_pp r5.xyz, r0, r0.w, r4
mad_pp r3.xyz, r3, r3.w, r5
mov_pp r4.x, r4.w
mad_pp r2.xyz, r2, r2.w, r3
mov_pp r4.y, r3.w
mov_pp r4.w, r1
mov_pp r4.z, r2.w
dp4_pp r0.x, r4, c0.x
add_pp r0.x, r0.w, r0
rcp_pp r0.x, r0.x
mad_pp r1.xyz, r1, r1.w, r2
mul_pp r0.xyz, r1, r0.x
mov_pp oC0, r0
"
}
}
 }
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
# 11 ALU, 5 TEX
PARAM c[1] = { { 0.2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1.xyz, fragment.texcoord[1], texture[0], 2D;
TEX R4.xyz, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R3.xyz, fragment.texcoord[2], texture[0], 2D;
TEX R2.xyz, fragment.texcoord[1].zwzw, texture[0], 2D;
ADD R0.xyz, R0, R1;
ADD R0.xyz, R0, R2;
ADD R0.xyz, R0, R3;
ADD R0.xyz, R0, R4;
MUL result.color.xyz, R0, c[0].x;
MOV result.color.w, R0;
END
# 11 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 12 ALU, 5 TEX
dcl_2d s0
def c0, 0.20000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2
texld r4, t2, s0
texld r2, t1, s0
mov r0.y, t2.w
mov r0.x, t2.z
mov r1.xy, r0
mov r0.y, t1.w
mov r0.x, t1.z
texld r3, r1, s0
texld r0, r0, s0
texld r1, t0, s0
add_pp r1.xyz, r1, r2
add_pp r0.xyz, r1, r0
add_pp r0.xyz, r0, r4
add_pp r0.xyz, r0, r3
mul_pp r0.xyz, r0, c0.x
mov_pp r0.w, r1
mov_pp oC0, r0
"
}
}
 }
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
SetTexture 0 [_TapMedium] 2D
SetTexture 1 [_TapLow] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[1] = { { 1.5 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[1], 2D;
TEX R1.xyz, fragment.texcoord[0], texture[0], 2D;
ADD R0.xyz, R0, -R1;
POW R1.w, R0.w, c[0].x;
MAD result.color.xyz, R1.w, R0, R1;
MOV result.color.w, R0;
END
# 6 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_TapMedium] 2D
SetTexture 1 [_TapLow] 2D
"ps_2_0
; 7 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 1.50000000, 0, 0, 0
dcl t0.xy
texld r1, t0, s1
texld r2, t0, s0
pow_pp r0.x, r1.w, c0.x
add_pp r1.xyz, r1, -r2
mov_pp r0.w, r1
mad_pp r0.xyz, r0.x, r1, r2
mov_pp oC0, r0
"
}
}
 }
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
# 1 ALU, 1 TEX
PARAM c[1] = { program.local[0] };
TEX result.color, fragment.texcoord[0], texture[0], 2D;
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 1 ALU, 1 TEX
dcl_2d s0
dcl t0.xy
texld r0, t0, s0
mov oC0, r0
"
}
}
 }
}
Fallback Off
}