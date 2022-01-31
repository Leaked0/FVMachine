using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace FVM
{
	// Token: 0x02000002 RID: 2
	public class Context
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Context(string path)
		{
			this.Path = path;
			this.Module = ModuleDefMD.Load(path, null);
			this.Initializer = Utils.CreateMethod(this.Module);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002080 File Offset: 0x00000280
		public void Save()
		{
			MethodDef cctor = this.Module.GlobalType.FindOrCreateStaticConstructor();
			cctor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(this.Initializer));
			string output = this.Path.Replace(".exe", "-fvm.exe");
			ModuleWriterOptions writerOptions = new ModuleWriterOptions(this.Module);
			writerOptions.Logger = DummyLogger.NoThrowInstance;
			writerOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
			this.Module.Write(output, writerOptions);
		}

		// Token: 0x04000001 RID: 1
		public string Path;

		// Token: 0x04000002 RID: 2
		public ModuleDefMD Module;

		// Token: 0x04000003 RID: 3
		public MethodDef Initializer;
	}
}
