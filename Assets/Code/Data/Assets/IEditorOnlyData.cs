namespace WeatherStation
{
    public interface IEditorOnlyData
    {
        #if UNITY_EDITOR

        void ClearEditorOnlyData();

        #endif // UNITY_EDITOR
    }
}