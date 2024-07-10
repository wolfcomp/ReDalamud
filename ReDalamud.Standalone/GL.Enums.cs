﻿namespace ReDalamud.Standalone;

public static partial class GL
{
    [Flags]
    public enum ClearBufferMask
    {
        DepthBufferBit = 0x00000100,
        AccumBufferBit = 0x00000200,
        StencilBufferBit = 0x00000400,
        ColorBufferBit = 0x00004000,
    }

    public enum EnableCap
    {
        LineSmooth = 0x0B20,
        PolygonSmooth = 0x0B41,
        CullFace = 0x0B44,
        DepthTest = 0x0B71,
        StencilTest = 0x0B90,
        Dither = 0x0BD0,
        Blend = 0x0BE2,
        IndexLogicOp = 0x0BF1,
        ColorLogicOp = 0x0BF2,
        ScissorTest = 0x0C11,
        AutoNormal = 0x0D80,
        Map1Color4 = 0x0D90,
        Map1Index = 0x0D91,
        Map1Normal = 0x0D92,
        Map1TextureCoord1 = 0x0D93,
        Map1TextureCoord2 = 0x0D94,
        Map1TextureCoord3 = 0x0D95,
        Map1TextureCoord4 = 0x0D96,
        Map1Vertex3 = 0x0D97,
        Map1Vertex4 = 0x0D98,
        Map2Color4 = 0x0DB0,
        Map2Index = 0x0DB1,
        Map2Normal = 0x0DB2,
        Map2TextureCoord1 = 0x0DB3,
        Map2TextureCoord2 = 0x0DB4,
        Map2TextureCoord3 = 0x0DB5,
        Map2TextureCoord4 = 0x0DB6,
        Map2Vertex3 = 0x0DB7,
        Map2Vertex4 = 0x0DB8,
        Texture1D = 0x0DE0,
        Texture2D = 0x0DE1,
        PolygonOffsetPoint = 0x2A01,
        PolygonOffsetLine = 0x2A02,
        ClipPlane0 = 0x3000,
        ClipPlane1 = 0x3001,
        ClipPlane2 = 0x3002,
        ClipPlane3 = 0x3003,
        ClipPlane4 = 0x3004,
        ClipPlane5 = 0x3005,
        Convolution1D = 0x8010,
        Convolution1DExt = 0x8010,
        Convolution2D = 0x8011,
        Convolution2DExt = 0x8011,
        Separable2D = 0x8012,
        Separable2DExt = 0x8012,
        Histogram = 0x8024,
        HistogramExt = 0x8024,
        MinmaxExt = 0x802E,
        PolygonOffsetFill = 0x8037,
        RescaleNormalExt = 0x803A,
        Texture3DExt = 0x806F,
        VertexArray = 0x8074,
        NormalArray = 0x8075,
        ColorArray = 0x8076,
        IndexArray = 0x8077,
        TextureCoordArray = 0x8078,
        EdgeFlagArray = 0x8079,
        InterlaceSgix = 0x8094,
        Multisample = 0x809D,
        SampleAlphaToCoverage = 0x809E,
        SampleAlphaToMaskSgis = 0x809E,
        SampleAlphaToOne = 0x809F,
        SampleAlphaToOneSgis = 0x809F,
        SampleCoverage = 0x80A0,
        SampleMaskSgis = 0x80A0,
        TextureColorTableSgi = 0x80BC,
        ColorTable = 0x80D0,
        ColorTableSgi = 0x80D0,
        PostConvolutionColorTable = 0x80D1,
        PostConvolutionColorTableSgi = 0x80D1,
        PostColorMatrixColorTable = 0x80D2,
        PostColorMatrixColorTableSgi = 0x80D2,
        Texture4DSgis = 0x8134,
        PixelTexGenSgix = 0x8139,
        SpriteSgix = 0x8148,
        ReferencePlaneSgix = 0x817D,
        IrInstrument1Sgix = 0x817F,
        CalligraphicFragmentSgix = 0x8183,
        FramezoomSgix = 0x818B,
        FogOffsetSgix = 0x8198,
        SharedTexturePaletteExt = 0x81FB,
        AsyncHistogramSgix = 0x832C,
        PixelTextureSgis = 0x8353,
        AsyncTexImageSgix = 0x835C,
        AsyncDrawPixelsSgix = 0x835D,
        AsyncReadPixelsSgix = 0x835E,
        FragmentLightingSgix = 0x8400,
        FragmentColorMaterialSgix = 0x8401,
        FragmentLight0Sgix = 0x840C,
        FragmentLight1Sgix = 0x840D,
        FragmentLight2Sgix = 0x840E,
        FragmentLight3Sgix = 0x840F,
        FragmentLight4Sgix = 0x8410,
        FragmentLight5Sgix = 0x8411,
        FragmentLight6Sgix = 0x8412,
        FragmentLight7Sgix = 0x8413,
        ColorSum = 0x8458,
        SecondaryColorArray = 0x845E,
        TextureCubeMap = 0x8513,
        ProgramPointSize = 0x8642,
        VertexProgramPointSize = 0x8642,
        DepthClamp = 0x864F,
        TextureCubeMapSeamless = 0x884F,
        PointSprite = 0x8861,
        RasterizerDiscard = 0x8C89,
        FramebufferSrgb = 0x8DB9,
        SampleMask = 0x8E51,
        PrimitiveRestart = 0x8F9D,
    }

