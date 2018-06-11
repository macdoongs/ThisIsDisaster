using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Environment
{
    public class EnvironmentModel : UnitModel
    {
        public EnvironmentUnit Unit {
            get; private set;
        }
        
        public EnvironmentTypeInfo MetaInfo {
            get; private set;
        }

        public EnvironmentScriptBase Script {
            get; private set;
        }

        public void SetMetaInfo(EnvironmentTypeInfo info) {
            MetaInfo = info;
            InvokeScript();
        }

        void InvokeScript()
        {
            string scriptSrc = MetaInfo.Script;
            if (string.IsNullOrEmpty(scriptSrc)) {
                scriptSrc = "EnvironmentScriptBase";
            }
            scriptSrc = "Environment." + scriptSrc;
            EnvironmentScriptBase script = (EnvironmentScriptBase)System.Activator.CreateInstance(System.Type.GetType(scriptSrc));
            Script = script;
            script.SetModel(this);
        }

        public void SetUnit(EnvironmentUnit unit) {
            Unit = unit;
        }

        public override void UpdatePosition(Vector3 pos)
        {
            Unit.transform.position = pos;
        }

        public override Vector3 GetCurrentPos()
        {
            return Unit.transform.position;
        }

        public override string GetUnitName()
        {
            return MetaInfo.Name;
        }

        public override TileUnit GetCurrentTile()
        {
            return GetTileSetter().GetCurrentTile();
        }

        public override AutoTileMovementSetter GetTileSetter()
        {
            return Unit.TileSetter;
        }

        public override void SetCurrentTileForcely(TileUnit tile)
        {
            Unit.TileSetter.SetCurrentTileForcely(tile);
            tile.OnEnterTile(this);
        }
    }
}
