
#include "cabecalho.h"

NTSTATUS Unload(IN FLT_FILTER_UNLOAD_FLAGS Flags)
{
	// Remova
	RemoverMonitorDeRegistro();

	// Remove o monitoramento de processos
	RemoverMonitorDeProcessos();

	// Remova o filtro
	FltUnregisterFilter(Filtro);

	// Delete o dispositivo
	IoDeleteDevice(DispositivoGlobal);

	// Delete o link simbolico
	IoDeleteSymbolicLink(&SymbolickNome);

	// Limpe as listas da memória
	LimparLista(&ObjetosSomenteLeitura);
	LimparLista(&ObjetosBloquear);
	LimparLista(&ObjetosOcultar);
	LimparLista(&ObjetosNaoExecucao);
	LimparLista(&ObjetosProtegerProcesso);
	LimparLista(&ProcessosPermitidos);

	// Remova o monitoramento de imagens
	PsRemoveLoadImageNotifyRoutine(ImageLoadCallback);

	return STATUS_SUCCESS;
}

/// <summary>
/// Atualiza a lista e configurações
/// </summary>
VOID AtualizarListas()
{

	// Se não existir, a proteção está habilitada
	ProtecaoHabilitada = !ArquivoExiste(&ArquivoProtecaoHabilitada, FALSE);

	// Verifique
	TerminarProcessos = ArquivoExiste(&ArquivoFinalizarProcesso, FALSE);
	MessageBox = ArquivoExiste(&ArquivoMessageBox, FALSE);

	// Leia o arquivo de somente leitura e adicione a lista
	LerArquivo(&ArquivoSomenteLeitura, &ObjetosSomenteLeitura);

	// Leia o arquivo de bloqueio e adicione a lista
	LerArquivo(&ArquivoBloquear, &ObjetosBloquear);

	// Leia o arquivo de ocultação e adiciona a lista
	LerArquivo(&ArquivoOcultar, &ObjetosOcultar);

	// Leia o arquivo de não executar e adicione a lista
	LerArquivo(&ArquivoNaoExecucao, &ObjetosNaoExecucao);

	// Leia o arquivo de proteger processos
	LerArquivo(&ArquivoProtegerFinalizar, &ObjetosProtegerProcesso);

	// Leia os processos permitidos e adicione a lista
	LerArquivo(&ArquivoProcessos, &ProcessosPermitidos);

}

/// <summary>
/// Lê todas as configurações dos arquivos
/// </summary>
VOID LerConfiguracoes()
{
	LendoArquivos = TRUE;

	// Instale a proteção pros drivers
	InstalarTodaAProtecaoProDriver();

	__try {

		// Se conseguiu ler
		if (ConseguiuLerNoBoot == TRUE)
		{
			AtualizarListas();
		}

		// Se não conseguiu
		else
		{
			// Verifique se a pasta existe
			BOOLEAN Existe = ArquivoExiste(&Pasta, TRUE);

			// Atualize
			if (Existe == TRUE)
			{
				ConseguiuLerNoBoot = TRUE;

				// Atualize
				AtualizarListas();
			}
		}
	}
	__except (EXCEPTION_EXECUTE_HANDLER) {}

	LendoArquivos = FALSE;
}

NTSTATUS DriverEntry(IN PDRIVER_OBJECT DriverObject, IN PUNICODE_STRING RegistryPath)
{

	/////////////////////////////////////////////////////////////////
	NTSTATUS Status = STATUS_SUCCESS;

	// Crie o dispositivo para as comunicações
	Status = IoCreateDevice(
		DriverObject, // DriverObject
		0,
		&DispositivoNome, // Nome do dispositivo
		FILE_DEVICE_UNKNOWN,
		NULL,
		TRUE, // Somente uma conexão por vez
		&DispositivoGlobal // Salve a sessão aqui
	);

	// Se não conseguir
	if (!NT_SUCCESS(Status))
	{
		// Função de sair
		goto sair;
	}

	// Crie o link simbolico
	Status = IoCreateSymbolicLink(&SymbolickNome, &DispositivoNome);

	// Se falhar
	if (!NT_SUCCESS(Status))
	{
		// Saia
		IoDeleteSymbolicLink(&SymbolickNome);
		goto sair;
	}

	// Registre o minifiltro
	Status = FltRegisterFilter(DriverObject, &RegistroMinifiltro, &Filtro);

	if (!NT_SUCCESS(Status))
	{
		goto sair;
	}

	// Inicie o filtro
	Status = FltStartFiltering(Filtro);

	if (!NT_SUCCESS(Status))
	{
		goto sair;
	}

	// Para bloquear execuções ou proteger processos
	Status = InstalarMonitorDeProcessos();

	if (!NT_SUCCESS(Status))
	{
		goto sair;
	}

	// Configure para receber mensagens
	DriverObject->MajorFunction[IRP_MJ_CREATE] = Criado;
	DriverObject->MajorFunction[IRP_MJ_CLOSE] = Fechado;

	// Inicie a lista
	InitializeListHead(&ObjetosSomenteLeitura);
	InitializeListHead(&ObjetosBloquear);
	InitializeListHead(&ObjetosOcultar);
	InitializeListHead(&ObjetosNaoExecucao);
	InitializeListHead(&ObjetosProtegerProcesso);
	InitializeListHead(&ProcessosPermitidos);

	// Inicie o MUTEXT
	KeInitializeMutex(&Mutex, 0);
	KeInitializeMutex(&MutexLerArquivo, 0);

	// Proteção de registro
	InstalarMonitoracaoDeRegistro(DriverObject);

	// Leia
	LerConfiguracoes();

	// Registre as operações, apenas para já tentarmos ler o arquivo de configurações
	PsSetLoadImageNotifyRoutine(ImageLoadCallback);

	return Status;

sair:

	if (Filtro != NULL)
		FltUnregisterFilter(Filtro);

	if (DispositivoGlobal != NULL)
		IoDeleteDevice(DispositivoGlobal);

	return Status;
}