    public enum BlendEquationMode
    {
        FuncAdd = 0x8006,
        Min = 0x8007,
        Max = 0x8008,
        FuncSubtract = 0x800A,
        FuncReverseSubtract = 0x800B,
    }

    public enum BlendingFactorDest
    {
        Zero = 0,
        SrcColor = 0x0300,
        OneMinusSrcColor = 0x0301,
        SrcAlpha = 0x0302,
        OneMinusSrcAlpha = 0x0303,
        DstAlpha = 0x0304,
        OneMinusDstAlpha = 0x0305,
        DstColor = 0x0306,
        OneMinusDstColor = 0x0307,
        ConstantColor = 0x8001,
        ConstantColorExt = 0x8001,
        OneMinusConstantColor = 0x8002,
        OneMinusConstantColorExt = 0x8002,
        ConstantAlpha = 0x8003,
        ConstantAlphaExt = 0x8003,
        OneMinusConstantAlpha = 0x8004,
        OneMinusConstantAlphaExt = 0x8004,
        One = 1,
    }

    public enum BlendingFactorSrc
    {
        Zero = 0,
        SrcAlpha = 0x0302,
        OneMinusSrcAlpha = 0x0303,
        DstAlpha = 0x0304,
        OneMinusDstAlpha = 0x0305,
        DstColor = 0x0306,
        OneMinusDstColor = 0x0307,
        SrcAlphaSaturate = 0x0308,
        ConstantColor = 0x8001,
        ConstantColorExt = 0x8001,
        OneMinusConstantColor = 0x8002,
        OneMinusConstantColorExt = 0x8002,
        ConstantAlpha = 0x8003,
        ConstantAlphaExt = 0x8003,
        OneMinusConstantAlpha = 0x8004,
        OneMinusConstantAlphaExt = 0x8004,
        One = 1,
    }

    public enum StringName
    {
        Vendor = 0x1F00,
        Renderer = 0x1F01,
        Version = 0x1F02,
        Extensions = 0x1F03,
        ShadingLanguageVersion = 0x8B8C,
    }

    public enum ShaderParameter
    {
        ShaderType = 0x8B4F,
        DeleteStatus = 0x8B80,
        CompileStatus = 0x8B81,
        InfoLogLength = 0x8B84,
        ShaderSourceLength = 0x8B88,
    }

    public enum ShaderType
    {
        FragmentShader = 0x8B30,
        VertexShader = 0x8B31,
        GeometryShader = 0x8DD9,
        TessControlShader = 0x8E88,
        TessEvaluationShader = 0x8E87,
        ComputeShader = 0x91B9
    }

