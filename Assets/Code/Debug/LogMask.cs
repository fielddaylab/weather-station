#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif

using System;

namespace WeatherStation
{
    [Flags]
    public enum LogMask : uint
    {
        Input = 1 << 0,
        Physics = 1 << 1,
        Scripting = 1 << 2,
        Modeling = 1 << 3,
        Audio = 1 << 4,
        Loading = 1 << 5,
        Camera = 1 << 6,
        DataService = 1 << 7,
        UI = 1 << 8,
        Experimentation = 1 << 9,
        Observation = 1 << 10,
        Argumentation = 1 << 11,
        Localization = 1 << 12,
        Time = 1 << 13,

        DEFAULT = Loading | DataService | Time,
        ALL = Input | Physics | Scripting | Modeling | Audio | Loading | Camera
            | DataService | UI | Experimentation | Observation | Argumentation | Localization | Time
    }
}