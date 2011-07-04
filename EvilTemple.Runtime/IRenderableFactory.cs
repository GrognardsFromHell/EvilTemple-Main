namespace EvilTemple.Runtime
{
    public interface IRenderableFactory
    {

        IBackgroundMap CreateBackgroundMap();

        IModelInstance CreateModelInstance();

        IDynamicLight CreateDynamicLight();

        ISelectionCircle CreateSelectionCircle();

    }
}