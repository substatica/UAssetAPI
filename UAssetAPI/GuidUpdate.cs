using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAssetAPI
{
    class GuidUpdate
    {
        public string ClassName;
        public List<GuidUpdateComponent> GuidUpdateComponents;

        public GuidUpdate() { }
    }

    class GuidUpdateComponent
    {
        public string ComponentName;
        public string DummyGuid;
        public string ProductionGuid;

        public GuidUpdateComponent() { }
    }
}
