
#include "cabecalho.h"

NTSTATUS Unload(IN FLT_FILTER_UNLOAD_FLAGS Flags)
{
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
	LimparLista(&ProcessosPermitidos);

	return STATUS_SUCCESS;
}

NTSTATUS DriverEntry(IN PDRIVER_OBJECT DriverObject, IN PUNICODE_STRING RegistryPath)
{

	/////////////////////////////////////////////////////////////////
	NTSTATUS Status = STATUS_SUCCESS;

	// Se não existir, a proteção está habilitada
	ProtecaoHabilitada = !ArquivoExiste(&ArquivoProtecaoHabilitada);

	// Verifique
	TerminarProcessos = ArquivoExiste(&ArquivoFinalizarProcesso);
	MessageBox = ArquivoExiste(&ArquivoMessageBox);

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

	// Inicie a lista
	InitializeListHead(&ObjetosSomenteLeitura);
	InitializeListHead(&ObjetosBloquear);
	InitializeListHead(&ObjetosOcultar);
	InitializeListHead(&ProcessosPermitidos);

	// Inicie o MUTEXT
	KeInitializeMutex(&Mutex, 0);

	// Leia o arquivo de somente leitura e adicione a lista
	LerArquivo(&ArquivoSomenteLeitura, &ObjetosSomenteLeitura);

	// Leia o arquivo de bloqueio e adicione a lista
	LerArquivo(&ArquivoBloquear, &ObjetosBloquear);

	// Leia o arquivo de ocultar e adicione a lista
	LerArquivo(&ArquivoOcultar, &ObjetosOcultar);

	// Leia os processos permitidos e adicione a lista
	LerArquivo(&ArquivoProcessos, &ProcessosPermitidos);

	// Configure para receber mensagens
	DriverObject->MajorFunction[IRP_MJ_CREATE] = Criado;
	DriverObject->MajorFunction[IRP_MJ_CLOSE] = Fechado;

	return Status;

sair:

	if (Filtro != NULL)
		FltUnregisterFilter(Filtro);

	if (DispositivoGlobal != NULL)
		IoDeleteDevice(DispositivoGlobal);

	return Status;
}
