#include "Generated/UEFI.Code.h"

#include "efi.h"

//////////////////////////////////////////////////////////////////////////
// IL2C <---> UEFI interop functions

static EFI_SYSTEM_TABLE* g_pSystemTable = NULL;

bool twtoi(const wchar_t *_Str, int32_t* value)
{
    bool sign = false;

    for (;; _Str++)
    {
        wchar_t ch = *_Str;
        if ((ch == L' ') || (ch == L'\t'))
        {
            continue;
        }

        if (ch == L'-')
        {
            sign = true;
            _Str++;
        }
        else if (ch == L'+')
        {
            _Str++;
        }
        else
        {
            return false;
        }

        break;
    }

    int32_t n = 0;
    while ((*_Str >= L'0') && (*_Str <= L'9'))
    {
        n = n * 10 + *_Str++ - L'0';
    }

    *value = sign ? -n : n;
    return true;
}

void itow(int32_t value, wchar_t* p)
{
    wchar_t *j;
    wchar_t b[6];

    if (value < 0)
    {
        *p++ = L'-';
        value = -value;
    }

    j = &b[5];
    *j-- = 0;

    do
    {
        *j-- = value % 10 + L'0';
        value /= 10;
    } while (value);

    do
    {
        *p++ = *++j;
    } while (*j);
}

void ReadLine(wchar_t* pBuffer, uint16_t length)
{
    wchar_t buffer[4];
    uint16_t index = 0;

    while ((index + 1) < length)
    {
        buffer[0] = L'_';
        buffer[1] = L'\0';
        g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, buffer);

        unsigned long long waitIndex = 0;
        g_pSystemTable->BootServices->WaitForEvent(
            1, &(g_pSystemTable->ConIn->WaitForKey), &waitIndex);

        EFI_INPUT_KEY efi_input_key;
        if (g_pSystemTable->ConIn->ReadKeyStroke(
            g_pSystemTable->ConIn, &efi_input_key) != 0)
        {
            continue;
        }

        if (efi_input_key.UnicodeChar < 0x20)
        {
            if (efi_input_key.UnicodeChar == L'\r')
            {
                break;
            }

            if (efi_input_key.UnicodeChar == L'\b')
            {
                if (index >= 1)
                {
                    index--;

                    buffer[0] = L'\b';
                    buffer[1] = L'\0';
                    g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, buffer);
                }
            }

            buffer[0] = L'\b';
            buffer[1] = L'\0';
            g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, buffer);

            continue;
        }
        else
        {
            pBuffer[index++] = efi_input_key.UnicodeChar;
        }

        buffer[0] = L'\b';
        buffer[1] = efi_input_key.UnicodeChar;
        buffer[2] = L'\0';
        g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, buffer);
    }

    pBuffer[index] = L'\0';

    buffer[0] = L'\b';
    buffer[1] = L'\r';
    buffer[2] = L'\n';
    buffer[3] = L'\0';

    g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, buffer);
}

void Write(const wchar_t* pMessage)
{
    g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, (wchar_t*)pMessage);
}

void WriteLine(const wchar_t* pMessage)
{
    g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, (wchar_t*)pMessage);
    g_pSystemTable->ConOut->OutputString(g_pSystemTable->ConOut, L"\r\n");
}

void WriteLineToError(const wchar_t* pMessage)
{
    g_pSystemTable->StdErr->OutputString(g_pSystemTable->StdErr, (wchar_t*)pMessage);
    g_pSystemTable->StdErr->OutputString(g_pSystemTable->StdErr, L"\r\n");
}

void __cdecl _wassert(
    wchar_t const* _Message,
    wchar_t const* _File,
    unsigned _Line)
{
    WriteLineToError(_Message);
}

void* __cdecl malloc(size_t _Size)
{
    void* ppAllocated = NULL;
    if (g_pSystemTable->BootServices->AllocatePool(
        EfiLoaderData, _Size, &ppAllocated) == EFI_SUCCESS)
    {
        return ppAllocated;
    }
    else
    {
        return NULL;
    }
}

void __cdecl free(void* _Block)
{
    g_pSystemTable->BootServices->FreePool(_Block);
}

//////////////////////////////////////////////////////////////////////////
// UEFI interface: code refer from

EFI_STATUS EfiMain(
    EFI_HANDLE imageHandle,
    EFI_SYSTEM_TABLE* pSystemTable)
{
    // Setup interop pointer
    g_pSystemTable = pSystemTable;

    // Disable default auto-reset watchdog timer
    g_pSystemTable->BootServices->SetWatchdogTimer(0, 0, 0, NULL);

    // Clear screen
    g_pSystemTable->ConOut->ClearScreen(g_pSystemTable->ConOut);

    __gc_initialize__();

    UEFI_Code_PolishNotation_Main();

    __gc_shutdown__();

    return 0;
}
