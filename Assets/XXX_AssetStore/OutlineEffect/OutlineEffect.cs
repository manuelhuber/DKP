/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR;

namespace XXX_AssetStore.OutlineEffect {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class OutlineEffect : MonoBehaviour {
        private static OutlineEffect _mInstance;

        private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();

        public bool AdditiveRendering;
        public bool addLinesBetweenColors;
        [Range(0.1f, .9f)] public float alphaCutoff = .5f;

        public bool BackfaceCulling = true;

        private CommandBuffer commandBuffer;

        [Header("These settings can affect performance!")] public bool CornerOutlines;
        [HideInInspector] public RenderTexture extraRenderTexture;
        [Range(0, 1)] public float FillAmount = 0.2f;
        public bool flipY;

        public Color LineColor0 = Color.red;
        public Color lineColor1 = Color.green;
        public Color LineColor2 = Color.blue;
        [Range(0, 10)] public float LineIntensity = .5f;

        [Range(1.0f, 6.0f)] public float LineThickness = 1.25f;

        private readonly List<Material> materialBuffer = new List<Material>();
        private Material outline1Material;
        private Material outline2Material;
        private Material outline3Material;
        private Shader outlineBufferShader;

        [HideInInspector] public Camera outlineCamera;
        private Material outlineEraseMaterial;
        private Shader outlineShader;
        [HideInInspector] public Material outlineShaderMaterial;
        [HideInInspector] public RenderTexture renderTexture;

        [Header("Advanced settings")] public bool scaleWithScreenSize = true;
        public Camera sourceCamera;

        private OutlineEffect() {
        }

        public static OutlineEffect Instance {
            get {
                if (Equals(_mInstance, null))
                    return _mInstance = FindObjectOfType(typeof(OutlineEffect)) as OutlineEffect;

                return _mInstance;
            }
        }

        private Material GetMaterialFromID(int ID) {
            if (ID == 0)
                return outline1Material;
            if (ID == 1)
                return outline2Material;
            return outline3Material;
        }

        private Material CreateMaterial(Color emissionColor) {
            var m = new Material(outlineBufferShader);
            m.SetColor("_Color", emissionColor);
            m.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            return m;
        }

        private void Awake() {
            _mInstance = this;
        }

        private void Start() {
            CreateMaterialsIfNeeded();
            UpdateMaterialsPublicProperties();

            if (sourceCamera == null) {
                sourceCamera = GetComponent<Camera>();

                if (sourceCamera == null)
                    sourceCamera = Camera.main;
            }

            if (outlineCamera == null) {
                var cameraGameObject = new GameObject("Outline Camera");
                cameraGameObject.transform.parent = sourceCamera.transform;
                outlineCamera = cameraGameObject.AddComponent<Camera>();
                outlineCamera.enabled = false;
            }

            renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16,
                RenderTextureFormat.Default);
            extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16,
                RenderTextureFormat.Default);
            UpdateOutlineCameraFromSource();

