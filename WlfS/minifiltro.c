
/// <summary>
/// Quando uma aquisição é criada
/// </summary>
/// 
/// <param name="Data">Data da operação</param>
/// <param name="Objeto">Ponteiros opacos para os objetos associados a uma operação</param>
/// <param name="Contexto">Contexto</param>
/// 
/// <returns>Retornar um STATUS, se é pra bloquear a operação ou continuar</returns>
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreCreate(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
)
{
	

	// Retornar
	NTSTATUS RetornarStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

	/*
	// Se estiver executando em nível de IRQL alto
	// https://docs.microsoft.com/en-us/windows-hardware/drivers/kernel/managing-hardware-priorities
	if (KeGetCurrentIrql() != PASSIVE_LEVEL
		)
	{
		// Pare
		return RetornarStatus;
	}

	// Outras verificações
	if (
		(Data->Iopb->IrpFlags & IRP_PAGING_IO) ||
		(Data->Iopb->IrpFlags & IRP_SYNCHRONOUS_PAGING_IO) ||
		IoGetTopLevelIrp()
	)
	{
		return RetornarStatus;
	}

	if (
		// Ignore operações em pastas já abertas
		FlagOn(Data->Iopb->OperationFlags, SL_OPEN_TARGET_DIRECTORY) ||

		// Pule arquivos de paginação
		FlagOn(Data->Iopb->OperationFlags, SL_OPEN_PAGING_FILE) ||

		// Ignore
		FlagOn(Data->Iopb->TargetFileObject->Flags, FO_VOLUME_OPEN) ||

		// Ignore
		FlagOn(Data->Iopb->Parameters.Create.Options, FILE_OPEN_BY_FILE_ID)
	)
	{
		// Detectou alguma operação inválida
		return RetornarStatus;
	}
	*/

	// Nome completo
	char* NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	// Inicie
	RtlInitUnicodeString(&LetraDisco, NULL);

	__try
	{
		

		// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
		PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, não podemos continuar
		if (!NT_SUCCESS(Status))
			return RetornarStatus;

		// Uma verificação de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
		Status = FltParseFileNameInformation(NomeInformacao);

		// Falha, não podemos continuar
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
		if (NT_SUCCESS(Status))
		{
			// Pegue o nome do disco
			Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);
		}

		// Tamanho
		ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

		// Aloque
		NomeCompleto = ExAllocatePoolWithTag(PagedPool, TamanhoNomeArquivo + LetraDisco.Length, 'phee');

		// Se não alocar
		if (!NomeCompleto)
		{

			goto sair;
		}

		// Falhou em obter o nome do Disco, então, apenas crie o \\ antes
		if (!NT_SUCCESS(Status))
		{
			// Nome sem disco
			Status = RtlStringCchPrintfA(
				NomeCompleto,
				2 + TamanhoNomeArquivo,
				"\\%wZ", &Data->Iopb->TargetFileObject->FileName
			);
		}
		else {

			// Nome com disco
			Status = RtlStringCchPrintfA(
				NomeCompleto,
				LetraDisco.Length + TamanhoNomeArquivo,
				"%wZ%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName
			);
		}

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			goto sair;
		}

		// Processo
		PEPROCESS Processo = IoThreadToProcess(Data->Thread);

		// Se for a pasta de configurações
		if (
			strstr(
				NomeCompleto,
				"C:\\Windows\\System32\\sas-Spoiler-dism"

			))
		{
			// Se não for o nottext
			if (Processo != ProcessoNottextBackup)
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

		// Faça essas verificações antes, porque é só uma.
		// Depois vem 3 verificações
		if (ProtecaoHabilitada == TRUE && !ProcessoIgnorarPeloUserMode(Processo) && Processo != ProcessoNottextBackup)
		{

			BOOLEAN Ocultar = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosOcultar);

			if (Ocultar == TRUE)
			{
				ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Error!", MB_OK);

				Data->IoStatus.Status = STATUS_OBJECT_NAME_NOT_FOUND;
				Data->IoStatus.Information = 0;
				RetornarStatus = FLT_PREOP_COMPLETE;
			}
			// Não achou, vamos pro próximo
			else {

				// Para sabermos se é pra proteger ou não
				BOOLEAN BloquearArquivo = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosBloquear);

				// Se for para bloquear
				if (BloquearArquivo == TRUE)
				{
					ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este objeto está protegido, e não pode ser acessado.", MB_OK);

					Data->IoStatus.Status = STATUS_ACCESS_DENIED;
					Data->IoStatus.Information = 0;
					RetornarStatus = FLT_PREOP_COMPLETE;
				}

				// Não é pra bloquear
				else if (

					// Se o arquivo estiver sendo deletado
					FlagOn(Data->Iopb->Parameters.Create.Options, FILE_DELETE_ON_CLOSE) ||

					// OU modificando o arquivo
					(Data->Iopb->Parameters.Create.SecurityContext->DesiredAccess &
						(
							FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES |
							FILE_WRITE_EA | FILE_CREATE | FILE_CREATED |
							FILE_APPEND_DATA | DELETE | WRITE_DAC | WRITE_OWNER
							)
						)
					)
				{
					// Agora, por último, verifique o somente-leitura
					BOOLEAN SomenteLeitura = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosSomenteLeitura);

					if (SomenteLeitura == TRUE)
					{
						ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este objeto está protegido, e não pode ser modificado.", MB_OK);

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
/// Quando uma pasta é aberta
/// 
/// 
/// Todo crédito para esse código vai para:
/// https://bbs.kanxue.com/thread-226174.htm, apenas adaptei a minha funcionalidade
/// 
/// Dica: se você não encontrar informações sobre desenvolvimento de drivers
/// para Windows, procure sua dúvida em chinês, no Google, você vai encontrar
/// muitas coisas úteis :)
/// </summary>
/// 
/// <param name="Data">Informações da operação</param>
/// <param name="Objeto">Objeto</param>
/// <param name="Contexto">Contexto da operação</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
FLT_POSTOP_CALLBACK_STATUS MiniFiltroControlsPost(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID Contexto,
	IN FLT_POST_OPERATION_FLAGS Flags
) {

	// Retornar
	NTSTATUS RetornarStatus = FLT_POSTOP_FINISHED_PROCESSING;

	// Se estiver executando em nível de IRQL alto
	// E outras verificações
	if (
		!NT_SUCCESS(Data->IoStatus.Status) ||
		KeGetCurrentIrql() != PASSIVE_LEVEL ||
		ProtecaoHabilitada == FALSE
	)
	{
		// Pare
		return RetornarStatus;
	}

	if (
		FlagOn(Flags, FLTFL_POST_OPERATION_DRAINING))
	{
		/* Se o bit Flags for FLTFL_POST_OPERATION_DRAINING e as operações finais comuns forem proibidas, o valor de retorno deverá ser FLT_POSTOP_FINISHED_PROCESSING */
		return RetornarStatus;
	}

	// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
	PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

	// Nome completo
	char* NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	// Inicie
	RtlInitUnicodeString(&LetraDisco, NULL);

	// Váriaveis iniciais
	ULONG proximoDesvio = 0;
	int modificado = 0;
	int removerTodasAsEntradas = 1;

	__try {


		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, não podemos continuar
		if (!NT_SUCCESS(Status))
		{
			return RetornarStatus;
		}

		// Solicitação de consulta de diretório, o tipo de mensagem varia de acordo com o sistema de arquivos
		if (Data->Iopb->MinorFunction == IRP_MN_QUERY_DIRECTORY &&
			Data->Iopb->Parameters.DirectoryControl.QueryDirectory.FileInformationClass == FileIdBothDirectoryInformation &&
			Data->Iopb->Parameters.DirectoryControl.QueryDirectory.Length > 0
			)
		{

			// Uma verificação de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
			Status = FltParseFileNameInformation(NomeInformacao);

			// Falha, não podemos continuar
			if (!NT_SUCCESS(Status))
			{
				FltReleaseFileNameInformation(NomeInformacao);
				goto sair;
			}

			// Libere
			FltReleaseFileNameInformation(NomeInformacao);

			// Objeto de dispositivo
			PDEVICE_OBJECT DiscoObjeto = NULL;

			// Retorne o objeto de dispositivo
			Status = FltGetDiskDeviceObject(Objeto->Volume, &DiscoObjeto);

			// Se conseguir
			if (NT_SUCCESS(Status))
			{
				// Pegue o nome do disco
				Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);
			}

			BOOLEAN pegouDisco = TRUE;

			if (!NT_SUCCESS(Status))
			{
				pegouDisco = FALSE;
			}

			// Outras
			PFILE_ID_BOTH_DIR_INFORMATION informacaoAtualDoArquivo, proximaInformacao, informacaoAnterior = 0;


			// Processo
			PEPROCESS Processo = IoThreadToProcess(Data->Thread);

			if (Processo == ProcessoNottextBackup)
			{
				goto sair;
			}

			// Tamanho
			ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

			// Aloque
			NomeCompleto = ExAllocatePoolWithTag(PagedPool, 2048, 'phee');

			/*
			if (strFilePathName.Buffer[strFilePathName.Length / sizeof(wchar_t) - sizeof(char)] != '\\')
			{
				strFilePathName.Buffer[strFilePathName.Length / sizeof(wchar_t)] = '\\';
			}
			FltReleaseFileNameInformation(NomeInformacao);
			*/

			if (Data->Iopb->Parameters.DirectoryControl.QueryDirectory.MdlAddress != NULL)
			{
				informacaoAtualDoArquivo = (PFILE_BOTH_DIR_INFORMATION)MmGetSystemAddressForMdlSafe(
					Data->Iopb->Parameters.DirectoryControl.QueryDirectory.MdlAddress,
					NormalPagePriority);
			}
			else
			{
				informacaoAtualDoArquivo = (PFILE_ID_BOTH_DIR_INFORMATION)Data->Iopb->Parameters.DirectoryControl.QueryDirectory.DirectoryBuffer;
			}

			if (informacaoAtualDoArquivo == NULL)
			{
				goto sair;
			}

			informacaoAnterior = informacaoAtualDoArquivo;

			do {

				proximoDesvio = informacaoAtualDoArquivo->NextEntryOffset;// Deslocamento do próximo nó
				proximaInformacao = (PFILE_ID_BOTH_DIR_INFORMATION)((PCHAR)(informacaoAtualDoArquivo)+proximoDesvio);// Próximo nó

				// Unicode, para pegar o nome do arquivo sem lixo
				UNICODE_STRING NomeCorreto;
				RtlInitUnicodeString(&NomeCorreto, informacaoAtualDoArquivo->FileName);

				// O tamanho para não vir lixo, e o máximo está ok
				NomeCorreto.Length = informacaoAtualDoArquivo->FileNameLength;
				NomeCorreto.MaximumLength = 2048 * sizeof(WCHAR);

				// Se for apenas 2, é uma barra invertida
				if (Data->Iopb->TargetFileObject->FileName.Length == 2)
				{
					// Com nome do disco
					if (pegouDisco == TRUE)
					{
						Status = RtlStringCchPrintfA(
							NomeCompleto, LetraDisco.Length + TamanhoNomeArquivo + 2 + informacaoAtualDoArquivo->FileNameLength, "%wZ\\%wZ", &LetraDisco, &NomeCorreto
						);
					}
					else {

						// Sem nome do disco
						Status = RtlStringCchPrintfA(
							NomeCompleto, 2 + TamanhoNomeArquivo + 2 + informacaoAtualDoArquivo->FileNameLength, "\\%wZ", &NomeCorreto
						);
					}
				}
				else {

					// Com nome do disco
					if (pegouDisco == TRUE)
					{
						Status = RtlStringCchPrintfA(

							// Nome completo
							NomeCompleto,

							// Tamanho exato
							LetraDisco.Length + TamanhoNomeArquivo + 2 + informacaoAtualDoArquivo->FileNameLength,

							// Converta
							"%wZ%wZ\\%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName, & NomeCorreto
						);
					}

					else {

						Status = RtlStringCchPrintfA(
							NomeCompleto,
							2 + TamanhoNomeArquivo + 2 + informacaoAtualDoArquivo->FileNameLength,
							"\\%wZ\\%wZ", &Data->Iopb->TargetFileObject->FileName, &NomeCorreto
						);
					}

				}

				// Se falhar
				if (!NT_SUCCESS(Status))
				{
					goto sair;
				}

				BOOLEAN Ocultar = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosOcultar);

				// Se for para ocultar
				if (Ocultar == TRUE)
				{

					// Se for para ignorar esse processo
					if (!ProcessoIgnorarPeloUserMode(Processo))
					{
						ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Error!", MB_OK);

						// Determine se é o último nó
						if (proximoDesvio == 0)
						{
							informacaoAnterior->NextEntryOffset = 0;
						}
						else
						{
							informacaoAnterior->NextEntryOffset = (ULONG)((PCHAR)informacaoAtualDoArquivo - (PCHAR)informacaoAnterior) + proximoDesvio;
						}

						modificado = 1;
					}
				}

				else
				{
					removerTodasAsEntradas = 0;
					informacaoAnterior = informacaoAtualDoArquivo;
				}
				informacaoAtualDoArquivo = proximaInformacao;

			} while (proximoDesvio != 0);
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER) {

	}

sair:

	// Libere
	if (&LetraDisco != NULL)
		RtlFreeUnicodeString(&LetraDisco);

	// Libere
	if (NomeCompleto != NULL)
		ExFreePoolWithTag(NomeCompleto, 'phee');

	// Se tiver modificado
	if (modificado)
	{
		if (removerTodasAsEntradas)
		{
			Data->IoStatus.Status = STATUS_NO_MORE_FILES;
		}
		else {
			FltSetCallbackDataDirty(Data);
		}
	}

	return RetornarStatus;
}

/// <summary>
/// Quando um arquivo é modificado
/// </summary>
/// 
/// <param name="Data">Data da operação</param>
/// <param name="Objeto">Ponteiros opacos para os objetos associados a uma operação</param>
/// <param name="Contexto">Contexto</param>
/// 
/// <returns>Retornar um STATUS, se é pra bloquear a operação ou continuar</returns>
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreSetInformation(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
)
{
	// Retornar
	NTSTATUS RetornarStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

	// Se estiver executando em nível de IRQL alto
	// https://docs.microsoft.com/en-us/windows-hardware/drivers/kernel/managing-hardware-priorities
	if (KeGetCurrentIrql() != PASSIVE_LEVEL || ProtecaoHabilitada == FALSE
		)
	{
		// Pare
		return RetornarStatus;
	}

	// Nome completo
	char* NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	// Inicie
	RtlInitUnicodeString(&LetraDisco, NULL);

	__try
	{
		// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
		PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, não podemos continuar
		if (!NT_SUCCESS(Status))
			return RetornarStatus;

		// Uma verificação de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
		Status = FltParseFileNameInformation(NomeInformacao);

		// Falha, não podemos continuar
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

		// Se conseguir
		if (NT_SUCCESS(Status))
		{
			// Pegue o nome do disco
			Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);

		}

		// Tamanho
		ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

		// Aloque
		NomeCompleto = ExAllocatePoolWithTag(PagedPool, TamanhoNomeArquivo + LetraDisco.Length, 'phee');

		// Se não alocar
		if (!NomeCompleto)
		{
			goto sair;
		}

		// Com nome do disco
		if (NT_SUCCESS(Status))
		{
			// Converta o nome
			Status = RtlStringCchPrintfA(
				NomeCompleto,
				LetraDisco.Length + TamanhoNomeArquivo,
				"%wZ%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName
			);
		}
		else {

			Status = RtlStringCchPrintfA(
				NomeCompleto,
				2 + TamanhoNomeArquivo,
				"\\%wZ", &Data->Iopb->TargetFileObject->FileName
			);
		}

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			goto sair;
		}

		PEPROCESS Processo = IoThreadToProcess(Data->Thread);

		// Se for a pasta de configurações
		if (
			strstr(
				NomeCompleto,
				"C:\\Windows\\System32\\sas-Spoiler-dism"

			))
		{
			// Se não for o nottext
			if (Processo != ProcessoNottextBackup)
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

		// Se for para não for para ignorar 
		if (!ProcessoIgnorarPeloUserMode(Processo) && Processo != ProcessoNottextBackup && ProtecaoHabilitada == TRUE)
		{
			// Para sabermos se é pra proteger ou não
			BOOLEAN Bloquear = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosBloquear);
			BOOLEAN SomenteLeitura = FALSE;

			// Antes de verificar as 2 listas e gastar mais CPU, verifique se não
			// Achou nada na lista de bloquear
			if (Bloquear == FALSE)
			{
				// Ok, não achou nada na lista de bloquear
				// Agora, verifique se está na parte de somente-leitura
				SomenteLeitura = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosSomenteLeitura);
			}

			// Se for para proteger contra modificação
			if (SomenteLeitura == TRUE || Bloquear == TRUE && ProtecaoHabilitada == TRUE)
			{
				ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este objeto está protegido, e não pode ser modificado.", MB_OK);

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

/// <summary>
/// Quando um objeto é fechado
/// </summary>
/// 
/// <param name="Data">Data</param>
/// <param name="Objeto">Objeto da operação</param>
/// <param name="Contexto">Contexto</param>
/// 
/// <returns>Retorna um NTSTATUS para negar o acesso ou não</returns>
PFLT_PREOP_CALLBACK_STATUS MiniFiltroCleanup(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
)
{
	// Retornar
	NTSTATUS RetornarStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

	// Se estiver executando em nível de IRQL alto
	// https://docs.microsoft.com/en-us/windows-hardware/drivers/kernel/managing-hardware-priorities
	if (KeGetCurrentIrql() != PASSIVE_LEVEL || ProtecaoHabilitada == FALSE
		)
	{
		// Pare
		return RetornarStatus;
	}

	// Nome completo
	char* NomeCompleto = NULL;

	// UNICODE
	UNICODE_STRING LetraDisco;

	// Inicie
	RtlInitUnicodeString(&LetraDisco, NULL);

	__try
	{
		// Usaremos para verificar se conseguimos obter o nome do arquivo corretamente
		PFLT_FILE_NAME_INFORMATION NomeInformacao = NULL;

		// Tente obter o nome do arquivo
		NTSTATUS Status = FltGetFileNameInformation(Data, FLT_FILE_NAME_NORMALIZED | FLT_FILE_NAME_QUERY_DEFAULT, &NomeInformacao);

		// Falha, não podemos continuar
		if (!NT_SUCCESS(Status))
			return RetornarStatus;

		// Uma verificação de certeza, queremos ter 100% de certeza que o nome foi passado corretamente
		Status = FltParseFileNameInformation(NomeInformacao);

		// Falha, não podemos continuar
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

		// Se conseguir
		if (NT_SUCCESS(Status))
		{
			// Pegue o nome do disco
			Status = RtlVolumeDeviceToDosName(DiscoObjeto, &LetraDisco);
		}

		// Tamanho
		ULONG TamanhoNomeArquivo = Data->Iopb->TargetFileObject->FileName.Length;

		// Aloque
		NomeCompleto = ExAllocatePoolWithTag(PagedPool, TamanhoNomeArquivo + LetraDisco.Length, 'phee');

		// Se não alocar
		if (!NomeCompleto)
		{
			goto sair;
		}

		// Com nome do disco
		if (NT_SUCCESS(Status))
		{
			// Converta o nome
			Status = RtlStringCchPrintfA(
				NomeCompleto,
				LetraDisco.Length + TamanhoNomeArquivo,
				"%wZ%wZ", &LetraDisco, &Data->Iopb->TargetFileObject->FileName
			);
		}
		else {

			Status = RtlStringCchPrintfA(
				NomeCompleto,
				2 + TamanhoNomeArquivo,
				"\\%wZ", &Data->Iopb->TargetFileObject->FileName
			);
		}

		// Se falhar
		if (!NT_SUCCESS(Status))
		{
			goto sair;
		}

		PEPROCESS Processo = IoThreadToProcess(Data->Thread);

		// Se estiver tudo OK
		if (ProtecaoHabilitada == TRUE && !ProcessoIgnorarPeloUserMode(Processo) && Processo != ProcessoNottextBackup)
		{

			// Para sabermos se é pra proteger ou não
			BOOLEAN Bloquear = ObjetoProtegido((PCHAR*)NomeCompleto, &ObjetosBloquear);

			// Se for para proteger contra modificação
			if (Bloquear == TRUE)
			{
				ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este objeto está protegido, e não pode ser fechado.", MB_OK);

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




