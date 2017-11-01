﻿#include "il2c_test_target.h"

int32_t il2c_test_target_ValueTypeTestTarget_Value1;
System_Uri* il2c_test_target_ClassTypeTestTarget_OR1 = new System_Uri("...");



int32_t il2c_test_target_ClassTypeTestTarget_GetValue2(il2c_test_target_ClassTypeTestTarget* __this, int32_t a, int32_t b)
{
    int32_t local0;

    il2c_test_target_ClassTypeTestTarget* __stack0_il2c_test_target_ClassTypeTestTarget_reference;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    __stack0_il2c_test_target_ClassTypeTestTarget_reference = __this;
    __stack0_int32_t = __stack0_il2c_test_target_ClassTypeTestTarget_reference->Value2;
    __stack1_int32_t = a;
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    __stack1_int32_t = b;
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    local0 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local0;
    return __stack0_int32_t;
}


void il2c_test_target_ClassTypeTestTarget__ctor(il2c_test_target_ClassTypeTestTarget* __this)
{

    il2c_test_target_ClassTypeTestTarget* __stack0_il2c_test_target_ClassTypeTestTarget_reference;

    __stack0_il2c_test_target_ClassTypeTestTarget_reference = __this;
    System_Object__ctor((System_Object*)__stack0_il2c_test_target_ClassTypeTestTarget_reference);
    return;
}

int32_t il2c_test_target_ClassTypeTest_Test4();


ExecutionContext* currentContext;

void mark_and_sweep()
{
    /////////////////////////////////////
    // Execute mark phase

    // TODO: clear gcmark

    // Mark phase
    ExecutionContext* current = currentContext;
    while (current != NULL)
    {
        const uint8_t count = current->count;
        for (uint8_t index = 0; index < count; index++)
        {
            uint8_t** target = (uint8_t**)(current->target[index]);
            Common* p = (Common*)(*target - sizeof(Common));
            if (p != NULL)
            {
                // Mark this instance.
                p->__gcmark = 1;

                // TODO: recursive mark
            }
        }

        current = current->previous;
    }

    // TODO: sweep
}

void initialize_GC()
{
    ExecutionContext rootContext;
    memset(&rootContext, 0, sizeof rootContext);

    currentContext = &rootContext;

    // Phase 1: Type initializing
    il2c_test_target_ClassTypeTestTarget_Value1 = 0;
    il2c_test_target_ClassTypeTestTarget_OR1 = NULL;

    il2c_test_target_ClassTypeTestTarget__cctor();

    // Phase 2: Chain object reference
    ExecutionContext globalContext;
    globalContext.count = 1;
    globalContext.target[0] = &il2c_test_target_ClassTypeTestTarget_OR1;

    globalContext.previous = currentContext;
    currentContext = &globalContext;
}

int main()
{
    initialize_GC();

    il2c_test_target_ClassTypeTest_Test4();

    mark_and_sweep();
}

int32_t il2c_test_target_ClassTypeTest_Test4()
{
    il2c_test_target_ClassTypeTestTarget* local0;
    int32_t local1;

    il2c_test_target_ClassTypeTestTarget* __stack0_il2c_test_target_ClassTypeTestTarget_reference;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    struct
    {
        void* previous;
        uint8_t count;
        il2c_test_target_ClassTypeTestTarget** plocal0;
        il2c_test_target_ClassTypeTestTarget** __pstack0;
    } functionContext;

    functionContext.count = 2;
    functionContext.plocal0 = &local0;
    functionContext.__pstack0 = &__stack0_il2c_test_target_ClassTypeTestTarget_reference;

    functionContext.previous = currentContext;
    currentContext = (ExecutionContext*)&functionContext;

    // IL Body

    // newobj
    uint8_t* p = (uint8_t*)malloc(
        sizeof(Common) + sizeof(il2c_test_target_ClassTypeTestTarget));
    memset(p, 0x00, sizeof(Common) + sizeof(il2c_test_target_ClassTypeTestTarget));
    __stack0_il2c_test_target_ClassTypeTestTarget_reference =
        (il2c_test_target_ClassTypeTestTarget*)(p + sizeof(Common));
    il2c_test_target_ClassTypeTestTarget__ctor(__stack0_il2c_test_target_ClassTypeTestTarget_reference);


    local0 = __stack0_il2c_test_target_ClassTypeTestTarget_reference;

    __stack0_il2c_test_target_ClassTypeTestTarget_reference = local0;
    __stack1_int32_t = 456;
    __stack0_il2c_test_target_ClassTypeTestTarget_reference->Value2 = __stack1_int32_t;
    __stack0_il2c_test_target_ClassTypeTestTarget_reference = local0;
    __stack0_int32_t = __stack0_il2c_test_target_ClassTypeTestTarget_reference->Value2;
    local1 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local1;

    currentContext = functionContext.previous;

    return __stack0_int32_t;
}


