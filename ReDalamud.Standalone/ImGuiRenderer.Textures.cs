using HexaGen.Runtime;
using SDLGPUCommandBuffer = Hexa.NET.SDL3.SDLGPUCommandBuffer;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ReDalamud.Standalone;

public unsafe partial class ImGuiRenderer
{
    private readonly List<Tuple<Pointer<SDLGPUTexture>, Pointer<SDLSurface>>> texturesToBind = [];
    private SDLGPUTransferBuffer* transferBuffer = null!;
    private uint uploadSize = 0;

    private void CreateTransferBuffer(uint size)
    {
        if (transferBuffer != null)
            SDL.ReleaseGPUTransferBuffer(_gpuDevice, transferBuffer);
        uploadSize = size;
        var transferCreateInfo = new SDLGPUTransferBufferCreateInfo
        {
            Size = uploadSize,
            Props = 0,
            Usage = SDLGPUTransferBufferUsage.Upload
        };

        transferBuffer = SDL.CreateGPUTransferBuffer(_gpuDevice, &transferCreateInfo);
    }

    public void ProcessTextures(SDLGPUCommandBuffer* commandBuffer)
    {
        lock (texturesToBind)
        {
            var copyPass = SDL.BeginGPUCopyPass(commandBuffer);
            foreach (var (texture, surface) in texturesToBind)
            {
                var size = (uint)(surface.Handle->Pitch * surface.Handle->H);
                if (uploadSize < size)
                    CreateTransferBuffer(size);
                var transferBufferPtr = SDL.MapGPUTransferBuffer(_gpuDevice, transferBuffer, true);
                new Span<byte>(surface.Handle->Pixels, (int)size).CopyTo(new Span<byte>(transferBufferPtr, (int)size));
                SDL.UnmapGPUTransferBuffer(_gpuDevice, transferBuffer);
                var transferInfo = new SDLGPUTextureTransferInfo
                {
                    TransferBuffer = transferBuffer,
                    PixelsPerRow = 0,
                    RowsPerLayer = 0
                };
                var textureRegion = new SDLGPUTextureRegion
                {
                    Texture = texture,
                    X = 0,
                    Y = 0,
                    W = (uint)surface.Handle->W,
                    H = (uint)surface.Handle->H,
                    Z = 0,
                    D = 1,
                    Layer = 0,
                    MipLevel = 0
                };
                SDL.UploadToGPUTexture(copyPass, &transferInfo, &textureRegion, true);
            }
            SDL.EndGPUCopyPass(copyPass);
            texturesToBind.Clear();
        }
    }

    public class TextureWrap : IDisposable, IEquatable<TextureWrap>
    {
        public ImTextureRef Texture;

        private readonly ImTextureData* textureData;
        private readonly SDLGPUTexture* deviceTexture;
        private readonly SDLSurface* surface;

        public Vector2 Size => new(surface->W, surface->H);

        public TextureWrap(ReadOnlySpan<byte> data)
        {
            var ioStream = SDL.IOFromMem(Unsafe.AsPointer(in data), (nuint)data.Length);
            surface = SDLImage.LoadIO(ioStream, true);

            if (surface->Format != SDLPixelFormat.Abgr8888)
                surface = SDL.ConvertSurface(surface, SDLPixelFormat.Abgr8888);
            var createInfo = new SDLGPUTextureCreateInfo
            {
                Width = (uint)surface->W,
                Height = (uint)surface->H,
                Format = SDLGPUTextureFormat.R8G8B8A8Unorm,
                Usage = (uint)SDLGPUTextureUsageFlags.Sampler,
                LayerCountOrDepth = 1,
                NumLevels = 1,
                Type = SDLGPUTextureType.Texturetype2D,
                SampleCount = SDLGPUSampleCount.Samplecount1,
                Props = 0
            };
            deviceTexture = SDL.CreateGPUTexture(Instance._gpuDevice, &createInfo);
            textureData = (ImTextureData*)Marshal.AllocHGlobal(sizeof(ImTextureData));
            textureData->Height = surface->H;
            textureData->Width = surface->W;
            textureData->Format = ImTextureFormat.Rgba32;
            textureData->TexID = deviceTexture;
            textureData->UsedRect.H = (ushort)surface->H;
            textureData->UsedRect.W = (ushort)surface->W;
            textureData->UsedRect.X = 0;
            textureData->UsedRect.Y = 0;
            textureData->WantDestroyNextFrame = 0;
            lock (Instance.texturesToBind)
                Instance.texturesToBind.Add(Tuple.Create((Pointer<SDLGPUTexture>)deviceTexture, (Pointer<SDLSurface>)surface));

            Texture = new(textureData, deviceTexture);
        }

        public void Dispose()
        {
            SDL.ReleaseGPUTexture(Instance._gpuDevice, deviceTexture);
            Marshal.FreeHGlobal((nint)textureData);
            SDL.DestroySurface(surface);
        }

        public bool Equals(TextureWrap? other) =>
            other is not null && (ReferenceEquals(this, other) || (textureData == other.textureData &&
                                                                   deviceTexture == other.deviceTexture &&
                                                                   surface == other.surface &&
                                                                   Texture.Equals(other.Texture)));

        public override bool Equals(object? obj) =>
            obj is not null &&
            (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((TextureWrap)obj)));

        public override int GetHashCode()
        {
            return HashCode.Combine((long)textureData, (long)deviceTexture, (long)surface, Texture);
        }
    }
}