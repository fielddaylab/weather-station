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
					WSAnalytics w = Find.State<WSAnalytics>();
					if(w != null)
					{
						w.LogSessionStart();
					}
					
					//if(!AdjustedFrameRate)
					//{
					OVRPlugin.systemDisplayFrequency = 90.0f;
						//AdjustedFrameRate = true;
						/*XRDisplaySubsystem displaySubsystem = null;
						// Omitted null checks for brevity. You should check each line for null.
						var xrSettings = XRGeneralSettings.Instance;
						if(xrSettings != null)
						{
							XRManagerSettings xrManagerSettings = xrSettings.Manager;
							if(xrManagerSettings != null)
							{
								XRLoader xrLoader = xrManagerSettings.activeLoader;
								if(xrLoader != null)
								{
									displaySubsystem = xrLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
								}
							}
						}

						if(displaySubsystem != null)
						{
							// Get the supported refresh rates.
							// If you will save the refresh rate values for longer than this frame, pass
							// Allocator.Persistent and remember to Dispose the array when you are done with it.
							//if (displaySubsystem.TryGetSupportedDisplayRefreshRates(
							//		Unity.Collections.Allocator.Temp,
							//		out var refreshRates))
							{
								// Request a refresh rate.
								// Returns false if you request a value that is not in the refreshRates array.
								//UnityEngine.Debug.Log(refreshRates[0]);
								bool success = displaySubsystem.TryRequestDisplayRefreshRate(90f);
								if(success)
								{
									Debug.Log("SET FRAME RATE");
									AdjustedFrameRate = true;
								}
								else
								{
									Debug.Log("Didn't set refresh rate");
									AdjustedFrameRate = true;
								}
							}
						}*/
					//}
					
                    ScriptUtility.Trigger("StartGame"/*, table*/);
					
                //}
            });
			

        }
    }
}