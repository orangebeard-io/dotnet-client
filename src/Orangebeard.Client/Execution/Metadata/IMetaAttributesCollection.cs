using System.Collections.Generic;

namespace Orangebeard.Shared.Execution.Metadata
{
    public interface IMetaAttributesCollection : ICollection<MetaAttribute>
    {
        void Add(string key, string value);
    }
}
