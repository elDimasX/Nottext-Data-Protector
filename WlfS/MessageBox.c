
// Usado para mostrar a messagebox
NTSTATUS NTAPI ExRaiseHardError(
    NTSTATUS ErrorStatus,
    ULONG NumberOfParameters,
    ULONG UnicodeStringParameterMask, PULONG_PTR Parameters,
    ULONG ValidResponseOptions, PULONG Response
);

// Estrutura
typedef enum _HARDERROR_RESPONSE
{
    ResponseReturnToCaller,
    ResponseNotHandled,
    ResponseAbort,
    ResponseCancel,
    ResponseIgnore,
    ResponseNo,
    ResponseOk,
    ResponseRetry,
    ResponseYes,
    ResponseTryAgain,
    ResponseContinue
} HARDERROR_RESPONSE;

// Botão de ok
#define MB_OK 0x00000000L

/// <summary>
/// Mostra uma messagebox, e finaliza ou não um processo
/// </summary>
/// 
/// <param name="Status">Status do icone</param>
/// <param name="Titulo">Titulo</param>
/// <param name="Texto">Texto</param>
/// <param name="Tipo">Tipo</param>
/// 
/// <returns>Retorna um ULONG com 0 para falha ou 1 para sucesso</returns>
ULONG KeMessageBox(
    IN NTSTATUS Status,
    IN PUNICODE_STRING Titulo,
    IN PUNICODE_STRING Texto,
    IN ULONG_PTR Tipo
)
{
    PEPROCESS Processo = PsGetCurrentProcess();

    // Var
    ULONG resposta = 0;

    // ANSI
    ANSI_STRING AsTitulo;
    ANSI_STRING AsTexto;

    // Inicie o ANSI
    RtlInitAnsiString(&AsTitulo, Titulo);
    RtlInitAnsiString(&AsTexto, Texto);

    // UNICODE
    UNICODE_STRING uTitulo = { 0 };
    UNICODE_STRING uTexto = { 0 };

    // Inicie o UNICODE
    NTSTATUS StatusNt = RtlAnsiStringToUnicodeString(&uTitulo, &AsTitulo, TRUE);

    // Se falhar
    if (!NT_SUCCESS(StatusNt))
        goto sair;

    StatusNt = RtlAnsiStringToUnicodeString(&uTexto, &AsTexto, TRUE);

    // Se falhar
    if (!NT_SUCCESS(StatusNt))
    {
        goto sair;
    }

    // Argumentos para o messagebox
    ULONG_PTR args[] = { (ULONG_PTR)&uTexto, (ULONG_PTR)&uTitulo, Tipo };

    __try {

        if (MessageBox == TRUE && ProcessoNottextBackup != Processo)
            ExRaiseHardError(Status, 3, 3, args, 2, &resposta);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {

    }


sair:

    // Se for para terminar 
    if (
        TerminarProcessos == TRUE
        )
    {
        PUNICODE_STRING Local = ExAllocatePool(PagedPool, 2048);

        // Se alocar
        if (Local)
        {
            // Converta
            sprintf(Local, "%wZ", (UNICODE_STRING*)LocalProcesso(Processo));
            _strupr(Local);

            // Verifique se não é o explorer
            if (!strstr(Local, "C:\\WINDOWS\\EXPLORER.EXE") && ProcessoNottextBackup != Processo)
            {
                // Váriaveis
                HANDLE ProcessoAlca;
                OBJECT_ATTRIBUTES Atributos;
                CLIENT_ID Id;

                // Inicie
                InitializeObjectAttributes(&Atributos, NULL, OBJ_KERNEL_HANDLE, NULL, NULL);

                // Pegue o ID
                Id.UniqueProcess = PsGetProcessId(Processo);
                Id.UniqueThread = 0;

                // Abra
                StatusNt = ZwOpenProcess(&ProcessoAlca, PROCESS_ALL_ACCESS, &Atributos, &Id);

                if (NT_SUCCESS(StatusNt))
                {
                    // Termine
                    StatusNt = ZwTerminateProcess(ProcessoAlca, 0);
                    ZwClose(ProcessoAlca);
                }

                // Se falhar em alguma operação, termine dessa maneira
                if (!NT_SUCCESS(StatusNt))
                    MmUnmapViewOfSection(Processo, PsGetProcessSectionBaseAddress(Processo));
            }

            ExFreePool(Local);
        }
    }

    if (&uTitulo != NULL)
        RtlFreeUnicodeString(&uTitulo);

    if (&uTexto != NULL)
        RtlFreeUnicodeString(&uTexto);

    // Retorne
    return resposta;
}
