using System;

namespace Perfmon.Storage
{
    public interface IStorage
    {
        void Set(DateTimeOffset timestamp, string name, double? value);
    }
}