    public enum ProgramParameter
    {
        ActiveUniformBlockMaxNameLength = 0x8A35,
        ActiveUniformBlocks = 0x8A36,
        DeleteStatus = 0x8B80,
        LinkStatus = 0x8B82,
        ValidateStatus = 0x8B83,
        InfoLogLength = 0x8B84,
        AttachedShaders = 0x8B85,
        ActiveUniforms = 0x8B86,
        ActiveUniformMaxLength = 0x8B87,
        ActiveAttributes = 0x8B89,
        ActiveAttributeMaxLength = 0x8B8A,
        TransformFeedbackVaryingMaxLength = 0x8C76,
        TransformFeedbackBufferMode = 0x8C7F,
        TransformFeedbackVaryings = 0x8C83,
        GeometryVerticesOut = 0x8DDA,
        GeometryInputType = 0x8DDB,
        GeometryOutputType = 0x8DDC,
    }

    public enum ActiveAttribType
    {
        Float = 0x1406,
        FloatVec2 = 0x8B50,
        FloatVec3 = 0x8B51,
        FloatVec4 = 0x8B52,
        FloatMat2 = 0x8B5A,
        FloatMat3 = 0x8B5B,
        FloatMat4 = 0x8B5C,
    }

    public enum ActiveUniformType
    {
        Int = 0x1404,
        Float = 0x1406,
        FloatVec2 = 0x8B50,
        FloatVec3 = 0x8B51,
        FloatVec4 = 0x8B52,
        IntVec2 = 0x8B53,
        IntVec3 = 0x8B54,
        IntVec4 = 0x8B55,
        Bool = 0x8B56,
        BoolVec2 = 0x8B57,
        BoolVec3 = 0x8B58,
        BoolVec4 = 0x8B59,
        FloatMat2 = 0x8B5A,
        FloatMat3 = 0x8B5B,
        FloatMat4 = 0x8B5C,
        Sampler1D = 0x8B5D,
        Sampler2D = 0x8B5E,
        Sampler3D = 0x8B5F,
        SamplerCube = 0x8B60,
        Sampler1DShadow = 0x8B61,
        Sampler2DShadow = 0x8B62,
        Sampler2DRect = 0x8B63,
        Sampler2DRectShadow = 0x8B64,
        FloatMat2x3 = 0x8B65,
        FloatMat2x4 = 0x8B66,
        FloatMat3x2 = 0x8B67,
        FloatMat3x4 = 0x8B68,
        FloatMat4x2 = 0x8B69,
        FloatMat4x3 = 0x8B6A,
        Sampler1DArray = 0x8DC0,
        Sampler2DArray = 0x8DC1,
        SamplerBuffer = 0x8DC2,
        Sampler1DArrayShadow = 0x8DC3,
        Sampler2DArrayShadow = 0x8DC4,
        SamplerCubeShadow = 0x8DC5,
        UnsignedIntVec2 = 0x8DC6,
        UnsignedIntVec3 = 0x8DC7,
        UnsignedIntVec4 = 0x8DC8,
        IntSampler1D = 0x8DC9,
        IntSampler2D = 0x8DCA,
        IntSampler3D = 0x8DCB,
        IntSamplerCube = 0x8DCC,
        IntSampler2DRect = 0x8DCD,
        IntSampler1DArray = 0x8DCE,
        IntSampler2DArray = 0x8DCF,
        IntSamplerBuffer = 0x8DD0,
        UnsignedIntSampler1D = 0x8DD1,
        UnsignedIntSampler2D = 0x8DD2,
        UnsignedIntSampler3D = 0x8DD3,
        UnsignedIntSamplerCube = 0x8DD4,
        UnsignedIntSampler2DRect = 0x8DD5,
        UnsignedIntSampler1DArray = 0x8DD6,
        UnsignedIntSampler2DArray = 0x8DD7,
        UnsignedIntSamplerBuffer = 0x8DD8,
        Sampler2DMultisample = 0x9108,
        IntSampler2DMultisample = 0x9109,
        UnsignedIntSampler2DMultisample = 0x910A,
        Sampler2DMultisampleArray = 0x910B,
        IntSampler2DMultisampleArray = 0x910C,
        UnsignedIntSampler2DMultisampleArray = 0x910D,
    }

