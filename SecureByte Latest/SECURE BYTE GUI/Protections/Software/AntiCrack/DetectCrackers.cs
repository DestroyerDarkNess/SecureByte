using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helpers.Injection;
using Helpers.Mutations;
using ICore;
using Protections.Mutation;
using Protections.Renamer;
using Runtime;
using SECURE_BYTE_GUI.Global_for_Obfuscation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Protections.Software
{
    class DetectCrackersYHook
    {
        public static void Inject(Context context)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiCrackingWithHook).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiCrackingWithHook).MetadataToken));
            IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.EntryPoint.DeclaringType, context.Module);
            //
            MethodDef Init = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Init");
            MethodDef DoWork = (MethodDef)source.Single((IDnlibDef method) => method.Name == "DoWork");
            MethodDef SendMSG = (MethodDef)source.Single((IDnlibDef method) => method.Name == "SendMSG");
            MethodDef Capture = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Capture");
            MethodDef uploadToImgBB = (MethodDef)source.Single((IDnlibDef method) => method.Name == "uploadToImgBB");
            MethodDef CalculateMD5Hash = (MethodDef)source.Single((IDnlibDef method) => method.Name == "CalculateMD5Hash");
            //
            var mutation = new MutationHelper("MutationClass");
            MethodDef entryPoint = context.Module.EntryPoint;
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(Init));
            mutation.InjectKey<string>(uploadToImgBB, 0, Global.Global.api);
            mutation.InjectKey<string>(DoWork, 1, Global.Global.rnd);
            mutation.InjectKey<string>(DoWork, 2, Global.Global.SIMG);
            mutation.InjectKey<string>(DoWork, 3, Global.Global.MSG);
            mutation.InjectKey<string>(DoWork, 4, Global.Global.MSGC);
            mutation.InjectKey<string>(DoWork, 5, Global.Global.Status);
            mutation.InjectKey<string>(DoWork, 7, Global.Global.ID);
            if (oGlobals.excludeforAC)
            {
                mutation.InjectKey<string>(DoWork, 6, oGlobals.excludeString);
            }
            else
            {
                mutation.InjectKey<string>(DoWork, 6, "Hi'I'm_Empty_String;')");
            }
            //          
            var meths = new MethodDef[]
            {
                DoWork, uploadToImgBB
            };           
            //
            var methstoRename = new MethodDef[]
            {
                Init, DoWork, SendMSG, Capture, uploadToImgBB, CalculateMD5Hash
            };
            Utils.MethodRenamig(methstoRename);
            Protections.Strings.stillWorkingOn2.EncodeFor(context, meths);
            MutationConfusion.executeFor(DoWork);
        }
    }
}