﻿using System.Linq;

using Mono.Cecil;

using IL2C.Metadata;
using IL2C.Translators;

namespace IL2C.Writers
{
    internal static class FunctionWriter
    {
        private static void InternalConvertFromFunction(
            CodeTextWriter tw,
            IExtractContextHost extractContext,
            PreparedMethodInformation preparedMethod,
            DebugInformationOptions debugInformationOption)
        {
            var locals = preparedMethod.Method.LocalVariables;

            tw.SplitLine();
            tw.WriteLine("///////////////////////////////////////");
            tw.WriteLine(
                "// [3] {0}{1}",
                preparedMethod.Method.IsVirtual ? "Virtual: " : string.Empty,
                preparedMethod.Method.FriendlyName);

            var codeStream = preparedMethod.Method.CodeStream;

            // Write exception filters:
            if (codeStream.ExceptionHandlers.Length >= 1)
            {
                tw.SplitLine();
                tw.WriteLine("//-------------------");
                tw.WriteLine("// [3-1] Exception filters:");
                tw.SplitLine();

                for (var handlerIndex = 0;
                    handlerIndex < codeStream.ExceptionHandlers.Length;
                    handlerIndex++)
                {
                    var handler = codeStream.ExceptionHandlers[handlerIndex];

                    var filterName = string.Format(
                        "{0}_ExceptionFilter{1}__",
                        preparedMethod.Method.CLanguageFunctionName,
                        handlerIndex);
                    tw.WriteLine(
                        "static int16_t {0}(System_Exception* ex)",
                        filterName);
                    tw.WriteLine("{");

                    using (var _ = tw.Shift())
                    {
                        tw.WriteLine("il2c_assert(ex != NULL);");

                        for (var catchHandlerIndex = 0;
                            catchHandlerIndex < handler.CatchHandlers.Length;
                            catchHandlerIndex++)
                        {
                            var catchHandler = handler.CatchHandlers[catchHandlerIndex];
                            if (catchHandler.CatchHandlerType == ExceptionCatchHandlerTypes.Catch)
                            {
                                tw.WriteLine(
                                    "if (il2c_isinst__(ex, il2c_typeof({0}))) return {1};",
                                    catchHandler.CatchType.MangledName,
                                    catchHandlerIndex + 1);
                            }
                        }

                        // Write finally block index if contains.
                        var finallyHandler = handler.CatchHandlers.
                            Select((catchHandler, index) => new { catchHandler, index }).
                            FirstOrDefault(entry => entry.catchHandler.CatchHandlerType == ExceptionCatchHandlerTypes.Finally);
                        if (finallyHandler != null)
                        {
                            tw.WriteLine("return IL2C_FILTER_FINALLY;  // Not matched (will go to finally)");
                        }
                        else
                        {
                            tw.WriteLine("return IL2C_FILTER_NOMATCH;  // Not matched");
                        }
                    }

                    tw.WriteLine("}");
                }
            }

            // Start function:
            tw.SplitLine();
            tw.WriteLine("//-------------------");
            tw.WriteLine("// [3-2] Function body:");
            tw.SplitLine();
            tw.WriteLine(preparedMethod.Method.CLanguageFunctionPrototype);
            tw.WriteLine("{");

            using (var _ = tw.Shift())
            {
                if (!preparedMethod.Method.IsStatic)
                {
                    tw.WriteLine("il2c_assert(this__ != NULL);");
                    tw.SplitLine();
                }

                var localDefinitions = preparedMethod.Method.LocalVariables.
                    Where(local => !local.TargetType.IsReferenceType).
                    ToArray();
                if (localDefinitions.Length >= 1)
                {
                    tw.WriteLine("//-------------------");
                    tw.WriteLine("// [3-3] Local variables (not objref):");
                    tw.SplitLine();

                    foreach (var local in localDefinitions)
                    {
                        // HACK: The local variables mark to "volatile."
                        //   Because the gcc misread these variables calculated statically (or maybe assigned to the registers)
                        //   at compile time with optimization.
                        //   It will cause the strange results for exception handling (with sjlj.)
                        tw.WriteLine(
                            "{0}{1} {2};",
                            (codeStream.ExceptionHandlers.Length >= 1) ? "volatile " : string.Empty,
                            local.TargetType.CLanguageTypeName,
                            extractContext.GetSymbolName(local));
                    }

                    tw.SplitLine();
                }

                var stackDefinitions = preparedMethod.Stacks.
                    Where(stack => !stack.TargetType.IsReferenceType).
                    ToArray();
                if (stackDefinitions.Length >= 1)
                {
                    tw.WriteLine("//-------------------");
                    tw.WriteLine("// [3-4] Evaluation stacks (not objref):");
                    tw.SplitLine();

                    foreach (var stack in stackDefinitions)
                    {
                        tw.WriteLine(
                            "{0} {1};",
                            stack.TargetType.CLanguageTypeName,
                            extractContext.GetSymbolName(stack));
                    }

                    tw.SplitLine();
                }

                var objRefEntries = locals.
                    Concat(preparedMethod.Stacks).
                    Where(v => v.TargetType.IsReferenceType).  // Only objref
                    ToArray();
                if (objRefEntries.Length >= 1)
                {
                    tw.WriteLine("//-------------------");
                    tw.WriteLine("// [3-5] Setup execution frame:");
                    tw.SplitLine();

                    tw.WriteLine(
                        "struct {0}_EXECUTION_FRAME__",
                        preparedMethod.Method.CLanguageFunctionName);
                    tw.WriteLine("{");

                    using (var __ = tw.Shift())
                    {
                        tw.WriteLine("uint8_t objRefCount__;");
                        tw.WriteLine("uint8_t objRefRefCount__;");
                        tw.WriteLine("IL2C_EXECUTION_FRAME* pNext__;");

                        foreach (var objRefEntry in objRefEntries)
                        {
                            tw.WriteLine(
                                "{0} {1};",
                                objRefEntry.TargetType.CLanguageTypeName,
                                extractContext.GetSymbolName(objRefEntry));
                        }
                    }

                    // Important NULL assigner (p = NULL):
                    //   Because these variables are pointer (of object reference 'O' type).
                    //   So GC will traverse these variables just setup the stack frame.
                    // TODO: https://github.com/kekyo/IL2C/issues/12
                    tw.WriteLine("}} frame__ = {{ {0}, 0 }};", objRefEntries.Length);
                    tw.WriteLine("il2c_link_execution_frame(&frame__);");
                    tw.SplitLine();
                }

                tw.WriteLine("//-------------------");
                tw.WriteLine("// [3-6] IL body:");
                tw.SplitLine();

                // Set symbol prefix to make valid access variables.
                using (var __ = extractContext.BeginLocalVariablePrefix(
                    local => local.TargetType.IsReferenceType ? "frame__." : null))
                {
                    // Construct exception handler controller.
                    var exceptionHandlerController = new ExceptionHandlerController(
                        codeStream.ExceptionHandlers,
                        (handler, handlerIndex, nestedIndex) =>
                        {
                            var nestedIndexName = string.Format("nest{0}", nestedIndex);
                            extractContext.SetNestedExceptionFrameIndexName(nestedIndexName);

                            // Reached try block:
                            var filterName = string.Format(
                                "{0}_ExceptionFilter{1}__",
                                preparedMethod.Method.CLanguageFunctionName,
                                handlerIndex);
                            tw.WriteLine("il2c_try({0}, {1})", nestedIndexName, filterName);
                            tw.WriteLine("{");
                            tw.Shift();
                        },
                        (handler, handlerIndex, nestedIndex) =>
                        {
                            // Reached try end block:
                            tw.Shift(-1);
                            tw.WriteLine("}");
                        },
                        (handler, handlerIndex, nestedIndex, catchHandler, catchHandlerIndex) =>
                        {
                            var nestedIndexName = extractContext.GetExceptionNestedFrameIndexName();
                            switch (catchHandler.CatchHandlerType)
                            {
                                case ExceptionCatchHandlerTypes.Catch:
                                    // Reached catch block:
                                    tw.WriteLine(
                                        "il2c_catch({0}, {1}, {2})  // catch ({3})",
                                        nestedIndexName,
                                        catchHandlerIndex + 1,
                                        extractContext.GetSymbolName(preparedMethod.CatchVariables[catchHandler.CatchStart]),
                                        catchHandler.CatchType.MangledName);
                                    break;
                                case ExceptionCatchHandlerTypes.Finally:
                                    // Reached finally block:
                                    tw.WriteLine("il2c_finally({0})", nestedIndexName);
                                    break;
                            }
                            tw.WriteLine("{");
                            tw.Shift();
                        },
                        (handler, handlerIndex, nestedIndex, catchHandler, catchHandlerIndex) =>
                        {
                            // Reached catch end block:
                            tw.Shift(-1);
                            tw.WriteLine("}");
                        },
                        (handler, handlerIndex, nestedIndex, parentHandler, parentHandlerIndex, parentNestedIndex) =>
                        {
                            var nestedIndexName = extractContext.GetExceptionNestedFrameIndexName();
                            var parentNestedIndexName = (parentNestedIndex >= 0) ? string.Format("nest{0}", parentNestedIndex) : null;

                            // Write leave bind expressions if needed.
                            // Extract the continuation fromOffset inside at mostly inner exception handler.
                            var bindEntries =
                                preparedMethod.LeaveContinuations.
                                SelectMany(entry => codeStream.ExceptionHandlers.
                                    // nested exception handlers: inner --> outer
                                    Reverse().
                                    // Is this handler contains leave continuation target?
                                    Where(h => entry.Value.fromOffsets.Any(offset => h.ContainsOffset(offset))).
                                    // Found.
                                    Select(h => new { handler = h, continuationIndex = entry.Key, entry.Value.targetOffset })).
                                // ... is current handler?
                                Where(entry => entry.handler.Equals(handler)).
                                ToArray();
                            if (bindEntries.Length >= 1)
                            {
                                tw.WriteLine("il2c_leave_to({0})", nestedIndexName);
                                tw.WriteLine("{");
                                using (var ___ = tw.Shift())
                                {
                                    foreach (var bind in bindEntries)
                                    {
                                        if ((parentNestedIndex < 0) ||
                                            codeStream.ExceptionHandlers[parentNestedIndex].ContainsOffset(bind.targetOffset))
                                        {
                                            var labelName = preparedMethod.LabelNames[bind.targetOffset];
                                            tw.WriteLine(
                                                "il2c_leave_bind({0}, {1}, {2});",
                                                nestedIndexName,
                                                bind.continuationIndex,
                                                labelName);
                                        }
                                        else
                                        {
                                            tw.WriteLine(
                                                "il2c_leave_through({0}, {1}, {2});",
                                                nestedIndexName, bind.continuationIndex, parentNestedIndexName);
                                        }
                                    }
                                }
                                tw.WriteLine("}");
                            }

                            // Reached end of entire try block.
                            tw.WriteLine("il2c_end_try({0});", nestedIndexName);

                            extractContext.SetNestedExceptionFrameIndexName(parentNestedIndexName);
                        });

                    // Traverse code fragments.
                    var canWriteSequencePoint = true;
                    foreach (var ci in codeStream)
                    {
                        // 1: Update the exception handler controller.
                        //    (Will write exception related sentences.)
                        exceptionHandlerController.Update(ci);

                        // 2: Write label if available and used.
                        if (preparedMethod.LabelNames.TryGetValue(ci.Offset, out var labelName))
                        {
                            using (var ___ = tw.Shift(-1))
                            {
                                tw.WriteLine("{0}:", labelName);
                            }
                        }

                        // 3: Write the line preprocessor directive if available.
                        if (canWriteSequencePoint && ci.Debug.Any())
                        {
                            var sp = ci.Debug.First();
                            switch (debugInformationOption)
                            {
                                case DebugInformationOptions.Full:
                                    tw.Parent.WriteLine(
                                        "#line {0} \"{1}\"",
                                        sp.Line,
                                        sp.Path.Replace("\\", "\\\\"));
                                    break;
                                case DebugInformationOptions.CommentOnly:
                                    tw.Parent.WriteLine(
                                        "/* {0}({1}): */",
                                        sp.Path.Replace("\\", "\\\\"),
                                        sp.Line);
                                    break;
                            }

                            canWriteSequencePoint = false;
                        }

                        // 4: Generate source code fragments and write.
                        if (debugInformationOption != DebugInformationOptions.None)
                        {
                            // Write debugging information.
                            tw.WriteLine(
                                "/* {0} */",
                                ci);
                        }

                        var sourceCodes = preparedMethod.Generators[ci.Offset](extractContext);
                        foreach (var sourceCode in sourceCodes)
                        {
                            // Dirty hack:
                            //   Write unlink execution frame code if cause exiting method.
                            if (sourceCode.StartsWith("return")
                                && (objRefEntries.Length >= 1))
                            {
                                tw.WriteLine(
                                    "il2c_unlink_execution_frame(&frame__);");
                            }

                            tw.WriteLine(
                                "{0};",
                                sourceCode);

                            canWriteSequencePoint = true;
                        }
                    }

                    if (!exceptionHandlerController.IsFinished)
                    {
                        throw new InvalidProgramSequenceException(
                            "Invalid exception handler range. MethodName={0}, ExceptionHandlers=[{1}]",
                            preparedMethod.Method.FriendlyName,
                            string.Join(
                                ",",
                                codeStream.ExceptionHandlers.
                                    Select(handler => string.Format("[{0}]", handler))));
                    }
                }
            }

            tw.WriteLine("}");
            tw.SplitLine();
        }

