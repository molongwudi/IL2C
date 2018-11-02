#include "il2c_private.h"

/////////////////////////////////////////////////////////////
// System.Array

int32_t System_Array_getLength(System_Array* this__)
{
    il2c_assert(this__ != NULL);
    il2c_assert(this__->vptr0__ == &__System_Array_VTABLE__);
    il2c_assert(this__->elementType__ != NULL);
    il2c_assert(this__->count__ >= 0);

    return (int32_t)(this__->count__);
}

extern System_Array* il2c_new_array__(
    IL2C_RUNTIME_TYPE_DECL* elementType, intptr_t count)
{
    il2c_assert(elementType != NULL);
    
    if (count < 0)
    {
        // throw OverflowException
        il2c_assert(0);
    }

    uint32_t elementSize = il2c_sizeof__(elementType);
    il2c_assert(elementSize >= 1);

    // -1 is "uint8_t item__[1]"
    uintptr_t size = (uintptr_t)sizeof(System_Array) - 1 + count * elementSize;
    
    System_Array* arr = il2c_get_uninitialized_object_internal__(
        &__System_Array_RUNTIME_TYPE__, size);
    arr->vptr0__ = &__System_Array_VTABLE__;

    arr->elementType__ = elementType;
    arr->count__ = count;

    return arr;
}

/////////////////////////////////////////////////
// VTable and runtime type info declarations

void __System_Array_IL2C_MarkHandler__(System_Array* this__)
{
    il2c_assert(this__ != NULL);
    il2c_assert(this__->vptr0__ == &__System_Array_VTABLE__);

    if (this__->elementType__->flags & IL2C_TYPE_VALUE)
    {
        return;
    }

    intptr_t index;
    for (index = 0; index < this__->count__; index++)
    {
        System_Object* pReference = (System_Object*)&this__->item__[index];
        il2c_try_mark_from_handler(pReference);
    }
}

IL2C_RUNTIME_TYPE_DECL __System_Array_RUNTIME_TYPE__ = {
    "System.Array",
    IL2C_TYPE_VARIABLE,
    0,
    /* internalcall */ (IL2C_MARK_HANDLER)__System_Array_IL2C_MarkHandler__,
    il2c_typeof(System_Object)
};
