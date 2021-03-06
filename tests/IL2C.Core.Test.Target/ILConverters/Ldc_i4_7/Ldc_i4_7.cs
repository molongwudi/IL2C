using System.Runtime.CompilerServices;

namespace IL2C.ILConverters
{
    [TestCase(7, "RawValue")]
    [TestCase(11, "Add", 4)]
    public sealed class Ldc_i4_7
    {
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int RawValue();

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Add(int num);
    }
}