        private static void InternalConvertFromDelegateFunction(
            CodeTextWriter tw,
            IMethodInformation method)
        {
            tw.WriteLine("///////////////////////////////////////");
            tw.WriteLine("// [4] Abstract: {0}", method.FriendlyName);
            tw.SplitLine();

            tw.WriteLine(method.CLanguageFunctionPrototype);
            tw.WriteLine("{");

            using (var _ = tw.Shift())
            {
                tw.WriteLine(
                    "// WARNING: Pure virtual function called.");
                tw.WriteLine(
                    "//TODO: throw : assert(0);");

                if (method.ReturnType.IsVoidType == false)
                {
                    tw.WriteLine(
                        "return ({0}){1};",
                        method.ReturnType.CLanguageTypeName,
                        method.ReturnType.IsNumericPrimitive ? "0" : "NULL");
                }
            }

            tw.WriteLine("}");
            tw.SplitLine();
        }

        private static void InternalConvertFromAbstractFunction(
            CodeTextWriter tw,
            IMethodInformation method)
        {
            tw.WriteLine("///////////////////////////////////////");
            tw.WriteLine("// [5] Abstract: {0}", method.FriendlyName);
            tw.SplitLine();

            tw.WriteLine(method.CLanguageFunctionPrototype);
            tw.WriteLine("{");

            using (var _ = tw.Shift())
            {
                tw.WriteLine(
                    "// WARNING: Pure virtual function called.");
                tw.WriteLine(
                    "// TODO: throw : assert(0);");

                if (method.ReturnType.IsVoidType == false)
                {
                    tw.WriteLine(
                        "return ({0}){1};",
                        method.ReturnType.CLanguageTypeName,
                        method.ReturnType.IsNumericPrimitive ? "0" : "NULL");
                }
            }

            tw.WriteLine("}");
            tw.SplitLine();
        }

