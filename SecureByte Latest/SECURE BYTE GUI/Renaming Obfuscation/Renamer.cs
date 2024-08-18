using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;

namespace Protections.Renamer
{
    public class Renamer
    {
        public static void Execute(Context ctx, Schemes schemes, bool prop, bool fields, bool events, bool methods, bool param, bool types)
        {
            foreach (TypeDef type in ctx.Module.Types)
            {
                if (prop)
                {
                    foreach (PropertyDef property in type.Properties)
                    {
                        if (Analyzer.CanRename(type, property))
                            property.Name = Utils.Generation(schemes);
                    }
                }
                if (fields)
                {
                    foreach (FieldDef field in type.Fields)
                    {
                        if (Analyzer.CanRename(type, field))
                            field.Name = Utils.Generation(schemes);
                    }
                }
                if (events)
                {
                    foreach (EventDef theEvent in type.Events)
                    {
                        if (Analyzer.CanRename(theEvent))
                            theEvent.Name = Utils.Generation(schemes);
                    }
                }
                if (methods)
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        if (Analyzer.CanRename(method))
                            method.Name = Utils.Generation(schemes);
                    }
                }
                if (param)
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        foreach (Parameter parameter in method.Parameters)
                        {
                            foreach (GenericParam genParam in type.GenericParameters)
                            {
                                if (Analyzer.CanRename(type, parameter))
                                    genParam.Name = Utils.Generation(schemes);
                                if (Analyzer.CanRename(type, parameter))
                                    parameter.Name = Utils.Generation(schemes);
                            }
                        }
                    }
                }
                if (types)
                {
                    if (Analyzer.CanRename(type))
                    {
                        string formNamespace = Utils.Generation(schemes);
                        string formName = Utils.Generation(schemes);
                        foreach (MethodDef method in type.Methods)
                        {
                            if (type.BaseType != null && type.BaseType.FullName.ToLower().Contains("form"))
                            {
                                foreach (Resource src in ctx.Module.Resources)
                                {
                                    if (src.Name.Contains(type.Name + ".resources"))
                                    {
                                        src.Name = formNamespace + "." + formName + ".resources";
                                    }
                                }
                            }
                            type.Namespace = formNamespace;
                            type.Name = formName;
                            if (method.Name.Equals("InitializeComponent") && method.HasBody)
                            {
                                foreach (Instruction instruction in method.Body.Instructions)
                                {
                                    if (instruction.OpCode.Equals(OpCodes.Ldstr))
                                    {
                                        string str = (string)instruction.Operand;
                                        if (str == type.Name)
                                        {
                                            instruction.Operand = formName;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}