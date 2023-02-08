
// Obtém a informação de um processo
NTSYSCALLAPI
NTSTATUS
NTAPI
ZwQueryInformationProcess(
	__in HANDLE ProcessHandle,
	__in ULONG ProcessInformationClass,
	__out_bcount_opt(ProcessInformationLength) PVOID ProcessInformation,
	__in ULONG ProcessInformationLength,
	__out_opt PULONG ReturnLength
);

// Usado para pegar o nome completo de um processo
NTSTATUS
PsReferenceProcessFilePointer(
	IN  PEPROCESS Process,
	OUT PVOID* OutFileObject 
);

/// <summary>
/// Retorna um nome de um processo
/// </summary>
/// 
/// <param name="Processo">Processo para verificar</param>
/// 
/// <returns>Retorna o nome de um processo</returns>
PUNICODE_STRING LocalProcesso(IN PEPROCESS Processo)
{
	__try {

		// Objeto arquivo
		PFILE_OBJECT ObjetoArquivo;

		// Onde salvaremos o nome do local do disco
		POBJECT_NAME_INFORMATION ObjetoArquivoInformacao = NULL;

		// Se não conseguir salvar as informações do processo no PFILE_OBJECT
		if (!NT_SUCCESS(PsReferenceProcessFilePointer(Processo, &ObjetoArquivo)))
		{
			// Não retorne nada
			return NULL;
		}

		// Se não conseguir obter o local do driver ("C:\") do arquivo
		if (!NT_SUCCESS(IoQueryFileDosDeviceName(ObjetoArquivo, &ObjetoArquivoInformacao)))
		{
			// Falhou, pare
			return NULL;
		}

		// Libere o objeto
		ObDereferenceObject(ObjetoArquivo);

		// Retorne o nome do arquivo
		return &(ObjetoArquivoInformacao->Name);
	}
	__except (EXCEPTION_EXECUTE_HANDLER) {

	}

	// Nulo
	return NULL;
}

/// <summary>
/// Ignora um processo de acordo com as exceções do usermode
/// </summary>
/// 
/// <param name="Processo">Processo para verificar</param>
/// 
/// <returns>Retorna um bool para ignorar ou não</returns>
BOOLEAN ProcessoIgnorarPeloUserMode(IN PEPROCESS Processo)
{
	BOOLEAN Ignorar = FALSE;

	// Aloque
	PUNICODE_STRING NomeProcesso = ExAllocatePool(PagedPool, 2048);

	// Nome processo
	if (NomeProcesso)
	{
		sprintf(NomeProcesso, "%wZ", (UNICODE_STRING*)LocalProcesso(Processo));

		if (ObjetoProtegido(NomeProcesso, &ProcessosPermitidos))
		{
			Ignorar = TRUE;
		}

		ExFreePool(NomeProcesso);
	}

	return Ignorar;
}

/// <summary>
/// Verifica se é um processo original do Nottext
/// </summary>
/// 
/// <param name="Processo">Processo para verificar</param>
/// 
/// <returns>Retorna um bool para informar se é um processo nottext ou não</returns>
BOOLEAN ProcessoNottext(IN PEPROCESS Processo)
{
	BOOLEAN ProcessoNottext = FALSE;

	// Váriavéis
	UNICODE_STRING LocalCompleto;
	PUNICODE_STRING Local = ExAllocatePool(PagedPool, 2048);

	// Se alocar
	if (Local)
	{
		// Converta
		sprintf(Local, "\\??\\%wZ", (UNICODE_STRING*)LocalProcesso(Processo));

		// Inicie o ANSI
		ANSI_STRING As;

		// Converta ANSI para UNICODE
		RtlInitAnsiString(&As, Local);
		NTSTATUS Status = RtlAnsiStringToUnicodeString(&LocalCompleto, &As, TRUE);

		// Libere
		ExFreePool(Local);

		if (NT_SUCCESS(Status))
		{
			// Variáveis
			HANDLE Alca;
			OBJECT_ATTRIBUTES Atributos;
			IO_STATUS_BLOCK Io;
			FILE_NETWORK_OPEN_INFORMATION Info;
			LARGE_INTEGER li;
			TIME_FIELDS tempo;

			// Inicie o atributo
			InitializeObjectAttributes(&Atributos, &LocalCompleto, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

			// Abra o arquivo com permissões de leitura
			NTSTATUS Status = ZwCreateFile(
				&Alca,
				GENERIC_READ,
				&Atributos,
				&Io,
				NULL, FILE_ATTRIBUTE_NORMAL, 0, FILE_OPEN,
				FILE_NON_DIRECTORY_FILE | FILE_SYNCHRONOUS_IO_NONALERT, NULL, 0
			);

			// Se falhar
			if (!NT_SUCCESS(Status))
			{
				RtlFreeUnicodeString(&LocalCompleto);
				return ProcessoNottext;
			}

			// Obtenha as informações
			Status = ZwQueryInformationFile(Alca, &Io, &Info, sizeof(Info), FileNetworkOpenInformation);

			// Se falhar
			if (!NT_SUCCESS(Status))
			{
				RtlFreeUnicodeString(&LocalCompleto);
				ZwClose(Alca);
				return ProcessoNottext;
			}

			// Feche a alca
			ZwClose(Alca);

			// Converta
			RtlTimeToTimeFields(&Info.LastWriteTime, &tempo);

			// Pegue
			PUNICODE_STRING DiaMesAno = ExAllocatePool(PagedPool, 15);

			if (DiaMesAno)
			{
				// Converta
				sprintf(DiaMesAno, "%02d/%02d/%04d", tempo.Day, tempo.Month, tempo.Year);

				// Data da última modificação do Nottext
				if (
					strstr(DiaMesAno, "08/02/2023") 
					)
				{
					ProcessoNottext = TRUE;
				}

				// Libere
				ExFreePool(DiaMesAno);
			}

			RtlFreeUnicodeString(&LocalCompleto);
		}

	}

	return ProcessoNottext;
}
