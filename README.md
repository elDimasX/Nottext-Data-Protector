# Nottext Data Protector
Obs: precisa do *"dotnet-runtime-8.0.27-win-x64.exe"* agora, para rodar a UI nova<br/>
Projeto para proteger pastas/arquivos, processos ou ocultar objetos. Com nova UI totalmente integrada.<br/>
Ele funciona como um programa portátil e funciona desde o Windows 7 até Windows 11, de 32 a 64 bits<br/>
Ele é extremamente fácil de modificar e possui o código todo comentado, portanto, creio que seja de fácil compreensão, mas se você tive dúvidas, me envie uma mensagem no Discord: eldimas

## Como funciona
O Nottext Data Protector funciona ativando o modo de teste do Windows (não tenho certificado digital) e instalando um driver que faz uma comunicação básica entre arquivos compartilhados por eles dois<br/>

Aqui está um exemplo: <br/>
<img width="1154" height="792" alt="2026-06-06 (1)" src="https://github.com/user-attachments/assets/19597c3b-ba92-4af9-9e34-912659816b0b" />


 <br/><br/>
E aqui em baixo segue algumas fotos dele <br/>

<img width="1122" height="788" alt="2026-06-06 (7)" src="https://github.com/user-attachments/assets/7f374ae6-557b-4bff-bd36-7db212dcbf4c" />
<img width="1122" height="788" alt="2026-06-06 (8)" src="https://github.com/user-attachments/assets/caf99759-8727-4951-a9fe-c050b70515ce" />
<img width="820" height="620" alt="2026-06-06 (9)" src="https://github.com/user-attachments/assets/24b02d4b-4456-47c9-aea5-8a5aecf89441" />
<img width="1122" height="788" alt="2026-06-06 (10)" src="https://github.com/user-attachments/assets/3ddbd6d6-5613-4a3a-ba2b-5d7a82ccd829" />
<img width="1289" height="908" alt="2026-06-06 (12)" src="https://github.com/user-attachments/assets/618723ce-41a9-4b54-8d56-797ae3be817b" />
<img width="1207" height="892" alt="2026-06-06 (13)" src="https://github.com/user-attachments/assets/36479ff8-ac1c-4a1e-b095-508f31e055e4" />
<img width="1337" height="935" alt="2026-06-06 (5)" src="https://github.com/user-attachments/assets/3b3d471b-47b5-4dda-9815-54979ab232ee" />

## 1.0.0.3
- Nova UI totalmente atualizada, com tema branco/preto e informações de forma mais organizada 

## 1.0.0.2

- Proteção por senha (agora, o aplicativo pode ser bloqueado de ser acessado)
- Compactado (o aplicativo tem somente um arquivo, tornando totalmente portável)
- Proteção adicionadas: Ocultar, Proteger Processo, Não Executar
- Botão de procurar arquivos ou pastas
- Alguns bugs corrigidos e aprimorados
- O driver agora inicia no boot, permitindo a proteção no modo segurança do Windows
- Autoproteção para o driver, agora, ele está mais difícil de ser removido por qualquer usuário
- Multiplas proteções para o mesmo objeto (agora você pode selecionar mais de uma proteção por arquivo ou pasta)
- Agora, o aplicativo também protege pastas por compartilhamento de rede

## 1.0.0.1

- Impede modificações por IRP (uma técnica muito avançada para modificar arquivos no kernel)
- MessageBox nos processos que infrijem as regras de arquivos protegidos
- Processos agora podem ser finalizado imedatamente após violar alguma regra de proteção
- Correção de bugs e desempenho corrigido

## 1.0.0.0

- Configurações básicas, como instalar drivers, habilita ou não a proteção global, e etc (básico)
- Ignorar processos, para que eles possam mexer nos objetos protegidos
- Proteje os arquivos contra modificações de somente-leitura ou qualquer acesso

## Avisos
Cuidado com este programa, ele não possui limitações ou restrições de pastas, você pode bloquear tudo, até mesmo o C:\, então escolha suas pastas com cuidado!
