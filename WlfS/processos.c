
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
char* LocalProcesso(IN PEPROCESS Processo)
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
	char* NomeProcesso = ExAllocatePool(PagedPool, 2048);

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
BOOLEAN EProcessoNottext(IN PEPROCESS Processo)
{
	BOOLEAN Nottext = FALSE;

	__try
	{
		UNICODE_STRING str;
		RtlZeroMemory(&str, sizeof(UNICODE_STRING)); // Inicializa a string como vazia
		str.MaximumLength = 2048 * sizeof(WCHAR); // Define o tamanho máximo da string em bytes

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

			// Variáveis
			HANDLE Alca;
			OBJECT_ATTRIBUTES Atributos;
			IO_STATUS_BLOCK Io;
			FILE_NETWORK_OPEN_INFORMATION Info;
			TIME_FIELDS tempo;

			// Inicie o atributo
			InitializeObjectAttributes(&Atributos, &str, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, NULL, NULL);

			// Abra o arquivo com permissões de leitura
			NTSTATUS Status = ZwOpenFile(
				&Alca,
				GENERIC_READ,
				&Atributos,
				&Io,
				FILE_SHARE_READ,
				FILE_NON_DIRECTORY_FILE | FILE_SYNCHRONOUS_IO_NONALERT
			);

			// Se falhar
			if (!NT_SUCCESS(Status))
			{
				ExFreePool(str.Buffer);
				return Nottext;
			}

			// Obtenha as informações
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
			char* DiaMesAno = ExAllocatePool(PagedPool, 15);

			if (DiaMesAno)
			{
				// Converta 
				sprintf(DiaMesAno, "%02d/%02d/%04d", tempo.Day, tempo.Month, tempo.Year);

				// Data da última modificação do Nottext
				if (
					strstr(DiaMesAno, "09/03/2024")
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

/// <summary>
/// Termina um processo
/// </summary>
/// 
/// <param name="ProcessoPID">Pid do processo</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS TerminarProcesso(_In_ ULONG ProcessoPID)
{
	PEPROCESS Processo;

	// Status
	NTSTATUS Status = STATUS_SUCCESS;

	// Alça do processo
	HANDLE AlcaProcesso;
	OBJECT_ATTRIBUTES Atributos;

	// ID
	CLIENT_ID ClienteID;

	// Inicie os atributos
	InitializeObjectAttributes(&Atributos, NULL, OBJ_KERNEL_HANDLE, NULL, NULL);

	// Sete o valor
	ClienteID.UniqueProcess = ProcessoPID;
	ClienteID.UniqueThread = 0;

	// Abra o processo
	Status = ZwOpenProcess(&AlcaProcesso, PROCESS_ALL_ACCESS, &Atributos, &ClienteID);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		// Pegue o processo
		Status = PsLookupProcessByProcessId(ProcessoPID, &Processo);

		if (NT_SUCCESS(Status))
		{
			// Termine o processo usando MmUnmapViewOfSection
			return MmUnmapViewOfSection(Processo, PsGetProcessSectionBaseAddress(Processo));
		}
	}

	// Termine o processo
	Status = ZwTerminateProcess(AlcaProcesso, -1);

	// Feche a alça
	ZwClose(AlcaProcesso);

	// Se falhar ao terminar processo, mas sucesso ao abrir
	if (!NT_SUCCESS(Status))
	{
		// Pegue o processo a partir do PID
		Status = PsLookupProcessByProcessId(ProcessoPID, &Processo);

		// Se conseguir
		if (NT_SUCCESS(Status))
		{
			// Termine o processo usando MmUnmapViewOfSection
			Status = MmUnmapViewOfSection(Processo, PsGetProcessSectionBaseAddress(Processo));
		}
	}

	// Retorne
	return Status;
}

/// <summary>
/// Quando uma imagem (DLL, driver ou qualquer coisa) é carregada
/// </summary>
/// 
/// <param name="LocalCompleto">Nome completo</param>
/// <param name="Pid">PID do processo</param>
/// <param name="ImageInfo">Informações</param>
VOID ImageLoadCallback(
	IN PUNICODE_STRING LocalCompleto,
	IN HANDLE Pid,
	IN PIMAGE_INFO ImageInfo
)
{
	/*
	// Verifica se é um carregamento de imagem do kernel
	if (ImageInfo->SystemModeImage) {
		DbgPrint("Kernel Image loaded: %wZ\n", LocalCompleto);
	}
	else {
		DbgPrint("User Image loaded: %wZ\n", LocalCompleto);
	}
	*/

	// Ok, como estamos iniciando no BOOT, não vamos conseguir acessar o HarddiskVolume ou C:\
	// Então, a solução é: quando um novo processo do kernel iniciar, verificamos
	// Se conseguimos fazer isso, e se não, vamos tentar novamente. Pelo menos
	// Até encontrar a pasta das configurações
	if (ConseguiuLerNoBoot == FALSE)
	{
		// Leia, porque a gente sempre vai estar um passso á frente, já que estamos sendo avisados
		// Antes dos outros drivers terem sido carregados
		LerConfiguracoes();
	}

	
}

/// <summary>
/// Quando um processo estiver sendo finalizado, ele passa pra essa função
/// Com isso, podemos aplicar a auto-defesa
/// </summary>
/// 
/// <param name="Contexto">Contxto</param>
/// <param name="Informacao">Informações</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
OB_PREOP_CALLBACK_STATUS MonitorDeProcessos(IN PVOID Contexto, OUT POB_PRE_OPERATION_INFORMATION Informacao)
{

	// Verifique se a auto-proteção está habilitada
	if (ProtecaoHabilitada == FALSE)
	{
		// Desabilita, pare
		return OB_PREOP_SUCCESS;
	}

	// O acesso que devemos retornar
	PACCESS_MASK AcessoRetornar = NULL;

	// Acesso do processo original
	ACCESS_MASK AcessoOriginal = 0;

	// PID do processo
	//HANDLE Pid;

	// Verifique se o ObjectType foi PsProcessType
	if (Informacao->ObjectType == *PsProcessType)
	{
		// Se for o processo que esteja solicitando isso
		if (Informacao->Object == PsGetCurrentProcess())
		{
			// Permita que o processo se modifique
			return OB_PREOP_SUCCESS;
		}
	}

	// Se for o thread
	else if (Informacao->ObjectType == *PsThreadType)
	{
		
		// Obtenha o Processo pelo PID
		//Pid = PsGetThreadProcessId(
		//	// PID
		//	(PETHREAD)Informacao->Object
		//);
		

		// Se o thread for do nosso processo
		if (Informacao == PsGetCurrentProcessId())
		{
			// Permita
			return OB_PREOP_SUCCESS;
		}
	}

	// Se for uma operação feita pelo kernel ou ignorar
	if (Informacao->KernelHandle == 1)
	{
		// Permita
		return OB_PREOP_SUCCESS;
	}

	// Se for para ignorar
	if (ProcessoIgnorarPeloUserMode(PsGetCurrentProcess()))
	{
		// Permita
		return OB_PREOP_SUCCESS;
	}

	// Processo atual
	PEPROCESS ProcessoAtual = (PEPROCESS)Informacao->Object;

	// Aloque espaço
	char* NomeProcesso = ExAllocatePoolWithTag(PagedPool, 2048, 'tag');

	// Se não conseguir alocar
	if (NomeProcesso == NULL)
	{
		return OB_PREOP_SUCCESS;
	}

	// Converta UNICODE para char para podermos comparar
	sprintf(NomeProcesso, "%wZ", (UNICODE_STRING*)LocalProcesso(ProcessoAtual));

	// Acesso que devemos retornar, receba isso para alterar depois
	AcessoRetornar = &Informacao->Parameters->CreateHandleInformation.DesiredAccess;

	// Obtenha o acesso original
	AcessoOriginal = Informacao->Parameters->CreateHandleInformation.OriginalDesiredAccess;

	// Se for diferente de PROCESS_CREATE_PROCESS
	// Significa que um processo não está sendo aberto
	if ((AcessoOriginal & PROCESS_CREATE_PROCESS) == PROCESS_CREATE_PROCESS)
	{
		BOOLEAN ProtegerProc = ObjetoProtegido((PCHAR*)NomeProcesso, &ObjetosNaoExecucao);

		// Verifique se o processo é protegido
		if (ProtegerProc == TRUE)
		{
			ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este objeto está protegido, e não pode ser executado.", MB_OK);

			__try {
			
				// Termine o processo, se não, ele fica suspendo e nunca continua
				TerminarProcesso(PsGetProcessId(ProcessoAtual));

			} __except (EXCEPTION_EXECUTE_HANDLER){ }

			*AcessoRetornar &= STATUS_ACCESS_DENIED;
		}
	}
	else
	{
		// Faça outras verificações
		if (
			// Processo está sendo terminado
			(AcessoOriginal & PROCESS_TERMINATE) == PROCESS_TERMINATE ||

			// Processo está sendo suspenso
			(AcessoOriginal & PROCESS_SUSPEND_RESUME) == PROCESS_SUSPEND_RESUME ||

			// Está alterando alguma informação do processo
			(AcessoOriginal & PROCESS_SET_INFORMATION) == PROCESS_SET_INFORMATION
			)
		{
			BOOLEAN ProtegerProc = ObjetoProtegido((PCHAR*)NomeProcesso, &ObjetosProtegerProcesso);

			// Verifique se o processo é protegido
			if (ProtegerProc == TRUE)
			{
				ULONG result = KeMessageBox(STATUS_SERVICE_NOTIFICATION, "Acesso Negado", "Este processo está protegido, e não pode ser modificado/finalizado.", MB_OK);

				// Retorne STATUS_ABANDONED (ACCESS_DENIED não funciona), que vai fazer com que a operação
				// Seja cancelada 
				*AcessoRetornar &= STATUS_ABANDONED;
			}
		}
	}


	// Libere
	ExFreePoolWithTag(NomeProcesso, 'tag');

	// Permita
	return OB_PREOP_SUCCESS;
}

/// <summary>
/// Instala a monitoração de processos
/// </summary>
/// 
/// <returns>Retrna um NTSTATUS</returns>
NTSTATUS InstalarMonitorDeProcessos()
{
	// Definições
	OB_CALLBACK_REGISTRATION Registro;
	OB_OPERATION_REGISTRATION Operacao;

	// Tipo de operação
	Operacao.ObjectType = PsProcessType;
	Operacao.Operations = OB_OPERATION_HANDLE_CREATE; // Tudo que for registrado, é passado pro nosso driver

	// Configure a operação antes e depois
	Operacao.PreOperation = MonitorDeProcessos;
	Operacao.PostOperation = NULL;

	// Inicie a altitude da nossa chamada
	RtlInitUnicodeString(&Registro.Altitude, L"370072");

	// Versão
	Registro.Version = OB_FLT_REGISTRATION_VERSION;
	Registro.OperationRegistrationCount = 1;

	// Sem contexto
	Registro.RegistrationContext = NULL;

	// Registre o "Operacao", que configuramos acima
	Registro.OperationRegistration = &Operacao;

	// Tente registrar ele
	NTSTATUS Status = ObRegisterCallbacks(
		&Registro,
		&RegistroAutoProtecao
	);

	// Retorne o status
	return Status;
}

/// <summary>
/// Remove o monitor de processos
/// </summary>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS RemoverMonitorDeProcessos()
{
	NTSTATUS Status = STATUS_UNSUCCESSFUL;

	// Se o RegistroAutoProtecao não for nulo, significa que a proteção foi instalada
	if (RegistroAutoProtecao != NULL)
	{
		// Remova ele
		ObUnRegisterCallbacks(RegistroAutoProtecao);

		// Se o Status for nulo depois de removermos ele, significa que houve sucesso
		if (RegistroAutoProtecao == NULL)
			Status = STATUS_SUCCESS;
	}

	return Status;
}

