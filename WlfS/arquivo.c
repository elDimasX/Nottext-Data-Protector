
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
/// Concatena duas string e a retorna
/// </summary>
/// 
/// <param name="str1">Primeiro UNICODE</param>
/// <param name="str2">Segundo UNICODE</param>
/// 
/// <returns>Retorna a UNICODE concatenada</returns>
UNICODE_STRING ConcatenarUnicodeStrings(const UNICODE_STRING* str1, const UNICODE_STRING* str2)
{
    UNICODE_STRING resultado;
    RtlInitEmptyUnicodeString(&resultado, NULL, str1->MaximumLength + str2->MaximumLength);

    // Aloca memória para o buffer do resultado
    resultado.Buffer = (WCHAR*)ExAllocatePool(PagedPool, resultado.MaximumLength);
    if (resultado.Buffer == NULL)
    {
        // Falha ao alocar memória
        resultado.Length = 0;
        return resultado;
    }

    // Copia a primeira string para o buffer do resultado
    RtlCopyUnicodeString(&resultado, str1);

    // Copia a segunda string para o buffer do resultado
    RtlAppendUnicodeStringToString(&resultado, str2);

    return resultado;
}

/// <summary>
/// Obtém quantos caracteres tem num int: 10 -> 2, 1000 -> 4
/// </summary>
/// 
/// <param name="num">Número</param>
/// 
/// <returns>Retorna a quantidade de caracteres</returns>
int ObterQuantidadeDeCaracteresEmInt(int num)
{
    if (num == 0)
        return 1;

    int count = 0;
    while (num != 0)
    {
        num /= 10;
        count++;
    }
    return count;
}

KMUTEX MutexLerArquivo;

// Necessário para o Windows 10
BOOLEAN ConseguiuCompletarALeituraNoWindows10 = FALSE;

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
    // Espere se algum outro thread estiver acessando a função terminar
    NTSTATUS Status = KeWaitForSingleObject(&MutexLerArquivo, Executive, KernelMode, FALSE, NULL);

    if (!NT_SUCCESS(Status))
    {
        ConseguiuLerNoBoot = FALSE;
        return Status; // Falha
    }

    LendoArquivos = TRUE;

    // Váriaveis
    HANDLE Alca = NULL;
    IO_STATUS_BLOCK Io;
    OBJECT_ATTRIBUTES Atributos;

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
        if (ConseguiuCompletarALeituraNoWindows10 == FALSE)
        {
            // Vamos alterar para ter certeza, porque do WINDOWS 10, faz o favor
            // Do arquivo não conseguir completar a leitura
            ConseguiuLerNoBoot = FALSE;
        }

        // Se for uma quebra de linha
        if (((char*)Buffer)[i] == '\n')
        {

            ((char*)Buffer)[i] = '\0';
            //RtlInitUnicodeString(&line, (PWCH) & ((char*)buffer)[startIndex]);

            // Proteja
            if (NT_SUCCESS(InserirObjeto(RemoverSubString(&((char*)Buffer)[inicio], "\r"), Lista)))
            {
                // Conseguiu ler, as próximas leituras, serão um sucesso
                ConseguiuCompletarALeituraNoWindows10 = TRUE;

                // Ok, tudo certo
                ConseguiuLerNoBoot = TRUE;
            }

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

    // Libera o mutex após a operação de leitura
    KeReleaseMutex(&MutexLerArquivo, FALSE);

    return Status;

}

/// <summary>
/// Verifica se algum arquivo existe
/// </summary>
/// 
/// <param name="Arquivo">Arquivo para verificar</param>
/// <param name="EPasta">Para verificar se é uma pasta ou não</param>
/// 
/// <returns>Retorna um BOOLEAN para informar se existe ou não</returns>
BOOLEAN ArquivoExiste(IN PUNICODE_STRING Arquivo, BOOLEAN EPasta)
{
    // Váriaveis
    BOOLEAN Existe = FALSE;
    HANDLE Alca = NULL;
    IO_STATUS_BLOCK Io;
    OBJECT_ATTRIBUTES Atributos;
    NTSTATUS Status;

    // Inicie os atributos
    InitializeObjectAttributes(&Atributos, Arquivo, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

    //Status = ZwOpenFile(&Alca, FILE_GENERIC_READ, &Atributos, NULL, FILE_SHARE_READ, FILE_DIRECTORY_FILE);

    if (EPasta == TRUE)
    {
        // Abra o arquivo como pasta (se não existir, crie-a)
        Status = ZwCreateFile(
            &Alca,
            FILE_LIST_DIRECTORY,
            &Atributos,
            &Io,
            NULL,
            FILE_ATTRIBUTE_NORMAL,
            FILE_SHARE_READ | FILE_SHARE_WRITE,
            FILE_OPEN,
            FILE_DIRECTORY_FILE, NULL, 0
        );
    }
    else {

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
    }

    // Se conseguir
    if (NT_SUCCESS(Status))
    {
        Existe = TRUE;
        ZwClose(Alca);
    }
    else {
    }

    return Existe;
}
