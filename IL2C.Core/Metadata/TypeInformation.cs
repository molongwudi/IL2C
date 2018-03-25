﻿using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace IL2C.Metadata
{
    public interface ITypeInformation : IMemberInformation
    {
        bool IsValidDefinition { get; }

        bool IsPublic { get; }
        bool IsNestedPublic { get; }
        bool IsNestedFamily { get; }
        bool IsNestedFamilyOrAssembly { get; }

        bool IsValueType { get; }
        bool IsClass { get; }
        bool IsInterface { get; }
        bool IsEnum { get; }
        bool IsDelegate { get; }
        bool IsByReference { get; }
        bool IsPointer { get; }
        bool IsPrimitive { get; }
        bool IsNumericPrimitive { get; }

        bool IsVoidType { get; }
        bool IsObjectType { get; }
        bool IsValueTypeType { get; }
        bool IsDelegateType { get; }
        bool IsBooleanType { get; }
        bool IsByteType { get; }
        bool IsSByteType { get; }
        bool IsInt16Type { get; }
        bool IsUInt16Type { get; }
        bool IsInt32Type { get; }
        bool IsUInt32Type { get; }
        bool IsInt64Type { get; }
        bool IsUInt64Type { get; }
        bool IsIntPtrType { get; }
        bool IsUIntPtrType { get; }
        bool IsCharType { get; }
        bool IsStringType { get; }

        ITypeInformation BaseType { get; }
        ITypeInformation ElementType { get; }
        ITypeInformation[] InterfaceTypes { get; }
        ITypeInformation[] NestedTypes { get; }

        ITypeInformation StackableType { get; }

        IFieldInformation[] Fields { get; }
        IMethodInformation[] DeclaredMethods { get; }
        IMethodInformation[] OverridedMethods { get; }
        IMethodInformation[] VirtualMethods { get; }

        string CLanguageDeclaration { get; }

        bool IsAssignableFrom(ITypeInformation rhs);

        ITypeInformation MakeByReference();
    }

    internal sealed class TypeInformation
        : MemberInformation<TypeReference, TypeDefinition>
        , ITypeInformation
    {
        private readonly Lazy<TypeInformation> baseType;
        private readonly Lazy<TypeInformation> elementType;
        private readonly Lazy<TypeInformation[]> interfaceTypes;
        private readonly Lazy<TypeInformation[]> nestedTypes;
        private readonly Lazy<ITypeInformation> stackableType;
        private readonly Lazy<FieldInformation[]> fields;
        private readonly Lazy<MethodInformation[]> declaredMethods;
        private readonly Lazy<MethodInformation[]> overridedMethods;
        private readonly Lazy<MethodInformation[]> virtualMethods;

        public TypeInformation(TypeReference type, ModuleInformation module)
            : base(type, module)
        {
            baseType = this.Context.LazyGetOrAddMember(
                () => this.Definition.BaseType,
                baseType => new TypeInformation(baseType, module));
            elementType = this.Context.LazyGetOrAddMember(
                () => this.Member.GetElementType(),
                elementType => new TypeInformation(elementType, module));
            interfaceTypes = this.Context.LazyGetOrAddMember(
                () => this.Definition.Interfaces.Select(interfaceImpl => interfaceImpl.InterfaceType),
                interfaceType => new TypeInformation(interfaceType, module));
            nestedTypes = this.Context.LazyGetOrAddMember(
                () => this.Definition.NestedTypes,
                nestedType => new TypeInformation(nestedType, module));
            stackableType = Lazy.Create(() =>
                {
                    if (this.IsByteType
                        || this.IsSByteType
                        || this.IsInt16Type
                        || this.IsUInt16Type
                        || this.IsUInt32Type
                        || this.IsBooleanType
                        || this.IsCharType)
                    {
                        return this.Context.Int32Type;
                    }

                    if (this.IsUInt64Type)
                    {
                        return this.Context.Int64Type;
                    }

                    if (this.IsUIntPtrType)
                    {
                        return this.Context.IntPtrType;
                    }

                    return this;
                });
            fields = this.Context.LazyGetOrAddMember(
                () => this.Definition.Fields,
                field => new FieldInformation(field, module));
            declaredMethods = this.Context.LazyGetOrAddMember(
                () => this.Definition.Methods,
                method => new MethodInformation(method, module));
            overridedMethods = this.Context.LazyGetOrAddMember(
                () => this.EnumerateOverridedMethods(),
                method => new MethodInformation(method, module));
            virtualMethods = this.Context.LazyGetOrAddMember(
                () => this.EnumerateOverridedMethods().Where(method => method.IsVirtual),
                method => new MethodInformation(method, module));
        }

        public override string MemberTypeName => this.IsEnum
            ? "Enum"
            : this.IsValueType
                ? "Struct"
                : this.IsInterface
                ? "Interface"
                : "Class";

        public bool IsValidDefinition =>
            (((this.Member.IsValueType || this.Definition.IsClass)
                && (this.Definition.BaseType != null))
             || this.Definition.IsInterface)
            && (this.Member.Name != "<Module>");

        public bool IsPublic => this.Definition.IsPublic;
        public bool IsNestedPublic => this.Definition.IsNestedPublic;
        public bool IsNestedFamily => this.Definition.IsNestedFamily;
        public bool IsNestedFamilyOrAssembly => this.Definition.IsNestedFamilyOrAssembly;

        public bool IsValueType => this.Member.IsValueType;
        public bool IsClass => this.Definition.IsClass;
        public bool IsInterface => this.Definition.IsInterface;
        public bool IsEnum => this.Definition.IsEnum;
        public bool IsDelegate => this.Context.DelegateType.IsAssignableFrom(this);
        public bool IsByReference => this.Member.IsByReference;
        public bool IsPointer => this.Member.IsPointer;
        public bool IsPrimitive => this.Member.IsPrimitive;
        public bool IsNumericPrimitive => this.Member.IsPrimitive
            && (this.Equals(this.Context.ByteType)
            || this.Equals(this.Context.SByteType)
            || this.Equals(this.Context.Int16Type)
            || this.Equals(this.Context.UInt16Type)
            || this.Equals(this.Context.Int32Type)
            || this.Equals(this.Context.UInt32Type)
            || this.Equals(this.Context.Int64Type)
            || this.Equals(this.Context.UInt64Type)
            || this.Equals(this.Context.IntPtrType)
            || this.Equals(this.Context.UIntPtrType)
            || this.Equals(this.Context.CharType));

        public bool IsVoidType => this.Equals(this.Context.VoidType);
        public bool IsObjectType => this.Equals(this.Context.ObjectType);
        public bool IsValueTypeType => this.Equals(this.Context.ValueTypeType);
        public bool IsDelegateType => this.Equals(this.Context.DelegateType);

        public bool IsByteType => this.Equals(this.Context.ByteType);
        public bool IsSByteType => this.Equals(this.Context.SByteType);
        public bool IsInt16Type => this.Equals(this.Context.Int16Type);
        public bool IsUInt16Type => this.Equals(this.Context.UInt16Type);
        public bool IsInt32Type => this.Equals(this.Context.Int32Type);
        public bool IsUInt32Type => this.Equals(this.Context.UInt32Type);
        public bool IsInt64Type => this.Equals(this.Context.Int64Type);
        public bool IsUInt64Type => this.Equals(this.Context.UInt64Type);
        public bool IsIntPtrType => this.Equals(this.Context.IntPtrType);
        public bool IsUIntPtrType => this.Equals(this.Context.UIntPtrType);
        public bool IsCharType => this.Equals(this.Context.CharType);
        public bool IsStringType => this.Equals(this.Context.StringType);
        public bool IsBooleanType => this.Equals(this.Context.BooleanType);

        public ITypeInformation BaseType => baseType.Value;
        public ITypeInformation ElementType => elementType.Value;
        public ITypeInformation[] InterfaceTypes => interfaceTypes.Value;
        public ITypeInformation[] NestedTypes => nestedTypes.Value;
        public ITypeInformation StackableType => stackableType.Value;

        public IFieldInformation[] Fields => fields.Value;
        public IMethodInformation[] DeclaredMethods => declaredMethods.Value;
        public IMethodInformation[] OverridedMethods => overridedMethods.Value;
        public IMethodInformation[] VirtualMethods => virtualMethods.Value;

        public string CLanguageDeclaration =>
            (!this.Member.IsValueType || this.Member.IsByReference || this.Member.IsPointer)
                ? string.Format("{0}*", this.MangledName)
                : this.MangledName;

        public bool IsAssignableFrom(ITypeInformation rhs)
        {
            if (this.Equals(rhs))
            {
                return true;
            }

            // BaseClass <-- DerivedClass
            if (this.IsClass && rhs.IsClass)
            {
                return rhs
                    .Traverse(type => type.BaseType, true)
                    .Any(this.Equals);
            }

            // System.Object <-- Any
            if (this.IsObjectType)
            {
                return true;
            }

            // System.ValueType <-- ValueType
            if (this.IsValueTypeType && rhs.IsValueType)
            {
                return true;
            }

            // IBase <-- DerivedClass
            // IBase <-- IDerived
            if (this.IsInterface)
            {
                return rhs.InterfaceTypes.Any(this.Equals);
            }

            return false;
        }

        protected override TypeDefinition OnResolve(TypeReference member)
        {
            return member.Resolve();
        }

        private IEnumerable<MethodDefinition> EnumerateOverridedMethods()
        {
            var results = new List<MethodDefinition>();

            foreach (var type in ((ITypeInformation)this)
                .Traverse(type => (type.IsValueType || type.IsClass)
                    ? type.BaseType
                    // TODO: Traverse interface inheritance

                    // HACK: Interface metadata not contains basic System.Object methods (ex: ToString, Equals...)
                    //   But we have to support for invoking these methods by callvirt opcode.
                    //   IL2C applies easy way to interface type has pseudo these method entries.
                    //   This code fragment makes it.
                    : this.Context.ObjectType)
                .Reverse())
            {
                foreach (var method in this.Definition.Methods
                    .Where(method => !method.IsStatic && !method.IsConstructor)  // Only instance methods
                    .OrderByParameters())   // Make stable in current type.
                {
                    // Not newslot: method become override
                    if (!method.IsNewSlot)
                    {
                        // Override rule:
                        //   1. Method name is same
                        //   2. Method parameter types are same
                        var index = results
                            .FindIndex(md =>
                                (md.Name == method.Name)
                                && (md.Parameters
                                    .Select(p => p.ParameterType)
                                    .SequenceEqual(
                                        method.Parameters.Select(p => p.ParameterType))));
                        if (index >= 0)
                        {
                            // Found: override
                            results[index] = method;
                        }
                        else
                        {
                            // Not fount: append new method
                            results.Add(method);
                        }
                    }
                    else
                    {
                        // Newslot: force append
                        results.Add(method);
                    }
                }
            }

            return results;
        }

        public ITypeInformation MakeByReference()
        {
            return this.Context.GetOrAddMember(
                this.Definition.MakeByReferenceType(),
                type => new TypeInformation(type, this.DeclaringModule));
        }
    }
}