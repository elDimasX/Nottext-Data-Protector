
/// <summary>
/// Quando uma aquisi��o � criada
/// </summary>
/// 
/// <param name="Data">Data da opera��o</param>
/// <param name="Objeto">Ponteiros opacos para os objetos associados a uma opera��o</param>
/// <param name="Contexto">Contexto</param>
/// 
/// <returns>Retornar um STATUS, se � pra bloquear a opera��o ou continuar</returns>
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreCreate(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
)
{
	// Retornar
	NTSTATUS RetornarStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

	// Se estiver executando em n�vel de IRQL alto
	// https://docs.microsoft.com/en-us/windows-hardware/drivers/kernel/managing-hardware-priorities
	if (KeGetCurrentIrql() != PASSIVE_LEVEL
		)
	{
		// Pare
		return RetornarStatus;
	}

	// Outras verifica��es
	if (
		(Data->Iopb->IrpFlags & IRP_PAGING_IO) ||
		(Data->Iopb->IrpFlags & IRP_SYNCHRONOUS_PAGING_IO) ||
		IoGetTopLevelIrp()
	)
	{
		return RetornarStatus;
	}

	if (
		// Ignore opera��es em pastas j� abertas
		FlagOn(Data->Iopb->OperationFlags, SL_OPEN_TARGET_DIRECTORY) ||

		// Pule arquivos de pagina��o
		FlagOn(Data->Iopb->OperationFlags, SL_OPEN_PAGING_FILE) ||

		// Ignore
		FlagOn(Data->Iopb->TargetFileObject->Flags, FO_VOLUME_OPEN) ||

		// Ignore
		FlagOn(Data->Iopb->Parameters.Create.Options, FILE_OPEN_BY_FILE_ID)
	)
	{
		// Detectou alguma opera��o inv�lida
		return RetornarStatus;
	}

	// Nome completo
	PUNICODE_STRING NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	__try
	{
		// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
		PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, n�o podemos continuar
		if (!NT_SUCCESS(Status))
			return RetornarStatus;

		// Uma verifica��o de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
		Status = FltParseFileNameInformation(NomeInformacao);

		// Falha, n�o podemos continuar
		if (!NT_SUCCESS(Status))
		{
			FltReleaseFileNameInformation(NomeInformacao);
			goto sair;
		}

		// Libere, o nome do arquivo vai vir corretamente
		FltReleaseFileNameInformation(NomeInformacao);

		// Objeto de dispositivo
		PDEVICE_OBJECT DiscoObjeto = NULL;

		// Retorne o objeto de dispositivo
		Status = FltGetDiskDeviceObject(Objeto->Volume, &DiscoObjeto);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			return RetornarStatus;
		}

		// Inicie
		RtlInitUnicodeString(&LetraDisco, NULL);

		// Pegue o nome do disco
		Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			return RetornarStatus;
		}

		// Tamanho
		ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

		// Aloque
		NomeCompleto = ExAllocatePoolWithTag(PagedPool, TamanhoNomeArquivo + LetraDisco.Length, 'phee');

		// Se n�o alocar
		if (!NomeCompleto)
		{
			goto sair;
		}

		// Converta o nome
		Status = RtlStringCchPrintfA(
			NomeCompleto,
			LetraDisco.Length + TamanhoNomeArquivo,
			"%wZ%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName
		);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			goto sair;
		}

		// Processo
		PEPROCESS Processo = IoThreadToProcess(Data->Thread);

		// Se for a pasta de configura��es
		if (
			strstr(
				NomeCompleto,
				"C:\\SystemGuards"

		))
		{
			// Se n�o for o nottext
			if (!ProcessoNottext(Processo))
			{
				// Se o kernel estiver lendo algum arquivo
				if (LendoArquivos == FALSE)
				{
					Data->IoStatus.Status = STATUS_ACCESS_DENIED;
					Data->IoStatus.Information = 0;
					RetornarStatus = FLT_PREOP_COMPLETE;
				}
			}

			goto sair;
		}

		// Para sabermos se � pra proteger ou n�o
		BOOLEAN BloquearArquivo = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosBloquear);

		// Se for para bloquear o arquivo
		if (BloquearArquivo == TRUE)
		{
			// Se nao for para ignorar e a prote��o estiver habilitada
			if (!ProcessoIgnorarPeloUserMode(Processo) && ProtecaoHabilitada == TRUE)
			{
				Data->IoStatus.Status = STATUS_ACCESS_DENIED;
				Data->IoStatus.Information = 0;
				RetornarStatus = FLT_PREOP_COMPLETE;
			}
		}

		// Se n�o for
		else {

			if (

				// Se o arquivo estiver sendo deletado
				FlagOn(Data->Iopb->Parameters.Create.Options, FILE_DELETE_ON_CLOSE) ||

				// OU modificando o arquivo
				(Data->Iopb->Parameters.Create.SecurityContext->DesiredAccess &
					(FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA |
						FILE_APPEND_DATA | DELETE | WRITE_DAC | WRITE_OWNER))
				)
			{
				BOOLEAN SomenteLeitura = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosSomenteLeitura);

				if (SomenteLeitura == TRUE)
				{
					// Se n�o for para ignorar e a prote��o estiver habilitada
					if (!ProcessoIgnorarPeloUserMode(Processo) && ProtecaoHabilitada == TRUE)
					{
						Data->IoStatus.Status = STATUS_ACCESS_DENIED;
						Data->IoStatus.Information = 0;
						RetornarStatus = FLT_PREOP_COMPLETE;
					}
				}
			}
		}

	}
	__except (EXCEPTION_EXECUTE_HANDLER) {}

