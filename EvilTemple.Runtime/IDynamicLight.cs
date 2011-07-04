using OpenTK;

namespace EvilTemple.Runtime
{
    public interface IDynamicLight : IRenderable
    {

        DynamicLightType Type { get; set; }

        float Range { get; set; }

        Vector4 Color { get; set; }

        Vector4 Direction { get; set; }

        float Attenuation { get; set; }

        float Phi { get; set; }

        float Theta { get; set; }

    }

    public enum DynamicLightType
    {
        Directional = 1,
        Point,
        Spot
    }

}
