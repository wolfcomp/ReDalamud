using Hexa.NET.ImGui.Backends.SDL3;
using SDLGPUCommandBuffer = Hexa.NET.SDL3.SDLGPUCommandBuffer;
using ImSDLGPUCommandBuffer = Hexa.NET.ImGui.Backends.SDL3.SDLGPUCommandBuffer;
using SDLGPURenderPass = Hexa.NET.SDL3.SDLGPURenderPass;
using ImSDLGPURenderPass = Hexa.NET.ImGui.Backends.SDL3.SDLGPURenderPass;

namespace ReDalamud.Standalone;

public unsafe partial class ImGuiRenderer
{
    public void Render()
    {
        ImGui.Render();ImDrawData* drawData = ImGui.GetDrawData();
        bool isMinimized = drawData->DisplaySize.X <= 0 || drawData->DisplaySize.Y <= 0;

        SDLGPUCommandBuffer* commandBuffer = SDL.AcquireGPUCommandBuffer(_gpuDevice);
        ProcessTextures(commandBuffer);
        SDLGPUTexture* swapTexture;
        SDL.AcquireGPUSwapchainTexture(commandBuffer, _window, &swapTexture, null, null);

        if (swapTexture != null && !isMinimized)
        {
            ImGuiImplSDL3.SDLGPU3PrepareDrawData(drawData, (ImSDLGPUCommandBuffer*)commandBuffer);

            SDLGPUColorTargetInfo targetInfo = new()
            {
                Texture = swapTexture,
                ClearColor = new SDLFColor
                {
                    R = _clearColor.X,
                    G = _clearColor.Y,
                    B = _clearColor.Z,
                    A = _clearColor.W
                },
                LoadOp = SDLGPULoadOp.Clear,
                StoreOp = SDLGPUStoreOp.Store,
                MipLevel = 0,
                LayerOrDepthPlane = 0,
                Cycle = 0
            };

            SDLGPURenderPass* renderPass = SDL.BeginGPURenderPass(commandBuffer, &targetInfo, 1, null);
            ImGuiImplSDL3.SDLGPU3RenderDrawData(drawData, (ImSDLGPUCommandBuffer*)commandBuffer, (ImSDLGPURenderPass*)renderPass, null);
            SDL.EndGPURenderPass(renderPass);
        }

        var io = ImGui.GetIO();

        if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        {
            ImGui.UpdatePlatformWindows();
            ImGui.RenderPlatformWindowsDefault();
        }

        SDL.SubmitGPUCommandBuffer(commandBuffer);
    }

    public void NewFrame()
    {
        ImGuiImplSDL3.SDLGPU3NewFrame();
        ImGuiImplSDL3.NewFrame();
        ImGui.NewFrame();
    }
}