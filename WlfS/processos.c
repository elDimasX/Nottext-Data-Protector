
// Obt�m a informa��o de um processo
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

// Usado para terminar processos
PVOID PsGetProcessSectionBaseAddress(PEPROCESS Process);
NTSTATUS MmUnmapViewOfSection(PEPROCESS Process, PVOID BaseAddress);

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

		// Se n�o conseguir salvar as informa��es do processo no PFILE_OBJECT
		if (!NT_SUCCESS(PsReferenceProcessFilePointer(Processo, &ObjetoArquivo)))
		{
			// N�o retorne nada
			return NULL;
		}

		// Se n�o conseguir obter o local do driver ("C:\") do arquivo
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
/// Ignora um processo de acordo com as exce��es do usermode
/// </summary>
/// 
/// <param name="Processo">Processo para verificar</param>
/// 
/// <returns>Retorna um bool para ignorar ou n�o</returns>
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
/// Verifica se � um processo original do Nottext
/// </summary>
/// 
/// <param name="Processo">Processo para verificar</param>
/// 
/// <returns>Retorna um bool para informar se � um processo nottext ou n�o</returns>
BOOLEAN EProcessoNottext(IN PEPROCESS Processo)
{
	BOOLEAN Nottext = FALSE;

	__try
	{
		UNICODE_STRING str;
		RtlZeroMemory(&str, sizeof(UNICODE_STRING)); // Inicializa a string como vazia
		str.MaximumLength = 2048 * sizeof(WCHAR); // Define o tamanho m�ximo da string em bytes

		str.Buffer = ExAllocatePool(PagedPool, str.MaximumLength);

		// Se alocar
		if (str.Buffer)
		{
			swprintf(str.Buffer, L"\\??\\%wZ", (UNICODE_STRING*)LocalProcesso(Processo));
			str.Length = wcslen(str.Buffer) * sizeof(WCHAR);

			// Se for null
			if (wcsstr(str.Buffer, L"\\??\\(null)"))
			{
				// Pare
				ExFreePool(str.Buffer);
				return Nottext;
			}

			// Vari�veis
			HANDLE Alca;
			OBJECT_ATTRIBUTES Atributos;
			IO_STATUS_BLOCK Io;
			FILE_NETWORK_OPEN_INFORMATION Info;
			LARGE_INTEGER li;
			TIME_FIELDS tempo;

			// Inicie o atributo
			InitializeObjectAttributes(&Atributos, &str, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

			// Abra o arquivo com permiss�es de leitura
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
				ExFreePool(str.Buffer);
				return Nottext;
			}

			// Obtenha as informa��es
			Status = ZwQueryInformationFile(Alca, &Io, &Info, sizeof(Info), FileNetworkOpenInformation);

			// Se falhar
			if (!NT_SUCCESS(Status))
			{
				ExFreePool(str.Buffer);
				ZwClose(Alca);
				return Nottext;
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

				// Data da �ltima modifica��o do Nottext
				if (
					strstr(DiaMesAno, "20/02/2023")
					)
				{
					Nottext = TRUE;
				}

				// Libere
				ExFreePool(DiaMesAno);
			}

			ExFreePool(str.Buffer);
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER) {}

	return Nottext;
}
