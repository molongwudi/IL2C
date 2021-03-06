﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IL2C
{
    public sealed class CodeTextWriter
    {
        private readonly string oneIndent;
        private int indentCount = 0;
        private string indent = string.Empty;
        private bool split = false;

        public CodeTextWriter(TextWriter tw, string indent)
        {
            this.Parent = tw;
            this.oneIndent = indent;
        }

        public TextWriter Parent { get; }

        private void UpdateIndent(int shiftCount)
        {
            split = false;
            indentCount += shiftCount;
            indent = string.Join(string.Empty, Enumerable.Range(0, indentCount).Select(_ => oneIndent));
        }

        public IDisposable Shift(int shiftCount = 1)
        {
            Debug.Assert(shiftCount != 0);

            if (split)
            {
                this.Parent.WriteLine();
                split = false;
            }
            this.UpdateIndent(shiftCount);
            return new Unshifter(this, 0 - shiftCount);
        }

        public void SplitLine()
        {
            split = true;
        }

        public void WriteLine(string message)
        {
            if (split)
            {
                this.Parent.WriteLine();
                split = false;
            }
            this.Parent.WriteLine(indent + message);
        }

        public void WriteLine(string format, params object[] args)
        {
            if (split)
            {
                this.Parent.WriteLine();
                split = false;
            }
            this.Parent.WriteLine(indent + format, args);
        }

        public void Flush()
        {
            this.Parent.Flush();
        }

        public static CodeTextWriter Create(TextWriter tw, string indent)
        {
            return new CodeTextWriter(tw, indent);
        }

        public static CodeTextWriter Create(Stream stream, string indent)
        {
            return new CodeTextWriter(new StreamWriter(stream, Encoding.UTF8), indent);
        }

        private sealed class Unshifter : IDisposable
        {
            private CodeTextWriter parent;
            private int shiftCount;

            public Unshifter(CodeTextWriter parent, int shiftCount)
            {
                this.parent = parent;
                this.shiftCount = shiftCount;
            }

            public void Dispose()
            {
                if (parent != null)
                {
                    parent.UpdateIndent(shiftCount);
                    parent = null;
                    shiftCount = 0;
                }
            }
        }
    }
}
