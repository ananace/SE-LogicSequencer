using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;

using ModParameterDefinition = VRage.MyTuple<string,string,System.Type,bool,object>;
using ModCanApplyProvider = System.Func<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, bool>;
using ModApplyProvider = System.Action<System.Collections.Generic.IEnumerable<Sandbox.ModAPI.IMyTerminalBlock>, System.Collections.Generic.Dictionary<string, object>>;

namespace LogicSequencer.API
{
    using ModParameterProvider = Func<IEnumerable<ModParameterDefinition>>;

    /**
    <summary>
        A service call parameter,
        <br/>
        This is both used to provide information in the sequence editor for what the user is expected to feed the service,
        as well as a filter and converter for what is to be actually sent to the service.
        <br/>
        Parameters will be automatically casted to their requested type.
    </summary>
    <example>
        To specify a parameter named "Red Color";<br/>
        The parameter will be provided as a <c>byte</c>, with a default value of <c>255</c> if none is provided.
        <code>
        new Parameter {
            Name = "Red Color",
            Description = "Red color value for the action",
            Type = typeof(byte),
            IsRequired = false,
            DefaultValue = 255
        }
        </code>
    </example>
    **/
    public class Parameter
    {
        /// <summary>Name of the parameter as used in the sequence</summary>
        public string Name { get; set; }
        /// <summary>Description of the parameter - shown in the editor</summary>
        public string Description { get; set; }
        /// <summary>
        /// The requested type of data that should be provided for the parameter.
        /// This has to match one of the supported script types.
        /// <br/>
        /// For an up-to-date list check <c>Script/Helper/VariableTypeExtensions.cs</c>
        /// <br/>
        /// Recommended to stick to <c>bool</c>/<c>long</c>/<c>double</c>/<c>string</c> where possible.
        /// </summary>
        public Type Type { get; set; }
        /// <summary>Should the parameter be required</summary>
        public bool IsRequired { get; set; }
        /// <summary>What default value - if any - should be injected if none is provided by the sequence</summary>
        public object DefaultValue { get; set; } = null;

        public static implicit operator ModParameterDefinition(Parameter param)
        {
            return new ModParameterDefinition(
                param.Name,
                param.Description,
                param.Type,
                param.IsRequired,
                param.DefaultValue
            );
        }
    }

    /**
    <summary>
        Service registration information. The name and description are only used to make editing nicer.
        <br/>
        Note that the provided ID must be unique, therefore it is recommended to prefix it with your mod name.
    </summary>
    **/
    public abstract class ServiceRegistration
    {
        /// <summary>Service ID, used to uniquely identiy the service.
        /// Recommended format is "[mod].[block].[action]"</summary>
        /// <example>bombmod.nuke.detonate</example>
        public abstract string ID { get; }
        /// <summary>A descriptive name for the service, only shown in the editor</summary>
        /// <example>Nuke - Detonate</example>
        public abstract string Name { get; }
        /// <summary>A short description for what effect the service will have on the affected blocks</summary>
        /// <example>Detonates any nukes in the provided list</example>
        public abstract string Description { get; }

        /// <summary>The list of accepted and required parameters for the service<br/>
        /// Refer to <see>Parameter</see> for more information.<br/>
        /// Note that this method will only be called once, and the resulting data will be cached.</summary>
        public abstract IEnumerable<Parameter> GetParameters();
        /// <summary>A method used by the editor to filter out applicable services for selected blocks.<br/>
        /// This method should be cheap to call, preferrably only checking subtypeid or gamelogic class</summary>
        public abstract bool CanApplyToAny(IEnumerable<IMyTerminalBlock> blocks);
        /// <summary>The method that will be used to apply the service onto a list of blocks.<br/>
        /// Note that this method will be given a general list of blocks, so be sure to filter out only the blocks that the service _should_ affect.</summary>
        /// <remark>The provided dictionary will only contain values for the names listed in <see>Parameters</see></remark>
        public abstract void Apply(IEnumerable<IMyTerminalBlock> blocks, Dictionary<string, object> parameters);

        public static implicit operator VRage.MyTuple<string, string, string, ModParameterProvider, ModCanApplyProvider, ModApplyProvider>(ServiceRegistration service)
        {
            return new VRage.MyTuple<string, string, string, ModParameterProvider, ModCanApplyProvider, ModApplyProvider>(
                service.ID,
                service.Name,
                service.Description,
                () => service.GetParameters().Select(p => (ModParameterDefinition)p),
                service.CanApplyToAny,
                service.Apply
            );
        }
    }
}
