using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Camera/Depth of Field (Lens Blur, Scatter, DX11)")]
    public class DepthOfField : PostEffectsBase
    {
        public bool visualizeFocus = false, nearBlur = false;
        public float focalLength = 10.0f, focalSize = 0.05f, aperture = 0.5f, maxBlurSize = 2.0f;
        public Transform focalTransform = null;
        public bool highResolution = false;
        public enum BlurType { DiscBlur = 0, DX11 = 1 }
        public enum BlurSampleCount { Low = 0, Medium = 1, High = 2 }
        public BlurType blurType = BlurType.DiscBlur;
        public BlurSampleCount blurSampleCount = BlurSampleCount.High;
        public float foregroundOverlap = 1.0f;
        public Shader dofHdrShader, dx11BokehShader;
        private Material dofHdrMaterial = null, dx11bokehMaterial;
        public float dx11BokehThreshold = 0.5f, dx11SpawnHeuristic = 0.0875f, dx11BokehScale = 1.2f, dx11BokehIntensity = 2.5f;

        private Camera cachedCamera;
        private float focalDistance01 = 10.0f;

        private ComputeBuffer cbDrawArgs, cbPoints;

        void ReleaseComputeResources()
        {
            cbDrawArgs?.Release();
            cbDrawArgs = null;
            cbPoints?.Release();
            cbPoints = null;
        }

        void CreateComputeResources()
        {
            cbDrawArgs ??= new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
            cbDrawArgs.SetData(new int[] { 0, 1, 0, 0 });

            cbPoints ??= new ComputeBuffer(90000, 12 + 16, ComputeBufferType.Append);
        }

        float FocalDistance01(float worldDist) =>
            cachedCamera.WorldToViewportPoint((worldDist - cachedCamera.nearClipPlane) * cachedCamera.transform.forward + cachedCamera.transform.position).z / (cachedCamera.farClipPlane - cachedCamera.nearClipPlane);

        void WriteCoc(RenderTexture fromTo, bool fgDilate)
        {
            dofHdrMaterial.SetTexture("_FgOverlap", null);

            if (nearBlur && fgDilate)
            {
                int rtW = fromTo.width / 2, rtH = fromTo.height / 2;

                RenderTexture temp2 = RenderTexture.GetTemporary(rtW, rtH, 0, fromTo.format);
                Graphics.Blit(fromTo, temp2, dofHdrMaterial, 4);

                float fgAdjustment = internalBlurWidth * foregroundOverlap;

                dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, fgAdjustment, 0.0f, fgAdjustment));
                RenderTexture temp1 = RenderTexture.GetTemporary(rtW, rtH, 0, fromTo.format);
                Graphics.Blit(temp2, temp1, dofHdrMaterial, 2);
                RenderTexture.ReleaseTemporary(temp2);

                dofHdrMaterial.SetVector("_Offsets", new Vector4(fgAdjustment, 0.0f, 0.0f, fgAdjustment));
                temp2 = RenderTexture.GetTemporary(rtW, rtH, 0, fromTo.format);
                Graphics.Blit(temp1, temp2, dofHdrMaterial, 2);
                RenderTexture.ReleaseTemporary(temp1);

                dofHdrMaterial.SetTexture("_FgOverlap", temp2);
                fromTo.MarkRestoreExpected();
                Graphics.Blit(fromTo, fromTo, dofHdrMaterial, 13);
                RenderTexture.ReleaseTemporary(temp2);
            }
            else
            {
                fromTo.MarkRestoreExpected();
                Graphics.Blit(fromTo, fromTo, dofHdrMaterial, 0);
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!CheckResources())
            {
                Graphics.Blit(source, destination);
                return;
            }

            aperture = Mathf.Max(aperture, 0.0f);
            maxBlurSize = Mathf.Max(maxBlurSize, 0.1f);
            focalSize = Mathf.Clamp(focalSize, 0.0f, 2.0f);
            internalBlurWidth = Mathf.Max(maxBlurSize, 0.0f);

            focalDistance01 = focalTransform ? (cachedCamera.WorldToViewportPoint(focalTransform.position)).z / (cachedCamera.farClipPlane) : FocalDistance01(focalLength);
            dofHdrMaterial.SetVector("_CurveParams", new Vector4(1.0f, focalSize, (1.0f / (1.0f - aperture) - 1.0f), focalDistance01));

            RenderTexture rtLow = null;

            if (visualizeFocus)
            {
                WriteCoc(source, true);
                Graphics.Blit(source, destination, dofHdrMaterial, 16);
            }
            else if (blurType == BlurType.DX11 && dx11bokehMaterial)
            {
                if (highResolution)
                {
                    internalBlurWidth = internalBlurWidth < 0.1f ? 0.1f : internalBlurWidth;
                    fgBlurDist = internalBlurWidth * foregroundOverlap;

                    rtLow = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
                    var dest2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

                    WriteCoc(source, false);

                    rtSuperLow1 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
                    rtSuperLow2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);

                    Graphics.Blit(source, rtSuperLow1, dofHdrMaterial, 15);
                    dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, 1.5f, 0.0f, 1.5f));
                    Graphics.Blit(rtSuperLow1, rtSuperLow2, dofHdrMaterial, 19);
                    dofHdrMaterial.SetVector("_Offsets", new Vector4(1.5f, 0.0f, 0.0f, 1.5f));
                    Graphics.Blit(rtSuperLow2, rtSuperLow1, dofHdrMaterial, 19);

                    Graphics.Blit(source, rtSuperLow2, dofHdrMaterial, 4);

                    dx11bokehMaterial.SetTexture("_BlurredColor", rtSuperLow1);
                    dx11bokehMaterial.SetFloat("_SpawnHeuristic", dx11SpawnHeuristic);
                    dx11bokehMaterial.SetVector("_BokehParams", new Vector4(dx11BokehScale, dx11BokehIntensity, Mathf.Clamp(dx11BokehThreshold, 0.005f, 4.0f), internalBlurWidth));
                    dx11bokehMaterial.SetTexture("_FgCocMask", nearBlur ? rtSuperLow2 : null);

                    Graphics.SetRandomWrite
                    Graphics.Blit(source, dest2, dx11bokehMaterial, 0);
                    Graphics.ClearRandomWriteTargets();

                    ReleaseComputeResources();

                    if (visualizeFocus)
                    {
                        dofHdrMaterial.SetTexture("_LowRez", dest2);
                        Graphics.Blit(source, destination, dofHdrMaterial, 16);
                    }
                    else
                    {
                        dofHdrMaterial.SetTexture("_LowRez", dest2);
                        Graphics.Blit(source, destination, dofHdrMaterial, 7);
                    }

                    RenderTexture.ReleaseTemporary(dest2);
                }
                else
                {
                    WriteCoc(source, false);

                    dx11bokehMaterial.SetTexture("_BlurredColor", source);
                    dx11bokehMaterial.SetFloat("_SpawnHeuristic", dx11SpawnHeuristic);
                    dx11bokehMaterial.SetVector("_BokehParams", new Vector4(dx11BokehScale, dx11BokehIntensity, Mathf.Clamp(dx11BokehThreshold, 0.005f, 4.0f), internalBlurWidth));
                    dx11bokehMaterial.SetTexture("_FgCocMask", nearBlur ? rtLow : null);

                    Graphics.SetRandomWriteTarget(1, cbPoints);
                    Graphics.Blit(source, destination, dx11bokehMaterial, 0);
                    Graphics.ClearRandomWriteTargets();

                    ReleaseComputeResources();
                }
            }
        }
    }
}