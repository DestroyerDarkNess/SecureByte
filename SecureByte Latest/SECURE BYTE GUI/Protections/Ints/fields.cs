using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Protections.Extra
{
	internal class fields
	{
		public static void ProcessMethod(MethodDef method)
		{
			bool isConstructor = method.IsConstructor;
			bool flag5 = !isConstructor;
			if (flag5)
			{
				bool flag = !method.HasBody;
				bool flag6 = !flag;
				if (flag6)
				{
					bool flag2 = !method.Body.HasInstructions;
					bool flag7 = !flag2;
					if (flag7)
					{
						method.Body.SimplifyMacros(method.Parameters);
						for (int i = 0; i < method.Body.Instructions.Count; i++)
						{
							Local local = method.Body.Instructions[i].Operand as Local;
							bool flag3 = local != null;
							bool flag8 = flag3;
							if (flag8)
							{
								bool flag4 = !convertedLocals.ContainsKey(local);
								bool flag9 = flag4;
								FieldDef fieldDef;
								if (flag9)
								{
									fieldDef = new FieldDefUser(ICore.Utils.MethodsRenamig(), new FieldSig(local.Type), FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static);
									method.Module.GlobalType.Fields.Add(fieldDef);
									convertedLocals.Add(local, fieldDef);
								}
								else
								{
									fieldDef = convertedLocals[local];
								}
								OpCode opCode = null;
								Code code = method.Body.Instructions[i].OpCode.Code;
								Code code2 = code;
								switch (code2)
								{
								case Code.Ldloc_0:
								case Code.Ldloc_1:
								case Code.Ldloc_2:
								case Code.Ldloc_3:
								case Code.Ldloc_S:
									goto IL_224;
								case Code.Stloc_0:
								case Code.Stloc_1:
								case Code.Stloc_2:
								case Code.Stloc_3:
								case Code.Stloc_S:
									goto IL_238;
								case Code.Ldarg_S:
								case Code.Ldarga_S:
								case Code.Starg_S:
									break;
								case Code.Ldloca_S:
									goto IL_22E;
								default:
									switch (code2)
									{
									case Code.Ldloc:
										goto IL_224;
									case Code.Ldloca:
										goto IL_22E;
									case Code.Stloc:
										goto IL_238;
									}
									break;
								}
								IL_1E5:
								method.Body.Instructions[i].OpCode = opCode;
								method.Body.Instructions[i].Operand = fieldDef;
								goto IL_242;
								IL_224:
								opCode = OpCodes.Ldsfld;
								goto IL_1E5;
								IL_22E:
								opCode = OpCodes.Ldsflda;
								goto IL_1E5;
								IL_238:
								opCode = OpCodes.Stsfld;
								goto IL_1E5;
							}
							IL_242:;
						}
						convertedLocals.ToList().ForEach(delegate(KeyValuePair<Local, FieldDef> x)
						{
							method.Body.Variables.Remove(x.Key);
						});
						convertedLocals = new Dictionary<Local, FieldDef>();
					}
				}
			}
		}
		public static void protect(ModuleDef md)
		{
            foreach(var t in md.GetTypes())
                foreach(var m in t.Methods)
                {
                    if (m.HasBody)
                    {
                        ProcessMethod(m);
                    }
                }
		}
		private static Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
	}
}
