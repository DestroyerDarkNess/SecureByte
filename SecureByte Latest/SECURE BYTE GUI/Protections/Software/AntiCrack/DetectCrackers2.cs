using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helpers.Mutations;
using ICore;
using Protections.Mutation;
using Runtime;
using SECURE_BYTE_GUI.Global_for_Obfuscation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Protections.Software
{
    class DetectCrackersNHook
    {
        public static void Inject(Context context)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiCracking).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiCracking).MetadataToken));
            IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.EntryPoint.DeclaringType, context.Module);
            MethodDef Init = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Init");
            MethodDef DoWork = (MethodDef)source.Single((IDnlibDef method) => method.Name == "DoWork");
            MethodDef entryPoint = context.Module.EntryPoint;
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(Init));
            var mutation = new MutationHelper("MutationClass");
            mutation.InjectKey<string>(DoWork, 8, Global.Global.Status);
            mutation.InjectKey<string>(DoWork, 9, Global.Global.MSG);
            mutation.InjectKey<string>(DoWork, 10, Global.Global.MSGC);
            if (oGlobals.excludeforAC)
            {
                mutation.InjectKey<string>(DoWork, 11, oGlobals.excludeString);
            }
            else
            {
                mutation.InjectKey<string>(DoWork, 11, "Hi'I'm_Empty_String;')");
            }
            //
            var meths = new MethodDef[]
{
                DoWork
};
            var methstoRename = new MethodDef[]
           {
                Init, DoWork
           };
            Utils.MethodRenamig(methstoRename);
            Protections.Strings.stillWorkingOn2.EncodeFor(context, meths);
            MutationConfusion.executeFor(DoWork);
        }
    }
}