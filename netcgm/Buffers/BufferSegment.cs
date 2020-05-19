using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Buffers
{
    internal class BufferSegment<T> : ReadOnlySequenceSegment<T>
    {
        public T[] Array { get; }

        public BufferSegment(T[] memory, int start, int length)
        {
            Array = memory;
            Memory = new Memory<T>(memory, start, length);
        }

        public BufferSegment<T> Append(T[] memory, int start, int length)
        {
            var segment = new BufferSegment<T>(memory, start, length)
            {
                RunningIndex = RunningIndex + Memory.Length
            };
            Next = segment;
            return segment;
        }
    }
}
