
#define TAGP 'ffgg'

/// <summary>
/// Insere um objeto em alguma lista
/// </summary>
/// 
/// <param name="Objeto">Pasta/Arquivo para proteger</param>
/// <param name="Lista">Lista para adicionar</param>
/// 
/// <returns>Retorna um NTSTATUS</returns>
NTSTATUS InserirObjeto(IN PCHAR Objeto, OUT PLIST_ENTRY Lista)
{
	// Váriaveis
	ULONG Tamanho = strlen(Objeto) + 1;
	POBJETOS_PROTEGIDOS pp = ExAllocatePoolWithTag(PagedPool, sizeof(OBJETOS_PROTEGIDOS) + Tamanho, TAGP);

	// Status
	NTSTATUS Status = STATUS_UNSUCCESSFUL;

	// Se alocar
	if (pp)
	{
		// Pasta
		pp->Objeto = (pp + 1);
		pp->Tamanho = Tamanho;

		// Copie
		RtlCopyMemory(pp->Objeto, Objeto, Tamanho);

		// Obtém o mutex que protege a lista de acessos paralelo
		Status = KeWaitForMutexObject(&Mutex, UserRequest, KernelMode, FALSE, NULL);

		if (!NT_SUCCESS(Status))
		{
			// Saia
			goto sair;
		}

		// Insira na lista
		InsertTailList(Lista, &pp->Lista);

		DbgPrint("Objeto inserida: %s", Objeto);

		// Libere o MUTEX
		KeReleaseMutex(&Mutex, FALSE);
	}

	// Sucesso, não devemos desalocar o "pp"
	return Status;

sair:

	if (pp != NULL)
	{
		// Libere
		ExFreePoolWithTag(pp, TAGP);
	}

	// Status
	return Status;
}

/// <summary>
/// Verifica se um item pertence a alguma lista
/// </summary>
/// 
/// <param name="Objeto">Objeto para comparar</param>
/// <param name="Lista">Lista para verificar</param>
/// 
/// <returns>Retorna um BOOL para informar se contém ou não</returns>
BOOLEAN ObjetoProtegido(IN PCHAR Objeto, IN PLIST_ENTRY ListaVerificar)
{
	// Retornar
	BOOLEAN Retornar = FALSE;

	// Se a lista estiver vazia
	if (IsListEmpty(ListaVerificar))
	{
		// Não podemos continuar
		return Retornar;
	}

	// Váriaveis
	PLIST_ENTRY pLista;
	POBJETOS_PROTEGIDOS ObjetosProtegidos;

	// Pegue
	pLista = ListaVerificar->Flink;

	// Faça um loop na lista
	while (pLista != ListaVerificar) {

		// Obtém o endereço a partir do nó
		ObjetosProtegidos = CONTAINING_RECORD(pLista, OBJETOS_PROTEGIDOS, Lista);
		
		if (ObjetosProtegidos->Objeto == NULL)
		{
			// Nulo, pare o loop
			break;
		}
		else {

			// Deixe os valores em maiusculo, para não haver diferença
			_strupr(Objeto);
			_strupr(ObjetosProtegidos->Objeto);

			// Se conter
			if (strstr(Objeto, ObjetosProtegidos->Objeto))
			{
				Retornar = TRUE;
				break;
			}
		}

		// Vamos para o próximo
		pLista = pLista->Flink;
	}

	return Retornar;
}

/// <summary>
/// Limpa a lista de pastas protegidas
/// </summary>
VOID LimparLista(OUT PLIST_ENTRY ListaRemover)
{
	PLIST_ENTRY pLista;
	POBJETOS_PROTEGIDOS Objeto;

	// Repetição até ficar vazio
	while (!IsListEmpty(ListaRemover))
	{
		// Pegue a primeiro nó da lista
		pLista = RemoveHeadList(ListaRemover);

		// Pegue o item do endereço
		Objeto = CONTAINING_RECORD(pLista, OBJETOS_PROTEGIDOS, Lista);

		// Libere
		ExFreePoolWithTag(Objeto, TAGP);
	}
}

