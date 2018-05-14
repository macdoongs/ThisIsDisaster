using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPC {
    public class NPCModel : UnitModel {
        public NPCUnit Unit {
            get; private set;
        }

        public NPCMetaInfo MetaInfo {
            get; private set;
        }


    }

    public class NPCMetaInfo {
        public NPCModel Model {
            get; private set;
        }
    }

    public class NPCUnit
    {
        public NPCModel Model
        {
            get; private set;
        }
    }
}