int32_t il2c_test_target_ClassTypeTest_Test5(void)
{
    il2c_test_target_ClassTypeTestTarget* local0;
    int32_t local1;
    int32_t local2;

    il2c_test_target_ClassTypeTestTarget* __stack0_il2c_test_target_ClassTypeTestTarget_reference;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;
    int32_t __stack2_int32_t;

    __stack0_il2c_test_target_ClassTypeTestTarget_reference = (il2c_test_target_ClassTypeTestTarget*)malloc(sizeof(il2c_test_target_ClassTypeTestTarget));
    memset(__stack0_il2c_test_target_ClassTypeTestTarget_reference, 0x00, sizeof(il2c_test_target_ClassTypeTestTarget));
    il2c_test_target_ClassTypeTestTarget__ctor(__stack0_il2c_test_target_ClassTypeTestTarget_reference);
    local0 = __stack0_il2c_test_target_ClassTypeTestTarget_reference;
    __stack0_il2c_test_target_ClassTypeTestTarget_reference = local0;
    __stack1_int32_t = 789;
    __stack0_il2c_test_target_ClassTypeTestTarget_reference->Value2 = __stack1_int32_t;
    __stack0_il2c_test_target_ClassTypeTestTarget_reference = local0;
    __stack1_int32_t = 123;
    __stack2_int32_t = 456;
    __stack0_int32_t = il2c_test_target_ClassTypeTestTarget_GetValue2(__stack0_il2c_test_target_ClassTypeTestTarget_reference, __stack1_int32_t, __stack2_int32_t);
    local1 = __stack0_int32_t;
    __stack0_int32_t = local1;
    local2 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local2;
    return __stack0_int32_t;
}


void il2c_test_target_ClassTypeTest__ctor(il2c_test_target_ClassTypeTest* __this)
{

    il2c_test_target_ClassTypeTest* __stack0_il2c_test_target_ClassTypeTest_reference;

    __stack0_il2c_test_target_ClassTypeTest_reference = __this;
    System_Object__ctor((System_Object*)__stack0_il2c_test_target_ClassTypeTest_reference);
    return;
}


int32_t il2c_test_target_ValueTypeTestTarget_GetValue2(il2c_test_target_ValueTypeTestTarget* __this, int32_t a, int32_t b)
{
    int32_t local0;

    il2c_test_target_ValueTypeTestTarget* __stack0_il2c_test_target_ValueTypeTestTarget_reference;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    __stack0_il2c_test_target_ValueTypeTestTarget_reference = __this;
    __stack0_int32_t = __stack0_il2c_test_target_ValueTypeTestTarget_reference->Value2;
    __stack1_int32_t = a;
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    __stack1_int32_t = b;
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    local0 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local0;
    return __stack0_int32_t;
}


int32_t il2c_test_target_ValueTypeTest_Test4(void)
{
    il2c_test_target_ValueTypeTestTarget local0;
    int32_t local1;

    il2c_test_target_ValueTypeTestTarget* __stack0_il2c_test_target_ValueTypeTestTarget_reference;
    il2c_test_target_ValueTypeTestTarget __stack0_il2c_test_target_ValueTypeTestTarget;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    struct
    {
        void* previous;
        uint8_t count;
        System_Uri** target0;
    } functionContext;

    functionContext.count = 1;
    functionContext.target0 = &local0.OR1;

    functionContext.previous = currentContext;
    currentContext = (ExecutionContext*)&functionContext;

    // IL Body

    // initobj
    __stack0_il2c_test_target_ValueTypeTestTarget_reference = &local0;
    memset(__stack0_il2c_test_target_ValueTypeTestTarget_reference, 0x00, sizeof(il2c_test_target_ValueTypeTestTarget));
    __stack0_il2c_test_target_ValueTypeTestTarget_reference = &local0;

    __stack1_int32_t = 456;
    __stack0_il2c_test_target_ValueTypeTestTarget_reference->Value2 = __stack1_int32_t;
    __stack0_il2c_test_target_ValueTypeTestTarget = local0;
    __stack0_int32_t = __stack0_il2c_test_target_ValueTypeTestTarget.Value2;
    local1 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local1;

    currentContext = functionContext.previous;

    return __stack0_int32_t;
}