        private static void InternalConvertFromPInvokeFunction(
            CodeTextWriter tw,
            IMethodInformation method,
            PInvokeInfo pinvokeInfo)
        {
            tw.WriteLine("///////////////////////////////////////");
            tw.WriteLine("// [6] P/Invoke: {0}", method.FriendlyName);
            tw.SplitLine();

            tw.WriteLine(method.CLanguageFunctionPrototype);
            tw.WriteLine("{");

            using (var _ = tw.Shift())
            {
                var arguments = string.Join(
                    ", ",
                    method.Parameters.
                        Select(parameter => parameter.GetMarshaledInExpression()));

                if (method.ReturnType.IsVoidType)
                {
                    tw.WriteLine("{0}({1});", pinvokeInfo.EntryPoint, arguments);
                }
                else
                {
                    tw.WriteLine("return {0}({1});", pinvokeInfo.EntryPoint, arguments);
                }
            }

            tw.WriteLine("}");
            tw.SplitLine();
        }

        private static void InternalConvertFromDelegateInvoker(
            CodeTextWriter tw,
            IExtractContext extractContext,
            IMethodInformation method)
        {
            if (method.Parameters.Length == 0)
            {
                throw new InvalidProgramSequenceException(
                    "Invalid delegate invoker. Name={0}",
                    method.FriendlyName);
            }

            tw.WriteLine("///////////////////////////////////////");
            tw.WriteLine("// [11-2] Delegate invoker: {0}", method.FriendlyName);
            tw.SplitLine();

            // DIRTY:
            //   Cause undefined symbol error at C compilation if "System.Delegate" type on the mscorlib assembly
            //   contains the fields with different symbol name.

            var thisName = method.Parameters[0].ParameterName;

            tw.WriteLine(method.CLanguageFunctionPrototype);
            tw.WriteLine("{");

            using (var _ = tw.Shift())
            {
                tw.WriteLine(
                    "il2c_assert({0} != NULL);",
                    thisName);
                tw.WriteLine(
                    "il2c_assert({0}->vptr0__ == &System_Delegate_VTABLE__);",
                    thisName);
                tw.WriteLine(
                    "il2c_assert({0}->count__ >= 1);",
                    thisName);
                tw.SplitLine();

                if (!method.ReturnType.IsVoidType)
                {
                    if (method.ReturnType.IsReferenceType)
                    {
                        tw.WriteLine(
                            "struct {0}_EXECUTION_FRAME__",
                            method.CLanguageFunctionName);
                        tw.WriteLine("{");
                        using (var __ = tw.Shift())
                        {
                            tw.WriteLine("uint8_t objRefCount__;");
                            tw.WriteLine("uint8_t objRefRefCount__;");
                            tw.WriteLine("IL2C_EXECUTION_FRAME* pNext__;");
                            tw.WriteLine(
                                "{0} result;",
                                method.ReturnType.CLanguageTypeName);
                        }
                        tw.WriteLine("} frame__ = { 1, 0 };");
                        tw.WriteLine("il2c_link_execution_frame(&frame__);");
                    }
                    else
                    {
                        tw.WriteLine(
                            "{0} result;",
                            method.ReturnType.CLanguageTypeName);
                    }
                }

                tw.SplitLine();
                tw.WriteLine(
                    "uintptr_t index = 0;");
                tw.WriteLine(
                    "do",
                    thisName);
                tw.WriteLine(
                    "{");

                using (var __ = tw.Shift())
                {
                    tw.WriteLine(
                        "IL2C_METHOD_TABLE_DECL* pMethodtbl = &{0}->methodtbl__[index];",
                        thisName);

                    if (method.ReturnType.IsVoidType)
                    {
                        tw.WriteLine("if (pMethodtbl->target != NULL)");
                        using (var ___ = tw.Shift())
                        {
                            tw.WriteLine(
                                "((void (*)(System_Object*{0}))(pMethodtbl->methodPtr))(pMethodtbl->target{1});",
                                string.Join(string.Empty, method.Parameters.
                                    Skip(1).
                                    Select(p => string.Format(", {0}", p.TargetType.CLanguageTypeName))),
                                string.Join(string.Empty, method.Parameters.
                                    Skip(1).
                                    Select(p => string.Format(", {0}", p.ParameterName))));
                        }
                        tw.WriteLine("else");
                        using (var ___ = tw.Shift())
                        {
                            tw.WriteLine(
                                "((void (*)({0}))(pMethodtbl->methodPtr))({1});",
                                string.Join(", ", method.Parameters.
                                    Skip(1).
                                    Select(p => p.TargetType.CLanguageTypeName)),
                                string.Join(", ", method.Parameters.
                                    Skip(1).
                                    Select(p => p.ParameterName)));
                        }
                    }
                    else
                    {
                        tw.WriteLine("if (pMethodtbl->target != NULL)");
                        using (var ___ = tw.Shift())
                        {
                            tw.WriteLine(
                                "{0}result = (({1} (*)(System_Object*{2}))(pMethodtbl->methodPtr))(pMethodtbl->target{3});",
                                method.ReturnType.IsReferenceType ? "frame__." : string.Empty,
                                method.ReturnType.CLanguageTypeName,
                                string.Join(string.Empty, method.Parameters.
                                    Skip(1).
                                    Select(p => string.Format(", {0}", p.TargetType.CLanguageTypeName))),
                                string.Join(string.Empty, method.Parameters.
                                    Skip(1).
                                    Select(p => string.Format(", {0}", p.ParameterName))));
                        }
                        tw.WriteLine("else");
                        using (var ___ = tw.Shift())
                        {
                            tw.WriteLine(
                                "{0}result = (({1} (*)({2}))(pMethodtbl->methodPtr))({3});",
                                method.ReturnType.IsReferenceType ? "frame__." : string.Empty,
                                method.ReturnType.CLanguageTypeName,
                                string.Join(", ", method.Parameters.
                                    Skip(1).
                                    Select(p => p.TargetType.CLanguageTypeName)),
                                string.Join(", ", method.Parameters.
                                    Skip(1).
                                    Select(p => p.ParameterName)));
                        }
                    }

                    tw.WriteLine("index++;");
                }
                tw.WriteLine("}");
                tw.WriteLine("while (index < this__->count__);");
                tw.SplitLine();

                if (!method.ReturnType.IsVoidType)
                {
                    if (method.ReturnType.IsReferenceType)
                    {
                        tw.WriteLine("il2c_unlink_execution_frame(&frame__);");
                        tw.WriteLine("return frame__.result;");
                    }
                    else
                    {
                        tw.WriteLine("return result;");
                    }
                }
            }

            tw.WriteLine("}");
            tw.SplitLine();
        }

