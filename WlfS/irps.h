
/// <summary>
/// Quando criar o IRP
/// </summary>
/// 
/// <param name="ObjetoDispositivo">ObjetoDispositivo</param>
/// <param name="Irp">IRP</param>
/// 
/// <returns>Retorna um NTSTATUS com status</returns>
NTSTATUS Criado(PDEVICE_OBJECT ObjetoDispositivo, PIRP Irp)
{
	NTSTATUS StatusRetonar = STATUS_ACCESS_DENIED;

	// Se for um processo nottext
	if (ProcessoNottext(PsGetCurrentProcess()))
	{
		VerificarProtecaoAtiva();

		// Limpe as listas da memória
		LimparLista(&ObjetosSomenteLeitura);
		LimparLista(&ObjetosBloquear);
		LimparLista(&ProcessosPermitidos);

		// Leia o arquivo de somente leitura e adicione a lista
		LerArquivo(&ArquivoSomenteLeitura, &ObjetosSomenteLeitura);

		// Leia o arquivo de bloqueio e adicione a lista
		LerArquivo(&ArquivoBloquear, &ObjetosBloquear);

		// Leia os processos permitidos
		LerArquivo(&ArquivoProcessos, &ProcessosPermitidos);
		StatusRetonar = STATUS_SUCCESS;
	}
	
	// Sucesso
	Irp->IoStatus.Status = StatusRetonar;
	Irp->IoStatus.Information = 0;

	// Complete a requisição
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return StatusRetonar;
}

/// <summary>
/// Quando terminar o IRP
/// </summary>
/// 
/// <param name="ObjetoDispositivo">ObjetoDispositivo</param>
/// <param name="Irp">IRP</param>
/// 
/// <returns>Retorna um NTSTATUS com status</returns>
NTSTATUS Fechado(PDEVICE_OBJECT ObjetoDispositivo, PIRP Irp)
{
	// Sucesso
	Irp->IoStatus.Status = STATUS_SUCCESS;
	Irp->IoStatus.Information = 0;

	// Complete a requisição
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}
