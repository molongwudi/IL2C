﻿#ifndef __MODULE_UEFI_Code__
#define __MODULE_UEFI_Code__

#include <il2c.h>
#include <stdint.h>
#include <wchar.h>
#include <stdbool.h>
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////////////
// Types:

typedef struct UEFI_Code_AbstractNode UEFI_Code_AbstractNode;
typedef struct UEFI_Code_OperatorNode UEFI_Code_OperatorNode;
typedef struct UEFI_Code_ReducibleNode UEFI_Code_ReducibleNode;
typedef struct UEFI_Code_NumericNode UEFI_Code_NumericNode;
typedef struct UEFI_Code_ExpressionNode UEFI_Code_ExpressionNode;
typedef struct UEFI_Code_PolishNotation UEFI_Code_PolishNotation;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.AbstractNode

struct UEFI_Code_AbstractNode
{
    int32_t NextIndex;
};

extern const __RUNTIME_TYPE__ __UEFI_Code_AbstractNode_RUNTIME_TYPE__;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.OperatorNode

struct UEFI_Code_OperatorNode
{
    int32_t NextIndex;
    wchar_t Operator;
};

extern const __RUNTIME_TYPE__ __UEFI_Code_OperatorNode_RUNTIME_TYPE__;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.ReducibleNode

struct UEFI_Code_ReducibleNode
{
    int32_t NextIndex;
};

extern const __RUNTIME_TYPE__ __UEFI_Code_ReducibleNode_RUNTIME_TYPE__;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.NumericNode

struct UEFI_Code_NumericNode
{
    int32_t NextIndex;
    int32_t Numeric;
};

extern const __RUNTIME_TYPE__ __UEFI_Code_NumericNode_RUNTIME_TYPE__;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.ExpressionNode

struct UEFI_Code_ExpressionNode
{
    int32_t NextIndex;
    UEFI_Code_OperatorNode* Operator;
    UEFI_Code_ReducibleNode* Left;
    UEFI_Code_ReducibleNode* Right;
};

extern const __RUNTIME_TYPE__ __UEFI_Code_ExpressionNode_RUNTIME_TYPE__;

////////////////////////////////////////////////////////////
// Class: UEFI.Code.PolishNotation


extern const __RUNTIME_TYPE__ __UEFI_Code_PolishNotation_RUNTIME_TYPE__;

//////////////////////////////////////////////////////////////////////////////////
// Public static fields:







//////////////////////////////////////////////////////////////////////////////////
// Methods:

extern void UEFI_Code_AbstractNode__ctor(UEFI_Code_AbstractNode* __this, int32_t nextIndex);

extern void UEFI_Code_OperatorNode__ctor(UEFI_Code_OperatorNode* __this, wchar_t oper, int32_t nextIndex);

extern void UEFI_Code_ReducibleNode__ctor(UEFI_Code_ReducibleNode* __this, int32_t nextIndex);
extern int32_t UEFI_Code_ReducibleNode_Reduce(UEFI_Code_ReducibleNode* __this);

extern void UEFI_Code_NumericNode__ctor(UEFI_Code_NumericNode* __this, int32_t numeric, int32_t nextIndex);
extern int32_t UEFI_Code_NumericNode_Reduce(UEFI_Code_NumericNode* __this);

extern void UEFI_Code_ExpressionNode__ctor(UEFI_Code_ExpressionNode* __this, UEFI_Code_OperatorNode* oper, UEFI_Code_ReducibleNode* left, UEFI_Code_ReducibleNode* right, int32_t nextIndex);
extern int32_t UEFI_Code_ExpressionNode_Reduce(UEFI_Code_ExpressionNode* __this);

extern int32_t UEFI_Code_PolishNotation_SkipWhiteSpace(System_String* line, int32_t startIndex);
extern UEFI_Code_OperatorNode* UEFI_Code_PolishNotation_ParseOperator(System_String* line, int32_t startIndex);
extern UEFI_Code_NumericNode* UEFI_Code_PolishNotation_ParseNumeric(System_String* line, int32_t startIndex);
extern UEFI_Code_ExpressionNode* UEFI_Code_PolishNotation_ParseExpression(System_String* line, int32_t startIndex);
extern void UEFI_Code_PolishNotation_Main(void);
extern void UEFI_Code_PolishNotation__ctor(UEFI_Code_PolishNotation* __this);

#ifdef __cplusplus
}
#endif

#endif