// Sai
sair:

	if (&LetraDisco != NULL)
		RtlFreeUnicodeString(&LetraDisco);

	if (NomeCompleto != NULL)
		ExFreePoolWithTag(NomeCompleto, 'phee');

	return RetornarStatus;
}

/// <summary>
/// Quando um arquivo � modificado
/// </summary>
/// 
/// <param name="Data">Data da opera��o</param>
/// <param name="Objeto">Ponteiros opacos para os objetos associados a uma opera��o</param>
/// <param name="Contexto">Contexto</param>
/// 
/// <returns>Retornar um STATUS, se � pra bloquear a opera��o ou continuar</returns>
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreSetInformation(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
)
{
	// Retornar
	NTSTATUS RetornarStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

	// Se estiver executando em n�vel de IRQL alto
	// https://docs.microsoft.com/en-us/windows-hardware/drivers/kernel/managing-hardware-priorities
	if (KeGetCurrentIrql() != PASSIVE_LEVEL || ProtecaoHabilitada == FALSE
		)
	{
		// Pare
		return RetornarStatus;
	}

	// Nome completo
	PUNICODE_STRING NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	__try
	{
		// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
		PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, n�o podemos continuar
		if (!NT_SUCCESS(Status))
			return RetornarStatus;

		// Uma verifica��o de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
		Status = FltParseFileNameInformation(NomeInformacao);

		// Falha, n�o podemos continuar
		if (!NT_SUCCESS(Status))
		{
			FltReleaseFileNameInformation(NomeInformacao);
			goto sair;
		}

		// Libere, o nome do arquivo vai vir corretamente
		FltReleaseFileNameInformation(NomeInformacao);

		// Objeto de dispositivo
		PDEVICE_OBJECT DiscoObjeto = NULL;

		// Retorne o objeto de dispositivo
		Status = FltGetDiskDeviceObject(Objeto->Volume, &DiscoObjeto);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			return RetornarStatus;
		}

		// Inicie
		RtlInitUnicodeString(&LetraDisco, NULL);

		// Pegue o nome do disco
		Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			return RetornarStatus;
		}

		// Tamanho
		ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

		// Aloque
		NomeCompleto = ExAllocatePoolWithTag(PagedPool, TamanhoNomeArquivo + LetraDisco.Length, 'phee');

		// Se n�o alocar
		if (!NomeCompleto)
		{
			goto sair;
		}

		// Converta o nome
		Status = RtlStringCchPrintfA(
			NomeCompleto,
			LetraDisco.Length + TamanhoNomeArquivo,
			"%wZ%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName
		);

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			goto sair;
		}

		PEPROCESS Processo = IoThreadToProcess(Data->Thread);

		// Se for a pasta de configura��es
		if (
			strstr(
				NomeCompleto,
				"C:\\SystemGuards"

			))
		{
			// Se n�o for o nottext
			if (!ProcessoNottext(Processo))
			{
				// Se o kernel estiver lendo algum arquivo
				if (LendoArquivos == FALSE)
				{
					Data->IoStatus.Status = STATUS_ACCESS_DENIED;
					Data->IoStatus.Information = 0;
					RetornarStatus = FLT_PREOP_COMPLETE;
				}
			}

			goto sair;
		}

		// Para sabermos se � pra proteger ou n�o
		BOOLEAN SomenteLeitura = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosSomenteLeitura);

		// Se for para proteger contra modifica��o
		if (SomenteLeitura == TRUE)
		{
			// Se n�o for para ignorar e a prote��o estiver habilitada
			if (!ProcessoIgnorarPeloUserMode(Processo) && ProtecaoHabilitada == TRUE)
			{
				Data->IoStatus.Status = STATUS_ACCESS_DENIED;
				Data->IoStatus.Information = 0;
				RetornarStatus = FLT_PREOP_COMPLETE;
			}
		}

	}
	__except (EXCEPTION_EXECUTE_HANDLER) {}

	// Sai
sair:

	if (&LetraDisco != NULL)
		RtlFreeUnicodeString(&LetraDisco);

	if (NomeCompleto != NULL)
		ExFreePoolWithTag(NomeCompleto, 'phee');

	return RetornarStatus;
}

