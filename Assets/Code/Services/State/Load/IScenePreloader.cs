using System.Collections;
using BeauUtil;

namespace WeatherStation
{
    public interface IScenePreloader
    {
        IEnumerator OnPreloadScene(SceneBinding inScene, object inContext);
    }
}