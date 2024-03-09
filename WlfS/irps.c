
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
	if (EProcessoNottext(PsGetCurrentProcess()))
	{
		// É o nosso processo confiável
		ProcessoNottextBackup = PsGetCurrentProcess();

		// Limpe as listas da memória
		LimparLista(&ObjetosSomenteLeitura);
		LimparLista(&ObjetosBloquear);
		LimparLista(&ObjetosOcultar);
		LimparLista(&ObjetosNaoExecucao);
		LimparLista(&ObjetosProtegerProcesso);
		LimparLista(&ProcessosPermitidos);

		// Releia tudo
		AtualizarListas();


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
