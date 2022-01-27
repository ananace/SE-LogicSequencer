using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;

namespace LogicSequencer.Script.Helper
{
    public static class BlockSelectorExtensions
    {
        public static IMyTerminalBlock Resolve(this BlockSelector selector, LogicProgramRun vm)
        {
            if (selector.Self ?? false)
                return vm.LogicSequencer.Block;

            var terminalSystem = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(vm.LogicSequencer.Block.CubeGrid);
            if (terminalSystem == null)
                throw new ArgumentException("Unable to find a terminal system");

            IMyTerminalBlock block = null;
            if (selector.IDSpecified)
                block = terminalSystem.GetBlockWithId(selector.ID.Value) as IMyTerminalBlock;
            if (block == null && selector.Name != null)
            {
                var name = selector.Name.Resolve(vm.Variables);
                name.ConvertToString();
                block = terminalSystem.GetBlockWithName(name.String);
            }

            return block;
        }

        public static IEnumerable<IMyTerminalBlock> Resolve(this MultiBlockSelector selector, LogicProgramRun vm)
        {
            if (selector.Blocks != null)
                return selector.Blocks.Select(s => s.Resolve(vm));

            var terminalSystem = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(vm.LogicSequencer.Block.CubeGrid);
            if (terminalSystem == null)
                throw new ArgumentException("Unable to find a terminal system");

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (selector.GroupName != null)
            {
                var groupName = selector.GroupName.Resolve(vm.Variables);
                groupName.ConvertToString();

                var group = terminalSystem.GetBlockGroupWithName(groupName.String);
                if (group == null)
                    throw new ArgumentException($"Unable to find a group with the name {selector.GroupName}");

                group.GetBlocks(blocks);
            }
            else if (selector.Name != null)
            {
                var name = selector.Name.Resolve(vm.Variables);
                name.ConvertToString();
                terminalSystem.SearchBlocksOfName(name.String, blocks);
            }
            else
                throw new ArgumentException("No applicable multiblock selector provided");

            return blocks;
        }
    }
}
