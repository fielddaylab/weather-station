using BeauUtil;

namespace WeatherStation
{
    static public class GameTriggers
    {
        static public readonly StringHash32 RequestPartnerHelp = "RequestPartnerHelp";
        
        static public readonly StringHash32 TravelingToStation = "TravelingToStation";
        static public readonly StringHash32 PlayerDream = "PlayerDream";
        static public readonly StringHash32 PlayerSpecter = "PlayerSpecter";
        static public readonly StringHash32 TimelineStarted = "TimelineStarted";

        static public readonly StringHash32 InteractObject = "InteractObject";
        static public readonly StringHash32 InspectObject = "InspectObject";
        static public readonly StringHash32 Talk = "Talk";
        static public readonly StringHash32 ScenePreload = "ScenePreload";
        static public readonly StringHash32 SceneStart = "SceneStart";
        static public readonly StringHash32 SceneLeave = "SceneLeave";

        static public readonly StringHash32 BestiaryEntryAdded = "BestiaryEntryAdded";
        static public readonly StringHash32 BestiaryFactAdded = "BestiaryFactAdded";

        static public readonly StringHash32 PortableOpened = "PortableOpened";
        static public readonly StringHash32 PortableAppOpened = "PortableAppOpened";

        static public readonly StringHash32 JobStarted = "JobStarted";
        static public readonly StringHash32 JobSwitched = "JobSwitched";
        static public readonly StringHash32 JobCompleted = "JobCompleted";
        static public readonly StringHash32 JobTaskCompleted = "JobTaskCompleted";
        static public readonly StringHash32 JobTasksUpdated = "JobTasksUpdated";

        static public readonly StringHash32 UpgradeAdded = "UpgradeAdded";

        static public readonly StringHash32 PlayerExpUp = "ExpUp";
        static public readonly StringHash32 PlayerLevelUp = "LevelUp";

        static public readonly StringHash32 PlayerEnterRegion = "PlayerEnterRegion";
        static public readonly StringHash32 PlayerExitRegion = "PlayerExitRegion";

        static public readonly StringHash32 JournalHidden = "JournalHidden";
    }
}