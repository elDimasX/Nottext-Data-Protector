
#include <fltKernel.h>
#include <ntstrsafe.h>

char* barraInvertida = "\\";

///
/// Váriaveis
///

BOOLEAN ConseguiuLerNoBoot = FALSE;

UNICODE_STRING Pasta = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism"
);

// Arquivo que contém objetos para somente leitura
UNICODE_STRING ArquivoSomenteLeitura = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.cat"
);

// Arquivo que contém objetos para bloquear todo o acesso
UNICODE_STRING ArquivoBloquear = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.sys"
);

// Arquivo que contém objetos para ocultar
UNICODE_STRING ArquivoOcultar = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.hnd"
);

// Arquivo que contém os objetos para não executar
UNICODE_STRING ArquivoNaoExecucao = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.enp"
);

// Arquivo que contém os objetos para proteger o processo de ser finalizado
UNICODE_STRING ArquivoProtegerFinalizar = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.prc"
);

// Arquivo que contém os objetos para ignorar processos
UNICODE_STRING ArquivoProcessos = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.inf"
);

// Arquivo que verifica se a proteção está habilitada ou não
UNICODE_STRING ArquivoProtecaoHabilitada = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.end"
);

// Verifica se é pra terminar processo que infrinjam as regras
UNICODE_STRING ArquivoFinalizarProcesso = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.term"
);

// Verifica se é pra mostrar a messagebox ou não
UNICODE_STRING ArquivoMessageBox = RTL_CONSTANT_STRING(
	L"\\??\\C:\\Windows\\System32\\sas-Spoiler-dism\\pp.msg"
);

// Atualiza as listas
VOID AtualizarListas();

// Lê todos os arquivos e lista para proteção
VOID LerConfiguracoes();

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

// Quando é uma operação de fechadura/limpeza do arquivo
PFLT_PREOP_CALLBACK_STATUS MiniFiltroCleanup(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID* Contexto
);

// Quando é uma operação de abertura de pasta
FLT_POSTOP_CALLBACK_STATUS MiniFiltroControlsPost(
	OUT PFLT_CALLBACK_DATA Data,
	IN PCFLT_RELATED_OBJECTS Objeto,
	IN PVOID Contexto,
	IN FLT_POST_OPERATION_FLAGS Flags
);

// Filtro
PFLT_FILTER Filtro = NULL;

// Chamdas que queremos registrar
const FLT_OPERATION_REGISTRATION Chamadas[] =
{
	{IRP_MJ_CREATE, 0, MiniFiltroPreCreate, NULL},

	{IRP_MJ_SET_INFORMATION, 0, MiniFiltroPreSetInformation, NULL},
	{IRP_MJ_SET_SECURITY, 0, MiniFiltroPreSetInformation, NULL},

	{IRP_MJ_DIRECTORY_CONTROL, 0, NULL, MiniFiltroControlsPost},

	{IRP_MJ_CLEANUP, 0, MiniFiltroCleanup, NULL},


	{IRP_MJ_OPERATION_END} // Fim

};

#define TestMode 0

// Se o modo de teste não estiver ativado
#if !TestMode

// Estrutura para criar o filtro
const FLT_REGISTRATION RegistroMinifiltro =
{
	sizeof(FLT_REGISTRATION), // Tamanho do nosso registro
	FLT_REGISTRATION_VERSION, // Nossa versão do registro
	FLTFL_REGISTRATION_DO_NOT_SUPPORT_SERVICE_STOP,
	NULL,
	Chamadas,			// Chamadas
	/*Unload*/NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
};
#else
const FLT_REGISTRATION RegistroMinifiltro =
{
	sizeof(FLT_REGISTRATION),
	FLT_REGISTRATION_VERSION,
	0,
	NULL,
	Chamadas,
	Unload,	// Permite que o driver seja descarregado
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
};
#endif

///
/// Funções de processos
///

#define PROCESS_TERMINATE		(0x0001)		// Processo terminado
#define PROCESS_VM_READ			(0x0010)		// Lê algumas informações
#define PROCESS_VM_WRITE		(0x0020)		// Escreve no processo
#define PROCESS_VM_OPERATION	(0x0008)		// Realiza alguma operação no processo
#define PROCESS_SUSPEND_RESUME	(0x0800)		// Suspende ou resume
#define PROCESS_SET_INFORMATION (0x0200)		// Altera alguma informação do processo
#define PROCESS_SET_PORT		(0x0800)		// Altera a porta do processo
#define PROCESS_SET_SESSIONID	(0x0004)		// Altera o ID do processo
#define PROCESS_CREATE_PROCESS	(0x0080)		// Um processo foi criado

// Registro para o monitoramento (para que possamos remover ele depois)
PVOID RegistroAutoProtecao = NULL;

// Quando uma imagem é carregada
VOID ImageLoadCallback(IN PUNICODE_STRING LocalCompleto, IN HANDLE Pid, IN PIMAGE_INFO ImageInfo);

// Monitora as aberturas e finalizações de processos
OB_PREOP_CALLBACK_STATUS MonitorDeProcessos(IN PVOID Contexto, OUT POB_PRE_OPERATION_INFORMATION Informacao);

// Instalador e removedor do monitor de processos
NTSTATUS InstalarMonitorDeProcessos();
NTSTATUS RemoverMonitorDeProcessos();

// Verifica se é pra ignorar um processo na whitelist do usermode
BOOLEAN ProcessoIgnorarPeloUserMode(IN PEPROCESS Processo);

// Retorna o local completo de um processo
char* LocalProcesso(IN PEPROCESS Processo);

// Saber se é processo nottext ou não
BOOLEAN EProcessoNottext(IN PEPROCESS Processo);

// Backup para fazer a verificação do Nottext mais rápido
PEPROCESS ProcessoNottextBackup = NULL;

// Se é pra terminar ou não os processo
BOOLEAN TerminarProcessos = FALSE;

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
LIST_ENTRY ObjetosSomenteLeitura, ObjetosBloquear, ObjetosOcultar, ObjetosNaoExecucao, ObjetosProtegerProcesso, ProcessosPermitidos;
KMUTEX Mutex;

///
/// Funções de arquivo.c
///

// Lê todo um arquivo, linha por linha, e adiciona as linas para uma lista
NTSTATUS LerArquivo(IN PUNICODE_STRING Arquivo, OUT PLIST_ENTRY Lista);

// Valor
BOOLEAN ProtecaoHabilitada = TRUE;

// Verifica se um arquivo existe
BOOLEAN ArquivoExiste(IN PUNICODE_STRING Arquivo, BOOLEAN EPasta);

// Remove uma substring
char* RemoverSubString(OUT char* String, IN const char* Sub);

// Concatena duas UNICODE_STRING
UNICODE_STRING ConcatenarUnicodeStrings(const UNICODE_STRING* str1, const UNICODE_STRING* str2);

// Função auxiliar para calcular o número de dígitos em um número inteiro
int ObterQuantidadeDeCaracteresEmInt(int num);

///
/// Funções de messagebox.c
///

// MessageBox
ULONG KeMessageBox(
	IN NTSTATUS Status,
	IN PUNICODE_STRING Titulo,
	IN PUNICODE_STRING Texto,
	IN ULONG_PTR Tipo
);

BOOLEAN MessageBox = FALSE;


// Botão de ok
#define MB_OK 0x00000000L


// Inclusões
#include "listas.c"
#include "arquivo.c"
#include "processos.c"
#include "MessageBox.c"
#include "irps.c"
#include "minifiltro.c"
#include "autoProtecaoDriver.h"