    public enum BufferTarget
    {
        ArrayBuffer = 0x8892,
        ElementArrayBuffer = 0x8893,
        PixelPackBuffer = 0x88EB,
        PixelUnpackBuffer = 0x88EC,
        UniformBuffer = 0x8A11,
        TextureBuffer = 0x8C2A,
        TransformFeedbackBuffer = 0x8C8E,
        CopyReadBuffer = 0x8F36,
        CopyWriteBuffer = 0x8F37,
        DrawIndirectBuffer = 0x8F3F,
        AtomicCounterBuffer = 0x92C0,
        DispatchIndirectBuffer = 0x90EE,
        QueryBuffer = 0x9192,
        ShaderStorageBuffer = 0x90D2,
    }

    public enum TextureTarget
    {
        Texture1D = 0x0DE0,
        Texture2D = 0x0DE1,
        Texture3D = 0x806F,
        Texture1DArray = 0x8C18,
        Texture2DArray = 0x8C1A,
        TextureRectangle = 0x84F5,
        TextureCubeMap = 0x8513,
        TextureCubeMapPositiveX = 0x8515,
        TextureCubeMapNegativeX = 0x8516,
        TextureCubeMapPositiveY = 0x8517,
        TextureCubeMapNegativeY = 0x8518,
        TextureCubeMapPositiveZ = 0x8519,
        TextureCubeMapNegativeZ = 0x851A,
        TextureCubeMapArray = 0x9009,
        Texture2DMultisample = 0x9100,
        Texture2DMultisampleArray = 0x9102
    }

    public enum BeginMode
    {
        Points = 0x0000,
        Lines = 0x0001,
        LineLoop = 0x0002,
        LineStrip = 0x0003,
        Triangles = 0x0004,
        TriangleStrip = 0x0005,
        TriangleFan = 0x0006,
        LinesAdjacency = 0xA,
        LineStripAdjacency = 0xB,
        TrianglesAdjacency = 0xC,
        TriangleStripAdjacency = 0xD,
        Patches = 0xE,
        [Obsolete("OpenGL 4 Core does not support quads.")]
        Quads = 0x0007,
        [Obsolete("OpenGL 4 Core does not support quads.")]
        QuadStrip = 0x0008
    }

    public enum DrawElementsType
    {
        UnsignedByte = 0x1401,
        UnsignedShort = 0x1403,
        UnsignedInt = 0x1405,
    }

