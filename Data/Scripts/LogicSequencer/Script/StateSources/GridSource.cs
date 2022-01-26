using Sandbox.ModAPI;

namespace LogicSequencer.Script.StateSources
{
    public class GridMassSource : ScriptStateSource
    {
        public override string ID => "grid.mass";
        public override string Name => "Grid - Mass";
        public override string Description => "Gets the grid mass in kg";
        public override VariableType ResultType => VariableType.Real;
        public override bool SelfOnly => true;

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            return true;
        }

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            return new ScriptValue { Real = block.CubeGrid.Physics.Mass };
        }
    }

    public class GridDynamicSource : ScriptStateSource
    {
        public override string ID => "grid.is_dynamic";
        public override string Name => "Grid - Is Dynamic";
        public override string Description => "Checks if the grid is dynamic (not static)";
        public override VariableType ResultType => VariableType.Boolean;
        public override bool SelfOnly => true;

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            return true;
        }

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            return new ScriptValue { Boolean = !block.CubeGrid.IsStatic };
        }
    }

    public class GridLargeSource : ScriptStateSource
    {
        public override string ID => "grid.is_large";
        public override string Name => "Grid - Is Large";
        public override string Description => "Checks if the grid is large (2.5m)";
        public override VariableType ResultType => VariableType.Boolean;
        public override bool SelfOnly => true;

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            return true;
        }

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            return new ScriptValue { Boolean = block.CubeGrid.GridSizeEnum == VRage.Game.MyCubeSize.Large };
        }
    }

    public class GridSmallSource : ScriptStateSource
    {
        public override string ID => "grid.is_small";
        public override string Name => "Grid - Is Small";
        public override string Description => "Checks if the grid is small (0.5m)";
        public override VariableType ResultType => VariableType.Boolean;

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            return true;
        }

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            return new ScriptValue { Boolean = block.CubeGrid.GridSizeEnum == VRage.Game.MyCubeSize.Small };
        }
    }

    public class GridStaticSource : ScriptStateSource
    {
        public override string ID => "grid.is_static";
        public override string Name => "Grid - Is Static";
        public override string Description => "Checks if the grid is static";
        public override VariableType ResultType => VariableType.Boolean;

        public override bool CanReadFrom(IMyTerminalBlock block)
        {
            return true;
        }

        public override ScriptValue Read(IMyTerminalBlock block)
        {
            return new ScriptValue { Boolean = block.CubeGrid.IsStatic };
        }
    }
}
