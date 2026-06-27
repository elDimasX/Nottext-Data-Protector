# Nottext Data Protector - WPF Migration

Esta pasta contém uma primeira migração do projeto WinForms original para WPF, mantendo a lógica principal em C#/.NET Framework 4.7.2.

## O que foi reaproveitado

- `Global.cs`
- `Kernel.cs`
- `SetAcess.cs`
- `Properties/Resources.resx`
- Ícone e imagens da pasta `Resources`
- Fluxos principais de instalação/verificação/remoção do driver `WlfS`
- Arquivos de configuração em `C:\Windows\System32\sas-Spoiler-dism\`
- Hash SHA-256 da senha em `pp.psh`

## O que virou WPF

- Janela principal com sidebar moderna
- Tela de objetos protegidos
- Tela de processos autorizados
- Tela de configurações
- Tela sobre/proteções
- Dialog de seleção de tipo de proteção
- Dialog de senha

## Como abrir

Abra `NottextDataProtectorWpf.sln` no Visual Studio em uma máquina Windows com .NET Framework 4.7.2 Developer Pack.

Compile em `x64`, porque o projeto original usa driver e recursos x64.

## Observações importantes

Esta migração foi feita sem executar o build dentro deste ambiente, porque WPF/.NET Framework precisa do toolchain Windows/Visual Studio. Pode ser necessário ajustar algum detalhe pequeno de referência/recurso ao abrir no Visual Studio.

A UI nova evita Guna UI e usa WPF puro, então o visual não depende mais da DLL `Guna.UI2.dll`.