        public static void InternalConvertFromMethod(
            CodeTextWriter tw,
            IExtractContextHost extractContext,
            PreparedInformations preparedFunctions,
            IMethodInformation method,
            DebugInformationOptions debugInformationOption = DebugInformationOptions.None)
        {
            if (method.IsVirtual)
            {
                if (method.IsAbstract)
                {
                    if (!method.DeclaringType.IsInterface)
                    {
                        InternalConvertFromAbstractFunction(
                            tw,
                            method);
                    }
                    return;
                }
            }

            // internalcall or DllImport
            if (method.IsExtern)
            {
                // DllImport
                var pinvokeInfo = method.PInvokeInfo;
                if (pinvokeInfo != null)
                {
                    InternalConvertFromPInvokeFunction(
                        tw,
                        method,
                        pinvokeInfo);
                    return;
                }

                // Specialize delegate type methods:
                if (method.DeclaringType.IsDelegate && !method.DeclaringType.IsAbstract)
                {
                    // Delegate constructor
                    if (method.IsConstructor)
                    {
                        // Ignore. We have to use the "il2c_new_delegate()" instead this constructor translated body.
                        return;
                    }

                    // Delegate "Invoke"
                    if (method.Name == "Invoke")
                    {
                        InternalConvertFromDelegateInvoker(
                            tw,
                            extractContext,
                            method);
                        return;
                    }
                }

                throw new InvalidProgramSequenceException(
                    "Unknown internallcall method declaration. Name={0}",
                    method.FriendlyName);
            }

            if (!preparedFunctions.Functions.TryGetValue(method, out var preparedMethod))
            {
                return;
            }

            InternalConvertFromFunction(
                tw,
                extractContext,
                preparedMethod,
                debugInformationOption);
        }
    }
}