            commandBuffer = new CommandBuffer();
            outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
        }

        public void OnPreRender() {
            if (commandBuffer == null)
                return;

            CreateMaterialsIfNeeded();

            if (renderTexture == null || renderTexture.width != sourceCamera.pixelWidth ||
                renderTexture.height != sourceCamera.pixelHeight) {
                renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16,
                    RenderTextureFormat.Default);
                extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16,
                    RenderTextureFormat.Default);
                outlineCamera.targetTexture = renderTexture;
            }
            UpdateMaterialsPublicProperties();
            UpdateOutlineCameraFromSource();
            outlineCamera.targetTexture = renderTexture;
            commandBuffer.SetRenderTarget(renderTexture);

            commandBuffer.Clear();
            if (outlines != null)
                foreach (var outline in outlines) {
                    LayerMask l = sourceCamera.cullingMask;

                    if (outline != null && l == (l | (1 << outline.originalLayer)))
                        for (var v = 0; v < outline.Renderer.sharedMaterials.Length; v++) {
                            Material m = null;

                            if (outline.Renderer.sharedMaterials[v].mainTexture != null &&
                                outline.Renderer.sharedMaterials[v]) {
                                foreach (var g in materialBuffer)
                                    if (g.mainTexture == outline.Renderer.sharedMaterials[v].mainTexture)
                                        if (outline.eraseRenderer && g.color == outlineEraseMaterial.color)
                                            m = g;
                                        else if (g.color == GetMaterialFromID(outline.color).color)
                                            m = g;

                                if (m == null) {
                                    if (outline.eraseRenderer)
                                        m = new Material(outlineEraseMaterial);
                                    else
                                        m = new Material(GetMaterialFromID(outline.color));
                                    m.mainTexture = outline.Renderer.sharedMaterials[v].mainTexture;
                                    materialBuffer.Add(m);
                                }
                            } else {
                                if (outline.eraseRenderer)
                                    m = outlineEraseMaterial;
                                else
                                    m = GetMaterialFromID(outline.color);
                            }

                            if (BackfaceCulling)
                                m.SetInt("_Culling", (int) CullMode.Back);
                            else
                                m.SetInt("_Culling", (int) CullMode.Off);

                            commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, 0, 0);
                            var mL = outline.GetComponent<MeshFilter>();
                            if (mL)
                                for (var i = 1; i < mL.sharedMesh.subMeshCount; i++)
                                    commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, i, 0);
                            var sMR = outline.GetComponent<SkinnedMeshRenderer>();
                            if (sMR)
                                for (var i = 1; i < sMR.sharedMesh.subMeshCount; i++)
                                    commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, i, 0);
                        }
                }

            outlineCamera.Render();
        }

        private void OnEnable() {
            var o = FindObjectsOfType<Outline>();

            foreach (var oL in o) {
                oL.enabled = false;
                oL.enabled = true;
            }
        }

        private void OnDestroy() {
            if (renderTexture != null)
                renderTexture.Release();
            if (extraRenderTexture != null)
                extraRenderTexture.Release();
            DestroyMaterials();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);

            if (addLinesBetweenColors) {
                UnityEngine.Graphics.Blit(source, extraRenderTexture, outlineShaderMaterial, 0);
                outlineShaderMaterial.SetTexture("_OutlineSource", extraRenderTexture);
            }
            UnityEngine.Graphics.Blit(source, destination, outlineShaderMaterial, 1);
        }

        private void CreateMaterialsIfNeeded() {
            if (outlineShader == null)
                outlineShader = Resources.Load<Shader>("OutlineShader");
            if (outlineBufferShader == null) outlineBufferShader = Resources.Load<Shader>("OutlineBufferShader");
            if (outlineShaderMaterial == null) {
                outlineShaderMaterial = new Material(outlineShader);
                outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
                UpdateMaterialsPublicProperties();
            }
            if (outlineEraseMaterial == null)
                outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
            if (outline1Material == null)
                outline1Material = CreateMaterial(new Color(1, 0, 0, 0));
            if (outline2Material == null)
                outline2Material = CreateMaterial(new Color(0, 1, 0, 0));
            if (outline3Material == null)
                outline3Material = CreateMaterial(new Color(0, 0, 1, 0));
        }

        private void DestroyMaterials() {
            foreach (var m in materialBuffer)
                DestroyImmediate(m);
            materialBuffer.Clear();
            DestroyImmediate(outlineShaderMaterial);
            DestroyImmediate(outlineEraseMaterial);
            DestroyImmediate(outline1Material);
            DestroyImmediate(outline2Material);
            DestroyImmediate(outline3Material);
            outlineShader = null;
            outlineBufferShader = null;
            outlineShaderMaterial = null;
            outlineEraseMaterial = null;
            outline1Material = null;
            outline2Material = null;
            outline3Material = null;
        }

        public void UpdateMaterialsPublicProperties() {
            if (outlineShaderMaterial) {
                float scalingFactor = 1;
                if (scaleWithScreenSize) scalingFactor = Screen.height / 360.0f;

                // If scaling is too small (height less than 360 pixels), make sure you still render the outlines, but render them with 1 thickness
                if (scaleWithScreenSize && scalingFactor < 1) {
                    if (VRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None) {
                        outlineShaderMaterial.SetFloat("_LineThicknessX",
                            1 / 1000.0f * (1.0f / VRSettings.eyeTextureWidth) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY",
                            1 / 1000.0f * (1.0f / VRSettings.eyeTextureHeight) * 1000.0f);
                    } else {
                        outlineShaderMaterial.SetFloat("_LineThicknessX",
                            1 / 1000.0f * (1.0f / Screen.width) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY",
                            1 / 1000.0f * (1.0f / Screen.height) * 1000.0f);
                    }
                } else {
                    if (VRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None) {
                        outlineShaderMaterial.SetFloat("_LineThicknessX",
                            scalingFactor * (LineThickness / 1000.0f) * (1.0f / VRSettings.eyeTextureWidth) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY",
                            scalingFactor * (LineThickness / 1000.0f) * (1.0f / VRSettings.eyeTextureHeight) * 1000.0f);
                    } else {
                        outlineShaderMaterial.SetFloat("_LineThicknessX",
                            scalingFactor * (LineThickness / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY",
                            scalingFactor * (LineThickness / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
                    }
                }
                outlineShaderMaterial.SetFloat("_LineIntensity", LineIntensity);
                outlineShaderMaterial.SetFloat("_FillAmount", FillAmount);
                outlineShaderMaterial.SetColor("_LineColor1", LineColor0 * LineColor0);
                outlineShaderMaterial.SetColor("_LineColor2", lineColor1 * lineColor1);
                outlineShaderMaterial.SetColor("_LineColor3", LineColor2 * LineColor2);
                if (flipY)
                    outlineShaderMaterial.SetInt("_FlipY", 1);
                else
                    outlineShaderMaterial.SetInt("_FlipY", 0);
                if (!AdditiveRendering)
                    outlineShaderMaterial.SetInt("_Dark", 1);
                else
                    outlineShaderMaterial.SetInt("_Dark", 0);
                if (CornerOutlines)
                    outlineShaderMaterial.SetInt("_CornerOutlines", 1);
                else
                    outlineShaderMaterial.SetInt("_CornerOutlines", 0);

                Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
            }
        }

        private void UpdateOutlineCameraFromSource() {
            outlineCamera.CopyFrom(sourceCamera);
            outlineCamera.renderingPath = RenderingPath.Forward;
            outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            outlineCamera.clearFlags = CameraClearFlags.SolidColor;
            outlineCamera.rect = new Rect(0, 0, 1, 1);
            outlineCamera.cullingMask = 0;
            outlineCamera.targetTexture = renderTexture;
            outlineCamera.enabled = false;
#if UNITY_5_6_OR_NEWER
            outlineCamera.allowHDR = false;
#else
            outlineCamera.hdr = false;
#endif
        }

        public void AddOutline(Outline outline) {
            if (!outlines.Contains(outline))
                outlines.Add(outline);
        }

        public void RemoveOutline(Outline outline) {
            if (outlines.Contains(outline))
                outlines.Remove(outline);
        }
    }
}