    public enum VertexAttribPointerType
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        Double = 0x140A,
        HalfFloat = 0x140B,
        UnsignedUInt2101010Reversed = 0x8368,
        UnsignedInt2101010Reversed = 0x8D9F,
        UnsignedUInt101111Reversed = 0x8C3B
    }

    public enum BufferUsageHint
    {
        StreamDraw = 0x88E0,
        StreamRead = 0x88E1,
        StreamCopy = 0x88E2,
        StaticDraw = 0x88E4,
        StaticRead = 0x88E5,
        StaticCopy = 0x88E6,
        DynamicDraw = 0x88E8,
        DynamicRead = 0x88E9,
        DynamicCopy = 0x88EA,
    }

    public enum PixelFormat
    {
        ColorIndex = 0x1900,
        StencilIndex = 0x1901,
        DepthComponent = 0x1902,
        Red = 0x1903,
        Green = 0x1904,
        Blue = 0x1905,
        Alpha = 0x1906,
        Rgb = 0x1907,
        Rgba = 0x1908,
        Luminance = 0x1909,
        LuminanceAlpha = 0x190A,
        AbgrExt = 0x8000,
        CmykExt = 0x800C,
        CmykaExt = 0x800D,
        Bgr = 0x80E0,
        Bgra = 0x80E1,
        Ycrcb422Sgix = 0x81BB,
        Ycrcb444Sgix = 0x81BC,
        Rg = 0x8227,
        RgInteger = 0x8228,
        DepthStencil = 0x84F9,
        RedInteger = 0x8D94,
        GreenInteger = 0x8D95,
        BlueInteger = 0x8D96,
        AlphaInteger = 0x8D97,
        RgbInteger = 0x8D98,
        RgbaInteger = 0x8D99,
        BgrInteger = 0x8D9A,
        BgraInteger = 0x8D9B,
    }

    public enum PixelInternalFormat
    {
        DepthComponent = 0x1902,
        Alpha = 0x1906,
        Rgb = 0x1907,
        Rgba = 0x1908,
        Luminance = 0x1909,
        LuminanceAlpha = 0x190A,
        R3G3B2 = 0x2A10,
        Alpha4 = 0x803B,
        Alpha8 = 0x803C,
        Alpha12 = 0x803D,
        Alpha16 = 0x803E,
        Luminance4 = 0x803F,
        Luminance8 = 0x8040,
        Luminance12 = 0x8041,
        Luminance16 = 0x8042,
        Luminance4Alpha4 = 0x8043,
        Luminance6Alpha2 = 0x8044,
        Luminance8Alpha8 = 0x8045,
        Luminance12Alpha4 = 0x8046,
        Luminance12Alpha12 = 0x8047,
        Luminance16Alpha16 = 0x8048,
        Intensity = 0x8049,
        Intensity4 = 0x804A,
        Intensity8 = 0x804B,
        Intensity12 = 0x804C,
        Intensity16 = 0x804D,
        Rgb2Ext = 0x804E,
        Rgb4 = 0x804F,
        Rgb5 = 0x8050,
        Rgb8 = 0x8051,
        Rgb10 = 0x8052,
        Rgb12 = 0x8053,
        Rgb16 = 0x8054,
        Rgba2 = 0x8055,
        Rgba4 = 0x8056,
        Rgb5A1 = 0x8057,
        Rgba8 = 0x8058,
        Rgb10A2 = 0x8059,
        Rgba12 = 0x805A,
        Rgba16 = 0x805B,
        DualAlpha4Sgis = 0x8110,
        DualAlpha8Sgis = 0x8111,
        DualAlpha12Sgis = 0x8112,
        DualAlpha16Sgis = 0x8113,
        DualLuminance4Sgis = 0x8114,
        DualLuminance8Sgis = 0x8115,
        DualLuminance12Sgis = 0x8116,
        DualLuminance16Sgis = 0x8117,
        DualIntensity4Sgis = 0x8118,
        DualIntensity8Sgis = 0x8119,
        DualIntensity12Sgis = 0x811A,
        DualIntensity16Sgis = 0x811B,
        DualLuminanceAlpha4Sgis = 0x811C,
        DualLuminanceAlpha8Sgis = 0x811D,
        QuadAlpha4Sgis = 0x811E,
        QuadAlpha8Sgis = 0x811F,
        QuadLuminance4Sgis = 0x8120,
        QuadLuminance8Sgis = 0x8121,
        QuadIntensity4Sgis = 0x8122,
        QuadIntensity8Sgis = 0x8123,
        DepthComponent16 = 0x81a5,
        DepthComponent16Sgix = 0x81A5,
        DepthComponent24 = 0x81a6,
        DepthComponent24Sgix = 0x81A6,
        DepthComponent32 = 0x81a7,
        DepthComponent32Sgix = 0x81A7,
        CompressedRed = 0x8225,
        CompressedRg = 0x8226,
        R8 = 0x8229,
        R16 = 0x822A,
        Rg8 = 0x822B,
        Rg16 = 0x822C,
        R16f = 0x822D,
        R32f = 0x822E,
        Rg16f = 0x822F,
        Rg32f = 0x8230,
        R8i = 0x8231,
        R8ui = 0x8232,
        R16i = 0x8233,
        R16ui = 0x8234,
        R32i = 0x8235,
        R32ui = 0x8236,
        Rg8i = 0x8237,
        Rg8ui = 0x8238,
        Rg16i = 0x8239,
        Rg16ui = 0x823A,
        Rg32i = 0x823B,
        Rg32ui = 0x823C,
        CompressedRgbS3tcDxt1Ext = 0x83F0,
        CompressedRgbaS3tcDxt1Ext = 0x83F1,
        CompressedRgbaS3tcDxt3Ext = 0x83F2,
        CompressedRgbaS3tcDxt5Ext = 0x83F3,
        CompressedAlpha = 0x84E9,
        CompressedLuminance = 0x84EA,
        CompressedLuminanceAlpha = 0x84EB,
        CompressedIntensity = 0x84EC,
        CompressedRgb = 0x84ED,
        CompressedRgba = 0x84EE,
        DepthStencil = 0x84F9,
        Rgba32f = 0x8814,
        Rgb32f = 0x8815,
        Rgba16f = 0x881A,
        Rgb16f = 0x881B,
        Depth24Stencil8 = 0x88F0,
        R11fG11fB10f = 0x8C3A,
        Rgb9E5 = 0x8C3D,
        Srgb = 0x8C40,
        Srgb8 = 0x8C41,
        SrgbAlpha = 0x8C42,
        Srgb8Alpha8 = 0x8C43,
        SluminanceAlpha = 0x8C44,
        Sluminance8Alpha8 = 0x8C45,
        Sluminance = 0x8C46,
        Sluminance8 = 0x8C47,
        CompressedSrgb = 0x8C48,
        CompressedSrgbAlpha = 0x8C49,
        CompressedSluminance = 0x8C4A,
        CompressedSluminanceAlpha = 0x8C4B,
        CompressedSrgbS3tcDxt1Ext = 0x8C4C,
        CompressedSrgbAlphaS3tcDxt1Ext = 0x8C4D,
        CompressedSrgbAlphaS3tcDxt3Ext = 0x8C4E,
        CompressedSrgbAlphaS3tcDxt5Ext = 0x8C4F,
        DepthComponent32f = 0x8CAC,
        Depth32fStencil8 = 0x8CAD,
        Rgba32ui = 0x8D70,
        Rgb32ui = 0x8D71,
        Rgba16ui = 0x8D76,
        Rgb16ui = 0x8D77,
        Rgba8ui = 0x8D7C,
        Rgb8ui = 0x8D7D,
        Rgba32i = 0x8D82,
        Rgb32i = 0x8D83,
        Rgba16i = 0x8D88,
        Rgb16i = 0x8D89,
        Rgba8i = 0x8D8E,
        Rgb8i = 0x8D8F,
        Float32UnsignedInt248Rev = 0x8DAD,
        CompressedRedRgtc1 = 0x8DBB,
        CompressedSignedRedRgtc1 = 0x8DBC,
        CompressedRgRgtc2 = 0x8DBD,
        CompressedSignedRgRgtc2 = 0x8DBE,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
    }

    public enum PixelStoreParameter
    {
        UnpackSwapBytes = 0x0CF0,
        UnpackLsbFirst = 0x0CF1,
        UnpackRowLength = 0x0CF2,
        UnpackSkipRows = 0x0CF3,
        UnpackSkipPixels = 0x0CF4,
        UnpackAlignment = 0x0CF5,
        PackSwapBytes = 0x0D00,
        PackLsbFirst = 0x0D01,
        PackRowLength = 0x0D02,
        PackSkipRows = 0x0D03,
        PackSkipPixels = 0x0D04,
        PackAlignment = 0x0D05,
        PackSkipImages = 0x806B,
        PackSkipImagesExt = 0x806B,
        PackImageHeight = 0x806C,
        PackImageHeightExt = 0x806C,
        UnpackSkipImages = 0x806D,
        UnpackSkipImagesExt = 0x806D,
        UnpackImageHeight = 0x806E,
        UnpackImageHeightExt = 0x806E,
        PackSkipVolumesSgis = 0x8130,
        PackImageDepthSgis = 0x8131,
        UnpackSkipVolumesSgis = 0x8132,
        UnpackImageDepthSgis = 0x8133,
        PixelTileWidthSgix = 0x8140,
        PixelTileHeightSgix = 0x8141,
        PixelTileGridWidthSgix = 0x8142,
        PixelTileGridHeightSgix = 0x8143,
        PixelTileGridDepthSgix = 0x8144,
        PixelTileCacheSizeSgix = 0x8145,
        PackResampleSgix = 0x842C,
        UnpackResampleSgix = 0x842D,
        PackSubsampleRateSgix = 0x85A0,
        UnpackSubsampleRateSgix = 0x85A1,
    }

    public enum TextureParameterName
    {
        TextureBaseLevel = 0x813C,
        TextureBorderColor = 0x1004,
        TextureCompareMode = 0x884C,
        TextureCompareFunc = 0x884D,
        TextureLodBias = 0x8501,
        TextureMagFilter = 0x2800,
        TextureMaxLevel = 0x813D,
        TextureMaxLod = 0x813B,
        TextureMinFilter = 0x2801,
        TextureMinLod = 0x813A,
        TextureSwizzleR = 0x8E42,
        TextureSwizzleG = 0x8E43,
        TextureSwizzleB = 0x8E44,
        TextureSwizzleA = 0x8E45,
        TextureSwizzleRGBA = 0x8E46,
        TextureWrapS = 0x2802,
        TextureWrapT = 0x2803,
        TextureWrapR = 0x8072,
        MaxAnisotropyExt = 0x84FE
    }

    public enum TextureParameter
    {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipMapNearest = 0x2700,
        LinearMipMapNearest = 0x2701,
        NearestMipMapLinear = 0x2702,
        LinearMipMapLinear = 0x2703,
        ClampToEdge = 0x812F,
        ClampToBorder = 0x812D,
        MirrorClampToEdge = 0x8743,
        MirroredRepeat = 0x8370,
        Repeat = 0x2901,
        Red = 0x1903,
        Green = 0x1904,
        Blue = 0x1905,
        Alpha = 0x1906,
        Zero = 0,
        One = 1,
        CompareRefToTexture = 0x884E,
        None = 0,
        StencilIndex = 0x1901,
        DepthComponent = 0x1902,
        MaxAnisotropyExt = 0x84FE
    }

    public enum PixelType
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        HalfFloat = 0x140B,
        Bitmap = 0x1A00,
        UnsignedByte332 = 0x8032,
        UnsignedByte332Ext = 0x8032,
        UnsignedShort4444 = 0x8033,
        UnsignedShort4444Ext = 0x8033,
        UnsignedShort5551 = 0x8034,
        UnsignedShort5551Ext = 0x8034,
        UnsignedInt8888 = 0x8035,
        UnsignedInt8888Ext = 0x8035,
        UnsignedInt1010102 = 0x8036,
        UnsignedInt1010102Ext = 0x8036,
        UnsignedByte233Reversed = 0x8362,
        UnsignedShort565 = 0x8363,
        UnsignedShort565Reversed = 0x8364,
        UnsignedShort4444Reversed = 0x8365,
        UnsignedShort1555Reversed = 0x8366,
        UnsignedInt8888Reversed = 0x8367,
        UnsignedInt2101010Reversed = 0x8368,
        UnsignedInt248 = 0x84FA,
        UnsignedInt10F11F11FRev = 0x8C3B,
        UnsignedInt5999Rev = 0x8C3E,
        Float32UnsignedInt248Rev = 0x8DAD,
    }
}