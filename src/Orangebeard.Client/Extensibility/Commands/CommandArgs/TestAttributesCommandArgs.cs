using Orangebeard.Shared.Execution.Metadata;
using System.Collections.Generic;

namespace Orangebeard.Shared.Extensibility.Commands.CommandArgs
{
    public class TestAttributesCommandArgs
    {
        public TestAttributesCommandArgs(ICollection<MetaAttribute> attributes)
        {
            Attributes = attributes ?? new List<MetaAttribute>();
        }

        public ICollection<MetaAttribute> Attributes { get; }
    }
}