int32_t il2c_test_target_ValueTypeTest_Test5(void)
{
    il2c_test_target_ValueTypeTestTarget local0;
    int32_t local1;
    int32_t local2;

    il2c_test_target_ValueTypeTestTarget* __stack0_il2c_test_target_ValueTypeTestTarget_reference;
    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;
    int32_t __stack2_int32_t;

    __stack0_il2c_test_target_ValueTypeTestTarget_reference = &local0;
    memset(__stack0_il2c_test_target_ValueTypeTestTarget_reference, 0x00, sizeof(il2c_test_target_ValueTypeTestTarget));
    __stack0_il2c_test_target_ValueTypeTestTarget_reference = &local0;
    __stack1_int32_t = 789;
    __stack0_il2c_test_target_ValueTypeTestTarget_reference->Value2 = __stack1_int32_t;
    __stack0_il2c_test_target_ValueTypeTestTarget_reference = &local0;
    __stack1_int32_t = 123;
    __stack2_int32_t = 456;
    __stack0_int32_t = il2c_test_target_ValueTypeTestTarget_GetValue2(__stack0_il2c_test_target_ValueTypeTestTarget_reference, __stack1_int32_t, __stack2_int32_t);
    local1 = __stack0_int32_t;
    __stack0_int32_t = local1;
    local2 = __stack0_int32_t;
    goto L_0000;
L_0000:
    __stack0_int32_t = local2;
    return __stack0_int32_t;
}


void il2c_test_target_ValueTypeTest__ctor(il2c_test_target_ValueTypeTest* __this)
{

    il2c_test_target_ValueTypeTest* __stack0_il2c_test_target_ValueTypeTest_reference;

    __stack0_il2c_test_target_ValueTypeTest_reference = __this;
    System_Object__ctor((System_Object*)__stack0_il2c_test_target_ValueTypeTest_reference);
    return;
}


uint8_t il2c_test_target_Hoge2_Add3(uint8_t a, bool isTwo)
{
    uint8_t local0;

    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    __stack0_int32_t = a;
    __stack1_int32_t = isTwo ? 1 : 0;
    if (__stack1_int32_t != 0) goto L_0000;
    __stack1_int32_t = 1;
    goto L_0001;
L_0000:
    __stack1_int32_t = 2;
L_0001:
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    __stack0_int32_t = (uint8_t)__stack0_int32_t;
    local0 = (uint8_t)__stack0_int32_t;
    goto L_0002;
L_0002:
    __stack0_int32_t = local0;
    return (uint8_t)__stack0_int32_t;
}


uint16_t il2c_test_target_Hoge2_Add4(uint16_t a, bool isTwo)
{
    uint16_t local0;

    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    __stack0_int32_t = a;
    __stack1_int32_t = isTwo ? 1 : 0;
    if (__stack1_int32_t != 0) goto L_0000;
    __stack1_int32_t = 1;
    goto L_0001;
L_0000:
    __stack1_int32_t = 2;
L_0001:
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    __stack0_int32_t = (uint16_t)__stack0_int32_t;
    local0 = (uint16_t)__stack0_int32_t;
    goto L_0002;
L_0002:
    __stack0_int32_t = local0;
    return (uint16_t)__stack0_int32_t;
}


void il2c_test_target_Hoge2__ctor(il2c_test_target_Hoge2* __this)
{

    il2c_test_target_Hoge2* __stack0_il2c_test_target_Hoge2_reference;

    __stack0_il2c_test_target_Hoge2_reference = __this;
    System_Object__ctor((System_Object*)__stack0_il2c_test_target_Hoge2_reference);
    return;
}


int32_t il2c_test_target_Hoge1_Add1(int32_t a, bool isTwo)
{
    int32_t local0;

    int32_t __stack0_int32_t;
    int32_t __stack1_int32_t;

    __stack0_int32_t = a;
    __stack1_int32_t = isTwo ? 1 : 0;
    if (__stack1_int32_t != 0) goto L_0000;
    __stack1_int32_t = 1;
    goto L_0001;
L_0000:
    __stack1_int32_t = 2;
L_0001:
    __stack0_int32_t = __stack0_int32_t + __stack1_int32_t;
    local0 = __stack0_int32_t;
    goto L_0002;
L_0002:
    __stack0_int32_t = local0;
    return __stack0_int32_t;
}


int64_t il2c_test_target_Hoge1_Add2(int64_t a, bool isTwo)
{
    int64_t local0;

    int64_t __stack0_int64_t;
    int32_t __stack1_int32_t;
    int64_t __stack1_int64_t;

    __stack0_int64_t = a;
    __stack1_int32_t = isTwo ? 1 : 0;
    if (__stack1_int32_t != 0) goto L_0000;
    __stack1_int32_t = 1;
    goto L_0001;
L_0000:
    __stack1_int32_t = 2;
L_0001:
    __stack1_int64_t = __stack1_int32_t;
    __stack0_int64_t = __stack0_int64_t + __stack1_int64_t;
    local0 = __stack0_int64_t;
    goto L_0002;
L_0002:
    __stack0_int64_t = local0;
    return __stack0_int64_t;
}


void il2c_test_target_Hoge1__ctor(il2c_test_target_Hoge1* __this)
{

    il2c_test_target_Hoge1* __stack0_il2c_test_target_Hoge1_reference;

    __stack0_il2c_test_target_Hoge1_reference = __this;
    System_Object__ctor((System_Object*)__stack0_il2c_test_target_Hoge1_reference);
    return;
}

