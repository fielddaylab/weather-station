using BeauData;

namespace WeatherStation
{
    public interface IProfileChunk : ISerializedObject
    {
        bool HasChanges();
        void MarkChangesPersisted();
        void Dump(EasyBugReporter.IDumpWriter writer);
    }
}