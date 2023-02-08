
/// <summary>
/// Remove uma substring de uma string
/// </summary>
/// 
/// <param name="String">String para procurar a substring</param>
/// <param name="Sub">Substring para remover</param>
/// 
/// <returns>Retorna um char com o nome sem substring</returns>
char* RemoverSubString(OUT char* String, IN const char* Sub)
{
    __try {

        char* p, * q, * r;
        if (*Sub && (q = r = strstr(String, Sub)) != NULL) {
            size_t len = strlen(Sub);
            while ((r = strstr(p = r + len, Sub)) != NULL) {
                memmove(q, p, r - p);
                q += r - p;
            }
            memmove(q, p, strlen(p) + 1);
        }
        return String;

    }
    __except (EXCEPTION_EXECUTE_HANDLER) {}

    // Retorne a string
    return String;
}

/// <summary>
/// Lê todas as linhas de um arquivo e adiciona numa lista
/// </summary>
/// 
/// <param name="Arquivo">Arquivo</param>
/// <param name="Lista">Lista para adicionar</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS LerArquivo(IN PUNICODE_STRING Arquivo, OUT PLIST_ENTRY Lista)
{
    LendoArquivos = TRUE;

    // Váriaveis
    HANDLE Alca = NULL;
    IO_STATUS_BLOCK Io;
    OBJECT_ATTRIBUTES Atributos;
    NTSTATUS Status;

    // Mensagem, e linha
    PVOID Buffer = NULL;
    UNICODE_STRING Linha;

    // Inicie os atributos
    InitializeObjectAttributes(&Atributos, Arquivo, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

    // Abra o arquivo
    Status = ZwCreateFile(
        &Alca,
        GENERIC_READ,
        &Atributos,
        &Io,
        NULL,
        FILE_ATTRIBUTE_NORMAL,
        0,
        FILE_OPEN,
        FILE_NON_DIRECTORY_FILE, NULL, 0
    );

    // Se falhar
    if (!NT_SUCCESS(Status))
    {
        goto sair;
    }

    // Obtenha as informações deste arquivo
    FILE_STANDARD_INFORMATION Informacao;
    Status = ZwQueryInformationFile(
        Alca, &Io, &Informacao, sizeof(FILE_STANDARD_INFORMATION), FileStandardInformation
    );

    if (!NT_SUCCESS(Status))
    {
        goto sair;
    }

    // Aloque o buffer
    ULONG Tamanho = Informacao.EndOfFile.LowPart;
    Buffer = ExAllocatePoolWithTag(NonPagedPool, Tamanho, 'file');

    if (!Buffer)
    {
        Status = STATUS_INSUFFICIENT_RESOURCES;
        goto sair;
    }

    // Leia o arquivo
    LARGE_INTEGER Offset = { 0 };
    Status = ZwReadFile(Alca, NULL, NULL, NULL, &Io, Buffer, Informacao.EndOfFile.LowPart, &Offset, NULL);

    if (!NT_SUCCESS(Status))
    {
        goto sair;
    }

    // Percorra o buffer e imprima cada linha
    ULONG lerBytes = (ULONG)Io.Information; 
    ULONG inicio = 0;

    // Leia os bytes
    for (ULONG i = 0; i < lerBytes; i++)
    {
        // Se for uma quebra de linha
        if (((char*)Buffer)[i] == '\n')
        {
            ((char*)Buffer)[i] = '\0';
            //RtlInitUnicodeString(&line, (PWCH) & ((char*)buffer)[startIndex]);

            // Proteja
            InserirObjeto(RemoverSubString(&((char*)Buffer)[inicio], "\r"), Lista);

            // Inicio
            inicio = i + 1;
        }
    }

    // Termine
    goto sair;

sair:

    LendoArquivos = FALSE;

    // Feche
    if (Alca != NULL)
        ZwClose(Alca);

    // Libere
    if (Buffer != NULL)
        ExFreePoolWithTag(Buffer, 'file');

    return Status;

}

/// <summary>
/// Altera o ProtecaoHabilitada verificando se um arquivo existe
/// </summary>
VOID VerificarProtecaoAtiva()
{
    // Váriaveis
    HANDLE Alca = NULL;
    IO_STATUS_BLOCK Io;
    OBJECT_ATTRIBUTES Atributos;
    NTSTATUS Status;

    // Inicie os atributos
    InitializeObjectAttributes(&Atributos, &ArquivoProtecaoHabilitada, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

    // Abra o arquivo
    Status = ZwCreateFile(
        &Alca,
        GENERIC_READ,
        &Atributos,
        &Io,
        NULL,
        FILE_ATTRIBUTE_NORMAL,
        0,
        FILE_OPEN,
        FILE_NON_DIRECTORY_FILE, NULL, 0
    );

    // Se conseguir
    if (NT_SUCCESS(Status))
    {
        ZwClose(Alca);
        ProtecaoHabilitada = FALSE;
    }
    else {

        ProtecaoHabilitada = TRUE;
    }
}
