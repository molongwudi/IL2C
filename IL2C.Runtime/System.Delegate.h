#ifndef System_Delegate_H__
#define System_Delegate_H__

#pragma once

#include <il2c.h>

#ifdef __cplusplus
extern "C" {
#endif

/////////////////////////////////////////////////////////////
// System.Delegate

typedef struct System_Delegate System_Delegate;

typedef System_Object_VTABLE_DECL__ System_Delegate_VTABLE_DECL__;

typedef struct IL2C_METHOD_TABLE_DECL
{
    System_Object* target;
    intptr_t methodPtr;
} IL2C_METHOD_TABLE_DECL;

// We are surprised what be variable size for System.Delegate instance ;)

struct System_Delegate
{
    System_Delegate_VTABLE_DECL__* vptr0__;

    uintptr_t count__;
    IL2C_METHOD_TABLE_DECL methodtbl__[1];
};

extern System_Delegate_VTABLE_DECL__ System_Delegate_VTABLE__;

IL2C_DECLARE_RUNTIME_TYPE(System_Delegate);

extern /* virtual */ int32_t System_Delegate_GetHashCode(System_Delegate* this__);
extern /* virtual */ bool System_Delegate_Equals(System_Delegate* this__, System_Object* obj);
extern /* static */ System_Delegate* System_Delegate_Combine(System_Delegate* a, System_Delegate* b);
extern /* static */ System_Delegate* System_Delegate_Remove(System_Delegate* source, System_Delegate* value);

/////////////////////////////////////////////////
// Delegate special functions

extern System_Delegate* il2c_new_delegate__(
    IL2C_RUNTIME_TYPE delegateType, System_Object* object, intptr_t method);
#define il2c_new_delegate(typeName, object, method) \
    il2c_new_delegate__(il2c_typeof(typeName), object, method)

extern void System_Delegate_MarkHandler__(System_Delegate* this__);

#ifdef __cplusplus
}
#endif

#endif
