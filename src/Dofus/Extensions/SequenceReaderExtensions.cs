using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Dofus.Extensions
{
    internal static class SequenceReaderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRead<T>(ref this SequenceReader<T> reader, Span<T> destination) where T : unmanaged, IEquatable<T> {
            if (!reader.TryCopyTo(destination))
            {
                return false;
            }

            reader.Advance(destination.Length);
            return true;
        }
    }
}
