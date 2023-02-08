
#include <fltKernel.h>
#include <ntstrsafe.h>

///
/// Váriaveis
///

UNICODE_STRING ArquivoSomenteLeitura = RTL_CONSTANT_STRING(
	L"\\??\\C:\\SystemGuards\\pp.cat"
);

UNICODE_STRING ArquivoBloquear = RTL_CONSTANT_STRING(
	L"\\??\\C:\\SystemGuards\\pp.sys"
);

UNICODE_STRING ArquivoProcessos = RTL_CONSTANT_STRING(
	L"\\??\\C:\\SystemGuards\\pp.inf"
);

UNICODE_STRING ArquivoProtecaoHabilitada = RTL_CONSTANT_STRING(
	L"\\??\\C:\\SystemGuards\\pp.end"
);

///
/// Funções do WlfS.c
///

NTSTATUS DriverEntry(
	IN PDRIVER_OBJECT DriverObject, IN PUNICODE_STRING RegistryPath
);

NTSTATUS Unload(IN FLT_FILTER_UNLOAD_FLAGS Flags);



///
/// Funções de irp.c
///

PDEVICE_OBJECT DispositivoGlobal = NULL;

UNICODE_STRING DispositivoNome = RTL_CONSTANT_STRING(L"\\Device\\WlfS"), SymbolickNome = RTL_CONSTANT_STRING(L"\\??\\WlfS");

// Quando um IRP é criado pra nós
NTSTATUS Criado(PDEVICE_OBJECT ObjetoDispositivo, PIRP Irp);

// IRP aberto fechado
NTSTATUS Fechado(PDEVICE_OBJECT ObjetoDispositivo, PIRP Irp);


///
/// Funções de minifiltro.c
///

BOOLEAN LendoArquivos = FALSE;

// Quando uma aquisição é criada
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreCreate(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
);

// Quando um arquivo é modificado
PFLT_PREOP_CALLBACK_STATUS MiniFiltroPreSetInformation(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
);

PFLT_FILTER Filtro = NULL;

// Chamdas que queremos registrar
const FLT_OPERATION_REGISTRATION Chamadas[] =
{
	// Requisição criada
	{IRP_MJ_CREATE, 0, MiniFiltroPreCreate, NULL},
	{IRP_MJ_SET_INFORMATION, 0, MiniFiltroPreSetInformation, NULL},
	{IRP_MJ_SET_SECURITY, 0, MiniFiltroPreSetInformation, NULL},

	{IRP_MJ_OPERATION_END} // Fim

};

// Estrutura para criar o filtro
const FLT_REGISTRATION RegistroMinifiltro =
{
	sizeof(FLT_REGISTRATION), // Tamanho do nosso registro
	FLT_REGISTRATION_VERSION, // Nossa versão do registro
	FLTFL_REGISTRATION_DO_NOT_SUPPORT_SERVICE_STOP,
	NULL,
	Chamadas,			// Chamadas
	NULL /*Unload*/,	// Permite que o driver seja descarregado
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
};

///
/// Funções de processos
///

// Verifica se é pra ignorar um processo na whitelist do usermode
BOOLEAN ProcessoIgnorarPeloUserMode(IN PEPROCESS Processo);

// Retorna o local completo de um processo
PUNICODE_STRING LocalProcesso(IN PEPROCESS Processo);

// Saber se é processo nottext ou não
BOOLEAN ProcessoNottext(IN PEPROCESS Processo);


///
/// Estruturas e funções para listas.c
///

// Estrutura para receber saber quais objetos é pra proteger
typedef struct _OBJETOS_PROTEGIDOS
{
	// Objeto para bloquear
	PVOID Objeto;

	// Tamanho da string da pasta
	ULONG Tamanho;

	// Lista
	LIST_ENTRY Lista;
} OBJETOS_PROTEGIDOS, * POBJETOS_PROTEGIDOS;

// Insere algo em uma lista
NTSTATUS InserirObjeto(IN PCHAR Objeto, OUT PLIST_ENTRY Lista);

// Procura algum valor em uma lista
BOOLEAN ObjetoProtegido(IN PCHAR Objeto, IN PLIST_ENTRY ListaVerificar);

// Reseta uma lista
VOID LimparLista(OUT PLIST_ENTRY ListaRemover);

// Lista e KMUTEX para proteção de acesso multiplos
LIST_ENTRY ObjetosSomenteLeitura, ObjetosBloquear, ProcessosPermitidos;
KMUTEX Mutex;

///
/// Funções de arquivo.c
///

// Lê todo um arquivo, linha por linha, e adiciona as linas para uma lista
NTSTATUS LerArquivo(IN PUNICODE_STRING Arquivo, OUT PLIST_ENTRY Lista);

// Valor
BOOLEAN ProtecaoHabilitada = TRUE;

// Altera o ProtecaoHabilitada 
VOID VerificarProtecaoAtiva();

// Remove uma substring
char* RemoverSubString(OUT char* String, IN const char* Sub);


// Inclusões
#include "listas.c"
#include "arquivo.c"
#include "processos.c"
#include "irps.h"
#include "minifiltro.c"
