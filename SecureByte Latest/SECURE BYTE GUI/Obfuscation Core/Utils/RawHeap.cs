using dnlib.DotNet.Writer;

namespace ICore
{
	internal class RawHeap : HeapBase
	{
		public RawHeap(string name, byte[] content)
		{
			Name = name;
			this.content = content;
		}
		public override string Name { get; }
		public override uint GetRawLength()
		{
			return (uint)content.Length;
		}
		protected override void WriteToImpl(DataWriter writer)
		{
			writer.WriteBytes(this.content);
		}
		private readonly byte[] content;
	}
}
