﻿using System;

namespace IL2C.Metadata
{
    public interface IVariableInformation
        : IEquatable<IVariableInformation>
    {
        int Index { get; }
        ITypeInformation TargetType { get; }
        IMethodInformation DeclaredMethod { get; }
        object HintInformation { get; }

        string UnsafeRawSymbolName { get; }
    }

    internal abstract class VariableInformation
        : IVariableInformation, IOperandPrintable
    {
        internal readonly string symbolName;

        protected VariableInformation(
            IMethodInformation declaredMethod,
            int index,
            string symbolName, 
            ITypeInformation targetType,
            object hintInformation = null)
        {
            this.Index = index;
            this.symbolName = symbolName;
            this.TargetType = targetType;
            this.DeclaredMethod = declaredMethod;
            this.HintInformation = hintInformation;
        }

        public int Index { get; }
        public ITypeInformation TargetType { get; }
        public IMethodInformation DeclaredMethod { get; }
        public object HintInformation { get; }

        public string UnsafeRawSymbolName => symbolName;

        public bool Equals(IVariableInformation rhs)
        {
            if (rhs == null)
            {
                return false;
            }

            return this.Index.Equals(rhs.Index) &&
                symbolName.Equals(rhs.UnsafeRawSymbolName) &&
                this.TargetType.Equals(rhs.TargetType) &&
                this.DeclaredMethod.Equals(rhs.DeclaredMethod);
        }

        public override bool Equals(object rhs)
        {
            return this.Equals(rhs as IVariableInformation);
        }

        public override int GetHashCode()
        {
            return
                this.Index.GetHashCode() ^
                symbolName.GetHashCode() ^
                this.TargetType.GetHashCode() ^
                this.DeclaredMethod.GetHashCode();
        }

        public override string ToString() =>
            string.Format("{0}: {1}", this.TargetType.FriendlyName, symbolName);

        string IOperandPrintable.PrintableString => symbolName;
    }
}
