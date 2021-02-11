using Umbraco.Core;
using Umbraco.Core.Composing;

namespace OSKI.solutions.MediaRemove
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MediaRemoveComposer:IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<MediaRemoveService>();
            composition.Components().Append<ServerRoutes>();
        }
    }
}
