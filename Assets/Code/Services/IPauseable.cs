namespace WeatherStation
{
    public interface IPauseable
    {
        bool IsPaused();
        void Pause();
        void Resume();
    }
}