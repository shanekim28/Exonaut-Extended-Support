Shader "Hidden/DepthOfField31" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _BgLowRez ("_BgLowRez (RGB)", 2D) = "" {}
 _FgLowRez ("_FgLowRez (RGB)", 2D) = "" {}
 _BgUnblurredTex ("_BgUnblurredTex (RGB)", 2D) = "" {}
 _MaskTex ("_BTest (RGB)", 2D) = "" {}
 _SourceTex ("_BTest (RGB)", 2D) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_TexelSize]
"!!ARBvp1.0
# 10 ALU
PARAM c[6] = { { -0.5, 1.5, 0.5, -1.5 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0, c[0];
MAD result.texcoord[1].xy, R0, c[5], vertex.texcoord[0];
MAD result.texcoord[2].xy, R0.zwzw, c[5], vertex.texcoord[0];
MAD result.texcoord[3].xy, R0.yzzw, c[5], vertex.texcoord[0];
MAD result.texcoord[4].xy, R0.wxzw, c[5], vertex.texcoord[0];
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 13 ALU
def c5, -0.50000000, 1.50000000, 0.50000000, -1.50000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mad oT1.xy, c5, r0, v1
mad oT2.xy, c5.zwzw, r0.zwzw, v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mov oT0.xy, v1
mad oT3.xy, c5.yzzw, r0, v1
mad oT4.xy, c5.wxzw, r0.zwzw, v1
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
SetTexture 1 [_BgLowRez] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 4 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0], texture[1], 2D;
ADD R1, -R0, R1;
MAD result.color, R0.w, R1, R0;
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BgLowRez] 2D
"ps_2_0
; 3 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0.xy
texld r0, t0, s1
texld r1, t0, s0
add_pp r0, -r1, r0
mad_pp r0, r1.w, r0, r1
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
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_TexelSize]
"!!ARBvp1.0
# 10 ALU
PARAM c[6] = { { -0.5, 1.5, 0.5, -1.5 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0, c[0];
MAD result.texcoord[1].xy, R0, c[5], vertex.texcoord[0];
MAD result.texcoord[2].xy, R0.zwzw, c[5], vertex.texcoord[0];
MAD result.texcoord[3].xy, R0.yzzw, c[5], vertex.texcoord[0];
MAD result.texcoord[4].xy, R0.wxzw, c[5], vertex.texcoord[0];
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 13 ALU
def c5, -0.50000000, 1.50000000, 0.50000000, -1.50000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mad oT1.xy, c5, r0, v1
mad oT2.xy, c5.zwzw, r0.zwzw, v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mov oT0.xy, v1
mad oT3.xy, c5.yzzw, r0, v1
mad oT4.xy, c5.wxzw, r0.zwzw, v1
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
SetTexture 1 [_BgLowRez] 2D
SetTexture 2 [_BgUnblurredTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 3 TEX
PARAM c[1] = { { 2, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R2, fragment.texcoord[0], texture[1], 2D;
TEX R1, fragment.texcoord[0], texture[2], 2D;
ADD R3, R2, -R1;
ADD_SAT R2.x, R2.w, -c[0].y;
MUL R2, R2.x, R3;
MAD R1, R2, c[0].x, R1;
ADD R1, -R0, R1;
MUL_SAT R2.x, R0.w, c[0];
MAD result.color, R2.x, R1, R0;
END
# 10 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BgLowRez] 2D
SetTexture 2 [_BgUnblurredTex] 2D
"ps_2_0
; 8 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c0, -0.50000000, 2.00000000, 0, 0
dcl t0.xy
texld r2, t0, s0
texld r1, t0, s2
texld r0, t0, s1
add_pp r3, r0, -r1
add_pp_sat r0.x, r0.w, c0
mul_pp r0, r0.x, r3
mad_pp r0, r0, c0.y, r1
add_pp r1, -r2, r0
mul_pp_sat r0.x, r2.w, c0.y
mad_pp r0, r0.x, r1, r2
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
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_TexelSize]
"!!ARBvp1.0
# 10 ALU
PARAM c[6] = { { -0.5, 1.5, 0.5, -1.5 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0, c[0];
MAD result.texcoord[1].xy, R0, c[5], vertex.texcoord[0];
MAD result.texcoord[2].xy, R0.zwzw, c[5], vertex.texcoord[0];
MAD result.texcoord[3].xy, R0.yzzw, c[5], vertex.texcoord[0];
MAD result.texcoord[4].xy, R0.wxzw, c[5], vertex.texcoord[0];
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 13 ALU
def c5, -0.50000000, 1.50000000, 0.50000000, -1.50000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mad oT1.xy, c5, r0, v1
mad oT2.xy, c5.zwzw, r0.zwzw, v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mov oT0.xy, v1
mad oT3.xy, c5.yzzw, r0, v1
mad oT4.xy, c5.wxzw, r0.zwzw, v1
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
SetTexture 1 [_BgLowRez] 2D
SetTexture 2 [_BgUnblurredTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 3 TEX
PARAM c[1] = { { 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R2, fragment.texcoord[0], texture[1], 2D;
TEX R1, fragment.texcoord[0], texture[2], 2D;
ADD R3, R2, -R1;
ADD_SAT R2.x, R2.w, -c[0];
MUL R2, R2.x, R3;
MAD R1, R2, c[0].y, R1;
ADD R1, -R0, R1;
MOV_SAT R2.x, R0.w;
MAD result.color, R2.x, R1, R0;
END
# 10 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BgLowRez] 2D
SetTexture 2 [_BgUnblurredTex] 2D
"ps_2_0
; 8 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c0, -0.50000000, 2.00000000, 0, 0
dcl t0.xy
texld r2, t0, s0
texld r1, t0, s2
texld r0, t0, s1
add_pp r3, r0, -r1
add_pp_sat r0.x, r0.w, c0
mul_pp r0, r0.x, r3
mad_pp r0, r0, c0.y, r1
add_pp r1, -r2, r0
mov_pp_sat r0.x, r2.w
mad_pp r0, r0.x, r1, r2
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
  ColorMask A
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_CameraDepthTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 1 TEX
PARAM c[6] = { program.local[0..4],
		{ 1, 0 } };
TEMP R0;
TEMP R1;
TEX R0.x, fragment.texcoord[0], texture[0], 2D;
MAD R0.x, R0, c[0], c[0].y;
RCP R0.z, R0.x;
SLT R0.y, c[1].x, R0.z;
MOV R0.x, c[3];
ADD R0.z, -R0, c[1].x;
ADD R1.x, -R0, c[1];
ADD_SAT R0.w, R0.z, -c[3].x;
ABS R0.y, R0;
RCP R1.x, R1.x;
MUL R0.w, R0, c[4].x;
MUL R0.w, R0, R1.x;
ADD R1.x, R0, c[1];
ADD_SAT R0.x, -R0.z, -c[3];
RCP R0.z, R1.x;
MUL R0.x, R0, c[4].y;
MUL R0.x, R0, R0.z;
CMP R0.y, -R0, c[5], c[5].x;
CMP R0.x, -R0.y, R0.w, R0;
MUL_SAT result.color, R0.x, c[2].x;
END
# 20 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_CameraDepthTexture] 2D
"ps_2_0
; 22 ALU, 1 TEX
dcl_2d s0
def c5, 0.00000000, 1.00000000, 0, 0
dcl t0.xy
texld r0, t0, s0
mad r0.x, r0, c0, c0.y
rcp r1.x, r0.x
add r0.x, -r1, c1
add r1.x, r1, -c1
add_sat r2.x, r1, -c3
cmp r0.x, r0, c5, c5.y
mov r3.x, c1
add r3.x, c3, r3
rcp r4.x, r3.x
mul r2.x, r2, c4.y
mov r3.x, c1
add r3.x, -c3, r3
add_sat r1.x, -r1, -c3
abs_pp r0.x, r0
mul r2.x, r2, r4
rcp r3.x, r3.x
mul r1.x, r1, c4
mul r1.x, r1, r3
cmp r0.x, -r0, r1, r2
mul_sat r0.x, r0, c2
mov_pp r0, r0.x
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
  ColorMask A
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_CameraDepthTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 1 TEX
PARAM c[6] = { program.local[0..4],
		{ 0 } };
TEMP R0;
TEX R0.x, fragment.texcoord[0], texture[0], 2D;
MAD R0.y, R0.x, c[0].x, c[0];
MOV R0.z, c[3].x;
ADD R0.x, R0.z, c[1];
RCP R0.y, R0.y;
ADD_SAT R0.w, R0.y, -R0.x;
RCP R0.z, R0.x;
MUL R0.w, R0, c[4].y;
MUL R0.z, R0.w, R0;
ADD R0.x, R0.y, -R0;
CMP R0.x, -R0, R0.z, c[5];
MUL_SAT result.color, R0.x, c[2].x;
END
# 12 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_CameraDepthTexture] 2D
"ps_2_0
; 13 ALU, 1 TEX
dcl_2d s0
def c5, 0.00000000, 0, 0, 0
dcl t0.xy
texld r1, t0, s0
mov r0.x, c1
add r0.x, c3, r0
mad r1.x, r1, c0, c0.y
rcp r1.x, r1.x
add_sat r3.x, r1, -r0
rcp r2.x, r0.x
mul r3.x, r3, c4.y
mul r2.x, r3, r2
add r0.x, r1, -r0
cmp_pp r0.x, -r0, c5, r2
mul_pp_sat r0.x, r0, c2
mov_pp r0, r0.x
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
  ColorMask A
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 0 TEX
PARAM c[1] = { { 1, 0 } };
MOV result.color, c[0].xyxy;
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
def c0, 1.00000000, 0.00000000, 0, 0
mov r0.yw, c0.y
mov r0.xz, c0.x
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
Vector 5 [_MainTex_TexelSize]
Vector 6 [_Vh]
"!!ARBvp1.0
# 10 ALU
PARAM c[8] = { { 2, -2, 3, -3 },
		state.matrix.mvp,
		program.local[5..6],
		{ 1, -1 } };
TEMP R0;
MOV R0.xy, c[6];
MUL R0.xy, R0, c[5];
MAD result.texcoord[1], R0.xyxy, c[7].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[2], R0.xyxy, c[0].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[3], R0.xyxy, c[0].zzww, vertex.texcoord[0].xyxy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Vector 5 [_Vh]
"vs_2_0
; 10 ALU
def c6, 1.00000000, -1.00000000, 2.00000000, -2.00000000
def c7, 3.00000000, -3.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mul r0.xy, c5, r0
mad oT1, r0.xyxy, c6.xxyy, v1.xyxy
mad oT2, r0.xyxy, c6.zzww, v1.xyxy
mad oT3, r0.xyxy, c7.xxyy, v1.xyxy
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
# 10 ALU, 5 TEX
PARAM c[1] = { { 0.2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R4, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R3, fragment.texcoord[2], texture[0], 2D;
TEX R2, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R0, R0, R1;
ADD R0, R0, R2;
ADD R0, R0, R3;
ADD R0, R0, R4;
MUL result.color, R0, c[0].x;
END
# 10 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 11 ALU, 5 TEX
dcl_2d s0
def c0, 0.20000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2
texld r4, t0, s0
texld r3, t1, s0
mov r1.y, t1.w
mov r1.x, t1.z
mov r2.xy, r1
mov r0.y, t2.w
mov r0.x, t2.z
add_pp r3, r4, r3
texld r0, r0, s0
texld r1, t2, s0
texld r2, r2, s0
add_pp r2, r3, r2
add_pp r1, r2, r1
add_pp r0, r1, r0
mul_pp r0, r0, c0.x
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
  ColorMask A
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 0 TEX
PARAM c[1] = { { 1, 0 } };
MOV result.color, c[0].xyxy;
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
def c0, 1.00000000, 0.00000000, 0, 0
mov r0.yw, c0.y
mov r0.xz, c0.x
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
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 1 ALU, 0 TEX
PARAM c[1] = { { 1, 0 } };
MOV result.color, c[0].xyxy;
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
def c0, 1.00000000, 0.00000000, 0, 0
mov r0.yw, c0.y
mov r0.xz, c0.x
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
Vector 5 [_MainTex_TexelSize]
Vector 6 [_Vh]
"!!ARBvp1.0
# 10 ALU
PARAM c[8] = { { 2, -2, 3, -3 },
		state.matrix.mvp,
		program.local[5..6],
		{ 1, -1 } };
TEMP R0;
MOV R0.xy, c[6];
MUL R0.xy, R0, c[5];
MAD result.texcoord[1], R0.xyxy, c[7].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[2], R0.xyxy, c[0].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[3], R0.xyxy, c[0].zzww, vertex.texcoord[0].xyxy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Vector 5 [_Vh]
"vs_2_0
; 10 ALU
def c6, 1.00000000, -1.00000000, 2.00000000, -2.00000000
def c7, 3.00000000, -3.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mul r0.xy, c5, r0
mad oT1, r0.xyxy, c6.xxyy, v1.xyxy
mad oT2, r0.xyxy, c6.zzww, v1.xyxy
mad oT3, r0.xyxy, c7.xxyy, v1.xyxy
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
Vector 0 [_BokehThreshhold]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 31 ALU, 5 TEX
PARAM c[3] = { program.local[0],
		{ 0.2, 1 },
		{ 0.2199707, 0.70703125, 0.070983887 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEX R4, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[0], 2D;
TEX R2, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R3, fragment.texcoord[2], texture[0], 2D;
ADD R5, R0, R1;
ADD R5, R5, R2;
ADD R5, R5, R3;
MUL R2.xyz, R2, R2.w;
MUL R1.xyz, R1, R1.w;
MUL R6.xyz, R4, R4.w;
MUL R3.xyz, R3, R3.w;
MAX R3.xyz, R3, R6;
MAX R2.xyz, R2, R3;
MAX R1.xyz, R1, R2;
MUL R3.xyz, R0, R0.w;
MAX R1.xyz, R1, R3;
DP3 R0.w, R1, c[2];
ADD R2, R5, R4;
MUL R2, R2, c[1].x;
ADD R0.w, R0, -c[0].x;
ADD R1.xyz, -R2, R1;
MUL_SAT R0.w, R0, c[0].y;
MAD R1.xyz, R0.w, R1, R2;
MAX R1.xyz, R2, R1;
ADD R0.xyz, -R0, R1;
ABS R0.xyz, R0;
DP3 R0.x, R0, c[2];
ADD R0.x, R0, c[1].y;
MUL_SAT R1.w, R2, R0.x;
MOV result.color, R1;
END
# 31 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_BokehThreshhold]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 37 ALU, 5 TEX
dcl_2d s0
def c1, 0.20000000, 0.21997070, 0.70703125, 0.07098389
def c2, 1.00000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2
texld r4, t0, s0
texld r3, t1, s0
mov r1.y, t1.w
mov r1.x, t1.z
mov r2.xy, r1
mov r0.y, t2.w
mov r0.x, t2.z
add_pp r5, r4, r3
texld r0, r0, s0
texld r1, t2, s0
texld r2, r2, s0
add_pp r5, r5, r2
add_pp r5, r5, r1
add_pp r5, r5, r0
mul_pp r0.xyz, r0, r0.w
mul_pp r1.xyz, r1, r1.w
max_pp r0.xyz, r1, r0
mul_pp r1.xyz, r2, r2.w
max_pp r0.xyz, r1, r0
mul_pp r2.xyz, r3, r3.w
max_pp r0.xyz, r2, r0
mul_pp r1.xyz, r4, r4.w
max_pp r1.xyz, r0, r1
mul_pp r2, r5, c1.x
mov r0.z, c1.w
mov r0.y, c1.z
mov r0.x, c1.y
dp3_pp r0.x, r1, r0
add r0.x, r0, -c0
add_pp r1.xyz, -r2, r1
mul_sat r0.x, r0, c0.y
mad_pp r0.xyz, r0.x, r1, r2
max_pp r0.xyz, r2, r0
add_pp r1.xyz, -r4, r0
abs_pp r2.xyz, r1
mov r1.x, c1.y
mov r1.z, c1.w
mov r1.y, c1.z
dp3_pp r1.x, r2, r1
add_pp r1.x, r1, c2
mul_pp_sat r0.w, r2, r1.x
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
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_TexelSize]
"!!ARBvp1.0
# 10 ALU
PARAM c[6] = { { -0.5, 1.5, 0.5, -1.5 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0, c[0];
MAD result.texcoord[1].xy, R0, c[5], vertex.texcoord[0];
MAD result.texcoord[2].xy, R0.zwzw, c[5], vertex.texcoord[0];
MAD result.texcoord[3].xy, R0.yzzw, c[5], vertex.texcoord[0];
MAD result.texcoord[4].xy, R0.wxzw, c[5], vertex.texcoord[0];
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 13 ALU
def c5, -0.50000000, 1.50000000, 0.50000000, -1.50000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mad oT1.xy, c5, r0, v1
mad oT2.xy, c5.zwzw, r0.zwzw, v1
mov r0.xy, c4
mov r0.zw, c4.xyxy
mov oT0.xy, v1
mad oT3.xy, c5.yzzw, r0, v1
mad oT4.xy, c5.wxzw, r0.zwzw, v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Float 0 [_ForegroundBlurWeight]
SetTexture 0 [_FgLowRez] 2D
SetTexture 1 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[1], 2D;
TEX R1, fragment.texcoord[0], texture[0], 2D;
MAD R1.w, R1, c[1].x, -R0;
MAX R1.w, R0, R1;
MUL R2.x, R1.w, c[0];
MOV R1.w, R2.x;
ADD R1, -R0, R1;
MOV_SAT R2.x, R2;
MAD result.color, R2.x, R1, R0;
END
# 9 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Float 0 [_ForegroundBlurWeight]
SetTexture 0 [_FgLowRez] 2D
SetTexture 1 [_MainTex] 2D
"ps_2_0
; 8 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 2.00000000, 0, 0, 0
dcl t0.xy
texld r1, t0, s1
texld r2, t0, s0
mad_pp r0.x, r2.w, c1, -r1.w
max_pp r0.x, r1.w, r0
mul r0.x, r0, c0
mov_pp r2.w, r0.x
add_pp r2, -r1, r2
mov_pp_sat r0.x, r0
mad_pp r0, r0.x, r2, r1
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
Vector 5 [_CameraDepthTexture_ST]
"!!ARBvp1.0
# 5 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Vector 4 [_CameraDepthTexture_ST]
"vs_2_0
; 5 ALU
dcl_position0 v0
dcl_texcoord0 v1
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_CameraDepthTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 2 TEX
PARAM c[6] = { program.local[0..4],
		{ 1, 0 } };
TEMP R0;
TEX R0.x, fragment.texcoord[0], texture[1], 2D;
TEX result.color.xyz, fragment.texcoord[0], texture[0], 2D;
MAD R0.x, R0, c[0], c[0].y;
MOV R0.y, c[3].x;
ADD R0.y, -R0, c[1].x;
RCP R0.x, R0.x;
SLT R0.z, R0.y, R0.x;
ADD_SAT R0.x, R0.y, -R0;
RCP R0.y, R0.y;
MUL R0.x, R0, c[4];
MUL R0.x, R0, R0.y;
ABS R0.z, R0;
CMP R0.y, -R0.z, c[5], c[5].x;
CMP R0.x, -R0.y, R0, c[5].y;
MUL_SAT result.color.w, R0.x, c[2].x;
END
# 15 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
Float 1 [focalDistance01]
Float 2 [focalFalloff]
Float 3 [focalSize]
Vector 4 [_CurveParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_CameraDepthTexture] 2D
"ps_2_0
; 14 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c5, 0.00000000, 1.00000000, 0, 0
dcl t0.xy
texld r3, t0, s0
texld r0, t0, s1
mad r0.x, r0, c0, c0.y
mov r1.x, c1
add r1.x, -c3, r1
rcp r2.x, r0.x
add r0.x, -r2, r1
add_sat r2.x, -r2, r1
cmp r0.x, r0, c5, c5.y
rcp r1.x, r1.x
mul r2.x, r2, c4
mul r1.x, r2, r1
abs_pp r0.x, r0
cmp_pp r0.x, -r0, r1, c5
mul_sat r3.w, r0.x, c2.x
mov_pp oC0, r3
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
Vector 5 [_InvRenderTargetSize]
"!!ARBvp1.0
# 8 ALU
PARAM c[6] = { { 1, -1 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0.xy, c[0];
MOV result.texcoord[0].xy, vertex.texcoord[0];
ADD result.texcoord[1].xy, vertex.texcoord[0], -c[5];
MAD result.texcoord[2].xy, R0, c[5], vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 8 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_InvRenderTargetSize]
"vs_2_0
; 8 ALU
def c5, 1.00000000, -1.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mov oT0.xy, v1
add oT1.xy, v1, -c4
mad oT2.xy, c5, r0, v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_InvRenderTargetSize]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 5 TEX
PARAM c[2] = { program.local[0],
		{ 0.2, 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R2, fragment.texcoord[2], texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[0], 2D;
MOV R0.y, c[0];
MOV R0.x, c[1].y;
MUL R0.xy, R0, c[1].z;
ADD R0.zw, fragment.texcoord[2].xyxy, R0.xyxy;
ADD R0.xy, fragment.texcoord[1], R0;
TEX R4, R0.zwzw, texture[0], 2D;
TEX R3, R0, texture[0], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R0, R0, R1;
ADD R0, R0, R2;
ADD R0, R0, R3;
ADD R0, R0, R4;
MUL result.color, R0, c[1].x;
END
# 15 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_InvRenderTargetSize]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 11 ALU, 5 TEX
dcl_2d s0
def c1, 0.00000000, 2.00000000, 0.20000000, 0
dcl t0.xy
dcl t1.xy
dcl t2.xy
texld r4, t0, s0
texld r2, t2, s0
texld r3, t1, s0
mov_pp r0.y, c0
mov_pp r0.x, c1
mul_pp r1.xy, r0, c1.y
add r0.xy, t2, r1
add r1.xy, t1, r1
add_pp r3, r4, r3
add_pp r2, r3, r2
texld r0, r0, s0
texld r1, r1, s0
add_pp r1, r2, r1
add_pp r0, r1, r0
mul_pp r0, r0, c1.z
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}