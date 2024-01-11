using BeauUtil;
using FieldDay;
using FieldDay.Scripting;

namespace WeatherStation {
    public class WeatherGame : Game {
        [InvokeOnBoot]
        static private void OnBoot() {
            Scenes.OnMainSceneReady.Register(() => {
                //using (var table = TempVarTable.Alloc()) {
                    //table.Set("someRandomValue", RNG.Instance.Next(60));
                    ScriptUtility.Trigger("GameReady"/*, table*/);
                //}
            });
        }
    }
}